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

            var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite("Data Source=katastata.db").Options;

            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated();
            }
        }
    }

}
