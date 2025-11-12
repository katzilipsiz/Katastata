using Katastata.Models;
using Katastata.Services;
using Katastata.ViewModels;
using System.Windows;

namespace Katastata
{
    public partial class ProgramDetailsWindow : Window
    {
        public ProgramDetailsWindow(Program program, int userId, AppMonitorService service)
        {
            InitializeComponent();
            var sessions = service.GetSessions(userId).Where(s => s.ProgramId == program.Id).ToList();
            var stat = service.GetStatistics(userId).FirstOrDefault(st => st.ProgramId == program.Id);
            DataContext = new ProgramDetailsViewModel(sessions, stat);
        }
    }
}