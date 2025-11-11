using Katastata.Data;
using Katastata.Models;
using System.Diagnostics;
using System.Linq;
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
            var processes = Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .ToList();

            foreach (var process in processes)
            {
                try
                {
                    string path = string.Empty;
                    try { path = process.MainModule?.FileName ?? ""; } catch { }

                    if (string.IsNullOrEmpty(path)) continue;
                    if (_context.Programs.Any(p => p.Path == path)) continue;

                    _context.Programs.Add(new Program
                    {
                        Name = process.ProcessName,
                        Path = path,
                        CategoryId = 1
                    });
                }
                catch { }
            }

            _context.SaveChanges();
        }

        public List<Program> GetAllPrograms(int userId)
        {
            return _context.Programs.Include(p => p.Category).ToList();
        }
    }
}
