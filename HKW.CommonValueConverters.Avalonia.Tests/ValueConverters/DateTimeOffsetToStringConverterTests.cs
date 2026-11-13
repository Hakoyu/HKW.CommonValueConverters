using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class DateTimeOffsetToStringConverterTests
{
    [TestMethod]
    public Task DateTimeOffsetToStringConverter_TwoWayTextBox() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyTwoWay(
                new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 2, 3, 0, 0, 0, TimeSpan.Zero),
                new TextBox(),
                TextBox.TextProperty,
                new DateTimeOffsetToStringConverter { Format = "O" },
                new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)
                    .ToLocalTime()
                    .ToString("O", CultureInfo.InvariantCulture),
                "2026-02-03T00:00:00.0000000+00:00"
            )
        );

    public static IEnumerable<object[]> SharedCases => DateTimeOffsetToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(DateTimeOffsetToStringConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<DateTimeOffsetToStringConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<DateTimeOffsetToStringConverterSuite.Configuration> CreateShared(
        DateTimeOffsetToStringConverterSuite.Configuration configuration
    )
    {
        var converter = new DateTimeOffsetToStringConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetFormat is not null)
                converter.Format = settings.GetFormat();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(DateTimeOffsetToStringConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:DateTimeOffsetToStringConverter", () =>
                    new ConverterCase(
                        "DateTimeOffsetToStringConverter",
                        typeof(DateTimeOffsetToStringConverter),
                        () => new DateTimeOffsetToStringConverter { Format = "yyyy-MM-dd" },
                        [new DateTimeOffset(new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local))],
                        "2026-01-02",
                        [
                            new(
                                0,
                                new DateTimeOffset(
                                    new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Local)
                                ),
                                "2026-02-03"
                            ),
                        ],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DateTimeOffsetToStringConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, true);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DateTimeOffsetToStringConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DateTimeOffsetToStringConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DateTimeOffsetToStringConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new DateTimeOffsetToStringConverter());
            var converter = (DateTimeOffsetToStringConverter)c.Create();
            var pristine = new DateTimeOffsetToStringConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                DateTimeOffsetToStringConverter.FormatProperty,
                nameof(DateTimeOffsetToStringConverter.Format),
                x => x.Format,
                (x, value) => x.Format = value,
                "alternate-Format"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                DateTimeOffsetToStringConverter.PreferredCultureProperty,
                nameof(DateTimeOffsetToStringConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (DateTimeOffsetToStringConverter)configured;
        var actual = (DateTimeOffsetToStringConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Format, actual.Format);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
