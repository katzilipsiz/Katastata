using System;
using System.Collections.Generic;
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
            Console.WriteLine("▶ Сканирование началось");

            var defaultCat = _context.Categories.FirstOrDefault(c => c.Id == 1);
            if (defaultCat == null)
            {
                defaultCat = new Category { Id = 1, Name = "Не классифицировано" };
                _context.Categories.Add(defaultCat);
                _context.SaveChanges();
            }

            var processes = Process.GetProcesses();

            foreach (var p in processes)
            {
                try
                {
                    var path = p.MainModule?.FileName;
                    if (string.IsNullOrEmpty(path)) continue;

                    if (_context.Programs.Any(x => x.Path == path)) continue;

                    var program = new Program
                    {
                        Name = p.ProcessName,
                        Path = path,
                        CategoryId = defaultCat.Id
                    };

                    _context.Programs.Add(program);
                }
                catch (Exception ex)
                {
                    // Игнорируем системные процессы
                    Debug.WriteLine($"Не удалось получить доступ к {p.ProcessName}: {ex.Message}");
                }
            }

            try
            {
                _context.SaveChanges();
                Console.WriteLine("✅ Сканирование завершено и сохранено в БД");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при сохранении: {ex.Message}");
                throw;
            }
        }

        public List<Program> GetAllPrograms()
        {
            return _context.Programs
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToList();
        }
    }
}
