using System.ComponentModel;

namespace random_experimental
{
    internal class CountItemViewModel : INotifyPropertyChanged
    {
        private int _count;
        public int Number { get; }

        public int Count
        {
            get => _count;
            private set
            {
                if (_count == value) return;
                _count = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
        }

        public CountItemViewModel(int number)
        {
            Number = number;
            _count = 0;
        }

        public void Increment() => Count++;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
