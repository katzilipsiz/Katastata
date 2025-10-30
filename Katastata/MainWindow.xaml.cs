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
        public MainWindow()
        {
            InitializeComponent();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseSqlite("Data Source=katastata.db")
                            .Options;

            var dbContext = new AppDbContext(options);
            var viewModel = new MainViewModel(dbContext); // передаём только контекст

            DataContext = viewModel;
        }

    }
}
