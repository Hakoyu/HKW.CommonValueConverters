using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 从分离的参数中选择布尔值或 null 分支, 并解析一个有限的均匀厚度.
/// 使用有效的转换文化, 并在输入无效时返回 DefaultResult.
/// </summary>
public class BoolToSplitThicknessConverter : BoolToSplitParameterConverter
{
    /// <inheritdoc/>
    public BoolToSplitThicknessConverter()
    {
        ConvertSplitValue = v =>
        {
            if (v is not string str || string.IsNullOrWhiteSpace(str))
                return new Thickness();
            var sp = str.Split(',');
            if (sp.Length == 1)
                return new Thickness(double.Parse(sp[0]));
            else if (sp.Length == 4)
                return new Thickness(
                    double.Parse(sp[0]),
                    double.Parse(sp[1]),
                    double.Parse(sp[2]),
                    double.Parse(sp[3])
                );
            else
                return new Thickness();
        };
    }
}
