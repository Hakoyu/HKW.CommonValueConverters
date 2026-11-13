using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class EqualsCountMultiConverterSuite
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
                new MultiConversionCase<Configuration>("EqualsCountMultiConverter", factory =>
                    EqualsCountMultiConverter(factory)
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

    private static void EqualsCountMultiConverter(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetValue = () => "Value" });

        Assert.AreEqual(2, c.Convert(["Value", "Other", "Value"], null, null, null));
        Assert.AreEqual(1, c.Convert(["Parameter", "Other"], null, "Parameter", null));
    }

    private static void NullEqualityIsConsistent(
        Func<Configuration, MultiConversionSubject<Configuration>> factory,
        bool inverted
    )
    {
        var c = factory(new Configuration { GetIsInverted = () => inverted });
        Assert.AreEqual(inverted ? 1 : 2, c.Convert([null, 1, null], null, null, null));
    }

    private static void EmptyAggregatesRetainDefinedResults(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreEqual(0, c.Convert([], null, null, null));
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
public sealed class EqualsCountMultiConverterTests
{
    public static IEnumerable<object[]> Cases =>
        EqualsCountMultiConverterSuite.Cases.Select(row =>
        {
            var scenario =
                (MultiConversionCase<EqualsCountMultiConverterSuite.Configuration>)row[0];
            return new object[] { scenario with { Id = "string/" + scenario.Id } };
        });

    public static IEnumerable<object[]> ObjectCases =>
        EqualsCountMultiConverterSuite.Cases.Select(row =>
        {
            var scenario =
                (MultiConversionCase<EqualsCountMultiConverterSuite.Configuration>)row[0];
            return new object[] { scenario with { Id = "object/" + scenario.Id } };
        });

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(EqualsCountMultiConverter<>), ConverterTestLayer.Behavior)]
    public void Behavior(
        MultiConversionCase<EqualsCountMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    [TestMethod]
    [DynamicData(nameof(ObjectCases))]
    [ConverterTest(typeof(EqualsCountMultiConverter<>), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        MultiConversionCase<EqualsCountMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(CreateObject);

    private static MultiConversionSubject<EqualsCountMultiConverterSuite.Configuration> CreateObject(
        EqualsCountMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new EqualsCountMultiConverter<object?>();
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

    private static MultiConversionSubject<EqualsCountMultiConverterSuite.Configuration> Create(
        EqualsCountMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new EqualsCountMultiConverter<string>();
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
}
