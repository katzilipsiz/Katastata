using System.Windows;
using Microsoft.EntityFrameworkCore;
using Katastata.Data;
using Katastata.Services;
using Katastata.ViewModels;


namespace Katastata
{
    public partial class MainWindow : Window
    {
        public MainWindow(int userId, DbContextOptions<AppDbContext> options)
        {
            InitializeComponent();

            var db = new AppDbContext(options);
            var service = new AppMonitorService(db);
            DataContext = new MainViewModel(service, userId);
        }

        // Для дизайнера оставим пустой ctor
        public MainWindow()
        {
            InitializeComponent();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseSqlite("Data Source=katastata.db")
                            .Options;

            var dbContext = new AppDbContext(options);
            var viewModel = new AppMonitorService(dbContext); // передаём только контекст

            DataContext = viewModel;
        }

        private void ApplyTheme(string themePath)
        {
            var themeDict = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(themeDict);
        }

        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Assets/Themes/Light.xaml");

        }

        // Переключение на тёмную тему
        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Assets/Themes/Dark.xaml");

        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"\"{exePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true, // скрываем окно CMD
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                };

                System.Diagnostics.Process.Start(startInfo);

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось перезапустить приложение: " + ex.Message);
            }
        }

    }
}
