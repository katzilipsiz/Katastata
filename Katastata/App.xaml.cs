using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Katastata.Data;

namespace Katastata
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=appdata.db")
                .Options;

            using var db = new AppDbContext(options);
            db.Database.EnsureCreated();

            // Открываем страницу логина
            var loginWindow = new AuthWindow();
            loginWindow.Show();
        }

    }

}
