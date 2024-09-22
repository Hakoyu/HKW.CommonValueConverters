//using System;
//using System.Globalization;

//namespace HKW.WPF.Converters;

///// <summary>
///// 字符串到十进制转换器
///// </summary>
//public class StringToDecimalConverter : ValueConverterBase<StringToDecimalConverter>
//{
//    /// <summary>
//    /// 默认数字风格
//    /// </summary>
//    private static readonly NumberStyles DefaultNumberStyles = NumberStyles.Any;

//    /// <inheritdoc/>
//    public override object? Convert(
//        object? value,
//        Type? targetType,
//        object? parameter,
//        CultureInfo? culture
//    )
//    {
//        if (value is decimal dec)
//        {
//            return dec.ToString("G", culture);
//        }

//        if (value is string str)
//        {
//            if (decimal.TryParse(str, DefaultNumberStyles, culture, out var result))
//                return result;
//            return result;
//        }

//        return null;
//    }

//    /// <inheritdoc/>
//    public override object? ConvertBack(
//        object? value,
//        Type? targetType,
//        object? parameter,
//        CultureInfo? culture
//    )
//    {
//        return Convert(value, targetType, parameter, culture);
//    }
//}
