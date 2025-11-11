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

        public MainViewModel(AppMonitorService monitorService, int userId)
        {
            _monitorService = monitorService;
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
            catch (System.Exception ex)
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
