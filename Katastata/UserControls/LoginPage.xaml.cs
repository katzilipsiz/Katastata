using System.Windows;
using System.Windows.Controls;

namespace Katastata.UserControls
{
    public partial class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();

            Loaded += UserControl_Loaded;
        }

        private void LoginPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.UserViewModel vm)
            {
                vm.LoginPassword = ((PasswordBox)sender).SecurePassword;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Focus();  
        }
    }
}
