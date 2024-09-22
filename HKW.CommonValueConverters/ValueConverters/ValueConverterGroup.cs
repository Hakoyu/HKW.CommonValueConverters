//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;

//namespace HKW.WPF.Converters;

///// <summary>
///// 值转换器组
///// <para>
///// 用于汇总转换器信息
///// </para>
///// </summary>
//public class ValueConverterGroup : ValueConverterBase<ValueConverterGroup>
//{
//    /// <summary>
//    /// 转换器
//    /// </summary>
//    public List<IValueConverter> Converters { get; set; } = new();

//    /// <inheritdoc/>
//    public override object? Convert(
//        object? value,
//        Type? targetType,
//        object? parameter,
//        CultureInfo? culture
//    )
//    {
//        if (Converters is not IEnumerable<IValueConverter> converters)
//            return null;

//        return converters.Aggregate(
//            value,
//            (current, converter) => converter.Convert(current, targetType, parameter, culture)
//        );
//    }

//    /// <inheritdoc/>
//    public override object? ConvertBack(
//        object? value,
//        Type? targetType,
//        object? parameter,
//        CultureInfo? culture
//    )
//    {
//        if (Converters is not IEnumerable<IValueConverter> converters)
//            return null;

//        return converters
//            .Reverse()
//            .Aggregate(
//                value,
//                (current, converter) => converter.Convert(current, targetType, parameter, culture)
//            );
//    }
//}
