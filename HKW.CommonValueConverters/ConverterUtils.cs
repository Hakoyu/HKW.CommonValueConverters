namespace HKW.CommonValueConverters;

internal static class ConverterUtils
{
    public static bool? GetBool(object? value, bool? nullValue = null)
    {
        if (value is null || value == CommonConverterBase.UnsetValue)
            return nullValue;

        var str = value.ToString();
        if (value is bool boolValue)
            return boolValue;
        else if (bool.TryParse(str, out boolValue))
            return boolValue;
        else if (int.TryParse(str, out var i))
            return i > 0;
        else
            return nullValue;
    }
}
