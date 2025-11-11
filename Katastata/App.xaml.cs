using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Katastata.Data;

namespace Katastata
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=appdata.db")
                .Options;

            // Ensure DB exists
            using (var ctx = new AppDbContext(options))
            {
                ctx.Database.EnsureCreated();
            }

            // Open auth dialog
            var auth = new AuthWindow(options);
            bool? res = auth.ShowDialog();

            if (res == true)
            {
                int userId = auth.LoggedInUserId;
                var main = new MainWindow(userId, options);
                main.Show();
            }
            else
            {
                Current.Shutdown();
            }
        }
    }
}
