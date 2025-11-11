using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Katastata.Data;

namespace Katastata
{
    public partial class App : Application
    {
        private const string DbFileName = "katastata.db";

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            // Создаём/инициируем БД (путь в каталоге с exe)
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbFileName);
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            // ensure db exist
            using (var ctx = new AppDbContext(options))
                ctx.Database.EnsureCreated();

            // Показываем окно авторизации как диалог — важно ShowDialog()
            var auth = new AuthWindow(options); // см. конструктор ниже
            var dialogResult = auth.ShowDialog();

            if (dialogResult == true)
            {
                // auth.LoggedInUserId должен быть установлен при успешной авторизации
                var mainWindow = new MainWindow(auth.LoggedInUserId, options);
                mainWindow.Show();
            }
            else
            {
                Shutdown(); // если пользователь закрыл/отказался
            }
        }
    }
}
