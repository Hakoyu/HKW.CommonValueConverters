using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class AllBoolToValueMultiConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<string>? GetTrueValue { get; set; }
        public Func<string>? GetFalseValue { get; set; }
        public Func<string>? GetNullValue { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new MultiConversionCase<Configuration>("AllBoolToValueMultiConverter", factory =>
                    AllBoolToValueMultiConverter(factory)
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

    private static void AllBoolToValueMultiConverter(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(
            new Configuration
            {
                GetTrueValue = () => "True",
                GetFalseValue = () => "False",
                GetNullValue = () => "Null",
                GetDefaultResult = () => "Default",
            }
        );

        Assert.AreEqual("True", c.Convert([true, true], null, null, null));
        Assert.AreEqual("False", c.Convert([false, false], null, null, null));
        Assert.AreEqual("Null", c.Convert([null, null], null, null, null));
        Assert.AreEqual("Default", c.Convert([true, false], null, null, null));
    }

    private static void EmptyAggregatesRetainDefinedResults(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetTrueValue = () => "true" });
        Assert.AreEqual("true", c.Convert([], null, null, null));
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
public sealed class AllBoolToValueMultiConverterTests
{
    public static IEnumerable<object[]> Cases =>
        AllBoolToValueMultiConverterSuite.Cases.Select(row =>
        {
            var scenario =
                (MultiConversionCase<AllBoolToValueMultiConverterSuite.Configuration>)row[0];
            return new object[] { scenario with { Id = "string/" + scenario.Id } };
        });

    public static IEnumerable<object[]> ObjectCases =>
        AllBoolToValueMultiConverterSuite.Cases.Select(row =>
        {
            var scenario =
                (MultiConversionCase<AllBoolToValueMultiConverterSuite.Configuration>)row[0];
            return new object[] { scenario with { Id = "object/" + scenario.Id } };
        });

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter<>), ConverterTestLayer.Behavior)]
    public void Behavior(
        MultiConversionCase<AllBoolToValueMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    [TestMethod]
    [DynamicData(nameof(ObjectCases))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter<>), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        MultiConversionCase<AllBoolToValueMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(CreateObject);

    private static MultiConversionSubject<AllBoolToValueMultiConverterSuite.Configuration> CreateObject(
        AllBoolToValueMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new AllBoolToValueMultiConverter<object?>();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetTrueValue is not null)
                    converter.GetTrueValue = settings.GetTrueValue;
                if (settings.GetFalseValue is not null)
                    converter.GetFalseValue = settings.GetFalseValue;
                if (settings.GetNullValue is not null)
                    converter.GetNullValue = settings.GetNullValue;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }

    private static MultiConversionSubject<AllBoolToValueMultiConverterSuite.Configuration> Create(
        AllBoolToValueMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new AllBoolToValueMultiConverter<string>();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetTrueValue is not null)
                    converter.GetTrueValue = settings.GetTrueValue;
                if (settings.GetFalseValue is not null)
                    converter.GetFalseValue = settings.GetFalseValue;
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
