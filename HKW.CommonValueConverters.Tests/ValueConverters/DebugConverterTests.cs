using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class DebugConverterSuite
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
                new SingleConversionCase<Configuration>("DebugConverter", factory =>
                    DebugConverter(factory)
                ),
            ];
        }
    }

    private static void DebugConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        var value = new object();

        Assert.AreSame(value, c.Convert(value, null, null, null));
        Assert.AreSame(value, c.ConvertBack(value, null, null, null));
    }
}

[TestClass]
public sealed class DebugConverterTests
{
    public static IEnumerable<object[]> Cases => DebugConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(DebugConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<DebugConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<DebugConverterSuite.Configuration> Create(
        DebugConverterSuite.Configuration configuration
    )
    {
        var converter = new DebugConverter();
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
