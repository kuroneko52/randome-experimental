using System.ComponentModel;

namespace random_experimental
{
    internal class RangeSettings : INotifyPropertyChanged
    {
        private int _startValue = 1;
        private int _endValue = 6;
        private bool _isStartNumberChangeEnabled = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int StartValue
        {
            get => _startValue;
            set
            {
                if (_startValue == value) return;
                _startValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartValue)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartText)));
            }
        }

        public int EndValue
        {
            get => _endValue;
            set
            {
                if (_endValue == value) return;
                _endValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndValue)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndText)));
            }
        }

        public string StartText
        {
            get => _startValue.ToString();
            set
            {
                if (int.TryParse(value, out var v)) StartValue = v;
            }
        }

        public string EndText
        {
            get => _endValue.ToString();
            set
            {
                if (int.TryParse(value, out var v)) EndValue = v;
            }
        }

        public bool IsStartNumberChangeEnabled
        {
            get => _isStartNumberChangeEnabled;
            set
            {
                if (_isStartNumberChangeEnabled == value) return;
                _isStartNumberChangeEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsStartNumberChangeEnabled)));
            }
        }
    }
}
