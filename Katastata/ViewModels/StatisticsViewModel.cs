using Katastata.Models;
using System.Collections.ObjectModel;

namespace Katastata.ViewModels
{
    public class StatisticsViewModel
    {
        public ObservableCollection<StatisticsViewModelItem> Statistics { get; } = new ObservableCollection<StatisticsViewModelItem>();

        public StatisticsViewModel(List<Statistics> stats)
        {
            foreach (var stat in stats)
            {
                Statistics.Add(new StatisticsViewModelItem(stat));
            }
        }
    }

    public class StatisticsViewModelItem
    {
        public string ProgramName { get; set; }
        public TimeSpan TotalTime { get; set; }
        public DateTime? LastLaunch { get; set; }

        public StatisticsViewModelItem(Statistics stat)
        {
            ProgramName = stat.Program?.Name ?? "Unknown";
            TotalTime = stat.TotalTime;
            LastLaunch = stat.LastLaunch;
        }
    }
}