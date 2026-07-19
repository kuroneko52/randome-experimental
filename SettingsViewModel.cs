using System.ComponentModel;

namespace random_experimental
{
    internal class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly RangeSettings _model;

        public event PropertyChangedEventHandler? PropertyChanged;

        // デフォルトで Random.Shared を有効にする
        private bool _useRandomShared = true;
        public bool UseRandomShared
        {
            get => _useRandomShared;
            set
            {
                if (_useRandomShared == value) return;
                _useRandomShared = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseRandomShared)));
            }
        }

        public SettingsViewModel() : this(new RangeSettings()) { }

        public SettingsViewModel(RangeSettings model)
        {
            _model = model;
            _model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(RangeSettings.StartValue) || e.PropertyName == nameof(RangeSettings.EndValue))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartValue)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndValue)));
                }
            };
        }

        public int StartValue
        {
            get => _model.StartValue;
            set
            {
                if (_model.StartValue == value) return;
                _model.StartValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartValue)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartText)));
            }
        }

        public int EndValue
        {
            get => _model.EndValue;
            set
            {
                if (_model.EndValue == value) return;
                _model.EndValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndValue)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndText)));
            }
        }

        public string StartText
        {
            get => _model.StartText;
            set { _model.StartText = value; }
        }

        public string EndText
        {
            get => _model.EndText;
            set { _model.EndText = value; }
        }

        public bool IsStartNumberChangeEnabled
        {
            get => _model.IsStartNumberChangeEnabled;
            set { _model.IsStartNumberChangeEnabled = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsStartNumberChangeEnabled))); }
        }
    }
}
