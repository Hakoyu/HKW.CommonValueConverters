using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class AllEqualsMultiConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<string>? GetValue { get; set; }
        public Func<bool>? GetIsStringEquals { get; set; }
        public Func<bool>? GetIsInverted { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new MultiConversionCase<Configuration>("AllEqualsMultiConverter", factory =>
                    AllEqualsMultiConverter(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>("NullEqualityIsConsistent/0", factory =>
                    NullEqualityIsConsistent(factory, false)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>("NullEqualityIsConsistent/1", factory =>
                    NullEqualityIsConsistent(factory, true)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "EmptyAggregatesRetainDefinedResults",
                    factory => EmptyAggregatesRetainDefinedResults(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "UnavailableInputsAreNotConvertedIntoOrdinaryValues",
                    factory => UnavailableInputsAreNotConvertedIntoOrdinaryValues(factory)
                ),
            ];
        }
    }

    private static void AllEqualsMultiConverter(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetValue = () => "Value" });

        Assert.AreEqual(true, c.Convert(["Value", "Value"], null, null, null));
        Assert.AreEqual(false, c.Convert(["Value", "Other"], null, null, null));
        Assert.AreEqual(true, c.Convert(["Parameter", "Parameter"], null, "Parameter", null));
    }

    private static void NullEqualityIsConsistent(
        Func<Configuration, MultiConversionSubject<Configuration>> factory,
        bool inverted
    )
    {
        var c = factory(new Configuration { GetIsInverted = () => inverted });
        Assert.AreEqual(inverted is false, c.Convert([null, null], null, null, null));
    }

    private static void EmptyAggregatesRetainDefinedResults(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreEqual(true, c.Convert([], null, null, null));
    }

    private static void UnavailableInputsAreNotConvertedIntoOrdinaryValues(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });
        Assert.AreSame(fallback, c.Convert([ConverterResults.UnsetValue, null], null, null, null));
        Assert.AreSame(fallback, c.Convert(null!, null, null, null));
    }
}

[TestClass]
public sealed class AllEqualsMultiConverterTests
{
    public static IEnumerable<object[]> Cases =>
        AllEqualsMultiConverterSuite.Cases.Select(row =>
        {
            var scenario = (MultiConversionCase<AllEqualsMultiConverterSuite.Configuration>)row[0];
            return new object[] { scenario with { Id = "string/" + scenario.Id } };
        });

    public static IEnumerable<object[]> ObjectCases =>
        AllEqualsMultiConverterSuite.Cases.Select(row =>
        {
            var scenario = (MultiConversionCase<AllEqualsMultiConverterSuite.Configuration>)row[0];
            return new object[] { scenario with { Id = "object/" + scenario.Id } };
        });

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(AllEqualsMultiConverter<>), ConverterTestLayer.Behavior)]
    public void Behavior(
        MultiConversionCase<AllEqualsMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    [TestMethod]
    [DynamicData(nameof(ObjectCases))]
    [ConverterTest(typeof(AllEqualsMultiConverter<>), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        MultiConversionCase<AllEqualsMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(CreateObject);

    private static MultiConversionSubject<AllEqualsMultiConverterSuite.Configuration> CreateObject(
        AllEqualsMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new AllEqualsMultiConverter<object?>();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetValue is not null)
                    converter.GetValue = settings.GetValue;
                if (settings.GetIsStringEquals is not null)
                    converter.GetIsStringEquals = settings.GetIsStringEquals;
                if (settings.GetIsInverted is not null)
                    converter.GetIsInverted = settings.GetIsInverted;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }

    private static MultiConversionSubject<AllEqualsMultiConverterSuite.Configuration> Create(
        AllEqualsMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new AllEqualsMultiConverter<string>();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetValue is not null)
                    converter.GetValue = settings.GetValue;
                if (settings.GetIsStringEquals is not null)
                    converter.GetIsStringEquals = settings.GetIsStringEquals;
                if (settings.GetIsInverted is not null)
                    converter.GetIsInverted = settings.GetIsInverted;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }

    [TestMethod]
    public void UnsupportedReverseConversionRemainsExplicit()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
            new AllEqualsMultiConverter<object>().ConvertBack(null, [], null, null)
        );
    }
}
