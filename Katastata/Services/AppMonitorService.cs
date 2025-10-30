using System;
using System.Diagnostics;
using System.Linq;
using Katastata.Data;
using Katastata.Models;
using Microsoft.EntityFrameworkCore;

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
            var runningProcesses = Process.GetProcesses();

            foreach (var proc in runningProcesses)
            {
                try
                {
                    string name = proc.ProcessName;
                    string path = proc.MainModule?.FileName ?? "";

                    if (string.IsNullOrEmpty(path))
                        continue;

                    // Проверяем, есть ли программа в базе
                    var program = _context.Programs.Include(p => p.Category)
                        .FirstOrDefault(p => p.Path == path);

                    if (program == null)
                    {
                        // Если нет, создаём новую запись
                        program = new Program
                        {
                            Name = name,
                            Path = path,
                            CategoryId = 1 // временно "Не классифицировано"
                        };
                        _context.Programs.Add(program);
                        _context.SaveChanges();
                    }

                    // Создаём новую сессию
                    var session = new Session
                    {
                        UserId = userId,
                        ProgramId = program.Id,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now // пока конец = начало, позже будем обновлять
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
                    stats.LastLaunch = DateTime.Now;

                    _context.SaveChanges();
                }
                catch
                {
                    // Игнорируем процессы, к которым нет доступа
                }
            }
        }

        public List<Program> GetAllPrograms(int userId)
        {
            var programIds = _context.Sessions
                .Where(s => s.UserId == userId)
                .Select(s => s.ProgramId)
                .Distinct()
                .ToList();

            return _context.Programs
                .Where(p => programIds.Contains(p.Id))
                .ToList();
        }

    }
}
