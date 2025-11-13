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
        public RelayCommand CreateCategoryCommand { get; }
        public RelayCommand OpenSettingsCommand { get; }
        public MainViewModel() { }

        public MainViewModel(AppMonitorService service, int userId)
        {
            _service = service;
            _userId = userId;

            ScanCommand = new RelayCommand(_ => ScanPrograms());
            ShowSessionsCommand = new RelayCommand(_ => ShowSessions());
            ShowStatisticsCommand = new RelayCommand(_ => ShowStatistics());
            CreateCategoryCommand = new RelayCommand(_ => CreateNewCategory());

            ExportStatisticsExcelCommand = new RelayCommand(_ => ExportStatisticsExcel());
            ExportStatisticsWordCommand = new RelayCommand(_ => ExportStatisticsWord());

            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());

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

        // Создание новой категории
        private void CreateNewCategory()
        {
            var newCategoryWindow = new NewCategoryWindow();
            if (newCategoryWindow.ShowDialog() == true)
            {
                var name = newCategoryWindow.CategoryName;
                if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
                {
                    System.Windows.MessageBox.Show("Имя категории должно быть минимум 3 символа.");
                    return;
                }
                if (_service.CategoryExists(name))
                {
                    System.Windows.MessageBox.Show("Категория уже существует.");
                    return;
                }
                _service.AddCategory(name);
                LoadPrograms();  // Обновить список
                System.Windows.MessageBox.Show("Категория создана.");
            }
        }
        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow(_service, _userId);
            settingsWindow.ShowDialog();
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
