using Katastata.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Katastata.UserControls
{
    public partial class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is UserViewModel vm)
            {
                vm.LoginPassword = ((PasswordBox)sender).Password;
            }
        }

        private void OnLoginSuccess(int userId)
        {
            var mainWindow = new MainWindow(userId);
            mainWindow.Show();
            Window.GetWindow(this)?.Close();
        }

    }
}
