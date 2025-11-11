using System.Windows;
using Microsoft.EntityFrameworkCore;
using Katastata.Data;
using Katastata.Services;
using Katastata.ViewModels;

namespace Katastata
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _db;
        private readonly AppMonitorService _service;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(); // или ViewModelLocator, если ты его используешь
        }

        public MainWindow(int userId, DbContextOptions<AppDbContext> options)
        {
            InitializeComponent();

            _db = new AppDbContext(options);
            _service = new AppMonitorService(_db);

            DataContext = new MainViewModel(_service, userId);
        }

        private void ApplyTheme(string themePath)
        {
            var themeDict = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(themeDict);
        }

        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Assets/Themes/Light.xaml");

        }

        // Переключение на тёмную тему
        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Assets/Themes/Dark.xaml");

        }

        
    }
}
