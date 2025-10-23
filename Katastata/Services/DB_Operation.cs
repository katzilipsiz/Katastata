using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.SQLite;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Katastata.Services
{
    public class DB_Operation
    {
            private static readonly string connectionString = @"Data Source=Katastata.db;Version=3";

            public DB_Operation() { }

            public void Query(string sql, SQLiteParameter[] parameters = null)
            {
                using SQLiteConnection connection = new(connectionString);
                connection.Open(); // Открываем соединение
                SQLiteCommand command = new(sql, connection);

                if (parameters != null)
                {
                    foreach (SQLiteParameter item in parameters)
                    {
                        _ = command.Parameters.Add(item);
                    }
                }

                _ = command.ExecuteNonQuery();
                // Здесь соединение автоматически закрывается
            }

            public List<T> ExecuteQuery<T>(string query, SQLiteParameter[] parameters = null) where T : new()
            {
                List<T> result = [];

                using (SQLiteConnection connection = new(connectionString))
                {
                    connection.Open();

                    using SQLiteCommand command = new(query, connection);
                    if (parameters != null)
                    {
                        foreach (SQLiteParameter item in parameters)
                        {
                            _ = command.Parameters.Add(item);
                        }
                    }

                    using SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        T obj = new();

                        // Заполняем основные свойства объекта
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object value = reader.GetValue(i);

                            // Получаем свойство объекта, которое соответствует имени столбца 
                            PropertyInfo property = typeof(T).GetProperty(columnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                            if (property != null && value != DBNull.Value)
                            {
                                property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                            }
                        }

                        // Обрабатываем связанные объекты динамически
                        foreach (PropertyInfo property in typeof(T).GetProperties())
                        {
                            // Проверяем, что это не строка и что это класс (то есть другой объект)
                            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                            {
                                object relatedEntity = Activator.CreateInstance(property.PropertyType);
                                bool hasRelatedData = false;

                                // Для каждого свойства связанного объекта
                                foreach (PropertyInfo relatedProperty in property.PropertyType.GetProperties())
                                {
                                    // Проверяем, есть ли колонка, которая соответствует этому свойству
                                    string relatedColumnName = $"{property.Name}_{relatedProperty.Name}"; // Динамическое создание имени колонки

                                    // Если колонка с именем существует в результате запроса и имеет данные
                                    int relatedOrdinal;
                                    try
                                    {
                                        relatedOrdinal = reader.GetOrdinal(relatedColumnName);
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        continue; // Если столбец не найден, продолжаем к следующему свойству
                                    }

                                    if (relatedOrdinal >= 0 && reader[relatedColumnName] != DBNull.Value)
                                    {
                                        relatedProperty.SetValue(relatedEntity, Convert.ChangeType(reader[relatedColumnName], relatedProperty.PropertyType));
                                        hasRelatedData = true;
                                    }
                                }

                                // Если данные для связанного объекта были найдены, устанавливаем его в основной объект
                                if (hasRelatedData)
                                {
                                    property.SetValue(obj, relatedEntity);
                                }
                            }
                        }

                        result.Add(obj);
                    }
                }

                return result;
            }
    }
}
