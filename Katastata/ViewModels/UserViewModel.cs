using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using Katastata.Models;
using Katastata.Data;

namespace Katastata.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        public UserViewModel(AppDbContext context)
        {
            _context = context;
            RegisterCommand = new RelayCommand(RegisterUser);
            LoginCommand = new RelayCommand(LoginUser);
        }

        // Регистрация
        public string RegisterUsername { get; set; }
        public string RegisterPassword { get; set; } // сюда будем сохранять пароль с PasswordBox
        public ICommand RegisterCommand { get; set; }

        // Вход
        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; } // сюда будем сохранять пароль с PasswordBox
        public ICommand LoginCommand { get; set; }

        public string ErrorMessage { get; set; }

        public User CurrentUser { get; private set; }

        private void RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(RegisterUsername) || string.IsNullOrWhiteSpace(RegisterPassword))
            {
                ErrorMessage = "Заполните все поля";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            if (_context.Users.Any(u => u.Username == RegisterUsername))
            {
                ErrorMessage = "Пользователь уже существует";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            var user = new User
            {
                Username = RegisterUsername,
                PasswordHash = HashPassword(RegisterPassword),
                PCName = Environment.MachineName
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            ErrorMessage = "Регистрация успешна!";
            OnPropertyChanged(nameof(ErrorMessage));
        }

        public event Action<User> LoginSucceeded;

        private void LoginUser()
        {
            var hash = HashPassword(LoginPassword);
            var user = _context.Users.FirstOrDefault(u => u.Username == LoginUsername && u.PasswordHash == hash);

            if (user == null)
            {
                ErrorMessage = "Неверный логин или пароль";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            CurrentUser = user;
            LoginSucceeded?.Invoke(user); // уведомляем MainViewModel

            ErrorMessage = $"Добро пожаловать, {user.Username}!";
            OnPropertyChanged(nameof(ErrorMessage));
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
