using System;
using System.Collections.Generic;
using Katastata.Models;
using Katastata.Services;
using System.Data.SQLite;

namespace Katastata.Services
{
    public class ApplicationsService
    {
        private readonly DB_Operation _db;

        public ApplicationsService(DB_Operation db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public void AddApplication(Applications app)
        {
            string sql = @"INSERT INTO Applications (title, process, category_id, description)
                          VALUES (@Title, @Process, @CategoryId, @Description)";

            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@Title", app.title),
                new SQLiteParameter("@Process", app.process),
                new SQLiteParameter("@CategoryId", app.category_id),
                new SQLiteParameter("@Description", app.description)
            };

            _db.Query(sql, parameters);
        }

        public List<Applications> GetAllApplications()
        {
            string sql = "SELECT * FROM Applications";
            return _db.ExecuteQuery<Applications>(sql);
        }

        public bool IsApplicationExists(string title)
        {
            string sql = "SELECT COUNT(*) FROM Applications WHERE title = @Title";
            var parameters = new SQLiteParameter("@Title", title);

            using (var connection = new SQLiteConnection(_db.connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.Add(parameters);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}
