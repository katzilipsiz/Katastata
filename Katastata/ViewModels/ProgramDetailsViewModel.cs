using Katastata.Models;
using System.Collections.ObjectModel;

namespace Katastata.ViewModels
{
    public class ProgramDetailsViewModel
    {
        public string ProgramName { get; }
        public string ProgramPath { get; }
        public ObservableCollection<SessionViewModel> Sessions { get; } = new ObservableCollection<SessionViewModel>();
        public TimeSpan TotalTime { get; }
        public DateTime? LastLaunch { get; }

        public ProgramDetailsViewModel(Program program, List<Session> sessions, Statistics stat)
        {
            ProgramName = program.Name ?? "Unknown";
            ProgramPath = program.Path ?? "";
            foreach (var session in sessions)
            {
                Sessions.Add(new SessionViewModel(session));
            }
            TotalTime = stat?.TotalTime ?? TimeSpan.Zero;
            LastLaunch = stat?.LastLaunch;
        }
    }
}