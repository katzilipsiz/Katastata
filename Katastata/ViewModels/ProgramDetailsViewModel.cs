using Katastata.Models;
using Katastata.Services;
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

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();
        public Category SelectedCategory { get; set; }
        private readonly AppMonitorService _service;
        private readonly Program _program;

        public RelayCommand SaveCategoryCommand { get; }

        public ProgramDetailsViewModel(Program program, List<Session> sessions, Statistics stat, AppMonitorService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));  // Валидация
            _program = program ?? throw new ArgumentNullException(nameof(program));

            ProgramName = program.Name ?? "Unknown";
            ProgramPath = program.Path ?? "";

            foreach (var session in sessions)
            {
                Sessions.Add(new SessionViewModel(session));
            }
            TotalTime = stat?.TotalTime ?? TimeSpan.Zero;
            LastLaunch = stat?.LastLaunch;

            LoadCategories();
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == program.CategoryId);

            SaveCategoryCommand = new RelayCommand(_ => SaveCategory());
        }

        private void LoadCategories()
        {
            Categories.Clear();
            var cats = _service.GetAllCategories();
            foreach (var cat in cats)
                Categories.Add(cat);
        }

        public void SaveCategory()
        {
            if (SelectedCategory != null)
            {
                _program.CategoryId = SelectedCategory.Id;
                _service.UpdateProgram(_program);
            }
        }
    }
}