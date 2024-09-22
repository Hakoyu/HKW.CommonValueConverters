namespace HKW.CommonValueConverters;

internal static class ConverterUtils
{
    public static bool GetBool(object? value, bool nullValue = false)
    {
        if (value is null || value == CommonConverterBase.UnsetValue)
            return nullValue;
        else if (value is bool boolValue)
            return boolValue;
        else if (bool.TryParse(value.ToString(), out boolValue))
            return boolValue;
        else
            return nullValue;
    }
}
