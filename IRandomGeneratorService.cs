using System;

namespace random_experimental
{
    internal interface IRandomGeneratorService
    {
        event Action<int>? NumberGenerated;
        bool IsRunning { get; }
        void Start(int start, int end, int intervalMs = 100);
        void Stop();
    }
}
