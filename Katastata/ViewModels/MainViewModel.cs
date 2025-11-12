using Katastata.Models;
using Katastata.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Katastata.ViewModels
{
    public class MainViewModel
    {
        private readonly AppMonitorService _service;
        private readonly int _userId;
        public AppMonitorService Service => _service;
        public int UserId => _userId;
        public ObservableCollection<Program> Programs { get; } = new ObservableCollection<Program>();
        public RelayCommand ScanCommand { get; }
        public RelayCommand ShowSessionsCommand { get; }
        public RelayCommand ShowStatisticsCommand { get; }
        public RelayCommand ExportStatisticsExcelCommand { get; }
        public RelayCommand ExportStatisticsWordCommand { get; }

        public MainViewModel() { }

        public MainViewModel(AppMonitorService service, int userId)
        {
            _service = service;
            _userId = userId;

            ScanCommand = new RelayCommand(_ => ScanPrograms());
            ShowSessionsCommand = new RelayCommand(_ => ShowSessions());
            ShowStatisticsCommand = new RelayCommand(_ => ShowStatistics());   

            ExportStatisticsExcelCommand = new RelayCommand(_ => ExportStatisticsExcel());
            ExportStatisticsWordCommand = new RelayCommand(_ => ExportStatisticsWord());

            LoadPrograms();
            _service.StartMonitoring(_userId);
        }

        // Сканировать программы
        private void ScanPrograms()
        {
            try
            {
                _service.ScanRunningPrograms(_userId);
                LoadPrograms();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        // Показ программ
        private void LoadPrograms()
        {
            Programs.Clear();
            var items = _service.GetAllPrograms(_userId);
            foreach (var p in items)
                Programs.Add(p);
        }

        // Показ сессий
        private void ShowSessions()
        {
            var sessions = _service.GetSessions(_userId);
            var sessionsWindow = new SessionsWindow(sessions);
            sessionsWindow.Show();
        }

        // Показ статистики
        private void ShowStatistics()
        {
            var stats = _service.GetStatistics(_userId);
            var statsWindow = new StatisticsWindow(stats);
            statsWindow.Show();
        }

        // Экспорт статистики в Excel
        private void ExportStatisticsExcel()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Excel files (*.xlsx)|*.xlsx", DefaultExt = "xlsx" };
            if (dialog.ShowDialog() == true)
            {
                _service.ExportStatisticsToExcel(_userId, dialog.FileName);
                System.Windows.MessageBox.Show("Экспорт завершен");
            }
        }

        // Экспорт статистики в Word
        private void ExportStatisticsWord()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Word files (.docx)|.docx", DefaultExt = "docx" };
            if (dialog.ShowDialog() == true)
            {
                _service.ExportStatisticsToExcel(_userId, dialog.FileName);
                System.Windows.MessageBox.Show("Экспорт завершен");
            }
        }
    }
}
