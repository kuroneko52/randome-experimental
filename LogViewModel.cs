using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace random_experimental
{
    internal class LogViewModel : INotifyPropertyChanged
    {
        private const int MaxLines = 100;
        private readonly Queue<string> _lines = new Queue<string>(MaxLines);
        private string _text = string.Empty;

        public string Text
        {
            get => _text;
            private set
            {
                if (_text == value) return;
                _text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        public void Append(string s)
        {
            if (string.IsNullOrEmpty(s)) return;

            var parts = s.Split(new[] { '\n' }, StringSplitOptions.None);
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                bool endsWithNewline = (i < parts.Length - 1) || s.EndsWith("\n", StringComparison.Ordinal);
                var line = part + (endsWithNewline ? "\n" : string.Empty);
                _lines.Enqueue(line);
                if (_lines.Count > MaxLines) _lines.Dequeue();
            }

            var sb = new StringBuilder();
            foreach (var line in _lines) sb.Append(line);
            Text = sb.ToString();
        }

        public void Clear()
        {
            _lines.Clear();
            Text = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
