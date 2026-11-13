using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class TimeSpanToStringConverterTests
{
    [TestMethod]
    public Task TimeSpanToStringConverter_TwoWayTextBox() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyTwoWay(
                TimeSpan.FromHours(1),
                TimeSpan.FromHours(2),
                new TextBox(),
                TextBox.TextProperty,
                new TimeSpanToStringConverter { Format = "c" },
                "01:00:00",
                "02:00:00"
            )
        );

    [TestMethod]
    public Task DateAndTimeFormats_UseConfiguredFormatAndParameterOverride() =>
        PlatformThread.Run(() =>
        {
            var french = CultureInfo.GetCultureInfo("fr-FR");
            Assert.AreEqual(
                TimeSpan.FromMilliseconds(1250),
                new TimeSpanToStringConverter().ConvertBack("0:00:01,25", null, null, french)
            );
        });

    public static IEnumerable<object[]> SharedCases => TimeSpanToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(TimeSpanToStringConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<TimeSpanToStringConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<TimeSpanToStringConverterSuite.Configuration> CreateShared(
        TimeSpanToStringConverterSuite.Configuration configuration
    )
    {
        var converter = new TimeSpanToStringConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetFormat is not null)
                converter.Format = settings.GetFormat();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(TimeSpanToStringConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:TimeSpanToStringConverter", () =>
                    new ConverterCase(
                        "TimeSpanToStringConverter",
                        typeof(TimeSpanToStringConverter),
                        () => new TimeSpanToStringConverter { Format = "c" },
                        [TimeSpan.FromHours(1)],
                        "01:00:00",
                        [new(0, TimeSpan.FromHours(2), "02:00:00")],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(TimeSpanToStringConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, true);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(TimeSpanToStringConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(TimeSpanToStringConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(TimeSpanToStringConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new TimeSpanToStringConverter());
            var converter = (TimeSpanToStringConverter)c.Create();
            var pristine = new TimeSpanToStringConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                TimeSpanToStringConverter.FormatProperty,
                nameof(TimeSpanToStringConverter.Format),
                x => x.Format,
                (x, value) => x.Format = value,
                "alternate-Format"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                TimeSpanToStringConverter.PreferredCultureProperty,
                nameof(TimeSpanToStringConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (TimeSpanToStringConverter)configured;
        var actual = (TimeSpanToStringConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Format, actual.Format);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
