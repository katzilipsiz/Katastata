using Katastata.Data;
using Katastata.Helpers;
using Katastata.Models;
using Microsoft.EntityFrameworkCore;
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

namespace Katastata
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private readonly AppDbContext _db;
        public AuthWindow()
        {
            InitializeComponent();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=katastata.db")
                .Options;

            _db = new AppDbContext(options);


        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Введите имя и пароль");
                return;
            }

            var exists = await _db.Users.AnyAsync(u => u.Username == username);
            if (exists)
            {
                ShowError("Такой пользователь уже существует");
                return;
            }

            var user = new User
            {
                Username = username,
                PasswordHash = PasswordHelper.HashPassword(password),
                PCName = Environment.MachineName
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            MessageBox.Show("Пользователь зарегистрирован!", "Katastata");
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string hash = PasswordHelper.HashPassword(password);

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hash);

            if (user == null)
            {
                ShowError("Неверное имя или пароль");
                return;
            }

            // Можно передать инфу о вошедшем пользователе владельцу окна или сохранить в статическое свойство приложения
            MessageBox.Show($"Добро пожаловать, {user.Username}!", "Katastata");
            DialogResult = true;
            Close();
        }

        private void ShowError(string text)
        {
            ErrorText.Text = text;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
