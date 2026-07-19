using System.Collections.Generic;

namespace random_experimental
{
    // Backwards-compatible static facade. Prefer DI (IStatisticsService) in new code.
    internal static class StatisticsService
    {
        private static IStatisticsService _instance = new StatisticsServiceImpl();

        public static IStatisticsService Instance
        {
            get => _instance;
            set => _instance = value ?? throw new System.ArgumentNullException(nameof(value));
        }

        public static string ComputeChiSquareSummary(int start, int end, IReadOnlyDictionary<int,int> counts)
        {
            return Instance.ComputeChiSquareSummary(start, end, counts);
        }
    }
}
