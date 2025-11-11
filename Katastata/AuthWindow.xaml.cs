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
            _vm.LoginSuccessful += OnLoginSuccessful;
            ShowLoginPage(null, null);
            DataContext = _vm;
        }

        private void OnLoginSuccessful(int userId)
        {
            Dispatcher.Invoke(() =>
            {
                LoggedInUserId = userId;
                DialogResult = true;
                Close();
            });
        }

        private void ShowLoginPage(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new LoginPage { DataContext = _vm };
        }

        private void ShowRegisterPage(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new RegisterPage { DataContext = _vm };
        }
    }
}
