using Katastata.Data;
using Katastata.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace Katastata.ViewModels
{
    public class UserViewModel
    {
        private readonly AppDbContext _db;

        public event Action<int> LoginSuccessful;

        public string RegisterUsername { get; set; }
        public string RegisterPassword { get; set; }

        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        public UserViewModel() { } // для дизайнера

        public UserViewModel(AppDbContext db)
        {
            _db = db;
            RegisterCommand = new RelayCommand(_ => RegisterUser());
            LoginCommand = new RelayCommand(_ => LoginUser());
        }

        private void RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(RegisterUsername) || string.IsNullOrWhiteSpace(RegisterPassword))
                return;

            if (_db.Users.Any(u => u.Username == RegisterUsername))
                return;

            var user = new User
            {
                Username = RegisterUsername,
                PasswordHash = HashPassword(RegisterPassword),
                PCName = Environment.MachineName
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            // После успешной регистрации логиним автоматически:
            LoginSuccessful?.Invoke(user.Id);
        }

        private void LoginUser()
        {
            var hash = HashPassword(LoginPassword);
            var user = _db.Users.FirstOrDefault(u => u.Username == LoginUsername && u.PasswordHash == hash);

            if (user == null) return;

            LoginSuccessful?.Invoke(user.Id);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? ""));
            return Convert.ToBase64String(hash);
        }
    }
}
