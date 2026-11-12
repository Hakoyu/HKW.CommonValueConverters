using System.Globalization;
using System.Windows;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 从分离的参数中选择布尔值或 null 分支, 并解析一个或四个有限的厚度分量.
/// 使用有效的转换文化, 无效或非有限分量返回 DefaultResult.
/// 空文本或不支持的分量数量保留零厚度行为.
/// </summary>
public class BoolToSplitThicknessConverter : BoolToSplitParameterConverter
{
    /// <inheritdoc/>
    public BoolToSplitThicknessConverter()
    {
        Separator = ";";
        ConvertSplitValue = (s, c) => ParseThickness(s, c)!;
    }

    private object? ParseThickness(string value, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new Thickness();
        var components = value.Split(',');
        if (components.Length is not (1 or 4))
            return new Thickness();
        Span<double> sides = stackalloc double[4];
        for (var index = 0; index < components.Length; index++)
        {
            if (
                double.TryParse(
                    components[index],
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    culture,
                    out sides[index]
                )
                    is false
                || double.IsFinite(sides[index]) is false
            )
                return DefaultResult;
        }
        return components.Length == 1
            ? new Thickness(sides[0])
            : new Thickness(sides[0], sides[1], sides[2], sides[3]);
    }
}
