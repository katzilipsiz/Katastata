using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Katastata.UserControls;

namespace Katastata
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ApplyTheme(string themeName)
        {
            Application.Current.Resources.MergedDictionaries.Clear();

            var themeUri = new Uri($"Assets/Themes/{themeName}.xaml", UriKind.Relative);
            var themeDict = new ResourceDictionary { Source = themeUri };

            Application.Current.Resources.MergedDictionaries.Add(themeDict);
        }

        private void CloseButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }
    }
}