using Katastata.Data;
using Katastata.Models;
using System;
using System.Linq;
using System.Security;
using System.Windows.Input;
using Katastata.Helpers;

namespace Katastata.ViewModels
{
    public class UserViewModel
    {
        private readonly AppDbContext _db;
        public event Action<int> LoginSuccessful;

        public string RegisterUsername { get; set; }
        public SecureString RegisterPassword { get; set; } = new SecureString();
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
            if (string.IsNullOrWhiteSpace(RegisterUsername) || RegisterPassword.Length == 0) return;
            if (_db.Users.Any(u => u.Username == RegisterUsername)) return;

            var passwordString = new System.Net.NetworkCredential(string.Empty, RegisterPassword).Password;
            var user = new User
            {
                Username = RegisterUsername,
                PasswordHash = PasswordHelper.HashPassword(passwordString),
                PCName = Environment.MachineName
            };
            _db.Users.Add(user);
            _db.SaveChanges();
            LoginSuccessful?.Invoke(user.Id);
        }

        private void LoginUser()
        {
            var passwordString = new System.Net.NetworkCredential(string.Empty, LoginPassword).Password;
            var hash = PasswordHelper.HashPassword(passwordString);
            var user = _db.Users.FirstOrDefault(u => u.Username == LoginUsername && u.PasswordHash == hash);
            if (user != null)
                LoginSuccessful?.Invoke(user.Id);
        }
    }
}