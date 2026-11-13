using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class FirstStringStateToOtherStringMultiConverterSuite
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
                new MultiConversionCase<Configuration>(
                    "FirstStringStateToOtherStringMultiConverter",
                    factory => FirstStringStateToOtherStringMultiConverter(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "FirstStringStateToOtherStringMultiConverter_Null",
                    factory => FirstStringStateToOtherStringMultiConverter_Null(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "FirstStringStateToOtherStringMultiConverter_Empty",
                    factory => FirstStringStateToOtherStringMultiConverter_Empty(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "FirstStringStateToOtherStringMultiConverter_WhiteSpace",
                    factory => FirstStringStateToOtherStringMultiConverter_WhiteSpace(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "SelectedBranchesIgnoreUnselectedUnavailableValues",
                    factory => SelectedBranchesIgnoreUnselectedUnavailableValues(factory)
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

    private static void FirstStringStateToOtherStringMultiConverter(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual("S2", c.Convert(["AAA", "S2", "S3", "S4", "S5"], null, null, null));
    }

    private static void FirstStringStateToOtherStringMultiConverter_Null(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual("S3", c.Convert([null, "S2", "S3", "S4", "S5"], null, null, null));
    }

    private static void FirstStringStateToOtherStringMultiConverter_Empty(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual("S4", c.Convert(["", "S2", "S3", "S4", "S5"], null, null, null));
    }

    private static void FirstStringStateToOtherStringMultiConverter_WhiteSpace(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual("S5", c.Convert(["   ", "S2", "S3", "S4", "S5"], null, null, null));
    }

    private static void SelectedBranchesIgnoreUnselectedUnavailableValues(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var unset = ConverterResults.UnsetValue;
        var str = factory(new Configuration());
        Assert.AreEqual("text", str.Convert(["input", "text", unset], null, null, null));
        Assert.AreEqual("null", str.Convert([null, unset, "null"], null, null, null));
        Assert.AreSame(unset, str.Convert([null], null, null, null));
        Assert.AreSame(unset, str.Convert([unset, "text", "null"], null, null, null));
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
public sealed class FirstStringStateToOtherStringMultiConverterTests
{
    public static IEnumerable<object[]> Cases =>
        FirstStringStateToOtherStringMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(
        typeof(FirstStringStateToOtherStringMultiConverter),
        ConverterTestLayer.Behavior
    )]
    public void Behavior(
        MultiConversionCase<FirstStringStateToOtherStringMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static MultiConversionSubject<FirstStringStateToOtherStringMultiConverterSuite.Configuration> Create(
        FirstStringStateToOtherStringMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new FirstStringStateToOtherStringMultiConverter();
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
