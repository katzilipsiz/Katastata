using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.EntityFrameworkCore;
using Katastata.Models;
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
        private readonly AppDbContext _dbContext;
        private readonly AppMonitorService _service;
        private readonly int _currentUserId;

        public MainWindow()
        {
            InitializeComponent();

            
        }

        private void AuthClick(object sender, RoutedEventArgs e)
        {
            var loginWindow = new AuthWindow();
            if (loginWindow.ShowDialog() == true)
            {
                MessageBox.Show("Вы вошли в систему!", "Katastata");
            }

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
