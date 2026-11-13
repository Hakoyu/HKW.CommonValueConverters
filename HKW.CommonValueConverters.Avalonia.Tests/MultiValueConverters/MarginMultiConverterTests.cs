using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class MarginMultiConverterTests
{
    [TestMethod]
    public Task MarginMultiConverter_BorderMarginAndEachSlotNotification() =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrorScope();
            var source = new TestViewModel(1, 2, 3, 4);
            var target = new Border { DataContext = source };
            var binding = new MultiBinding { Converter = new MarginMultiConverter() };
            for (var index = 0; index < 4; index++)
                binding.Bindings.Add(new Binding($"V{index}"));
            using var subscription = target.Bind(Border.MarginProperty, binding);
            Assert.AreEqual(new Thickness(1, 2, 3, 4), target.Margin);
            source.V1 = 7;
            PlatformThread.Drain();
            Assert.AreEqual(new Thickness(1, 7, 3, 4), target.Margin);
            source.V3 = 8;
            PlatformThread.Drain();
            Assert.AreEqual(new Thickness(1, 7, 3, 8), target.Margin);
        });

    [TestMethod]
    public Task MarginMultiConverter_RatioRejectsOverflowForUniformThickness() =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var converter = new MarginMultiConverter { HasRatio = true, DefaultResult = marker };
            Assert.AreEqual(
                new Thickness(3),
                converter.Convert([2, "1,5"], null, null, CultureInfo.GetCultureInfo("fr-FR"))
            );
            Assert.AreSame(
                marker,
                converter.Convert([double.MaxValue, 2], null, null, CultureInfo.InvariantCulture)
            );
            Assert.AreSame(
                marker,
                converter.Convert(
                    [double.MaxValue, 2, 3, 4, 5],
                    null,
                    null,
                    CultureInfo.InvariantCulture
                )
            );
        });

    [TestMethod]
    public Task MarginMultiConverter_MissingSidesDefaultToZeroAndEveryInputIsValidated() =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var converter = new MarginMultiConverter { DefaultResult = marker };
            Assert.AreEqual(new Thickness(), converter.Convert([], null, null, null));
            Assert.AreEqual(new Thickness(2), converter.Convert([2], null, null, null));
            Assert.AreEqual(new Thickness(1, 2, 0, 0), converter.Convert([1, 2], null, null, null));
            Assert.AreEqual(
                new Thickness(1, 2, 3, 0),
                converter.Convert([1, 2, 3], null, null, null)
            );
            Assert.AreEqual(
                new Thickness(1, 0, 3, 0),
                converter.Convert([1, null, 3], null, null, null)
            );
            Assert.AreSame(marker, converter.Convert([1, double.NaN], null, null, null));
            Assert.AreSame(
                marker,
                converter.Convert([1, double.PositiveInfinity], null, null, null)
            );
            Assert.AreSame(marker, converter.Convert([1, 2, 3, 4, 5], null, null, null));
        });

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:MarginMultiConverter", () =>
                    new ConverterCase(
                        "MarginMultiConverter",
                        typeof(MarginMultiConverter),
                        () => new MarginMultiConverter(),
                        [1, 2, 3, 4],
                        new Thickness(1, 2, 3, 4),
                        [
                            new(0, 5, new Thickness(5, 2, 3, 4)),
                            new(1, 6, new Thickness(5, 6, 3, 4)),
                            new(2, 7, new Thickness(5, 6, 7, 4)),
                            new(3, 8, new Thickness(5, 6, 7, 8)),
                        ],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(MarginMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(MarginMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(MarginMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(MarginMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new MarginMultiConverter());
            var converter = (MarginMultiConverter)c.Create();
            var pristine = new MarginMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                MarginMultiConverter.HasRatioProperty,
                nameof(MarginMultiConverter.HasRatio),
                x => x.HasRatio,
                (x, value) => x.HasRatio = value,
                converter.HasRatio is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                MarginMultiConverter.PreferredCultureProperty,
                nameof(MarginMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (MarginMultiConverter)configured;
        var actual = (MarginMultiConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.HasRatio, actual.HasRatio);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }

    [TestMethod]
    public Task DefaultFallbackMatchesPlatformPreset() =>
        PlatformThread.Run(() =>
        {
            var margin = new MarginMultiConverter();
            Assert.AreEqual(
                new Thickness(),
                margin.Convert(["invalid"], typeof(Thickness), null, null)
            );
            margin.DefaultResult = "fallback";
            Assert.AreEqual("fallback", margin.Convert(["invalid"], typeof(Thickness), null, null));
            margin.ClearValue(ConverterBase.DefaultResultProperty);
            Assert.AreEqual(
                new Thickness(),
                margin.Convert([double.NaN], typeof(Thickness), null, null)
            );
            var ratio = new MarginMultiConverter { HasRatio = true };
            Assert.AreEqual(new Thickness(2), ratio.Convert([2, 1], null, null, null));
            Assert.AreEqual(new Thickness(2, 4, 0, 0), ratio.Convert([2, 1, 2], null, null, null));
            Assert.AreEqual(
                new Thickness(2, 4, 6, 8),
                ratio.Convert([2, 1, 2, 3, 4], null, null, null)
            );
        });
}
