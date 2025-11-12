using Katastata.Models;
using System.Collections.ObjectModel;

namespace Katastata.ViewModels
{
    public class ProgramDetailsViewModel
    {
        public ObservableCollection<SessionViewModel> Sessions { get; } = new ObservableCollection<SessionViewModel>();
        public TimeSpan TotalTime { get; }
        public DateTime? LastLaunch { get; }

        public ProgramDetailsViewModel(List<Session> sessions, Statistics stat)
        {
            foreach (var session in sessions)
            {
                Sessions.Add(new SessionViewModel(session));
            }
            TotalTime = stat?.TotalTime ?? TimeSpan.Zero;
            LastLaunch = stat?.LastLaunch;
        }
    }
}