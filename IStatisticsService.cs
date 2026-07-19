using System.Collections.Generic;

namespace random_experimental
{
    internal interface IStatisticsService
    {
        string ComputeChiSquareSummary(int start, int end, IReadOnlyDictionary<int,int> counts);
    }
}
