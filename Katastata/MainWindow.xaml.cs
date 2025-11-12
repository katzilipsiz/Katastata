using Katastata.Data;
using Katastata.Services;
using Katastata.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace Katastata
{
    public partial class MainWindow : Window
    {
        private bool isFullscreen = false;

        public MainWindow(int userId, DbContextOptions<AppDbContext> options)
        {
            InitializeComponent();

            var db = new AppDbContext(options);
            var service = new AppMonitorService(db);
            DataContext = new MainViewModel(service, userId);

            HighlightActiveTheme("Dark");
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
            HighlightActiveTheme("Light");

        }

        // Переключение на тёмную тему
        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Assets/Themes/Dark.xaml");
            HighlightActiveTheme("Dark");

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

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FullscreenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!isFullscreen)
            {
                // Сохраняем текущие размеры и положение, если нужно
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                FullscreenBtn.Content = "🗗"; // символ выхода из полноэкрана
                FullscreenBtn.ToolTip = "Выйти из полноэкранного режима";
                isFullscreen = true;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.None; // чтобы осталась твоя кастомная шапка
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
                FullscreenBtn.Content = "🗖"; // символ входа в полноэкран
                FullscreenBtn.ToolTip = "Полноэкранный режим";
                isFullscreen = false;
            }
        }

        private void HighlightActiveTheme(string activeTheme)
        {
            var accent = (Brush)Application.Current.Resources["AccentBrushActive"];
            var normal = Brushes.Transparent;

            // Сброс фона
            LightThemeBtn.Background = normal;
            DarkThemeBtn.Background = normal;

            // Подсветка активной
            if (activeTheme == "Light")
                LightThemeBtn.Background = accent;
            else if (activeTheme == "Dark")
                DarkThemeBtn.Background = accent;
        }

    }
}
