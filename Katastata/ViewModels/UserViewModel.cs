using Katastata.Data;
using Katastata.Helpers;
using Katastata.Models;
using System;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Input;

namespace Katastata.ViewModels
{
    public class UserViewModel
    {
        private readonly AppDbContext _db;
        public event Action<int> LoginSuccessful;

        public string RegisterUsername { get; set; }
        public SecureString RegisterPassword { get; set; } = new SecureString();
        public SecureString RegisterPasswordConfirm { get; set; } = new SecureString();
        public string LoginUsername { get; set; }
        public SecureString LoginPassword { get; set; } = new SecureString();

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        public UserViewModel() { }

        public UserViewModel(AppDbContext db)
        {
            _db = db;
            RegisterCommand = new RelayCommand(_ => RegisterUser());
            LoginCommand = new RelayCommand(_ => LoginUser());
        }

        private void RegisterUser()
        {
            var passwordString = new System.Net.NetworkCredential(string.Empty, RegisterPassword).Password;
            var confirmString = new System.Net.NetworkCredential(string.Empty, RegisterPasswordConfirm).Password;


            if (string.IsNullOrWhiteSpace(RegisterUsername))
            {
                MessageBox.Show("Введите имя пользователя.", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RegisterPassword.Length == 0)
            {
                MessageBox.Show("Введите пароль.", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RegisterUsername.Length < 3)
            {
                MessageBox.Show("Имя пользователя должно содержать минимум 3 символа.", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (passwordString.Length < 5)
            {
                MessageBox.Show("Пароль должен содержать минимум 5 символов.", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (passwordString != confirmString)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_db.Users.Any(u => u.Username == RegisterUsername))
            {
                MessageBox.Show("Такой пользователь уже существует.", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = new User
            {
                Username = RegisterUsername,
                PasswordHash = PasswordHelper.HashPassword(passwordString),
                PCName = Environment.MachineName
            };
            _db.Users.Add(user);
            _db.SaveChanges();

            MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            LoginSuccessful?.Invoke(user.Id);
        }

        private void LoginUser()
        {
            var passwordString = new System.Net.NetworkCredential(string.Empty, LoginPassword).Password;

            if (string.IsNullOrWhiteSpace(LoginUsername))
            {
                MessageBox.Show("Введите имя пользователя.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (LoginPassword.Length == 0)
            {
                MessageBox.Show("Введите пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var hash = PasswordHelper.HashPassword(passwordString);
            var user = _db.Users.FirstOrDefault(u => u.Username == LoginUsername && u.PasswordHash == hash);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show($"Добро пожаловать, {user.Username}!", "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);
            LoginSuccessful?.Invoke(user.Id);
        }
    }
}