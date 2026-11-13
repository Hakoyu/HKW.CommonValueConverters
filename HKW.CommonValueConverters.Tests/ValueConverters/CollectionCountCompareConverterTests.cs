using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class CollectionCountCompareConverterSuite
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
                    "CollectionCountCompareConverter",
                    factory => CollectionCountCompareConverter(factory)
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

    private static void CollectionCountCompareConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(0, c.Convert(new[] { 1, 2, 3 }, null, 3, null));
        Assert.AreEqual(1, c.Convert("test", null, "3", null));
        Assert.AreEqual(-1, c.Convert(new[] { 1, 2 }, null, 3, null));
        Assert.AreEqual(ConverterResults.UnsetValue, c.Convert(new object(), null, 1, null));
        Assert.AreEqual(ConverterResults.UnsetValue, c.Convert(new[] { 1 }, null, "invalid", null));
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
public sealed class CollectionCountCompareConverterTests
{
    public static IEnumerable<object[]> Cases => CollectionCountCompareConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(CollectionCountCompareConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<CollectionCountCompareConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<CollectionCountCompareConverterSuite.Configuration> Create(
        CollectionCountCompareConverterSuite.Configuration configuration
    )
    {
        var converter = new CollectionCountCompareConverter();
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
