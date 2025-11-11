using Katastata.Models;
using Katastata.Services;
using Katastata.ViewModels;
using System.Collections.ObjectModel;

public class MainViewModel : BaseViewModel
{
    private readonly AppMonitorService _monitorService;
    private readonly int _currentUserId;

    public ObservableCollection<Program> Programs { get; set; } = new();
    public RelayCommand ScanCommand { get; }

    public MainViewModel(AppMonitorService service, int userId)
    {
        _monitorService = service;
        _currentUserId = userId;

        ScanCommand = new RelayCommand(_ => ScanPrograms());
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
        foreach (var program in _monitorService.GetAllPrograms())
            Programs.Add(program);
    }
}
