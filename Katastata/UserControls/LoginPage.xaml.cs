using System.Windows.Controls;
using Katastata.ViewModels;

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
    }
}
