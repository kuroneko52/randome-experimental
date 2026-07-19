using System.Globalization;
using System.Windows.Controls;

namespace random_experimental
{
    internal class IntRangeValidationRule : ValidationRule
    {
        public int Min { get; set; } = int.MinValue;
        public int Max { get; set; } = int.MaxValue;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (value ?? string.Empty).ToString();
            if (string.IsNullOrWhiteSpace(s)) return new ValidationResult(false, "整数を入力してください");
            if (!int.TryParse(s, out var v)) return new ValidationResult(false, "整数を入力してください");
            if (v < Min || v > Max) return new ValidationResult(false, $"範囲外です ({Min}〜{Max})");
            return ValidationResult.ValidResult;
        }
    }
}
