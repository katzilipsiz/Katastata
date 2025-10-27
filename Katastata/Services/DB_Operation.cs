using System;
using System.Data.SQLite;
using System.Configuration;
using System.Reflection;

namespace Katastata.Services
{
    public class DB_Operation
    {
        public string connectionString;

        public DB_Operation()
        {
            connectionString = ConfigurationManager.ConnectionStrings["KatastataDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Строка подключения не найдена в App.config");
        }

        public List<T> ExecuteQuery<T>(string query, SQLiteParameter[] parameters = null) where T : new()
        {
            var result = new List<T>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obj = new T();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                object value = reader.GetValue(i);

                                var property = typeof(T).GetProperty(columnName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                                if (property != null && value != DBNull.Value)
                                    property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                            }
                            result.Add(obj);
                        }
                    }
                }
            }
            return result;
        }

        public void Query(string sql, SQLiteParameter[] parameters = null)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
