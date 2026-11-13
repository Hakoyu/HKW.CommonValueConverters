using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class VersionToStringConverterSuite
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
                new SingleConversionCase<Configuration>("VersionToStringConverter", factory =>
                    VersionToStringConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "EnumGuidVersionAndDictionaryValidateOrdinaryFailures",
                    factory => EnumGuidVersionAndDictionaryValidateOrdinaryFailures(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "UnavailableInputsAreNotConvertedIntoOrdinaryValues",
                    factory => UnavailableInputsAreNotConvertedIntoOrdinaryValues(factory)
                ),
            ];
        }
    }

    private static void VersionToStringConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        var value = new Version(1, 2, 3, 4);

        Assert.AreEqual("1.2.3.4", c.Convert(value, null, null, null));
        Assert.AreEqual("1.2", c.Convert(value, null, "2", null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert("1.2.3.4", null, null, null));
    }

    private static void EnumGuidVersionAndDictionaryValidateOrdinaryFailures(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        Assert.AreSame(
            ConverterResults.UnsetValue,
            factory(new Configuration()).Convert(new Version(1, 2), null, "4", null)
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
}

[TestClass]
public sealed class VersionToStringConverterTests
{
    public static IEnumerable<object[]> Cases => VersionToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(VersionToStringConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<VersionToStringConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<VersionToStringConverterSuite.Configuration> Create(
        VersionToStringConverterSuite.Configuration configuration
    )
    {
        var converter = new VersionToStringConverter();
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
