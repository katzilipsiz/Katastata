using Katastata.Data;
using Katastata.UserControls;
using Katastata.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Katastata
{
    public partial class AuthWindow : Window
    {
        private readonly AppDbContext _db;
        private readonly UserViewModel _vm;
        public int LoggedInUserId { get; private set; } = -1;

        public AuthWindow(DbContextOptions<AppDbContext> options)
        {
            InitializeComponent();
            _db = new AppDbContext(options);
            _vm = new UserViewModel(_db);
            _vm.LoginSuccessful += OnLoginSuccessful;
            ShowLoginPage(null, null);
            DataContext = _vm;
        }

        private async void OnLoginSuccessful(int userId)
        {
            LoggedInUserId = userId;
            await PlaySuccessAnimationAsync();
            DialogResult = true;
            Close();
        }

        private async Task PlaySuccessAnimationAsync()
        {
            // Блокируем кнопки
            LoginButton.IsEnabled = false;
            RegisterButton.IsEnabled = false;

            // Отображаем оверлей
            SuccessOverlay.Visibility = Visibility.Visible;

            // Затухание содержимого
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            RootGrid.BeginAnimation(OpacityProperty, fadeOut);

            await Task.Delay(300);

            // Плавное появление надписи
            var fadeInOverlay = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(700));
            var fadeInText = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(700));

            SuccessOverlay.BeginAnimation(OpacityProperty, fadeInOverlay);
            SuccessText.BeginAnimation(OpacityProperty, fadeInText);

            await Task.Delay(2000);
        }

        private void ShowLoginPage(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new LoginPage { DataContext = _vm };
            HighlightActiveButton(LoginButton);
        }

        private void ShowRegisterPage(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new RegisterPage { DataContext = _vm };
            HighlightActiveButton(RegisterButton);
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void HighlightActiveButton(Button activeButton)
        {
            var activeBrush = (Brush)Application.Current.Resources["AccentBrush"];
            var inactiveBrush = (Brush)Application.Current.Resources["WindowBackgroundBrush"];
            var activeForeground = (Brush)Application.Current.Resources["AccentBrushActive"];
            var inactiveForeground = Brushes.LightGray;

            LoginButton.Background = inactiveBrush;
            RegisterButton.Background = inactiveBrush;
            LoginButton.Foreground = inactiveForeground;
            RegisterButton.Foreground = inactiveForeground;

            activeButton.Background = activeBrush;
            activeButton.Foreground = activeForeground;
        }
    }
}
