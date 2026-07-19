using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;

namespace random_experimental
{
    internal class CountsViewModel
    {
        private readonly ObservableCollection<CountItemViewModel> _counts = new ObservableCollection<CountItemViewModel>();
        private readonly ConcurrentDictionary<int, CountItemViewModel> _lookup = new ConcurrentDictionary<int, CountItemViewModel>();

        public ReadOnlyObservableCollection<CountItemViewModel> Counts { get; }

        public CountsViewModel()
        {
            Counts = new ReadOnlyObservableCollection<CountItemViewModel>(_counts);
        }

        public void InitializeRange(int start, int end)
        {
            Clear();
            for (int v = start; v <= end; v++)
            {
                var ci = new CountItemViewModel(v);
                _counts.Add(ci);
                _lookup.TryAdd(v, ci);
            }
        }

        public void Increment(int number)
        {
            var item = _lookup.GetOrAdd(number, n =>
            {
                var ci = new CountItemViewModel(n);
                // Ensure UI collection updated on same thread (caller should be UI thread)
                _counts.Add(ci);
                return ci;
            });
            item.Increment();
        }

        public void Clear()
        {
            _counts.Clear();
            foreach (var k in _lookup.Keys.ToList()) _lookup.TryRemove(k, out _);
        }

        public int Total => _lookup.Values.Sum(c => c.Count);

        public IReadOnlyDictionary<int,int> SnapshotCounts()
        {
            return _lookup.ToDictionary(kv => kv.Key, kv => kv.Value.Count);
        }
    }
}
