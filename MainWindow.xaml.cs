using System;
using System.Windows;

namespace random_experimental
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel? _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _vm?.Dispose();
        }
    }
}
