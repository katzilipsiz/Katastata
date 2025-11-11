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

        public int LoggedInUserId { get; private set; } = -1;

        public AuthWindow(DbContextOptions<AppDbContext> options)
        {
            InitializeComponent();

            _db = new AppDbContext(options);
            _vm = new UserViewModel(_db);

            // Подписываемся на событие LoginSuccessful — VM вызовет его при успехе
            _vm.LoginSuccessful += OnLoginSuccessful;

            // Показываем страницу логина по умолчанию:
            ShowLoginPage(null, null);

            DataContext = _vm;
        }

        private void OnLoginSuccessful(int userId)
        {
            // Отладочное сообщение — если понадобится
            Dispatcher.Invoke(() =>
            {
                LoggedInUserId = userId;
                // Устанавливаем DialogResult = true только когда окно открыто как диалог
                DialogResult = true; // <-- должно сработать только из ShowDialog
                Close();
            });
        }

        // Методы переключения контента — убедись, что в XAML есть ContentControl с x:Name="ContentArea"
        private void ShowLoginPage(object sender, RoutedEventArgs e)
        {
            var login = new LoginPage();
            login.DataContext = _vm;
            ContentArea.Content = login;
        }

        private void ShowRegisterPage(object sender, RoutedEventArgs e)
        {
            var reg = new RegisterPage();
            reg.DataContext = _vm;
            ContentArea.Content = reg;
        }
    }
}
