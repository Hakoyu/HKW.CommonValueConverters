using System.Globalization;
using System.Numerics;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

internal static class ConverterUtils
{
    public static bool IsUnavailable(object? value) =>
        ReferenceEquals(value, ConverterResults.UnsetValue)
        || ReferenceEquals(value, ConverterResults.DoNothing);

    public static bool HasUnavailable(IList<object?>? values) =>
        values is null || values.Any(IsUnavailable);

    public static bool? GetBool(object? value, CultureInfo culture, bool? nullValue = null)
    {
        if (value is null || IsUnavailable(value))
            return nullValue;
        if (value is bool boolValue)
            return boolValue;
        var str = value is IFormattable formattable
            ? formattable.ToString(null, culture)
            : value.ToString();
        if (bool.TryParse(str, out boolValue))
            return boolValue;
        return int.TryParse(str, NumberStyles.Integer, culture, out var i) ? i > 0 : nullValue;
    }

    public static bool TryNumber(
        object? value,
        NumberType type,
        CultureInfo culture,
        out object result
    )
    {
        result = null!;
        if (value is null || IsUnavailable(value))
            return false;
        if (value is string str)
            return NumberUtils.TryParse(type, str, NumberStyles.Any, culture, out result!);
        if (
            value
            is not (
                byte
                or sbyte
                or short
                or ushort
                or int
                or uint
                or long
                or ulong
                or float
                or double
                or decimal
                or Half
                or Int128
                or UInt128
                or BigInteger
                or nint
                or nuint
                or bool
                or char
            )
        )
            return false;
        try
        {
            result = NumberUtils.ConvertTo(value, type, culture);
            return true;
        }
        catch (Exception ex)
            when (culture.GetType() == typeof(CultureInfo)
                && ex is (FormatException or OverflowException or InvalidCastException)
            )
        {
            return false;
        }
    }

    public static bool TryArithmetic(
        object left,
        object right,
        NumberType type,
        ArithmeticOperatorType operation,
        out object result
    )
    {
        result = null!;
        if (!Enum.IsDefined(operation))
            return false;
        try
        {
            result = NumberUtils.Arithmetic(left, right, type, operation);
            return true;
        }
        catch (Exception ex) when (ex is DivideByZeroException or OverflowException)
        {
            return false;
        }
    }
}
