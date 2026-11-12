using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using HKW.CommonValueConverters;
using HKW.HKWUtils;

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
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(true, null, null, null));
    }

    [TestMethod]
    public void BoolToSplitParameterConverter_OtherSeparator()
    {
        var c = new BoolToSplitParameterConverter() { GetSeparator = () => " " };

        Assert.AreEqual("True", c.Convert(true, null, "True False Null", null));
        Assert.AreEqual("False", c.Convert(false, null, "True False Null", null));
        Assert.AreEqual("Null", c.Convert(null, null, "True False Null", null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(true, null, null, null));
    }

    [TestMethod]
    [DataRow("8")]
    [DataRow("+8")]
    [DataRow("-8")]
    [DataRow("*8")]
    [DataRow("/8")]
    [DataRow("%8")]
    public void CalculatorConverter(string parameter)
    {
        var c = new CalculatorConverter();
        var op = ArithmeticOperatorType.Addition;
        if (char.IsNumber(parameter[0]) is false)
            op = NumberUtils.GetArithmeticOperatorType(parameter[0]);
        Assert.AreEqual(
            NumberUtils.Arithmetic(
                2,
                parameter[(char.IsNumber(parameter[0]) ? 0 : 1)..],
                c.GetNumberType(),
                op
            ),
            c.Convert(2, null, parameter, null)
        );
    }

    [TestMethod]
    public void CollectionCountCompareConverter()
    {
        var c = new CollectionCountCompareConverter();

        Assert.AreEqual(0, c.Convert(new[] { 1, 2, 3 }, null, 3, null));
        Assert.AreEqual(1, c.Convert("test", null, "3", null));
        Assert.AreEqual(-1, c.Convert(new[] { 1, 2 }, null, 3, null));
        Assert.AreEqual(c.GetDefaultResult(), c.Convert(new object(), null, 1, null));
        Assert.AreEqual(c.GetDefaultResult(), c.Convert(new[] { 1 }, null, "invalid", null));
    }

    [TestMethod]
    public void CollectionCountCompareByConverter()
    {
        var c = new CollectionCountCompareByConverter();

        Assert.AreEqual(true, c.Convert(new[] { 1, 2, 3 }, null, ">2", null));
        Assert.AreEqual(false, c.Convert(new[] { 1, 2, 3 }, null, ">3", null));
        Assert.AreEqual(true, c.Convert("test", null, "<=4", null));
        Assert.AreEqual(false, c.Convert(new[] { 1, 2 }, null, "<2", null));
        Assert.AreEqual(c.GetDefaultResult(), c.Convert(new object(), null, ">0", null));
        Assert.AreEqual(c.GetDefaultResult(), c.Convert(new[] { 1 }, null, "invalid", null));
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
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(null, null, null, null));
    }

    [TestMethod]
    public void EnumToEnumInfoDisplayConverter_DefaultName()
    {
        var c = new EnumToEnumInfoDisplayConverter();

        Assert.AreEqual("First", c.Convert(TestEnumType.First, null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert("First", null, null, null));
    }

    [TestMethod]
    [DataRow(EnumInfoDisplayTarget.Name, "First name")]
    [DataRow(EnumInfoDisplayTarget.ShortName, "First short name")]
    [DataRow(EnumInfoDisplayTarget.Description, "First description")]
    public void EnumToEnumInfoDisplayConverter_DisplayMetadata(
        EnumInfoDisplayTarget target,
        string expected
    )
    {
        var c = new EnumToEnumInfoDisplayConverter { GetEnumInfoDisplayTarget = () => target };

        Assert.AreEqual(expected, c.Convert(DisplayEnumType.First, null, null, null));
        var fallback = new object();
        c.GetDefaultResult = () => fallback;
        Assert.AreSame(fallback, c.Convert(null, null, null, null));
        Assert.AreSame(fallback, c.Convert(0, null, null, null));
    }

    [TestMethod]
    public void EnumToEnumInfoConverter_RoundTrip()
    {
        var c = new EnumToEnumInfoConverter();
        var info = c.Convert(DisplayEnumType.First, null, null, null) as IEnumInfo;

        Assert.IsNotNull(info);
        Assert.AreEqual(DisplayEnumType.First, info.Value);
        Assert.AreEqual(typeof(DisplayEnumType), info.EnumType);
        Assert.AreEqual("First name", info.DisplayName);
        Assert.AreEqual(DisplayEnumType.First, c.ConvertBack(info, null, null, null));
    }

    [TestMethod]
    public void EnumToEnumInfoConverter_InvalidInputsUseDefault()
    {
        var fallback = new object();
        var c = new EnumToEnumInfoConverter { GetDefaultResult = () => fallback };

        foreach (
            var value in new object?[]
            {
                null,
                "First",
                0,
                ConverterResults.UnsetValue,
                ConverterResults.DoNothing,
            }
        )
        {
            Assert.AreSame(fallback, c.Convert(value, null, null, null));
            Assert.AreSame(fallback, c.ConvertBack(value, null, null, null));
        }
        Assert.AreSame(fallback, c.ConvertBack(TestEnumType.First, null, null, null));
    }

    [TestMethod]
    public void EqualsConverter()
    {
        var c = new EqualsConverter();

        Assert.AreEqual(false, c.Convert("Other", null, null, null));
        Assert.AreEqual(true, c.Convert("Parameter", null, "Parameter", null));
        Assert.AreEqual(true, c.Convert(null, null, null, null));
    }

    [TestMethod]
    public void EqualsConverter_GetOther()
    {
        var c = new EqualsConverter() { GetOther = () => "Value" };

        Assert.AreEqual(true, c.Convert("Value", null, null, null));
        Assert.AreEqual(false, c.Convert("Other", null, null, null));
        Assert.AreEqual(true, c.Convert("Parameter", null, "Parameter", null));
    }

    [TestMethod]
    public void EqualsToValueConverter()
    {
        var c = new EqualsToValueConverter<string>()
        {
            GetOther = () => "Target",
            GetTrueValue = () => "True",
            GetFalseValue = () => "False",
            GetNullValue = () => "Null",
        };

        Assert.AreEqual("True", c.Convert("Target", null, null, null));
        Assert.AreEqual("False", c.Convert("Other", null, null, null));
        Assert.AreEqual("Null", c.Convert(null, null, null, null));
    }

    [TestMethod]
    public void FirstOrDefaultConverter()
    {
        var c = new FirstOrDefaultConverter() { GetDefaultResult = () => "Default" };
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
    public void NullToTrueConverter()
    {
        var c = new NullToTrueConverter();

        Assert.AreEqual(true, c.Convert(null, null, null, null));
        Assert.AreEqual(false, c.Convert("Value", null, null, null));

        c.GetIsInverted = () => true;

        Assert.AreEqual(false, c.Convert(null, null, null, null));
        Assert.AreEqual(true, c.Convert("Value", null, null, null));
    }

    [TestMethod]
    public void NumberClampConverter()
    {
        var c = new NumberClampConverter();

        Assert.AreEqual(true, c.Convert(5, null, "2,8", null));
    }

    [TestMethod]
    public void NumberCompareConverter()
    {
        var c = new NumberCompareConverter();

        Assert.AreEqual(0, c.Convert(5, null, 5, null));
        Assert.AreEqual(1, c.Convert(6, null, 5, null));
        Assert.AreEqual(-1, c.Convert(4, null, 5, null));
    }

    [TestMethod]
    public void NumberCompareByConverter()
    {
        var c = new NumberCompareByConverter();

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
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert("value", null, "Unknown", CultureInfo.InvariantCulture)
        );
    }

    [TestMethod]
    public void StringCheckConverter()
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
        Assert.AreEqual(value, c.ConvertBack("01:30:00", null, null, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void VersionToStringConverter()
    {
        var c = new VersionToStringConverter();
        var value = new Version(1, 2, 3, 4);

        Assert.AreEqual("1.2.3.4", c.Convert(value, null, null, null));
        Assert.AreEqual("1.2", c.Convert(value, null, "2", null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert("1.2.3.4", null, null, null));
    }

    [TestMethod]
    [DataRow(true, false)]
    [DataRow(false, true)]
    public void BoolInverterConverter_InvertsBoolean(bool value, bool expected)
    {
        var c = new BoolInverterConverter();

        Assert.AreEqual(expected, c.Convert(value, null, null, null));
    }

    [TestMethod]
    public void BoolInverterConverter_NullAndInvalidInputs()
    {
        var fallback = new object();
        var c = new BoolInverterConverter { GetDefaultResult = () => fallback };

        Assert.AreEqual(false, c.Convert(null, null, null, null));
        c.GetNullValue = () => true;
        Assert.AreEqual(true, c.Convert(null, null, null, null));
        foreach (
            var value in new object[]
            {
                "true",
                1,
                new object(),
                ConverterResults.UnsetValue,
                ConverterResults.DoNothing,
            }
        )
            Assert.AreSame(fallback, c.Convert(value, null, null, null));
    }

    [TestMethod]
    public void FirstOrDefaultConverter_PreservesFirstValueAndRejectsUnavailableValues()
    {
        var fallback = new object();
        var c = new FirstOrDefaultConverter { GetDefaultResult = () => fallback };
        var first = new object();

        Assert.AreSame(first, c.Convert(new[] { first, new object() }, null, null, null));
        Assert.IsNull(c.Convert(new object?[] { null, first }, null, null, null));
        Assert.AreEqual('a', c.Convert("abc", null, null, null));
        Assert.AreSame(fallback, c.Convert("", null, null, null));
        Assert.AreSame(fallback, c.Convert(42, null, null, null));
        foreach (
            var unavailable in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing }
        )
            Assert.AreSame(fallback, c.Convert(new[] { unavailable, first }, null, null, null));
    }

    [TestMethod]
    [DataRow("==5", true)]
    [DataRow("!=5", false)]
    [DataRow("<6", true)]
    [DataRow("<=5", true)]
    [DataRow(">4", true)]
    [DataRow(">=5", true)]
    [DataRow(" > 5 ", false)]
    public void NumberCompareByConverter_ComparisonOperators(string parameter, bool expected)
    {
        var c = new NumberCompareByConverter();

        Assert.AreEqual(expected, c.Convert(5, null, parameter, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void NumberCompareByConverter_ConfiguredComparisonAndInvalidInputs()
    {
        var fallback = new object();
        var c = new NumberCompareByConverter
        {
            GetComparisonType = () => ComparisonOperatorType.GreaterThan,
            GetDefaultResult = () => fallback,
        };

        Assert.AreEqual(true, c.Convert(6, null, 5, null));
        Assert.AreEqual(false, c.Convert(5, null, "5", null));
        Assert.AreEqual(true, c.Convert(5, null, "==5", null));
        foreach (
            var parameter in new object?[]
            {
                null,
                "",
                ">bad",
                new object(),
                ConverterResults.UnsetValue,
            }
        )
            Assert.AreSame(fallback, c.Convert(5, null, parameter, null));
        Assert.AreSame(fallback, c.Convert("bad", null, 5, null));
        c.GetComparisonType = () => (ComparisonOperatorType)int.MaxValue;
        Assert.AreSame(fallback, c.Convert(5, null, 5, null));
    }

    [TestMethod]
    [DataRow(StringCheckType.Null, null, true)]
    [DataRow(StringCheckType.Null, "", false)]
    [DataRow(StringCheckType.NullOrEmpty, "", true)]
    [DataRow(StringCheckType.NullOrEmpty, " ", false)]
    [DataRow(StringCheckType.NullOrWhiteSpace, " \t", true)]
    [DataRow(StringCheckType.NullOrWhiteSpace, "value", false)]
    public void StringCheckConverter_ConfiguredCheckAndInversion(
        StringCheckType checkType,
        string? value,
        bool expected
    )
    {
        var c = new StringCheckConverter { GetStringCheckType = () => checkType };

        Assert.AreEqual(expected, c.Convert(value, null, null, null));
        Assert.AreEqual(expected, c.Convert(value, null, "invalid", null));
        c.GetIsInverted = () => true;
        Assert.AreEqual(expected is false, c.Convert(value, null, null, null));
    }

    [TestMethod]
    public void StringCheckConverter_ParameterOverridesConfigurationAndInvalidValuesUseDefault()
    {
        var fallback = new object();
        var c = new StringCheckConverter { GetDefaultResult = () => fallback };

        Assert.AreEqual(true, c.Convert("", null, "NullOrEmpty", null));
        Assert.AreSame(fallback, c.Convert("value", null, (StringCheckType)int.MaxValue, null));
        foreach (
            var unavailable in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing }
        )
        {
            Assert.AreSame(fallback, c.Convert(unavailable, null, null, null));
            Assert.AreSame(fallback, c.Convert("value", null, unavailable, null));
        }
    }

    [TestMethod]
    public void StringToNumberConverter_NumberTypesAndStyles()
    {
        var c = new StringToNumberConverter
        {
            PreferredCulture = PreferredCulture.ConverterCulture,
        };
        var culture = CultureInfo.InvariantCulture;

        Assert.AreEqual(42, c.Convert("42", null, null, culture));
        c.GetNumberStyles = () => NumberStyles.HexNumber;
        Assert.AreEqual(255, c.Convert("FF", null, null, culture));
        c.GetNumberStyles = () => NumberStyles.Number;
        c.GetNumberType = () => NumberType.Decimal;
        Assert.AreEqual(12.5m, c.Convert("12,5", null, null, CultureInfo.GetCultureInfo("fr-FR")));
        Assert.AreEqual(
            "12,5",
            c.ConvertBack(12.5m, null, null, CultureInfo.GetCultureInfo("fr-FR"))
        );
    }

    [TestMethod]
    public void StringToNumberConverter_InvalidInputsAndReverseConversion()
    {
        var fallback = new object();
        var c = new StringToNumberConverter { GetDefaultResult = () => fallback };

        foreach (
            var value in new object?[]
            {
                null,
                "",
                "bad",
                "2147483648",
                ConverterResults.UnsetValue,
                ConverterResults.DoNothing,
            }
        )
            Assert.AreSame(fallback, c.Convert(value, null, null, CultureInfo.InvariantCulture));
        foreach (
            var unavailable in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing }
        )
            Assert.AreSame(fallback, c.ConvertBack(unavailable, null, null, null));
        Assert.IsNull(c.ConvertBack(null, null, null, null));
        Assert.AreEqual("text", c.ConvertBack("text", null, null, null));
        Assert.AreEqual("42", c.ConvertBack(42, null, null, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    [DataRow(true, true)]
    [DataRow(false, false)]
    [DataRow("true", true)]
    [DataRow("FALSE", false)]
    [DataRow("1", true)]
    [DataRow("0", false)]
    [DataRow("-1", false)]
    [DataRow(2, true)]
    [DataRow(0, false)]
    [DataRow(-2, false)]
    public void ToBoolConverter_BooleanAndIntegerInputs(object value, bool expected)
    {
        var c = new ToBoolConverter();

        Assert.AreEqual(expected, c.Convert(value, null, null, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void ToBoolConverter_NullPolicyAndUnavailableInputs()
    {
        var fallback = new object();
        var c = new ToBoolConverter { GetDefaultResult = () => fallback };

        Assert.AreEqual(false, c.Convert(null, null, null, null));
        Assert.AreEqual(false, c.Convert("invalid", null, null, null));
        c.GetNullValue = () => true;
        Assert.AreEqual(true, c.Convert(null, null, null, null));
        Assert.AreEqual(true, c.Convert("invalid", null, null, null));
        c.GetNullValue = () => null;
        Assert.IsNull(c.Convert(null, null, null, null));
        Assert.IsNull(c.Convert("invalid", null, null, null));
        foreach (
            var unavailable in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing }
        )
            Assert.AreSame(fallback, c.Convert(unavailable, null, null, null));
    }

    [TestMethod]
    public void ValueToEnumerableConverter_WrapsOneValueWithoutFlattening()
    {
        var c = new ValueToEnumerableConverter();
        foreach (var value in new object[] { new object(), "text", new[] { 1, 2 }, 42 })
        {
            var result = c.Convert(value, null, null, null) as object[];
            Assert.IsNotNull(result);
            Assert.HasCount(1, result);
            Assert.AreSame(value, result[0]);
        }
    }

    [TestMethod]
    public void ValueToEnumerableConverter_NullAndUnavailableInputs()
    {
        var fallback = new object();
        var c = new ValueToEnumerableConverter { GetDefaultResult = () => fallback };

        Assert.IsNull(c.Convert(null, null, null, null));
        foreach (
            var unavailable in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing }
        )
            Assert.AreSame(fallback, c.Convert(unavailable, null, null, null));
    }

    private enum DisplayEnumType
    {
        [Display(
            Name = "First name",
            ShortName = "First short name",
            Description = "First description"
        )]
        First,
    }

    private enum TestEnumType
    {
        First,
        Second,
    }
}
#pragma warning restore S6562,S2701,MSTEST0037
