using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Styling;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class BoolToStyleConverterTests
{
    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:BoolToStyleConverter", () =>
                    new ConverterCase(
                        "BoolToStyleConverter",
                        typeof(BoolToStyleConverter),
                        () =>
                            new BoolToStyleConverter
                            {
                                TrueValue = CreateStyle("yes-style"),
                                FalseValue = CreateStyle("no-style"),
                            },
                        [true],
                        "yes-style",
                        [new(0, false, "no-style")],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToStyleConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToStyleConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToStyleConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToStyleConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new BoolToStyleConverter());
            var converter = (BoolToStyleConverter)c.Create();
            var pristine = new BoolToStyleConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToStyleConverter.TrueValueProperty,
                nameof(BoolToStyleConverter.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                new Style()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToStyleConverter.FalseValueProperty,
                nameof(BoolToStyleConverter.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                new Style()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToStyleConverter.IsNullableProperty,
                nameof(BoolToStyleConverter.IsNullable),
                x => x.IsNullable,
                (x, value) => x.IsNullable = value,
                converter.IsNullable is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToStyleConverter.NullValueProperty,
                nameof(BoolToStyleConverter.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                new Style()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToStyleConverter.IsInvertedProperty,
                nameof(BoolToStyleConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToStyleConverter.PreferredCultureProperty,
                nameof(BoolToStyleConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (BoolToStyleConverter)configured;
        var actual = (BoolToStyleConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.IsNullable, actual.IsNullable);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }

    private static Style CreateStyle(string tag) =>
        new() { Setters = { new Setter(Control.TagProperty, tag) } };
}
