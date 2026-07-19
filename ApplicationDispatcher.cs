using System;
using System.Windows.Threading;
using System.Windows;

namespace random_experimental
{
    internal class ApplicationDispatcher : IDispatcher
    {
        private Dispatcher? Dispatcher => Application.Current?.Dispatcher;

        public bool CheckAccess()
        {
            var d = Dispatcher;
            return d == null || d.CheckAccess();
        }

        public void Invoke(Action action)
        {
            var d = Dispatcher;
            if (d == null || d.CheckAccess()) action();
            else d.Invoke(action);
        }

        public void BeginInvoke(Action action)
        {
            var d = Dispatcher;
            if (d == null || d.CheckAccess()) action();
            else d.BeginInvoke(action);
        }
    }
}
