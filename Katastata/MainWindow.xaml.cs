using Katastata.Data;
using Katastata.Models;
using Katastata.Services;
using Katastata.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;  // Для tray
using System.Windows.Input;
using System.Windows.Media;

namespace Katastata
{
    public partial class MainWindow : Window
    {
        private NotifyIcon trayIcon;
        private bool isFullscreen = false;

        public MainWindow(int userId, DbContextOptions<AppDbContext> options)
        {
            InitializeComponent();
            var db = new AppDbContext(options);
            var service = new AppMonitorService(db);
            DataContext = new MainViewModel(service, userId);
            HighlightActiveTheme("Dark");

            trayIcon = new NotifyIcon();
            trayIcon.Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Assets/Logo/app.ico")).Stream);
            trayIcon.Text = "Katastata";
            trayIcon.Visible = false;


            // Контекстное меню для выхода
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Показать", null, (s, e) => TrayIcon_Click());
            contextMenu.Items.Add("Выход", null, (s, e) => System.Windows.Application.Current.Shutdown());
            trayIcon.ContextMenuStrip = contextMenu;
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
            System.Windows.Application.Current.Resources.MergedDictionaries.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(themeDict);
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
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Не удалось перезапустить приложение: " + ex.Message);
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

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true; // Отменяем закрытие
            this.Hide(); // Скрываем окно
            trayIcon.Visible = true; // Показываем значок
        }

        private void TrayIcon_Click()
        {
            this.Show(); // Восстанавливаем окно
            this.WindowState = WindowState.Normal;
            trayIcon.Visible = false; // Скрываем значок
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
            System.Windows.Media.Brush accent = (System.Windows.Media.Brush)System.Windows.Application.Current.Resources["AccentBrushActive"];
            System.Windows.Media.Brush normal = System.Windows.Media.Brushes.Transparent;
            // Сброс фона
            LightThemeBtn.Background = normal;
            DarkThemeBtn.Background = normal;
            // Подсветка активной
            if (activeTheme == "Light")
                LightThemeBtn.Background = accent;
            else if (activeTheme == "Dark")
                DarkThemeBtn.Background = accent;
        }

        private void ScannerBtn_Click(object sender, RoutedEventArgs e)
        {
            ScanText.Text = "Сканирование завершено!";
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            var choiceWindow = new ExportWindow();
            choiceWindow.Owner = this; // модально
            if (choiceWindow.ShowDialog() == true)
            {
                if (DataContext is MainViewModel vm)
                {
                    if (choiceWindow.SelectedFormat == "Excel")
                        vm.ExportStatisticsExcelCommand.Execute(null);
                    else if (choiceWindow.SelectedFormat == "Word")
                        vm.ExportStatisticsWordCommand.Execute(null);
                }
            }
        }

        private void ProgramTile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Program program)
            {
                var vm = (MainViewModel)DataContext;
                var detailsWindow = new ProgramDetailsWindow(program, vm.UserId, vm.Service);
                detailsWindow.Show();
            }
        }
    }
}