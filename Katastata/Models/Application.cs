using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Katastata.Models
{
    public class Application : INotifyPropertyChanged
    {
        private string _processName = string.Empty;
        private string _friendlyName = string.Empty;
        private string _windowTitle = string.Empty;
        private bool _isTracking = true;
        private string? _iconPath;

        public int Id { get; set; }

        public string ProcessName
        {
            get => _processName;
            set
            {
                _processName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string FriendlyName
        {
            get => _friendlyName;
            set
            {
                _friendlyName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged();
            }
        }

        public bool IsTracking
        {
            get => _isTracking;
            set
            {
                _isTracking = value;
                OnPropertyChanged();
            }
        }

        public string? IconPath
        {
            get => _iconPath;
            set
            {
                _iconPath = value;
                OnPropertyChanged();
            }
        }

        // Для отображения в UI - вычисляемое свойство
        public string DisplayName => string.IsNullOrEmpty(FriendlyName)
            ? ProcessName
            : $"{FriendlyName} ({ProcessName})";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}