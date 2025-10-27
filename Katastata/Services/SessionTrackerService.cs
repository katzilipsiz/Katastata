using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Katastata.Models;
using Katastata.Services;
using System.Data.SQLite;

namespace Katastata.Services
{
    public class SessionTrackerService
    {
        private readonly DB_Operation _db;
        private readonly ApplicationService _appService;
        private System.Timers.Timer _timer;
        private Sessions _currentSession;
        private int _userId;

        public SessionTrackerService(DB_Operation db, ApplicationService appService)
        {
            _db = db;
            _appService = appService;
        }

        // Запуск отслеживания для конкретного пользователя
        public void StartTracking(int userId)
        {
            _userId = userId;
            _currentSession = null;

            _timer = new System.Timers.Timer(5000); // Проверка каждые 5 секунд
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        // Остановка отслеживания
        public void StopTracking()
        {
            _timer?.Stop();
            _timer?.Dispose();

            // Завершаем текущую сессию
            if (_currentSession != null && _currentSession.end_at == null)
            {
                EndCurrentSession();
            }
        }

        private void OnTimerElapsed(object source, ElapsedEventArgs e)
        {
            var activeApps = _appService.GetRunningApplications();
            if (activeApps.Count == 0) return;

            string currentAppTitle = activeApps[0]; // Берём первое активное окно
            string currentProcess = Process.GetProcesses()
                .FirstOrDefault(p => p.MainWindowTitle == currentAppTitle)?.ProcessName ?? string.Empty;


            int appId = _appService.GetOrCreateApplicationId(currentAppTitle, currentProcess);

            // Если приложение изменилось — завершаем текущую сессию и начинаем новую
            if (_currentSession == null || _currentSession.app_id != appId)
            {
                if (_currentSession != null && _currentSession.end_at == null)
                {
                    EndCurrentSession();
                }

                StartNewSession(appId);
            }
        }

        private void StartNewSession(int appId)
        {
            _currentSession = new Sessions
            {
                user_id = _userId,
                app_id = appId,
                begin_at = DateTime.Now,
                end_at = null,
                duration = TimeSpan.Zero
            };

            string sql = @"INSERT INTO Sessions (user_id, app_id, begin_at, end_at, duration)
                        VALUES (@UserId, @AppId, @BeginAt, @EndAt, @Duration)";
            var parameters = new[]
            {
                new SQLiteParameter("@UserId", _currentSession.user_id),
                new SQLiteParameter("@AppId", _currentSession.app_id),
                new SQLiteParameter("@BeginAt", _currentSession.begin_at),
                new SQLiteParameter("@EndAt", _currentSession.end_at),
                new SQLiteParameter("@Duration", _currentSession.duration.ToString())
            };

            _db.Query(sql, parameters);
        }

        private void EndCurrentSession()
        {
            _currentSession.end_at = DateTime.Now;
            _currentSession.duration = _currentSession.GetDuration();

            string sql = @"UPDATE Sessions 
                        SET end_at = @EndAt, duration = @Duration
                        WHERE id = @Id";
            var parameters = new[]
            {
                new SQLiteParameter("@EndAt", _currentSession.end_at),
                new SQLiteParameter("@Duration", _currentSession.duration.ToString()),
                new SQLiteParameter("@Id", _currentSession.id)
            };

            _db.Query(sql, parameters);

            UpdateStatistics(_currentSession);
        }

        private void UpdateStatistics(Sessions session)
        {
            // Получаем текущую статистику для приложения и пользователя
            string statSql = "SELECT * FROM Statistic WHERE user_id = @UserId AND app_id = @AppId";
            var statParams = new[]
            {
                new SQLiteParameter("@UserId", session.user_id),
                new SQLiteParameter("@AppId", session.app_id)
            };
            var stats = _db.ExecuteQuery<Statistic>(statSql, statParams);

            var stat = stats.FirstOrDefault() ?? new Statistic
            {
                user_id = session.user_id,
                app_id = session.app_id,
                total_time = TimeSpan.Zero,
                last_launch_at = DateTime.MinValue
            };

            stat.UpdateFromSession(session);

            if (stats.Count > 0)
            {
                // Обновляем существующую запись
                string updateSql = @"UPDATE Statistic
                                SET total_time = @TotalTime, last_launch_at = @LastLaunchAt
                                WHERE id = @Id";
                var updateParams = new[]
                {
                    new SQLiteParameter("@TotalTime", stat.total_time.ToString()),
                    new SQLiteParameter("@LastLaunchAt", stat.last_launch_at),
                    new SQLiteParameter("@Id", stat.id)
                };
                _db.Query(updateSql, updateParams);
            }
            else
            {
                // Создаём новую запись
                string insertSql = @"INSERT INTO Statistic (user_id, app_id, total_time, last_launch_at)
                                VALUES (@UserId, @AppId, @TotalTime, @LastLaunchAt)";
                var insertParams = new[]
                {
                    new SQLiteParameter("@UserId", stat.user_id),
                    new SQLiteParameter("@AppId", stat.app_id),
                    new SQLiteParameter("@TotalTime", stat.total_time.ToString()),
                    new SQLiteParameter("@LastLaunchAt", stat.last_launch_at)
                };
                _db.Query(insertSql, insertParams);
            }
        }
    }
}
