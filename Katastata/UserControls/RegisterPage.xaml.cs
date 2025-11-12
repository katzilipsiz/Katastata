using Katastata.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Katastata.UserControls
{
    public partial class RegisterPage : UserControl
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.UserViewModel vm)
            {
                vm.RegisterPassword = ((PasswordBox)sender).SecurePassword;
            }
        }

        private void RegisterPasswordConfirmBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserViewModel vm)
                vm.RegisterPasswordConfirm = (sender as PasswordBox)?.SecurePassword;
        }

    }
}