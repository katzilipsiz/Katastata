using System.Diagnostics;
using Katastata.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Katastata.Services
{
    public class ApplicationService
    {
        private readonly DB_Operation _db;
        private Dictionary<string, int> _appCache = new();

        public ApplicationService(DB_Operation db)
        {
            _db = db;
        }

        // Получает список всех запущенных приложений с заголовками окон
        public List<string> GetRunningApplications()
        {
            return Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .Select(p => p.MainWindowTitle)
                .ToList();
        }

        // Находит ID приложения в БД или создаёт новое
        public int GetOrCreateApplicationId(string title, string processName)
        {
            // Проверяем кэш
            if (_appCache.TryGetValue(title, out int cachedId))
                return cachedId;

            // Ищем в БД
            string sql = "SELECT id FROM Applications WHERE title = @Title AND process = @Process";
            var parameters = new[]
            {
                new SQLiteParameter("@Title", title),
                new SQLiteParameter("@Process", processName)
            };

            var existing = _db.ExecuteQuery<Applications>(sql, parameters);
            if (existing.Count > 0)
            {
                int id = existing[0].id;
                _appCache[title] = id;
                return id;
            }

            // Создаём новое приложение
            var newApp = new Applications
            {
                title = title,
                process = processName,
                category_id = 1 // Категория по умолчанию
            };

            string insertSql = @"INSERT INTO Applications (title, process, category_id) 
                               VALUES (@Title, @Process, @CategoryId)";
            var insertParams = new[]
            {
                new SQLiteParameter("@Title", newApp.title),
                new SQLiteParameter("@Process", newApp.process),
                new SQLiteParameter("@CategoryId", newApp.category_id)
            };

            _db.Query(insertSql, insertParams);

            // Получаем ID новой записи
            string idSql = "SELECT last_insert_rowid()";
            var idResult = _db.ExecuteQuery<dynamic>(idSql);
            int newId = Convert.ToInt32(idResult[0]);

            _appCache[title] = newId;
            return newId;
        }
    }
}
