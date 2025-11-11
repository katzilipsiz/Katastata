using Katastata.Data;
using Katastata.UserControls;
using Katastata.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Threading;

namespace Katastata
{
    public partial class AuthWindow : Window
    {
        private readonly AppDbContext _dbContext;
        private readonly UserViewModel _viewModel;

        public int LoggedInUserId { get; private set; }

        public AuthWindow()
        {
            InitializeComponent();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=appdata.db")
                .Options;
            _dbContext = new AppDbContext(options);

            _viewModel = new UserViewModel(_dbContext);
            _viewModel.LoginSuccessful += OnLoginSuccessful;

            DataContext = _viewModel;


        }

        private void OnLoginSuccessful(int userId)
        {
            LoggedInUserId = userId;
            Dispatcher.Invoke(() =>
            {
                DialogResult = true;
                Close();
            });
        }

        public void ShowLoginPage(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new LoginPage();
        }

        public void ShowRegisterPage(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new RegisterPage();
        }

    }
}
