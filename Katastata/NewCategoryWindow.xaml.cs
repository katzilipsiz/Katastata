using System.Windows;

namespace Katastata
{
    public partial class NewCategoryWindow : Window
    {
        public string CategoryName => NameTextBox.Text;

        public NewCategoryWindow()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}