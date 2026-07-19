using System;
using System.Threading;
using System.Threading.Tasks;

namespace random_experimental
{
    internal class RandomGeneratorService : IRandomGeneratorService
    {
        private CancellationTokenSource? _cts;
        public event Action<int>? NumberGenerated;
        private readonly SettingsViewModel? _settings;

        public bool IsRunning => _cts != null && !_cts.IsCancellationRequested;

        public RandomGeneratorService() : this(null) { }

        // settings を注入すると、生成時に UseRandomShared の値を参照できます
        public RandomGeneratorService(SettingsViewModel? settings)
        {
            _settings = settings;
        }

        public void Start(int start, int end, int intervalMs = 100)
        {
            if (start > end) throw new ArgumentException("start must be <= end", nameof(start));
            if (intervalMs < 0) throw new ArgumentOutOfRangeException(nameof(intervalMs));
            if (IsRunning) return;

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    // Determine which RNG to use at start. If dynamic switching is required
                    // during runtime, change this logic to read _settings.UseRandomShared inside the loop.
                    bool useShared = _settings?.UseRandomShared ?? true;
                    Random? localRand = null;
                    if (!useShared) localRand = new Random();

                    while (!token.IsCancellationRequested)
                    {
                        int n = (localRand ?? Random.Shared).Next(start, end + 1);
                        NumberGenerated?.Invoke(n);
                        await Task.Delay(intervalMs, token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    // stopped
                }
                finally
                {
                    try { _cts?.Dispose(); } catch { }
                    _cts = null;
                }
            }, token);
        }

        public void Stop()
        {
            try
            {
                _cts?.Cancel();
            }
            finally
            {
                try { _cts?.Dispose(); } catch { }
                _cts = null;
            }
        }
    }
}
