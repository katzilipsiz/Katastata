using Katastata.Data;
using Katastata.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using Microsoft.EntityFrameworkCore;

namespace Katastata.Services
{
    public class AppMonitorService
    {
        private readonly AppDbContext _context;
        private Dictionary<int, Session> activeSessions = new Dictionary<int, Session>();
        private System.Timers.Timer monitoringTimer;

        // P/Invoke для определения активного окна
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public AppMonitorService(AppDbContext context) => _context = context;

        public void StartMonitoring(int userId)
        {
            if (monitoringTimer == null)
            {
                monitoringTimer = new System.Timers.Timer(10000); // Каждые 10 секунд
                monitoringTimer.Elapsed += (sender, e) => MonitorProcesses(userId);
                monitoringTimer.Start();
            }
        }   

        private void MonitorProcesses(int userId)
        {
            try
            {
                var currentProcesses = Process.GetProcesses()
                    .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                    .ToDictionary(p => p.Id, p => p);

                // Завершить закрытые сессии
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

                // Определить активное окно
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
                            EndTime = DateTime.Now // Будет обновлено при закрытии
                        };
                        activeSessions[fgProcess.Id] = newSession;
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки, если нужно
            }
        }

        private Program AddNewProgram(Process proc)
        {
            if (!_context.Categories.Any(c => c.Id == 1))
            {
                _context.Categories.Add(new Category { Id = 1, Name = "Не классифицировано" });
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

        public void ScanRunningPrograms(int userId)
        {
            var processes = Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .ToList();

            if (!_context.Categories.Any(c => c.Id == 1))
            {
                _context.Categories.Add(new Category { Id = 1, Name = "Не классифицировано" });
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

        public List<Program> GetAllPrograms(int userId)
        {
            return _context.Programs.Include(p => p.Category).ToList();
        }

        // Новый метод для получения сессий пользователя
        public List<Session> GetSessions(int userId)
        {
            return _context.Sessions
                .Where(s => s.UserId == userId)
                .Include(s => s.Program)
                .ThenInclude(p => p.Category)
                .OrderByDescending(s => s.StartTime)
                .ToList();
        }
    }
}