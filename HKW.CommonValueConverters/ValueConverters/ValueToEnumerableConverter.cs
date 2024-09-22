//using System;
//using System.Globalization;

//namespace HKW.WPF.Converters;

///// <summary>
///// 值到集合转换器
///// </summary>
//public class ValueToEnumerableConverter : ValueConverterBase<ValueToEnumerableConverter>
//{
//    /// <inheritdoc/>
//    public override object? Convert(
//        object? value,
//        Type? targetType,
//        object? parameter,
//        CultureInfo? culture
//    )
//    {
//        if (value is not null)
//            return new object[] { value };

//        return null;
//    }
//}
