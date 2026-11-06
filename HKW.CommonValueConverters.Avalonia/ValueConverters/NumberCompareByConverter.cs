using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.NumberCompareByConverter"/> 的 Avalonia 包装器.
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
            defaultValue: Core.NumberCompareByConverter.DefaultNumberType
        );

    /// <summary>
    /// 数值转换或运算所使用的数值类型.
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
            defaultValue: Core.NumberCompareByConverter.DefaultComparisonType
        );

    /// <summary>
    /// 数值比较使用的关系运算符.
    /// </summary>
    public ComparisonOperatorType ComparisonType
    {
        get => GetValue(ComparisonTypeProperty);
        set => SetValue(ComparisonTypeProperty, value);
    }
}
