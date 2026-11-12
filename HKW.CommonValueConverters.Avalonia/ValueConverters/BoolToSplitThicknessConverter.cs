using System.Globalization;
using Avalonia;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// 从分离的参数中选择布尔值或 null 分支, 并解析有限的均匀或四边厚度.
/// 使用有效的转换文化, 并在输入无效时返回 DefaultResult.
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
        var parts = value.Split(',');
        if (parts.Length is not (1 or 4))
            return new Thickness();
        Span<double> sides = stackalloc double[4];
        for (var index = 0; index < parts.Length; index++)
        {
            if (
                double.TryParse(
                    parts[index],
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    culture,
                    out sides[index]
                )
                    is false
                || double.IsFinite(sides[index]) is false
            )
                return DefaultResult;
        }
        return parts.Length == 1
            ? new Thickness(sides[0])
            : new Thickness(sides[0], sides[1], sides[2], sides[3]);
    }
}
