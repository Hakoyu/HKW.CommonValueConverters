using System.Globalization;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class ToBoolConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<bool?>? GetNullValue { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_BooleanAndIntegerInputs/0",
                    factory => ToBoolConverter_BooleanAndIntegerInputs(factory, true, true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_BooleanAndIntegerInputs/1",
                    factory => ToBoolConverter_BooleanAndIntegerInputs(factory, false, false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_BooleanAndIntegerInputs/2",
                    factory => ToBoolConverter_BooleanAndIntegerInputs(factory, "true", true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_BooleanAndIntegerInputs/3",
                    factory => ToBoolConverter_BooleanAndIntegerInputs(factory, "FALSE", false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_BooleanAndIntegerInputs/4",
                    factory => ToBoolConverter_BooleanAndIntegerInputs(factory, "1", true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_BooleanAndIntegerInputs/5",
                    factory => ToBoolConverter_BooleanAndIntegerInputs(factory, "0", false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_BooleanAndIntegerInputs/6",
                    factory => ToBoolConverter_BooleanAndIntegerInputs(factory, "-1", false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_BooleanAndIntegerInputs/7",
                    factory => ToBoolConverter_BooleanAndIntegerInputs(factory, 2, true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_BooleanAndIntegerInputs/8",
                    factory => ToBoolConverter_BooleanAndIntegerInputs(factory, 0, false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_BooleanAndIntegerInputs/9",
                    factory => ToBoolConverter_BooleanAndIntegerInputs(factory, -2, false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ToBoolConverter_NullPolicyAndUnavailableInputs",
                    factory => ToBoolConverter_NullPolicyAndUnavailableInputs(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "UnavailableInputsAreNotConvertedIntoOrdinaryValues",
                    factory => UnavailableInputsAreNotConvertedIntoOrdinaryValues(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BooleanNumericInputsUseEffectiveCulture",
                    factory => BooleanNumericInputsUseEffectiveCulture(factory)
                ),
            ];
        }
    }

    private static void ToBoolConverter_BooleanAndIntegerInputs(
        Func<Configuration, SingleConversionSubject<Configuration>> factory,
        object value,
        bool expected
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(expected, c.Convert(value, null, null, CultureInfo.InvariantCulture));
    }

    private static void ToBoolConverter_NullPolicyAndUnavailableInputs(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });

        Assert.AreEqual(false, c.Convert(null, null, null, null));
        Assert.AreEqual(false, c.Convert("invalid", null, null, null));
        c.Configuration.GetNullValue = () => true;
        Assert.AreEqual(true, c.Convert(null, null, null, null));
        Assert.AreEqual(true, c.Convert("invalid", null, null, null));
        c.Configuration.GetNullValue = () => null;
        Assert.IsNull(c.Convert(null, null, null, null));
        Assert.IsNull(c.Convert("invalid", null, null, null));
        foreach (
            var unavailable in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing }
        )
            Assert.AreSame(fallback, c.Convert(unavailable, null, null, null));
    }

    private static void UnavailableInputsAreNotConvertedIntoOrdinaryValues(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });
        foreach (var sentinel in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing })
            Assert.AreSame(fallback, c.Convert(sentinel, null, null, null));
    }

    private static void BooleanNumericInputsUseEffectiveCulture(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.PositiveSign = "p";
        var c = factory(new Configuration());
        Assert.AreEqual(true, c.Convert("p1", null, null, culture));
    }
}

[TestClass]
[DoNotParallelize]
public sealed class ToBoolConverterTests
{
    public static IEnumerable<object[]> Cases => ToBoolConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(ToBoolConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<ToBoolConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<ToBoolConverterSuite.Configuration> Create(
        ToBoolConverterSuite.Configuration configuration
    )
    {
        var converter = new ToBoolConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetNullValue is not null)
                    converter.GetNullValue = settings.GetNullValue;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
