using Katastata.Data;
using Katastata.Models;
using Katastata.Services;
using Katastata.UserControls;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Katastata.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private UserControl _currentPage;
        public UserControl CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(nameof(CurrentPage)); }
        }

        public RelayCommand ShowLoginCommand { get; set; }
        public RelayCommand ShowRegisterCommand { get; set; }

        public ObservableCollection<Program> Programs { get; set; } = new ObservableCollection<Program>();
        public ObservableCollection<SessionViewModel> Sessions { get; set; } = new ObservableCollection<SessionViewModel>();

        private readonly AppMonitorService _monitorService;
        private readonly int _currentUserId = 1; // временно фиксируем пользователя

        public ICommand ScanCommand { get; set; }

        public MainViewModel(AppMonitorService monitorService)
        {
            _monitorService = monitorService;

            // Команды переключения страниц
            ShowLoginCommand = new RelayCommand(ShowLogin);
            ShowRegisterCommand = new RelayCommand(ShowRegister);

            // По умолчанию показываем Login
            ShowLogin();

            // Команда сканирования
            ScanCommand = new RelayCommand(ScanPrograms);

            // Загружаем начальные данные
            LoadPrograms();
            LoadSessions();
        }

        private void ShowLogin()
        {
            CurrentPage = new LoginPage
            {
                DataContext = new UserViewModel(_monitorService.GetDbContext())
            };
        }

        private void ShowRegister()
        {
            CurrentPage = new RegisterPage
            {
                DataContext = new UserViewModel(_monitorService.GetDbContext())
            };
        }
        private void ScanPrograms()
        {
            // 1. Сканируем программы
            _monitorService.ScanRunningPrograms(_currentUserId);

            // 2. Обновляем сессии (создаём новые сессии для текущих программ)
            foreach (var prog in _monitorService.GetAllPrograms(_currentUserId))
            {
                _monitorService.StartProgramSession(_currentUserId, prog.Path);
            }

            // 3. Обновляем коллекцию программ для DataGrid
            LoadPrograms();

            // 4. Обновляем коллекцию сессий для DataGrid
            LoadSessions();
        }

        private void LoadPrograms()
        {
            Programs.Clear();
            foreach (var prog in _monitorService.GetAllPrograms(_currentUserId))
            {
                Programs.Add(prog);
            }
        }

        private void LoadSessions()
        {
            Sessions.Clear();
            var userSessions = _monitorService.GetUserSessions(_currentUserId);
            foreach (var session in userSessions)
            {
                Sessions.Add(new SessionViewModel(session));
            }
        }
    }
}
