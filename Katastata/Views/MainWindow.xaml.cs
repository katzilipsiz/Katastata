using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows;
using Katastata.Models;
using Katastata.Services;

namespace Katastata.Views
{
    public partial class MainWindow : Window
    {
        private DB_Operation _db;
        private StatisticService _statisticService;
        private ApplicationsService _applicationsService;
        private InstalledAppsService _installedAppsService;
        private int _userId = 1; // Временный ID пользователя


        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Инициализация сервисов
                _db = new DB_Operation();
                _statisticService = new StatisticService(_db);
                _applicationsService = new ApplicationsService(_db);
                _installedAppsService = new InstalledAppsService();

                // Загружаем данные при старте
                LoadStatistics();
                LoadApplicationsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка инициализации: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Загрузка статистики использования
        private void LoadStatistics()
        {
            if (_statisticService == null)
            {
                MessageBox.Show("Сервис статистики не инициализирован!", "Ошибка");
                return;
            }

            DateTime startDate, endDate;

            switch (PeriodComboBox.SelectedIndex)
            {
                case 0: // Последняя неделя
                    startDate = DateTime.Now.AddDays(-7);
                    endDate = DateTime.Now;
                    break;
                case 1: // Последний месяц
                    startDate = DateTime.Now.AddMonths(-1);
                    endDate = DateTime.Now;
                    break;
                default: // Всё время
                    startDate = DateTime.MinValue;
                    endDate = DateTime.Now;
                    break;
            }

            try
            {
                var topApps = _statisticService.GetTopApplications(_userId, startDate, endDate);
                var totalTime = _statisticService.GetTotalUsageTime(_userId, startDate, endDate);
                var launchCount = _statisticService.GetLaunchCount(_userId, startDate, endDate);

                TopAppsList.ItemsSource = topApps;
                TotalTimeText.Text = totalTime.ToString(@"hh\:mm\:ss");
                LaunchCountText.Text = launchCount.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка загрузки статистики: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Загрузка списка приложений из БД
        private void LoadApplicationsList()
        {
            try
            {
                var apps = _applicationsService.GetAllApplications();
                ApplicationsList.ItemsSource = apps;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка загрузки списка приложений: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Обработка изменения периода в ComboBox
        private void PeriodComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadStatistics();
        }

        // Кнопка: Сканировать программы ПК
        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            ScanAndSaveApplications();
        }

        // Сканирование и сохранение установленных программ
        private void ScanAndSaveApplications()
        {
            try
            {
                var appsFromPc = _installedAppsService.GetInstalledApplications();

                foreach (var app in appsFromPc)
                {
                    if (!_applicationsService.IsApplicationExists(app.title))
                    {
                        _applicationsService.AddApplication(app);
                        Debug.WriteLine($"Добавлена программа: {app.title}");
                    }
                    else
                    {
                        Debug.WriteLine($"Программа уже есть в БД: {app.title}");
                    }
                }

                MessageBox.Show(
                    "Сканирование завершено. Программы добавлены в базу данных.",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Обновляем список в интерфейсе
                LoadApplicationsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при сканировании: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
