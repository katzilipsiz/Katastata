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

        public ObservableCollection<Program> Programs { get; } = new ObservableCollection<Program>();
        public RelayCommand ScanCommand { get; }

        public MainViewModel() { } // для дизайнера

        public MainViewModel(AppMonitorService service, int userId)
        {
            _service = service;
            _userId = userId;

            ScanCommand = new RelayCommand(_ => ScanPrograms());

            LoadPrograms();
        }

        private void ScanPrograms()
        {
            try
            {
                _service.ScanRunningPrograms(_userId);
                LoadPrograms();
                MessageBox.Show("Сканирование завершено");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void LoadPrograms()
        {
            Programs.Clear();
            var items = _service.GetAllPrograms(_userId);
            foreach (var p in items)
                Programs.Add(p);
        }
    }
}
