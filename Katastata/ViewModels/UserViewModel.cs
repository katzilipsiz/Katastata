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
        private readonly AppDbContext _db;

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        public string RegisterUsername { get; set; }
        public string RegisterPassword { get; set; }

        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }

        public event Action<int> LoginSuccessful;

        // Parameterless ctor for designer (optional)
        public UserViewModel() { }

        public UserViewModel(AppDbContext db)
        {
            _db = db;
            RegisterCommand = new RelayCommand(RegisterUser);
            LoginCommand = new RelayCommand(LoginUser);
        }

        private void RegisterUser(object obj)
        {
            if (string.IsNullOrWhiteSpace(RegisterUsername) || string.IsNullOrWhiteSpace(RegisterPassword))
            {
                MessageBox.Show("Введите имя и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_db.Users.Any(u => u.Username == RegisterUsername))
            {
                MessageBox.Show("Пользователь уже есть", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = new User
            {
                Username = RegisterUsername,
                PasswordHash = HashPassword(RegisterPassword),
                PCName = Environment.MachineName
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            // автологин после регистрации:
            LoginSuccessful?.Invoke(user.Id);
        }

        private void LoginUser(object obj)
        {
            if (string.IsNullOrWhiteSpace(LoginUsername) || string.IsNullOrWhiteSpace(LoginPassword))
            {
                MessageBox.Show("Введите имя и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _db.Users.FirstOrDefault(u => u.Username == LoginUsername);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.PasswordHash != HashPassword(LoginPassword))
            {
                MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoginSuccessful?.Invoke(user.Id);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var b = Encoding.UTF8.GetBytes(password);
            var h = sha.ComputeHash(b);
            return Convert.ToBase64String(h);
        }
    }
}
