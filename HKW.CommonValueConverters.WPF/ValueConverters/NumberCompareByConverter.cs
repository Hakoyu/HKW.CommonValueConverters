using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.NumberCompareByConverter"/> 的 WPF 包装器.
/// </summary>
public class NumberCompareByConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public NumberCompareByConverter()
        : base(new Core.NumberCompareByConverter())
    {
        var core = (Core.NumberCompareByConverter)CoreConverter;
        core.GetNumberType = () => NumberType;
        core.GetComparisonType = () => ComparisonType;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<NumberType> NumberTypeProperty =
        CommonDependencyProperty.Register<NumberCompareByConverter, NumberType>(
            nameof(NumberType),
            Core.NumberCompareByConverter.DefaultNumberType
        );

    /// <summary>
    /// 解析或计算时使用的数值类型.
    /// </summary>
    public NumberType NumberType
    {
        get => GetValue(NumberTypeProperty);
        set => SetValue(NumberTypeProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<ComparisonOperatorType> ComparisonTypeProperty =
        CommonDependencyProperty.Register<NumberCompareByConverter, ComparisonOperatorType>(
            nameof(ComparisonType),
            Core.NumberCompareByConverter.DefaultComparisonType
        );

    /// <summary>
    /// 将输入与转换参数进行比较时使用的比较运算符.
    /// </summary>
    public ComparisonOperatorType ComparisonType
    {
        get => GetValue(ComparisonTypeProperty);
        set => SetValue(ComparisonTypeProperty, value);
    }
}
