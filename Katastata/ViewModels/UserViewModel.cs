using Katastata.Data;
using Katastata.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Katastata.ViewModels
{
    public class UserViewModel
    {
        private readonly AppDbContext _dbContext;

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        // Поля для регистрации
        public string RegisterUsername { get; set; }
        public string RegisterPassword { get; set; }

        // Поля для логина
        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }

        public UserViewModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            RegisterCommand = new RelayCommand(RegisterUser);
            LoginCommand = new RelayCommand(LoginUser);
        }

        private void RegisterUser(object obj)
        {
            if (string.IsNullOrWhiteSpace(RegisterUsername) || string.IsNullOrWhiteSpace(RegisterPassword))
            {
                MessageBox.Show("Введите имя пользователя и пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_dbContext.Users.Any(u => u.Username == RegisterUsername))
            {
                MessageBox.Show("Такой пользователь уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var hashedPassword = HashPassword(RegisterPassword);

            var newUser = new User
            {
                Username = RegisterUsername,
                PasswordHash = hashedPassword,
                PCName = Environment.MachineName
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            MessageBox.Show("Регистрация успешна!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoginUser(object obj)
        {
            if (string.IsNullOrWhiteSpace(LoginUsername) || string.IsNullOrWhiteSpace(LoginPassword))
            {
                MessageBox.Show("Введите имя пользователя и пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Username == LoginUsername);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.PasswordHash != HashPassword(LoginPassword))
            {
                MessageBox.Show("Неверный пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show($"Добро пожаловать, {user.Username}!", "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
