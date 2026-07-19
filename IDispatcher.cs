using System;

namespace random_experimental
{
    internal interface IDispatcher
    {
        bool CheckAccess();
        void Invoke(Action action);
        void BeginInvoke(Action action);
    }
}
