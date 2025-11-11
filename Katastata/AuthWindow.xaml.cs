using System.Windows;
using Microsoft.EntityFrameworkCore;
using Katastata.Data;
using Katastata.ViewModels;
using Katastata.UserControls;

namespace Katastata
{
    public partial class AuthWindow : Window
    {
        private readonly AppDbContext _db;
        private readonly UserViewModel _vm;

        public int LoggedInUserId { get; private set; }

        public AuthWindow(DbContextOptions<AppDbContext> options)
        {
            InitializeComponent();

            _db = new AppDbContext(options);
            _vm = new UserViewModel(_db);

            // Когда VM вызывает событие — окно реагирует
            _vm.LoginSuccessful += OnLoginSuccessful;

            // Показываем Login по умолчанию
            ShowLoginPage(null, null);
        }

        private void OnLoginSuccessful(int userId)
        {
            LoggedInUserId = userId;
            // Закрываем окно как диалог (в UI-потоке)
            Dispatcher.Invoke(() =>
            {
                DialogResult = true;
                Close();
            });
        }

        private void ShowLoginPage(object sender, RoutedEventArgs e)
        {
            var page = new LoginPage();
            page.DataContext = _vm;
            ContentArea.Content = page;
        }

        private void ShowRegisterPage(object sender, RoutedEventArgs e)
        {
            var page = new RegisterPage();
            page.DataContext = _vm;
            ContentArea.Content = page;
        }
    }
}
