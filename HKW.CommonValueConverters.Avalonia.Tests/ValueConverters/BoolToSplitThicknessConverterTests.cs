using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class BoolToSplitThicknessConverterTests
{
    [TestMethod]
    public Task BoolToSplitThicknessConverter_BorderMargin() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyBoolean(
                new Border(),
                Border.MarginProperty,
                new BoolToSplitThicknessConverter { Separator = "|" },
                new Thickness(2),
                new Thickness(4),
                "2|4"
            )
        );

    [TestMethod]
    public Task BoolToSplitThicknessConverter_UsesEffectiveCulture() =>
        PlatformThread.Run(() =>
        {
            var saved = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                var converter = new BoolToSplitThicknessConverter { Separator = "|" };
                Assert.AreEqual(
                    new Thickness(1.5),
                    converter.Convert(true, null, "1.5|2.5", CultureInfo.GetCultureInfo("en-US"))
                );
                Assert.AreEqual(
                    new Thickness(1, 2, 3, 4),
                    converter.Convert(true, null, "1,2,3,4|0", CultureInfo.InvariantCulture)
                );
            }
            finally
            {
                CultureInfo.CurrentCulture = saved;
            }
        });

    [TestMethod]
    [DataRow("invalid")]
    [DataRow("NaN")]
    [DataRow("Infinity")]
    [DataRow("1e999")]
    public Task BoolToSplitThicknessConverter_InvalidValuesUseFallback(string text) =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var converter = new BoolToSplitThicknessConverter
            {
                Separator = "|",
                DefaultResult = marker,
            };
            Assert.AreSame(
                marker,
                converter.Convert(true, null, text + "|0", CultureInfo.InvariantCulture)
            );
        });

    [TestMethod]
    public Task SplitDelegateConfiguration_RemainsLiveAfterConstruction() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToSplitThicknessConverter { Separator = "|" };
            Assert.AreEqual(
                new Thickness(2),
                converter.Convert(true, null, "2|3", CultureInfo.InvariantCulture)
            );
            converter.ConvertSplitValue = (text, c) => "custom:" + text;
            Assert.AreEqual(
                "custom:2",
                converter.Convert(true, null, "2|3", CultureInfo.InvariantCulture)
            );
        });

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:BoolToSplitThicknessConverter", () =>
                    new ConverterCase(
                        "BoolToSplitThicknessConverter",
                        typeof(BoolToSplitThicknessConverter),
                        () => new BoolToSplitThicknessConverter { Separator = "|" },
                        [true],
                        new Thickness(2),
                        [new(0, false, new Thickness(4))],
                        "2|4|0",
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSplitThicknessConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSplitThicknessConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSplitThicknessConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSplitThicknessConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new BoolToSplitThicknessConverter());
            var converter = (BoolToSplitThicknessConverter)c.Create();
            var pristine = new BoolToSplitThicknessConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToSplitThicknessConverter.SeparatorProperty,
                nameof(BoolToSplitThicknessConverter.Separator),
                x => x.Separator,
                (x, value) => x.Separator = value,
                "alternate-Separator"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToSplitThicknessConverter.PreferredCultureProperty,
                nameof(BoolToSplitThicknessConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (BoolToSplitThicknessConverter)configured;
        var actual = (BoolToSplitThicknessConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Separator, actual.Separator);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
