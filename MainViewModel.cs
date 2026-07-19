using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace random_experimental
{
    internal class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool _isRunning;
        private string _statusText = string.Empty;
        private string _resultText = string.Empty;

        public ICommand StartStopCommand { get; }
        public ICommand ResetCommand { get; }

        public SettingsViewModel Settings { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IRandomGeneratorService? _generator;
        private readonly IStatisticsService _statistics;

        public CountsViewModel CountsVM { get; }
        public LogViewModel LogVM { get; }

        public ReadOnlyObservableCollection<CountItemViewModel> Counts => CountsVM.Counts;

        public MainViewModel()
        {
            // Create shared Settings instance and pass into generator so it can read UseRandomShared
            var settings = new SettingsViewModel();
            var generator = new RandomGeneratorService(settings);
            var statistics = new StatisticsServiceImpl();
            // Initialize instance
            Settings = settings;
            StartStopCommand = new RelayCommand(_ => OnStartStop(null));
            ResetCommand = new RelayCommand(_ => OnReset(null));

            CountsVM = new CountsViewModel();
            LogVM = new LogViewModel();

            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            // forward log changes to update LogText binding
            LogVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(LogViewModel.Text)) OnPropertyChanged(nameof(LogText));
            };
            _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));

            bool isDesign = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
            if (!isDesign)
            {
                _generator.NumberGenerated += OnNumberGenerated;
            }
            else
            {
                // provide sample for designer
                CountsVM.InitializeRange(Settings.StartValue, Settings.EndValue);
                LogVM.Append("[Design preview]\n");
                ResultText = _statistics.ComputeChiSquareSummary(Settings.StartValue, Settings.EndValue, CountsVM.SnapshotCounts());
            }
        }

        // Legacy constructor kept for tests/mocking
        public MainViewModel(IRandomGeneratorService generator, IStatisticsService statistics)
        {
            Settings = new SettingsViewModel();
            StartStopCommand = new RelayCommand(_ => OnStartStop(null));
            ResetCommand = new RelayCommand(_ => OnReset(null));

            CountsVM = new CountsViewModel();
            LogVM = new LogViewModel();

            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            // forward log changes to update LogText binding
            LogVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(LogViewModel.Text)) OnPropertyChanged(nameof(LogText));
            };
            _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));

            bool isDesign = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
            if (!isDesign)
            {
                _generator.NumberGenerated += OnNumberGenerated;
            }
            else
            {
                // provide sample for designer
                CountsVM.InitializeRange(Settings.StartValue, Settings.EndValue);
                LogVM.Append("[Design preview]\n");
                ResultText = _statistics.ComputeChiSquareSummary(Settings.StartValue, Settings.EndValue, CountsVM.SnapshotCounts());
            }
        }

        public string StartStopButtonText
        {
            get
            {
                if (_isRunning) return "Pause";
                // if not running but data exists -> Resume
                if (CountsVM.Total > 0) return "Resume";
                return "Start";
            }
        }

        public string StatusText
        {
            get => _statusText;
            private set
            {
                if (_statusText == value) return;
                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        // Forwarded for XAML compatibility
        public string LogText => LogVM.Text;

        public string ResultText
        {
            get => _resultText;
            private set
            {
                if (_resultText == value) return;
                _resultText = value;
                OnPropertyChanged(nameof(ResultText));
            }
        }

        private void OnNumberGenerated(int number)
        {
            // Update counts and log, then recompute stats
            CountsVM.Increment(number);
            LogVM.Append($"Random Number: {number}\n");
            ResultText = StatisticsService.ComputeChiSquareSummary(Settings.StartValue, Settings.EndValue, CountsVM.SnapshotCounts());

            // Notify that StartStopButtonText may change (Resume vs Pause)
            OnPropertyChanged(nameof(StartStopButtonText));
            OnPropertyChanged(nameof(LogText));
        }

        private void OnStartStop(object? parameter)
        {
            if (!_isRunning)
            {
                // start or resume
                // initialize counts for the configured range if empty
                if (CountsVM.Total == 0)
                {
                    CountsVM.InitializeRange(Settings.StartValue, Settings.EndValue);
                    ResultText = _statistics.ComputeChiSquareSummary(Settings.StartValue, Settings.EndValue, CountsVM.SnapshotCounts());
                }

                _generator?.Start(Settings.StartValue, Settings.EndValue, 100);
                _isRunning = true;
                StatusText = "Running";
            }
            else
            {
                // pause
                _generator?.Stop();
                _isRunning = false;
                StatusText = "Paused";
            }
            OnPropertyChanged(nameof(StartStopButtonText));
        }

        private void OnReset(object? parameter)
        {
            if (_isRunning)
            {
                _generator?.Stop();
                _isRunning = false;
                OnPropertyChanged(nameof(StartStopButtonText));
            }

            CountsVM.Clear();
            LogVM.Clear();
            ResultText = string.Empty;
            StatusText = "Reset";
            OnPropertyChanged(nameof(LogText));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            if (_generator != null)
            {
                try { _generator.NumberGenerated -= OnNumberGenerated; } catch { }
                try { _generator.Stop(); } catch { }
            }
        }
    }
}
