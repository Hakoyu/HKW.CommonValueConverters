using System.Collections;
using System.Globalization;
using HKW.CommonValueConverters;

namespace HKW.CommonValueConvertersTest;

#pragma warning disable S6562, S2701, MSTEST0037
[TestClass]
public sealed class ValueConverterTests
{
    [TestMethod]
    public void BoolToValueConverter()
    {
        var c = new BoolToValueConverter<string>()
        {
            GetTrueValue = () => "True",
            GetFalseValue = () => "False",
            GetNullValue = () => "Null",
        };

        Assert.AreEqual(c.GetTrueValue(), c.Convert(true, null, null, null));
        Assert.AreEqual(c.GetFalseValue(), c.Convert(false, null, null, null));
        Assert.AreEqual(c.GetFalseValue(), c.Convert(null, null, null, null));
    }

    [TestMethod]
    public void BoolToValueConverter_IsNullable()
    {
        var c = new BoolToValueConverter<string>()
        {
            GetTrueValue = () => "True",
            GetFalseValue = () => "False",
            GetNullValue = () => "Null",
            GetIsNullable = () => true,
        };

        Assert.AreEqual(c.GetTrueValue(), c.Convert(true, null, null, null));
        Assert.AreEqual(c.GetFalseValue(), c.Convert(false, null, null, null));
        Assert.AreEqual(c.GetNullValue(), c.Convert(null, null, null, null));
    }

    [TestMethod]
    public void BoolToSplitParameterConverter()
    {
        var c = new BoolToSplitParameterConverter();

        Assert.AreEqual("True", c.Convert(true, null, "True,False,Null", null));
        Assert.AreEqual("False", c.Convert(false, null, "True,False,Null", null));
        Assert.AreEqual("Null", c.Convert(null, null, "True,False,Null", null));
        Assert.IsNull(c.Convert(true, null, null, null));
    }

    [TestMethod]
    public void BoolToSplitParameterConverter_OtherSeparator()
    {
        var c = new BoolToSplitParameterConverter() { GetSeparator = () => " " };

        Assert.AreEqual("True", c.Convert(true, null, "True False Null", null));
        Assert.AreEqual("False", c.Convert(false, null, "True False Null", null));
        Assert.AreEqual("Null", c.Convert(null, null, "True False Null", null));
        Assert.IsNull(c.Convert(true, null, null, null));
    }

    [TestMethod]
    public void CalculatorConverter_Generic()
    {
        var c = new CalculatorConverter<int>();

        Assert.AreEqual(5, c.Convert(2, null, 3, null));
        Assert.AreEqual(5, c.ConvertBack(2, null, 3, null));
    }

    [TestMethod]
    public void CollectionCountEqualsConverter()
    {
        var c = new CollectionCountEqualsConverter();

        var array = new[] { 1, 2, 3 };
        Assert.AreEqual(false, c.Convert(array, null, 2, null));
        Assert.AreEqual(true, c.Convert(array, null, 3, null));
        Assert.AreEqual(false, c.Convert("invalid", null, 3, null));
    }

    [TestMethod]
    public void CollectionCountCompareConverter()
    {
        var c = new CollectionCountDifferenceConverter();

        var array = new[] { 1, 2, 3 };
        Assert.AreEqual(1, c.Convert(array, null, 2, null));
        Assert.AreEqual(0, c.Convert(array, null, 3, null));
        Assert.AreEqual(4, c.Convert("invalid", null, 3, null));
    }

    [TestMethod]
    public void DateTimeToStringConverter()
    {
        var c = new DateTimeToStringConverter() { GetFormat = () => "yyyy-MM-dd" };

        var value = new DateTime(2026, 9, 4);

        Assert.IsNotNull(c.Convert(value, null, "yyyy-MM-dd", CultureInfo.InvariantCulture));
        Assert.IsInstanceOfType(c.ConvertBack("2026-09-04", null, null, null), typeof(DateTime));
    }

    [TestMethod]
    public void DateTimeOffsetToStringConverter()
    {
        var c = new DateTimeOffsetToStringConverter() { GetFormat = () => "O" };
        var value = new DateTimeOffset(2026, 9, 4, 12, 0, 0, TimeSpan.Zero);

        Assert.IsNotNull(c.Convert(value, null, "O", CultureInfo.InvariantCulture));
        Assert.IsInstanceOfType(
            c.ConvertBack(value.ToString("O", CultureInfo.InvariantCulture), null, null, null),
            typeof(DateTimeOffset)
        );
    }

    [TestMethod]
    public void DebugConverter()
    {
        var c = new DebugConverter();
        var value = new object();

        Assert.AreSame(value, c.Convert(value, null, null, null));
        Assert.AreSame(value, c.ConvertBack(value, null, null, null));
    }

    [TestMethod]
    public void EnumEqualsConverter()
    {
        var c = new EnumEqualsConverter();

        Assert.AreEqual(true, c.Convert(TestEnumType.First, null, "First", null));
        Assert.AreEqual(false, c.Convert(TestEnumType.First, null, "Second", null));
        Assert.AreEqual(true, c.Convert(TestEnumType.First, null, TestEnumType.First, null));
        Assert.AreEqual(false, c.Convert(null, null, "First", null));
    }

    [TestMethod]
    public void EnumsToEnumInfosConverter()
    {
        var c = new EnumsToEnumInfosConverter();

        Assert.IsNotNull(
            c.Convert(new[] { TestEnumType.First, TestEnumType.Second }, null, null, null)
        );
        Assert.IsNull(c.Convert(null, null, null, null));
    }

    [TestMethod]
    public void EnumToEnumInfoTargetConverter()
    {
        var c = new EnumToEnumInfoDisplayConverter();

        Assert.IsNotNull(c.Convert(TestEnumType.First, null, null, null));
        Assert.IsNull(c.Convert("First", null, null, null));
    }

    [TestMethod]
    public void EqualsConverter()
    {
        var c = new EqualsConverter<string>() { GetValue = () => "Value" };

        Assert.AreEqual(true, c.Convert("Value", null, null, null));
        Assert.AreEqual(false, c.Convert("Other", null, null, null));
        Assert.AreEqual(true, c.Convert("Parameter", null, "Parameter", null));
    }

    [TestMethod]
    public void EqualsToValueConverter()
    {
        var c = new EqualsToValueConverter<string>()
        {
            GetTargetValue = () => "Target",
            GetTrueValue = () => "True",
            GetFalseValue = () => "False",
            GetNullValue = () => "Null",
            GetIsNullable = () => true,
        };

        Assert.AreEqual("True", c.Convert("Target", null, null, null));
        Assert.AreEqual("False", c.Convert("Other", null, null, null));
        Assert.AreEqual("Null", c.Convert(null, null, null, null));
    }

    [TestMethod]
    public void FirstOrDefaultResultConverter()
    {
        var c = new FirstOrDefaultResultConverter() { GetDefaultResult = () => "Default" };
        var array = new[] { 1, 2, 3 };
        Assert.AreEqual(1, c.Convert(array, null, null, null));
        Assert.AreEqual("Default", c.Convert(Array.Empty<int>(), null, null, null));
        Assert.AreEqual("Default", c.Convert(null, null, null, null));
    }

    [TestMethod]
    public void GetDictionaryValueConverter()
    {
        var c = new GetDictionaryValueConverter() { GetDefaultResult = () => "Default" };
        IDictionary dictionary = new Dictionary<string, string> { ["Key"] = "Value" };

        Assert.AreEqual("Value", c.Convert(dictionary, null, "Key", null));
        Assert.AreEqual("Default", c.Convert(dictionary, null, "Missing", null));
        Assert.AreEqual("Default", c.Convert(null, null, "Key", null));
    }

    [TestMethod]
    public void GuidToStringConverter()
    {
        var c = new GuidToStringConverter() { GetFormat = () => "N", GetStringTo = () => true };
        var value = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");

        Assert.AreEqual("0123456789ABCDEF0123456789ABCDEF", c.Convert(value, null, null, null));
        Assert.AreEqual(value, c.ConvertBack(value.ToString(), null, null, null));
    }

    [TestMethod]
    public void NullToBoolConverter()
    {
        var c = new NullToBoolConverter();

        Assert.AreEqual(true, c.Convert(null, null, null, null));
        Assert.AreEqual(false, c.Convert("Value", null, null, null));

        c.GetIsInverted = () => true;

        Assert.AreEqual(false, c.Convert(null, null, null, null));
        Assert.AreEqual(true, c.Convert("Value", null, null, null));
    }

    [TestMethod]
    public void NumberClampConverter()
    {
        var c = new NumberClampConverter<int>() { GetMinValue = () => 1, GetMaxValue = () => 10 };

        Assert.AreEqual(true, c.Convert(1, null, null, null));
        Assert.AreEqual(true, c.Convert(5, null, null, null));
        Assert.AreEqual(true, c.Convert(10, null, null, null));
        Assert.AreEqual(false, c.Convert(0, null, null, null));
        Assert.AreEqual(false, c.Convert(11, null, null, null));

        Assert.AreEqual(true, c.Convert(5, null, "2,8", null));
    }

    [TestMethod]
    public void NumberCompareConverter()
    {
        var c = new NumberCompareConverter<int>();

        Assert.AreEqual(0, c.Convert(5, null, 5, null));
        Assert.AreEqual(1, c.Convert(6, null, 5, null));
        Assert.AreEqual(-1, c.Convert(4, null, 5, null));
    }

    [TestMethod]
    public void NumberCompareXConverter()
    {
        var c = new NumberCompareXConverter<int>();

        Assert.AreEqual(true, c.Convert(6, null, ">5", null));
        Assert.AreEqual(false, c.Convert(5, null, ">5", null));
        Assert.AreEqual(true, c.Convert(5, null, "5", null));
    }

    [TestMethod]
    public void StringCaseConverter()
    {
        var c = new StringCaseConverter();

        Assert.AreEqual("VALUE", c.Convert("value", null, "U", CultureInfo.InvariantCulture));
        Assert.AreEqual("value", c.Convert("VALUE", null, "L", CultureInfo.InvariantCulture));
        Assert.AreEqual("Value", c.Convert("value", null, "T", CultureInfo.InvariantCulture));
        Assert.IsNull(c.Convert("value", null, "Unknown", CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void StringIsNullOrEmptyOrWhiteSpaceConverter()
    {
        var c = new StringCheckConverter();

        Assert.AreEqual(true, c.Convert(null, null, StringCheckType.Null, null));
        Assert.AreEqual(true, c.Convert("", null, StringCheckType.NullOrEmpty, null));
        Assert.AreEqual(true, c.Convert(" ", null, StringCheckType.NullOrWhiteSpace, null));
        Assert.AreEqual(false, c.Convert("Value", null, StringCheckType.NullOrWhiteSpace, null));
    }

    [TestMethod]
    public void TimeSpanToStringConverter()
    {
        var c = new TimeSpanToStringConverter() { GetFormat = () => "c" };
        var value = TimeSpan.FromHours(1.5);

        Assert.AreEqual("01:30:00", c.Convert(value, null, null, CultureInfo.InvariantCulture));
        Assert.AreEqual("Min", c.Convert(TimeSpan.MinValue, null, null, null));
        Assert.AreEqual(value, c.ConvertBack("01:30:00", null, null, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void VersionToStringConverter()
    {
        var c = new VersionToStringConverter();
        var value = new Version(1, 2, 3, 4);

        Assert.AreEqual("1.2.3.4", c.Convert(value, null, null, null));
        Assert.AreEqual("1.2", c.Convert(value, null, "2", null));
        Assert.IsNull(c.Convert("1.2.3.4", null, null, null));
    }

    private enum TestEnumType
    {
        First,
        Second,
    }
}
#pragma warning restore S6562,S2701,MSTEST0037
