using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class ValueToEnumerableConverterSuite
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
                new SingleConversionCase<Configuration>(
                    "ValueToEnumerableConverter_WrapsOneValueWithoutFlattening",
                    factory => ValueToEnumerableConverter_WrapsOneValueWithoutFlattening(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ValueToEnumerableConverter_NullAndUnavailableInputs",
                    factory => ValueToEnumerableConverter_NullAndUnavailableInputs(factory)
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

    private static void ValueToEnumerableConverter_WrapsOneValueWithoutFlattening(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        foreach (var value in new object[] { new object(), "text", new[] { 1, 2 }, 42 })
        {
            var result = c.Convert(value, null, null, null) as object[];
            Assert.IsNotNull(result);
            Assert.HasCount(1, result);
            Assert.AreSame(value, result[0]);
        }
    }

    private static void ValueToEnumerableConverter_NullAndUnavailableInputs(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });

        Assert.IsNull(c.Convert(null, null, null, null));
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
}

[TestClass]
public sealed class ValueToEnumerableConverterTests
{
    public static IEnumerable<object[]> Cases => ValueToEnumerableConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(ValueToEnumerableConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<ValueToEnumerableConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<ValueToEnumerableConverterSuite.Configuration> Create(
        ValueToEnumerableConverterSuite.Configuration configuration
    )
    {
        var converter = new ValueToEnumerableConverter();
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
