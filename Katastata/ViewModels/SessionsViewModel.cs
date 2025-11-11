using Katastata.Models;
using System.Collections.ObjectModel;

namespace Katastata.ViewModels
{
    public class SessionsViewModel
    {
        public ObservableCollection<SessionViewModel> Sessions { get; } = new ObservableCollection<SessionViewModel>();

        public SessionsViewModel(List<Session> sessions)
        {
            foreach (var session in sessions)
            {
                Sessions.Add(new SessionViewModel(session));
            }
        }
    }
}