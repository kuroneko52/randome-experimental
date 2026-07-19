using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace random_experimental
{
    internal class StatisticsServiceImpl : IStatisticsService
    {
        public string ComputeChiSquareSummary(int start, int end, IReadOnlyDictionary<int,int> counts)
        {
            int n = end - start + 1;
            int total = counts.Values.Sum();
            if (n <= 0 || total == 0) return "No results";

            double expected = (double)total / n;
            double chi2 = 0.0;
            for (int v = start; v <= end; v++)
            {
                counts.TryGetValue(v, out var obs);
                double d = obs - expected;
                chi2 += (d * d) / (expected > 0 ? expected : 1.0);
            }

            int df = Math.Max(1, n - 1);
            double pValue;
            try { pValue = ChiSquareUpperTail(chi2, df); }
            catch { pValue = double.NaN; }

            string summary = $"Chi2={chi2:F3} df={df} p={(double.IsNaN(pValue) ? "?" : pValue.ToString("F4"))}";

            var sb = new StringBuilder();
            sb.AppendLine($"Total={total}");
            sb.AppendLine($"Categories={n} {summary}");
            sb.AppendLine($"Expected: {expected:F2}");
            sb.AppendLine("Counts:");
            foreach (var kv in counts.OrderBy(kv => kv.Key)) sb.AppendLine($"  {kv.Key,4}: {kv.Value,8}");
            return sb.ToString();
        }

        // chi-square tail functions reused from previous implementation
        private double ChiSquareUpperTail(double x, int df)
        {
            if (x < 0.0 || df < 1) return double.NaN;
            double a = df / 2.0;
            double xx = x / 2.0;
            return GammaQ(a, xx);
        }

        private double GammaQ(double a, double x)
        {
            if (x < 0 || a <= 0) return double.NaN;
            if (x == 0) return 1.0;
            if (x < a + 1.0)
            {
                double ap = a;
                double sum = 1.0 / a;
                double del = sum;
                for (int n = 1; n <= 100; n++)
                {
                    ap += 1.0;
                    del *= x / ap;
                    sum += del;
                    if (Math.Abs(del) < Math.Abs(sum) * 1e-14) break;
                }
                double ln = a * Math.Log(x) - x - LogGamma(a);
                double p = sum * Math.Exp(ln);
                return 1.0 - p;
            }
            else
            {
                double b = x + 1.0 - a;
                double c = 1.0 / 1.0e-30;
                double d = 1.0 / b;
                double h = d;
                for (int i = 1; i <= 100; i++)
                {
                    double an = -i * (i - a);
                    b += 2.0;
                    d = an * d + b;
                    if (Math.Abs(d) < 1e-30) d = 1e-30;
                    c = b + an / c;
                    if (Math.Abs(c) < 1e-30) c = 1e-30;
                    d = 1.0 / d;
                    double delta = d * c;
                    h *= delta;
                    if (Math.Abs(delta - 1.0) < 1e-14) break;
                }
                double ln = a * Math.Log(x) - x - LogGamma(a);
                double q = Math.Exp(ln) * h;
                return q;
            }
        }

        private double LogGamma(double x)
        {
            double[] cof = new double[] {
                76.18009172947146,
                -86.50532032941677,
                24.01409824083091,
                -1.231739572450155,
                0.001208650973866179,
                -0.000005395239384953
            };
            double y = x;
            double tmp = x + 5.5;
            tmp -= (x + 0.5) * Math.Log(tmp);
            double ser = 1.000000000190015;
            for (int j = 0; j < cof.Length; j++) ser += cof[j] / ++y;
            return -tmp + Math.Log(2.5066282746310005 * ser / x);
        }
    }
}
