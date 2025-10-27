using System;
using System.Collections.Generic;
using System.Data.SQLite;  // ВАЖНО: добавляем using
using Katastata.Models;
using Katastata.Services;


namespace Katastata.Services
{
    public class StatisticService
    {
        private readonly DB_Operation _db;

        public StatisticService(DB_Operation db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public List<Statistic> GetUserStatistics(int userId, DateTime startDate, DateTime endDate)
        {
            string sql = @"SELECT * FROM Statistic
                           WHERE user_id = @UserId
                           AND last_launch_at BETWEEN @StartDate AND @EndDate
                           ORDER BY last_launch_at DESC";

            // Используем System.Data.SQLite.SQLiteParameter явно
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@UserId", userId),
                new SQLiteParameter("@StartDate", startDate),
                new SQLiteParameter("@EndDate", endDate)
            };

            return _db.ExecuteQuery<Statistic>(sql, parameters);
        }

        public List<(Applications app, TimeSpan total_time)> GetTopApplications(
            int userId, DateTime startDate, DateTime endDate)
        {
            var stats = GetUserStatistics(userId, startDate, endDate);

            string appSql = @"SELECT * FROM Applications
                             WHERE id IN (SELECT app_id FROM Statistic WHERE user_id = @UserId)";
            var appParams = new SQLiteParameter[] { new SQLiteParameter("@UserId", userId) };
            var apps = _db.ExecuteQuery<Applications>(appSql, appParams);

            return stats
                .Select(s =>
                {
                    var app = apps.FirstOrDefault(a => a.id == s.app_id);
                    return (app, s.total_time);  // Правильно: total_time (не totalTime!)
                })
                .OrderByDescending(x => x.total_time)  // Здесь тоже total_time!
                .Take(5)
                .ToList();
        }

        public TimeSpan GetTotalUsageTime(int userId, DateTime startDate, DateTime endDate)
        {
            var stats = GetUserStatistics(userId, startDate, endDate);
            return TimeSpan.FromSeconds(stats.Sum(s => s.total_time.TotalSeconds));
        }

        public int GetLaunchCount(int userId, DateTime startDate, DateTime endDate)
        {
            var stats = GetUserStatistics(userId, startDate, endDate);
            return stats.Count;
        }
    }
}
