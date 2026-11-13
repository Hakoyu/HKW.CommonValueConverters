using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class FirstEqualsSecondMultiConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
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
                new MultiConversionCase<Configuration>("FirstEqualsSecondMultiConverter", factory =>
                    FirstEqualsSecondMultiConverter(factory)
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
                    "MultiConvertersHandleNullAndEmptyRequiredInputs",
                    factory => MultiConvertersHandleNullAndEmptyRequiredInputs(factory)
                ),
            ];
        }
    }

    private static void FirstEqualsSecondMultiConverter(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(true, c.Convert(["Value", "Value"], null, null, null));
        Assert.AreEqual(false, c.Convert(["Value", "Other"], null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(["Value"], null, null, null));
    }

    private static void NullEqualityIsConsistent(
        Func<Configuration, MultiConversionSubject<Configuration>> factory,
        bool inverted
    )
    {
        var c = factory(new Configuration { GetIsInverted = () => inverted });
        Assert.AreEqual(inverted is false, c.Convert([null, null], null, null, null));
        Assert.AreEqual(inverted, c.Convert([null, 1], null, null, null));
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert([ConverterResults.UnsetValue, ConverterResults.UnsetValue], null, null, null)
        );
    }

    private static void MultiConvertersHandleNullAndEmptyRequiredInputs(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert([], null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(null!, null, null, null));
    }
}

[TestClass]
public sealed class FirstEqualsSecondMultiConverterTests
{
    public static IEnumerable<object[]> Cases => FirstEqualsSecondMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(FirstEqualsSecondMultiConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        MultiConversionCase<FirstEqualsSecondMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static MultiConversionSubject<FirstEqualsSecondMultiConverterSuite.Configuration> Create(
        FirstEqualsSecondMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new FirstEqualsSecondMultiConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
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
