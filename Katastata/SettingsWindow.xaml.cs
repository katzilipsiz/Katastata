using Katastata.Services;
using System.Windows;
using System.Windows.Media;
using Katastata.ViewModels;

namespace Katastata
{
    public partial class SettingsWindow : Window
    {
        private readonly AppMonitorService _service;
        private readonly int _userId;

        public SettingsWindow(AppMonitorService service, int userId)
        {
            InitializeComponent();
            _service = service;
            _userId = userId;
            DataContext = new SettingsViewModel(service, userId);
        }

        private void CreateCategory_Click(object sender, RoutedEventArgs e)
        {
            var name = NewCategoryName.Text;
            if (string.IsNullOrWhiteSpace(name)) return;
            _service.AddCategory(name);
            System.Windows.MessageBox.Show("Категория создана");
        }

        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Assets/Themes/Light.xaml");
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Assets/Themes/Dark.xaml");
        }

        private void ApplyTheme(string themePath)
        {
            var themeDict = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
            System.Windows.Application.Current.Resources.MergedDictionaries.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(themeDict);
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Удалить аккаунт? Это необратимо.", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _service.DeleteUser(_userId);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var choiceWindow = new ExportWindow();
            choiceWindow.Owner = this;
            if (choiceWindow.ShowDialog() == true)
            {
                var vm = (SettingsViewModel)DataContext;
                if (choiceWindow.SelectedFormat == "Excel")
                    vm.ExportToExcel();
                else if (choiceWindow.SelectedFormat == "Word")
                    vm.ExportToWord();
            }
        }

        private void OpenStatistics_Click(object sender, RoutedEventArgs e)
        {
            var stats = (this.DataContext as SettingsViewModel)?.StatisticsList;
            if (stats != null && stats.Count > 0)
            {
                var statisticsWindow = new StatisticsWindow(stats)
                {
                    Owner = this
                };
                statisticsWindow.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Данные статистики отсутствуют.");
            }
        }

        private void OpenSessions_Click(object sender, RoutedEventArgs e)
        {
            var sessions = (this.DataContext as SettingsViewModel)?.SessionsList;
            if (sessions != null && sessions.Count > 0)
            {
                var sessionsWindow = new SessionsWindow(sessions)
                {
                    Owner = this
                };
                sessionsWindow.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Данные сессий отсутствуют.");
            }
        }



    }
}