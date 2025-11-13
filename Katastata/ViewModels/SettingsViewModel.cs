using Katastata.Models;
using Katastata.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Katastata.ViewModels
{
    public class SettingsViewModel
    {
        private readonly AppMonitorService _service;
        private readonly int _userId;

        public List<Statistics> StatisticsList { get; set; }
        public List<Session> SessionsList { get; set; }

        public ObservableCollection<StatisticsViewModelItem> Statistics { get; } = new ObservableCollection<StatisticsViewModelItem>();
        public ObservableCollection<SessionViewModel> Sessions { get; } = new ObservableCollection<SessionViewModel>();
        public RelayCommand ExportExcelCommand { get; }
        public RelayCommand ExportWordCommand { get; }

        public SettingsViewModel(AppMonitorService service, int userId)
        {
            _service = service;
            _userId = userId;
            LoadData();
            ExportExcelCommand = new RelayCommand(_ => ExportToExcel());
            ExportWordCommand = new RelayCommand(_ => ExportToWord());
        }

        private void LoadData()
        {
            // Получаем списки из сервиса и сразу сохраняем в свойства для окон
            StatisticsList = _service.GetStatistics(_userId);
            SessionsList = _service.GetSessions(_userId);

            // Заполняем ObservableCollection для биндинга в интерфейсе
            Statistics.Clear();
            foreach (var stat in StatisticsList)
                Statistics.Add(new StatisticsViewModelItem(stat));

            Sessions.Clear();
            foreach (var session in SessionsList)
                Sessions.Add(new SessionViewModel(session));
        }


        public void ExportToExcel()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Excel files (*.xlsx)|*.xlsx", DefaultExt = "xlsx" };
            if (dialog.ShowDialog() == true)
            {
                _service.ExportStatisticsToExcel(_userId, dialog.FileName);
                System.Windows.MessageBox.Show("Экспорт завершен");
            }
        }

        public void ExportToWord()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Word files (*.docx)|*.docx", DefaultExt = "docx" };
            if (dialog.ShowDialog() == true)
            {
                _service.ExportStatisticsToWord(_userId, dialog.FileName);
                System.Windows.MessageBox.Show("Экспорт завершен");
            }
        }
    }
}