using System.Text;

namespace random_experimental
{
    internal class LogViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private readonly StringBuilder _sb = new StringBuilder();

        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            private set
            {
                if (_text == value) return;
                _text = value;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Text)));
            }
        }

        public void Append(string line)
        {
            _sb.Append(line);
            Text = _sb.ToString();
        }

        public void Clear()
        {
            _sb.Clear();
            Text = string.Empty;
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    }
}
