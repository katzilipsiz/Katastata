using Katastata.Data;
using Katastata.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Katastata.Services
{
    public class AppMonitorService
    {
        private readonly AppDbContext _context;

        public AppMonitorService(AppDbContext context)
        {
            _context = context;
        }

        public void ScanRunningPrograms(int userId)
        {
            var processes = Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)) // Только видимые окна
                .ToList();

            // Убедимся, что дефолтная категория есть
            var defaultCategory = _context.Categories.FirstOrDefault(c => c.Id == 1);
            if (defaultCategory == null)
            {
                defaultCategory = new Category { Id = 1, Name = "Не классифицировано" };
                _context.Categories.Add(defaultCategory);
                _context.SaveChanges();
            }

            foreach (var process in processes)
            {
                try
                {
                    string path = string.Empty;
                    try
                    {
                        path = process.MainModule?.FileName ?? "";
                    }
                    catch { }

                    if (string.IsNullOrEmpty(path)) continue;

                    // Проверяем, есть ли программа уже в базе
                    var existingProgram = _context.Programs.FirstOrDefault(p => p.Path == path);
                    if (existingProgram != null) continue;

                    // Добавляем новую программу
                    var newProgram = new Program
                    {
                        Name = string.IsNullOrEmpty(process.ProcessName) ? "Unknown" : process.ProcessName,
                        Path = path,
                        CategoryId = defaultCategory.Id
                    };

                    _context.Programs.Add(newProgram);
                }
                catch
                {
                    // просто пропускаем процессы без доступа
                    continue;
                }
            }

            // Сохраняем все изменения в БД
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Логируем внутреннюю ошибку для диагностики
                Console.WriteLine(ex.InnerException?.Message);
                throw; // можно убрать throw, если не хотим падения приложения
            }
        }

        public List<Program> GetAllPrograms(int userId)
        {
            return _context.Programs
                .Include(p => p.Category) // чтобы Category.Name работал в DataGrid
                .ToList();
        }

        public void StartProgramSession(int userId, string programPath)
        {
            // Ищем программу по пути
            var program = _context.Programs.FirstOrDefault(p => p.Path == programPath);
            if (program == null) return;

            // Создаём новую сессию
            var session = new Session
            {
                UserId = userId,
                ProgramId = program.Id,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now // пока что ставим одинаково
            };

            _context.Sessions.Add(session);

            // Обновляем статистику
            var stats = _context.Statistics.FirstOrDefault(s => s.UserId == userId && s.ProgramId == program.Id);
            if (stats == null)
            {
                stats = new Statistics
                {
                    UserId = userId,
                    ProgramId = program.Id,
                    TotalTime = TimeSpan.Zero,
                    LastLaunch = DateTime.Now
                };
                _context.Statistics.Add(stats);
            }
            else
            {
                stats.LastLaunch = DateTime.Now;
            }

            _context.SaveChanges();
        }

        public List<Session> GetUserSessions(int userId)
        {
            return _context.Sessions
                .Include(s => s.Program)
                    .ThenInclude(p => p.Category)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartTime)
                .ToList();
        }

        public AppDbContext GetDbContext()
        {
            return _context;
        }

    }
}
