using Katastata.Data;
using Katastata.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using Microsoft.EntityFrameworkCore;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using DocumentFormat.OpenXml.Wordprocessing;

namespace Katastata.Services
{
    public class AppMonitorService
    {
        private readonly AppDbContext _context;

        private Dictionary<int, Session> activeSessions = new Dictionary<int, Session>();
        private System.Timers.Timer monitoringTimer; 

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // Конструктор
        public AppMonitorService(AppDbContext context) => _context = context;

        // Начало мониторинга
        public void StartMonitoring(int userId)
        {
            if (monitoringTimer == null)
            {
                monitoringTimer = new System.Timers.Timer(10000);
                monitoringTimer.Elapsed += (sender, e) => MonitorProcesses(userId);
                monitoringTimer.Start();
            }
        }

        // Мониторинг процессов
        private void MonitorProcesses(int userId)
        {
            try
            {
                var currentProcesses = Process.GetProcesses()
                    .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                    .ToDictionary(p => p.Id, p => p);

                foreach (var sessionId in activeSessions.Keys.ToList())
                {
                    if (!currentProcesses.ContainsKey(sessionId))
                    {
                        var session = activeSessions[sessionId];
                        session.EndTime = DateTime.Now;
                        _context.Sessions.Add(session);
                        _context.SaveChanges();
                        UpdateStatistics(session);
                        activeSessions.Remove(sessionId);
                    }
                }

                IntPtr foregroundWindow = GetForegroundWindow();
                uint fgProcessId;
                GetWindowThreadProcessId(foregroundWindow, out fgProcessId);

                if (fgProcessId > 0 && currentProcesses.TryGetValue((int)fgProcessId, out var fgProcess) &&
                    !activeSessions.ContainsKey(fgProcess.Id))
                {
                    string path = "";
                    try { path = fgProcess.MainModule?.FileName ?? ""; } catch { }

                    if (!string.IsNullOrEmpty(path))
                    {
                        var program = _context.Programs.FirstOrDefault(p => p.Path == path) ??
                                      AddNewProgram(fgProcess);
                        var newSession = new Session
                        {
                            UserId = userId,
                            ProgramId = program.Id,
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now
                        };
                        activeSessions[fgProcess.Id] = newSession;
                    }
                }
            }
            catch { }
        }

        // Сканирование программ
        private Program AddNewProgram(Process proc)
        {
            if (!_context.Categories.Any(c => c.Id == 1))
            {
                _context.Categories.Add(new Models.Category { Id = 1, Name = "Не классифицировано" });
                _context.SaveChanges();
            }

            var program = new Program
            {
                Name = proc.ProcessName ?? "Unknown",
                Path = proc.MainModule?.FileName ?? "",
                CategoryId = 1
            };
            _context.Programs.Add(program);
            _context.SaveChanges();
            return program;
        }


        // Обновление статистики
        private void UpdateStatistics(Session session)
        {
            var stat = _context.Statistics.FirstOrDefault(s => s.UserId == session.UserId && s.ProgramId == session.ProgramId);
            if (stat == null)
            {
                stat = new Statistics
                {
                    UserId = session.UserId,
                    ProgramId = session.ProgramId,
                    TotalTime = TimeSpan.Zero,
                    LastLaunch = null
                };
                _context.Statistics.Add(stat);
            }

            stat.TotalTime += session.EndTime - session.StartTime;
            stat.LastLaunch = session.StartTime > (stat.LastLaunch ?? DateTime.MinValue) ? session.StartTime : stat.LastLaunch;
            _context.SaveChanges();
        }

        // Сканирование запущенный программ
        public void ScanRunningPrograms(int userId)
        {
            var processes = Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .ToList();

            if (!_context.Categories.Any(c => c.Id == 1))
            {
                _context.Categories.Add(new Models.Category { Id = 1, Name = "Не классифицировано" });
                _context.SaveChanges();
            }

            foreach (var proc in processes)
            {
                try
                {
                    string path = proc.MainModule?.FileName ?? "";
                    if (string.IsNullOrEmpty(path)) continue;

                    if (!_context.Programs.Any(p => p.Path == path))
                    {
                        _context.Programs.Add(new Program
                        {
                            Name = proc.ProcessName ?? "Unknown",
                            Path = path,
                            CategoryId = 1
                        });
                    }
                }
                catch { }
            }
            _context.SaveChanges();
        }

        // Получение списка программ
        public List<Program> GetAllPrograms(int userId)
        {
            return _context.Programs.Include(p => p.Category).ToList();
        }

        // Получение сессий
        public List<Session> GetSessions(int userId)
        {
            return _context.Sessions
                .Where(s => s.UserId == userId)
                .Include(s => s.Program)
                .ThenInclude(p => p.Category)
                .OrderByDescending(s => s.StartTime)
                .ToList();
        }

        // Получение статистики
        public List<Statistics> GetStatistics(int userId)
        {
            var stats = _context.Statistics
                .Where(st => st.UserId == userId)
                .Include(st => st.Program)
                .ToList();

            return stats
                .OrderByDescending(st => st.TotalTime)
                .ToList();
        }

        // Экспорт статистики в Excel
        public void ExportStatisticsToExcel(int userId, string filePath)
        {
            var stats = GetStatistics(userId);
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Statistics" };
                sheets.Append(sheet);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Заголовки
                Row headerRow = new Row();
                headerRow.Append(CreateTextCell("Программа"));
                headerRow.Append(CreateTextCell("Общее время"));
                headerRow.Append(CreateTextCell("Последний запуск"));
                sheetData.Append(headerRow);

                // Данные
                foreach (var stat in stats)
                {
                    Row dataRow = new Row();
                    dataRow.Append(CreateTextCell(stat.Program?.Name ?? "Unknown"));
                    dataRow.Append(CreateTextCell(stat.TotalTime.ToString()));
                    dataRow.Append(CreateTextCell(stat.LastLaunch?.ToString() ?? "N/A"));
                    dataRow.Append(CreateTextCell(stat.UserId.ToString() ?? "Guest"));
                    sheetData.Append(dataRow);
                }

                workbookPart.Workbook.Save();
            }
        }

        private Cell CreateTextCell(string text)
        {
            return new Cell { CellValue = new CellValue(text), DataType = CellValues.String };
        }

        public void ExportStatisticsToWord(int userId, string filePath)
        {
            var stats = GetStatistics(userId);
            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                DocumentFormat.OpenXml.Wordprocessing.Table table = new DocumentFormat.OpenXml.Wordprocessing.Table();
                TableRow headerRow = new TableRow();
                headerRow.Append(new TableCell(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Программа")))));
                headerRow.Append(new TableCell(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Общее время")))));
                headerRow.Append(new TableCell(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Последний запуск")))));
                table.Append(headerRow);

                foreach (var stat in stats)
                {
                    TableRow dataRow = new TableRow();
                    dataRow.Append(new TableCell(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(stat.Program?.Name ?? "Unknown")))));
                    dataRow.Append(new TableCell(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(stat.TotalTime.ToString())))));
                    dataRow.Append(new TableCell(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(stat.LastLaunch?.ToString() ?? "N/A")))));
                    dataRow.Append(new TableCell(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(stat.User.ToString() ?? "Guest")))));
                    table.Append(dataRow);
                }

                body.Append(table);
                mainPart.Document.Save();
            }
        }
    }
}
