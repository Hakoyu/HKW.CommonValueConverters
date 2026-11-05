using System.Collections;
using System.Globalization;
using HKW.CommonValueConverters;
using HKW.HKWUtils;

namespace HKW.CommonValueConvertersTest;

[TestClass]
public sealed class InputBoundaryTests
{
    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void BoolInversionPreservesNullBranch(bool nullable, bool inverted)
    {
        var c = new BoolToValueConverter<string>
        {
            GetIsNullable = () => nullable, GetIsInverted = () => inverted,
            GetTrueValue = () => "true", GetFalseValue = () => "false", GetNullValue = () => "null",
        };
        Assert.AreEqual(inverted ? "false" : "true", c.Convert(true, null, null, null));
        Assert.AreEqual(inverted ? "true" : "false", c.Convert(false, null, null, null));
        Assert.AreEqual(nullable ? "null" : inverted ? "true" : "false", c.Convert(null, null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(ConverterResults.UnsetValue, null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(ConverterResults.DoNothing, null, null, null));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void NullEqualityIsConsistent(bool inverted)
    {
        var all = new AllEqualsMultiConverter<object?> { GetIsInverted = () => inverted };
        var any = new AnyEqualsMultiConverter<object?> { GetIsInverted = () => inverted };
        var count = new EqualsCountMultiConverter<object?> { GetIsInverted = () => inverted };
        var pair = new FirstEqualsSecondMultiConverter { GetIsInverted = () => inverted };
        Assert.AreEqual(!inverted, all.Convert([null, null], null, null, null));
        Assert.AreEqual(!inverted, any.Convert([1, null], null, null, null));
        Assert.AreEqual(inverted ? 1 : 2, count.Convert([null, 1, null], null, null, null));
        Assert.AreEqual(!inverted, pair.Convert([null, null], null, null, null));
        Assert.AreEqual(inverted, pair.Convert([null, 1], null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, pair.Convert([ConverterResults.UnsetValue, ConverterResults.UnsetValue], null, null, null));
    }

    [TestMethod]
    public void EmptyAggregatesRetainDefinedResults()
    {
        Assert.AreEqual(true, new AllEqualsMultiConverter<object>().Convert([], null, null, null));
        Assert.AreEqual(false, new AnyEqualsMultiConverter<object>().Convert([], null, null, null));
        Assert.AreEqual(0, new EqualsCountMultiConverter<object>().Convert([], null, null, null));
        var allBool = new AllBoolToValueMultiConverter<string> { GetTrueValue = () => "true" };
        Assert.AreEqual("true", allBool.Convert([], null, null, null));
    }

    [TestMethod]
    public void SelectedBranchesIgnoreUnselectedUnavailableValues()
    {
        var unset = ConverterResults.UnsetValue;
        var c = new FirstBoolToValueMultiConverter();
        Assert.AreEqual("yes", c.Convert([true, "yes", unset, unset], null, null, null));
        Assert.IsNull(c.Convert([false, unset, null], null, null, null));
        Assert.AreSame(unset, c.Convert([true, unset, "no"], null, null, null));
        Assert.AreSame(unset, c.Convert([false, "yes"], null, null, null));
        Assert.AreSame(unset, c.Convert([unset, "yes", "no"], null, null, null));
        var str = new FirstStringStateToOtherStringMultiConverter();
        Assert.AreEqual("text", str.Convert(["input", "text", unset], null, null, null));
        Assert.AreEqual("null", str.Convert([null, unset, "null"], null, null, null));
        Assert.AreSame(unset, str.Convert([null], null, null, null));
        Assert.AreSame(unset, str.Convert([unset, "text", "null"], null, null, null));
    }

    [TestMethod]
    public void MultiConvertersHandleNullAndEmptyRequiredInputs()
    {
        MultiValueConverterBase[] converters = [new CalculatorMultiConverter(), new FirstBoolToValueMultiConverter(),
            new FirstEqualsSecondMultiConverter(), new FirstStringStateToOtherStringMultiConverter(),
            new GetDictionaryValueMultiConverter(), new StringFormatMultiConverter()];
        foreach (var converter in converters)
        {
            Assert.AreSame(ConverterResults.UnsetValue, converter.Convert([], null, null, null), converter.GetType().Name);
            Assert.AreSame(ConverterResults.UnsetValue, converter.Convert(null!, null, null, null), converter.GetType().Name);
        }
    }

    [TestMethod]
    public void FormattingValidatesInputsAndPreservesReplacement()
    {
        var c = new StringFormatMultiConverter();
        var unset = ConverterResults.UnsetValue;
        Assert.AreSame(unset, c.Convert([1], null, "{1}", null));
        Assert.AreSame(unset, c.Convert([1], null, "{", null));
        Assert.AreSame(unset, c.Convert([1], null, "{0:Q}", null));
        Assert.AreSame(unset, c.Convert([1, 2], null, null, null));
        Assert.AreSame(unset, c.Convert([unset], null, "{0}", null));
        c.GetReplaceUnsetValue = () => "pending";
        Assert.AreEqual("pending", c.Convert([unset], null, "{0}", null));
        Assert.AreEqual("pending:", c.Convert(["{0}:{1}", unset, null], null, null, null));
        Assert.AreSame(unset, c.Convert([ConverterResults.DoNothing], null, "{0}", null));
        Assert.AreSame(unset, c.Convert([unset, 1], null, null, null));
    }

    [TestMethod]
    public void NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic()
    {
        var unset = ConverterResults.UnsetValue;
        var calculator = new CalculatorMultiConverter();
        Assert.AreSame(unset, calculator.Convert([1, "+", "bad"], null, null, null));
        Assert.AreSame(unset, calculator.Convert([1, "", 2], null, null, null));
        Assert.AreSame(unset, calculator.Convert([1, 2], null, "?", null));
        Assert.AreSame(unset, calculator.Convert([1, 2, 3], null, "+", null));
        Assert.AreSame(unset, calculator.Convert([1, 0], null, "/", null));
        Assert.AreEqual(15, calculator.Convert([2, "+", 3, "*", 3], null, null, null));
        var single = new CalculatorConverter { GetOperatorType = () => ArithmeticOperatorType.Division };
        Assert.AreSame(unset, single.Convert(1, null, 0, null));
        Assert.AreSame(unset, single.Convert("bad", null, 2, null));
        var clamp = new NumberClampConverter();
        foreach (var parameter in new[] { "2", "2,3,4", "x,4", "8,2" })
            Assert.AreSame(unset, clamp.Convert(3, null, parameter, null));
        var compare = new NumberCompareByConverter();
        Assert.AreEqual(true, compare.Convert(-1, null, "-1", null));
        Assert.AreEqual(true, compare.Convert(-1, null, ">-2", null));
        Assert.AreSame(unset, compare.Convert(1, null, ">bad", null));
        Assert.AreSame(unset, new NumberCompareConverter().Convert("bad", null, 1, null));
        Assert.AreSame(unset, new CollectionCountCompareByConverter().Convert(new[] { 1 }, null, ">bad", null));
    }

    [TestMethod]
    public void EnumGuidVersionAndDictionaryValidateOrdinaryFailures()
    {
        var unset = ConverterResults.UnsetValue;
        Assert.AreSame(unset, new EnumsToEnumInfosConverter().Convert(Array.Empty<DayOfWeek>(), null, null, null));
        Assert.AreSame(unset, new EnumsToEnumInfosConverter().Convert(new[] { 1 }, null, null, null));
        Assert.AreEqual(false, new EnumEqualsConverter().Convert(1, null, "Monday", null));
        Assert.AreSame(unset, new GuidToStringConverter().ConvertBack("invalid", null, null, null));
        Assert.AreSame(unset, new GuidToStringConverter().Convert(Guid.Empty, null, "bad", null));
        Assert.AreSame(unset, new VersionToStringConverter().Convert(new Version(1, 2), null, "4", null));
        IDictionary dictionary = new Dictionary<string, object?> { ["null"] = null, ["pending"] = unset };
        var c = new GetDictionaryValueConverter();
        Assert.IsNull(c.Convert(dictionary, null, "null", null));
        Assert.AreSame(unset, c.Convert(dictionary, null, 1, null));
        Assert.AreSame(unset, c.Convert(dictionary, null, unset, null));
        Assert.AreSame(unset, c.Convert(dictionary, null, "pending", null));
    }

    [TestMethod]
    public void DefaultResultIsLazyAndUserExceptionsPropagate()
    {
        object? Fail() => throw new InvalidOperationException("default called on success");
        var dictionary = new Hashtable { ["key"] = 1 };
        Assert.AreEqual(1, new GetDictionaryValueConverter { GetDefaultResult = Fail }.Convert(dictionary, null, "key", null));
        Assert.AreEqual(1, new GetDictionaryValueMultiConverter { GetDefaultResult = Fail }.Convert([dictionary, "key"], null, null, null));
        Assert.AreEqual(1, new FirstOrDefaultConverter { GetDefaultResult = Fail }.Convert(new[] { 1 }, null, null, null));
        Assert.AreEqual(true, new EnumEqualsConverter { GetDefaultResult = Fail }.Convert(DayOfWeek.Monday, null, "Monday", null));
        Assert.IsNotNull(new EnumToEnumInfoConverter { GetDefaultResult = Fail }.Convert(DayOfWeek.Monday, null, null, null));
        Assert.IsNotNull(new EnumsToEnumInfosConverter { GetDefaultResult = Fail }.Convert(new[] { DayOfWeek.Monday }, null, null, null));
        Assert.AreEqual(Guid.Empty, new GuidToStringConverter { GetDefaultResult = Fail }.ConvertBack(Guid.Empty.ToString(), null, null, null));
        var exception = new FormatException("user configuration");
        var c = new BoolToValueConverter<string> { GetTrueValue = () => throw exception };
        Assert.AreSame(exception, Assert.ThrowsExactly<FormatException>(() => c.Convert(true, null, null, null)));
        var formatter = new StringFormatMultiConverter();
        Assert.ThrowsExactly<FormatException>(() => formatter.Convert([new ThrowingValue()], null, "{0}", null));
    }

    [TestMethod]
    public void FirstOrDefaultDisposesEmptyEnumerator()
    {
        var source = new EmptyEnumerable();
        Assert.AreSame(ConverterResults.UnsetValue, new FirstOrDefaultConverter().Convert(source, null, null, null));
        Assert.IsTrue(source.Disposed);
        Assert.IsNull(new FirstOrDefaultConverter().Convert(new object?[] { null }, null, null, null));
    }

    [TestMethod]
    public void UnavailableInputsAreNotConvertedIntoOrdinaryValues()
    {
        ValueConverterBase[] singles = [new BoolToValueConverter<object>(), new BoolToSplitParameterConverter(),
            new CalculatorConverter(), new CollectionCountCompareConverter(), new CollectionCountCompareByConverter(),
            new DateTimeToStringConverter(), new DateTimeOffsetToStringConverter(), new EnumEqualsConverter(),
            new EnumToEnumInfoConverter(), new EnumToEnumInfoDisplayConverter(), new EnumsToEnumInfosConverter(),
            new EqualsConverter(), new EqualsToValueConverter<object>(), new FirstOrDefaultConverter(),
            new GetDictionaryValueConverter(), new GuidToStringConverter(), new NullToBoolConverter(),
            new NumberClampConverter(), new NumberCompareConverter(), new NumberCompareByConverter(),
            new StringCaseConverter(), new StringCheckConverter(), new StringToNumberConverter(),
            new TimeSpanToStringConverter(), new ToBoolConverter(), new ValueToEnumerableConverter(), new VersionToStringConverter()];
        var fallback = new object();
        foreach (var c in singles)
        {
            c.GetDefaultResult = () => fallback;
            foreach (var sentinel in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing })
                Assert.AreSame(fallback, c.Convert(sentinel, null, null, null), c.GetType().Name);
        }
        MultiValueConverterBase[] multis = [new AllBoolToValueMultiConverter<object>(), new AllEqualsMultiConverter<object>(),
            new AnyEqualsMultiConverter<object>(), new EqualsCountMultiConverter<object>()];
        foreach (var c in multis)
        {
            c.GetDefaultResult = () => fallback;
            Assert.AreSame(fallback, c.Convert([ConverterResults.UnsetValue, null], null, null, null), c.GetType().Name);
            Assert.AreSame(fallback, c.Convert(null!, null, null, null), c.GetType().Name);
        }
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("=")]
    [DataRow("!")]
    [DataRow(">")]
    [DataRow("<>3")]
    [DataRow("===2")]
    [DataRow("bad")]
    public void InvalidComparisonParametersReturnDefault(string parameter)
    {
        Assert.AreSame(ConverterResults.UnsetValue, new NumberCompareByConverter().Convert(1, null, parameter, null));
        Assert.AreSame(ConverterResults.UnsetValue, new CollectionCountCompareByConverter().Convert(new[] { 1 }, null, parameter, null));
    }

    private sealed class ThrowingValue
    {
        public override string ToString() => throw new FormatException("user formatting");
    }

    private sealed class EmptyEnumerable : IEnumerable, IEnumerator, IDisposable
    {
        public bool Disposed { get; private set; }
        public object Current => throw new InvalidOperationException();
        public IEnumerator GetEnumerator() => this;
        public bool MoveNext() => false;
        public void Reset() => throw new NotSupportedException();
        public void Dispose() => Disposed = true;
    }
}
