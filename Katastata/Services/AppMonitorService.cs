using Katastata.Data;
using Katastata.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Katastata.Services
{
    public class AppMonitorService
    {
        private readonly AppDbContext _context;

        public AppMonitorService(AppDbContext context) => _context = context;

        public void ScanRunningPrograms(int userId)
        {
            var processes = Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .ToList();

            // ensure default category exists
            if (!_context.Categories.Any(c => c.Id == 1))
            {
                _context.Categories.Add(new Category { Id = 1, Name = "Не классифицировано" });
                _context.SaveChanges();
            }

            foreach (var proc in processes)
            {
                try
                {
                    string path = "";
                    try { path = proc.MainModule?.FileName ?? ""; } catch { }

                    if (string.IsNullOrEmpty(path)) continue;

                    if (!_context.Programs.Any(p => p.Path == path))
                    {
                        _context.Programs.Add(new Program
                        {
                            Name = string.IsNullOrEmpty(proc.ProcessName) ? "Unknown" : proc.ProcessName,
                            Path = path,
                            CategoryId = 1
                        });
                    }
                }
                catch { /* ignore access denied */ }
            }

            _context.SaveChanges();
        }

        public List<Program> GetAllPrograms(int userId)
        {
            return _context.Programs.Include(p => p.Category).ToList();
        }
    }
}
