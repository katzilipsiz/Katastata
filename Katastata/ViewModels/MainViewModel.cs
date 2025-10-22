using System.Collections.ObjectModel;
using System.Windows.Input;
using Katastata.Models;
using Katastata.Services;

namespace Katastata
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ApplicationService _applicationService;

        private string _title = "Katastata - Мониторинг времени в программах";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _statusMessage = "Готов к работе";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private Application? _selectedApplication;
        public Application? SelectedApplication
        {
            get => _selectedApplication;
            set
            {
                SetProperty(ref _selectedApplication, value);
                StatusMessage = value != null
                    ? $"Выбрано: {value.DisplayName}"
                    : "Готов к работе";
                OnPropertyChanged(nameof(IsApplicationSelected));
            }
        }

        public bool IsApplicationSelected => SelectedApplication != null;

        // Коллекция приложений для отображения
        public ObservableCollection<Application> Applications { get; } = new();

        // Команды
        public ICommand LoadApplicationsCommand { get; }
        public ICommand ToggleTrackingCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowStatisticsCommand { get; }

        public MainViewModel()
        {
            _applicationService = new ApplicationService();

            LoadApplicationsCommand = new RelayCommand(_ => LoadApplications());
            ToggleTrackingCommand = new RelayCommand(ToggleTracking, _ => SelectedApplication != null);
            ShowSettingsCommand = new RelayCommand(_ => ShowSettings());
            ShowStatisticsCommand = new RelayCommand(_ => ShowStatistics());

            // Загружаем данные при создании ViewModel
            LoadApplications();
        }

        private void LoadApplications()
        {
            Applications.Clear();
            var apps = _applicationService.GetSampleApplications();
            foreach (var app in apps)
            {
                Applications.Add(app);
            }

            StatusMessage = $"Загружено приложений: {Applications.Count}";
        }

        private void ToggleTracking(object? parameter)
        {
            if (SelectedApplication != null)
            {
                SelectedApplication.IsTracking = !SelectedApplication.IsTracking;
                StatusMessage = $"{SelectedApplication.FriendlyName} - " +
                              $"отслеживание {(SelectedApplication.IsTracking ? "включено" : "отключено")}";
            }
        }

        private void ShowSettings()
        {
            StatusMessage = "Открытие настроек...";
            // Здесь будет логика открытия окна настроек
        }

        private void ShowStatistics()
        {
            StatusMessage = "Открытие статистики...";
            // Здесь будет логика открытия статистики
        }
    }
}