using System.Collections.ObjectModel;
using System.Windows.Input;
using Katastata.Models;
using Katastata.Services;

namespace Katastata.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly AppMonitorService _monitorService;
        private readonly int _currentUserId = 1; // временно фиксируем пользователя

        public ObservableCollection<Program> Programs { get; set; }

        public ICommand ScanCommand { get; set; }

        public MainViewModel(AppMonitorService monitorService)
        {
            _monitorService = monitorService;
            Programs = new ObservableCollection<Program>();

            ScanCommand = new RelayCommand(ScanPrograms);

            LoadPrograms();
        }

        private void ScanPrograms()
        {
            _monitorService.ScanRunningPrograms(_currentUserId);
            LoadPrograms();
        }

        private void LoadPrograms()
        {
            Programs.Clear();
            foreach (var prog in _monitorService.GetAllPrograms(_currentUserId))
            {
                Programs.Add(prog);
            }
        }
    }
}
