using System.Globalization;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class StringCaseConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("StringCaseConverter", factory =>
                    StringCaseConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ExplicitNullAndDoNothingFallback",
                    factory => ExplicitNullAndDoNothingFallback(factory)
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
                    "InstancePreferenceAffectsRealConverters",
                    factory => InstancePreferenceAffectsRealConverters(factory)
                ),
            ];
        }
    }

    private static void ExplicitNullAndDoNothingFallback(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var converter = factory(new Configuration());
        Assert.AreSame(ConverterResults.UnsetValue, converter.Convert(1, null, null, null));
        converter.Configuration.GetDefaultResult = () => null;
        Assert.IsNull(converter.Convert(1, null, null, null));
        converter.Configuration.GetDefaultResult = () => ConverterResults.DoNothing;
        Assert.AreSame(ConverterResults.DoNothing, converter.Convert(1, null, null, null));
    }

    private static void StringCaseConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual("VALUE", c.Convert("value", null, "U", CultureInfo.InvariantCulture));
        Assert.AreEqual("value", c.Convert("VALUE", null, "L", CultureInfo.InvariantCulture));
        Assert.AreEqual("Value", c.Convert("value", null, "T", CultureInfo.InvariantCulture));
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert("value", null, "Unknown", CultureInfo.InvariantCulture)
        );
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

    private static void InstancePreferenceAffectsRealConverters(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var saved = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("tr-TR");
            var converter = factory(
                new Configuration { PreferredCulture = PreferredCulture.CurrentUICulture }
            );
            Assert.AreEqual("İ", converter.Convert("i", null, "U", CultureInfo.InvariantCulture));
        }
        finally
        {
            CultureInfo.CurrentUICulture = saved;
        }
    }
}

[TestClass]
[DoNotParallelize]
public sealed class StringCaseConverterTests
{
    public static IEnumerable<object[]> Cases => StringCaseConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(StringCaseConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<StringCaseConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<StringCaseConverterSuite.Configuration> Create(
        StringCaseConverterSuite.Configuration configuration
    )
    {
        var converter = new StringCaseConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
