using System.Windows.Controls;

namespace Katastata.UserControls
{
    public partial class RegisterPage : UserControl
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.UserViewModel vm)
            {
                vm.RegisterPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
