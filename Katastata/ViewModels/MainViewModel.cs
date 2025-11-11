using Katastata.Models;
using Katastata.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Katastata.ViewModels
{
    public class MainViewModel
    {
        private readonly AppMonitorService _monitorService;
        private readonly int _currentUserId;

        public ObservableCollection<Program> Programs { get; set; } = new();

        public RelayCommand ScanCommand { get; }

        // Конструктор для XAML (нужен для дизайнера)
        public MainViewModel()
        {
            // Не используется, но нужен для XAML-дизайнера, чтобы не падал
        }

        // Рабочий конструктор
        public MainViewModel(AppMonitorService service, int userId)
        {
            _monitorService = service;
            _currentUserId = userId;

            ScanCommand = new RelayCommand(_ => ScanPrograms());
            LoadPrograms();
        }

        private void ScanPrograms()
        {
            try
            {
                _monitorService.ScanRunningPrograms(_currentUserId);
                LoadPrograms();
                MessageBox.Show("Сканирование завершено");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сканировании: {ex.Message}");
            }
        }

        private void LoadPrograms()
        {
            Programs.Clear();
            var items = _monitorService.GetAllPrograms(_currentUserId);
            foreach (var p in items)
                Programs.Add(p);
        }
    }
}
