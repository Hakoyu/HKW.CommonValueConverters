using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class FirstBoolToValueMultiConverterSuite
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
                new MultiConversionCase<Configuration>("FirstBoolToValueMultiConverter", factory =>
                    FirstBoolToValueMultiConverter(factory)
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

    private static void FirstBoolToValueMultiConverter(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetDefaultResult = () => "Default" });

        Assert.AreEqual("True", c.Convert([true, "True", "False", "Null"], null, null, null));
        Assert.AreEqual("False", c.Convert([false, "True", "False", "Null"], null, null, null));
        Assert.AreEqual("Null", c.Convert([null, "True", "False", "Null"], null, null, null));
        Assert.AreEqual("Default", c.Convert([true], null, null, null));
    }

    private static void SelectedBranchesIgnoreUnselectedUnavailableValues(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var unset = ConverterResults.UnsetValue;
        var c = factory(new Configuration());
        Assert.AreEqual("yes", c.Convert([true, "yes", unset, unset], null, null, null));
        Assert.IsNull(c.Convert([false, unset, null], null, null, null));
        Assert.AreSame(unset, c.Convert([true, unset, "no"], null, null, null));
        Assert.AreSame(unset, c.Convert([false, "yes"], null, null, null));
        Assert.AreSame(unset, c.Convert([unset, "yes", "no"], null, null, null));
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
public sealed class FirstBoolToValueMultiConverterTests
{
    public static IEnumerable<object[]> Cases => FirstBoolToValueMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(FirstBoolToValueMultiConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        MultiConversionCase<FirstBoolToValueMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static MultiConversionSubject<FirstBoolToValueMultiConverterSuite.Configuration> Create(
        FirstBoolToValueMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new FirstBoolToValueMultiConverter();
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
