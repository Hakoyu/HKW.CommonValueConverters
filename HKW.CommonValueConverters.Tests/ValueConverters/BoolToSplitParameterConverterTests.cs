using System.Globalization;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class BoolToSplitParameterConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<string>? GetSeparator { get; set; }
        public Func<string, CultureInfo, object>? ConvertSplitValue { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("BoolToSplitParameterConverter", factory =>
                    BoolToSplitParameterConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BoolToSplitParameterConverter_OtherSeparator",
                    factory => BoolToSplitParameterConverter_OtherSeparator(factory)
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

    private static void BoolToSplitParameterConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual("True", c.Convert(true, null, "True,False,Null", null));
        Assert.AreEqual("False", c.Convert(false, null, "True,False,Null", null));
        Assert.AreEqual("Null", c.Convert(null, null, "True,False,Null", null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(true, null, null, null));
    }

    private static void BoolToSplitParameterConverter_OtherSeparator(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetSeparator = () => " " });

        Assert.AreEqual("True", c.Convert(true, null, "True False Null", null));
        Assert.AreEqual("False", c.Convert(false, null, "True False Null", null));
        Assert.AreEqual("Null", c.Convert(null, null, "True False Null", null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(true, null, null, null));
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
        Assert.AreEqual("yes", c.Convert("p1", null, "yes,no", culture));
    }
}

[TestClass]
[DoNotParallelize]
public sealed class BoolToSplitParameterConverterTests
{
    public static IEnumerable<object[]> Cases => BoolToSplitParameterConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(BoolToSplitParameterConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<BoolToSplitParameterConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<BoolToSplitParameterConverterSuite.Configuration> Create(
        BoolToSplitParameterConverterSuite.Configuration configuration
    )
    {
        var converter = new BoolToSplitParameterConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetSeparator is not null)
                    converter.GetSeparator = settings.GetSeparator;
                if (settings.ConvertSplitValue is not null)
                    converter.ConvertSplitValue = settings.ConvertSplitValue;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
