using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class DateTimeToStringConverterTests
{
    [TestMethod]
    public Task DateTimeToStringConverter_TwoWayTextBox() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyTwoWay(
                new DateTime(2026, 1, 2),
                new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Local).ToUniversalTime(),
                new TextBox(),
                TextBox.TextProperty,
                new DateTimeToStringConverter { Format = "yyyy-MM-dd" },
                "2026-01-02",
                "2026-02-03"
            )
        );

    [TestMethod]
    public Task DateAndTimeFormats_UseConfiguredFormatAndParameterOverride() =>
        PlatformThread.Run(() =>
        {
            var french = CultureInfo.GetCultureInfo("fr-FR");
            var converter = new DateTimeToStringConverter { Format = "dd MMMM yyyy" };
            Assert.AreEqual(
                "14 septembre 2026",
                converter.Convert(new DateTime(2026, 9, 14), null, null, french)
            );
            Assert.AreEqual(
                "2026/09/14",
                converter.Convert(new DateTime(2026, 9, 14), null, "yyyy/MM/dd", french)
            );
        });

    public static IEnumerable<object[]> SharedCases => DateTimeToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(DateTimeToStringConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<DateTimeToStringConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<DateTimeToStringConverterSuite.Configuration> CreateShared(
        DateTimeToStringConverterSuite.Configuration configuration
    )
    {
        var converter = new DateTimeToStringConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetFormat is not null)
                converter.Format = settings.GetFormat();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(DateTimeToStringConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:DateTimeToStringConverter", () =>
                    new ConverterCase(
                        "DateTimeToStringConverter",
                        typeof(DateTimeToStringConverter),
                        () => new DateTimeToStringConverter { Format = "yyyy-MM-dd" },
                        [new DateTime(2026, 1, 2)],
                        "2026-01-02",
                        [new(0, new DateTime(2026, 2, 3), "2026-02-03")],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DateTimeToStringConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, true);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DateTimeToStringConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DateTimeToStringConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DateTimeToStringConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new DateTimeToStringConverter());
            var converter = (DateTimeToStringConverter)c.Create();
            var pristine = new DateTimeToStringConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                DateTimeToStringConverter.FormatProperty,
                nameof(DateTimeToStringConverter.Format),
                x => x.Format,
                (x, value) => x.Format = value,
                "alternate-Format"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                DateTimeToStringConverter.PreferredCultureProperty,
                nameof(DateTimeToStringConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (DateTimeToStringConverter)configured;
        var actual = (DateTimeToStringConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Format, actual.Format);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
