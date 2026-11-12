using System.Globalization;
using System.Windows;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 将最多四个数值转换为 <see cref="Thickness"/>, 顺序为左、上、右、下.
/// 缺失的边和为空的数值槽默认设为零；仅有一个数值时, 填充全部槽.
/// </summary>
/// <remarks>
/// 启用 <see cref="HasRatio"/> 时, 第一个值将按比例缩放其余边.
/// 空输入或未指定边的比例将产生零厚度, 除非某个输入不可用.
/// 数字字符串使用有效的 <see cref="ConverterBase.PreferredCulture"/>.
/// 不可用的输入、多余的值、已知的内置数值转换错误, 以及非有限值或
/// 乘积将返回 <see cref="ConverterBase.DefaultResult"/>, 其默认值为零厚度.
/// </remarks>
public class MarginMultiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    public MarginMultiConverter()
        : base(new Implementation())
    {
        var core = (Implementation)CoreConverter;
        core.GetHasRatio = () => HasRatio;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool> HasRatioProperty =
        CommonDependencyProperty.Register<MarginMultiConverter, bool>(nameof(HasRatio), false);

    /// <summary>
    /// 含有比例
    /// </summary>
    public bool HasRatio
    {
        get => GetValue(HasRatioProperty);
        set => SetValue(HasRatioProperty, value);
    }

    internal sealed class Implementation : Core.MultiValueConverterBase
    {
        internal Implementation() => GetDefaultResult = () => new Thickness();

        internal Func<bool> GetHasRatio { get; set; } = () => false;

#pragma warning disable S3776
        protected override object? ConvertCore(
#pragma warning restore S3776
            IList<object?> values,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            var hasRatio = GetHasRatio();
            if (values is null || values.Count > (hasRatio ? 5 : 4))
            {
                return GetDefaultResult();
            }
            if (
                values.Any(v =>
                    ReferenceEquals(v, Core.ConverterResults.UnsetValue)
                    || ReferenceEquals(v, Core.ConverterResults.DoNothing)
                )
            )
                return GetDefaultResult();

            if (values.Count == 0 || (hasRatio && values.Count == 1))
            {
                return new Thickness();
            }

            var ratio = 1d;
            if (hasRatio && TryGetFiniteNumber(values[0], culture, out ratio) is false)
            {
                return GetDefaultResult();
            }
            if (values.Count - (hasRatio ? 1 : 0) == 1)
            {
                if (
                    TryGetFiniteNumber(values[(hasRatio ? 1 : 0)], culture, out var number) is false
                )
                    return GetDefaultResult();
                var result = number * ratio;
                return double.IsFinite(result) ? new Thickness(result) : GetDefaultResult();
            }
            Span<double> sides = stackalloc double[4];
            sides.Clear();
            var offset = hasRatio ? 1 : 0;
            for (var index = offset; index < values.Count; index++)
            {
                if (TryGetFiniteNumber(values[index], culture, out var number) is false)
                {
                    return GetDefaultResult();
                }
                var result = number * ratio;
                if (double.IsFinite(result) is false)
                {
                    return GetDefaultResult();
                }
                sides[index - offset] = result;
            }
            return new Thickness(sides[0], sides[1], sides[2], sides[3]);
        }

        private static bool TryGetFiniteNumber(
            object? value,
            CultureInfo culture,
            out double number
        )
        {
            var handlesConversionErrors =
                value is not IConvertible
                || value is string or decimal or DateTime or Enum or DBNull
                || value.GetType().IsPrimitive;
            try
            {
                number = System.Convert.ToDouble(value, culture);
                return double.IsFinite(number);
            }
            catch (Exception exception)
                when (handlesConversionErrors
                    && exception is FormatException or InvalidCastException or OverflowException
                )
            {
                number = 0;
                return false;
            }
        }
    }
}
