using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class GuidToStringConverterTests
{
    [TestMethod]
    public Task GuidToStringConverter_TwoWayTextBox() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyTwoWay(
                Guid.Empty,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                new TextBox(),
                TextBox.TextProperty,
                new GuidToStringConverter(),
                "00000000-0000-0000-0000-000000000000",
                "00000000-0000-0000-0000-000000000001"
            )
        );

    [TestMethod]
    public Task ReverseDoNothing_PreservesTypedSource() =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrorScope();
            var source = new TypedSource<Guid>(Guid.Empty);
            var converter = new GuidToStringConverter
            {
                DefaultResult = BindingOperations.DoNothing,
            };
            var target = new TextBox();
            using var subscription = target.Bind(
                TextBox.TextProperty,
                new Binding(nameof(source.Value))
                {
                    Source = source,
                    Converter = converter,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                }
            );
            target.SetCurrentValue(TextBox.TextProperty, "invalid");
            PlatformThread.Drain();
            Assert.AreEqual(Guid.Empty, source.Value);
            Assert.AreEqual("invalid", target.Text);
        });

    public static IEnumerable<object[]> SharedCases => GuidToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(GuidToStringConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<GuidToStringConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<GuidToStringConverterSuite.Configuration> CreateShared(
        GuidToStringConverterSuite.Configuration configuration
    )
    {
        var converter = new GuidToStringConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetStringTo is not null)
                converter.StringTo = settings.GetStringTo();
            if (settings.GetFormat is not null)
                converter.Format = settings.GetFormat();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(GuidToStringConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:GuidToStringConverter", () =>
                    new ConverterCase(
                        "GuidToStringConverter",
                        typeof(GuidToStringConverter),
                        () => new GuidToStringConverter(),
                        [Guid.Parse("00000000-0000-0000-0000-000000000001")],
                        "00000000-0000-0000-0000-000000000001",
                        [
                            new(
                                0,
                                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                                "00000000-0000-0000-0000-000000000002"
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
    [ConverterTest(typeof(GuidToStringConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, true);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(GuidToStringConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(GuidToStringConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(GuidToStringConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new GuidToStringConverter());
            var converter = (GuidToStringConverter)c.Create();
            var pristine = new GuidToStringConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                GuidToStringConverter.StringToProperty,
                nameof(GuidToStringConverter.StringTo),
                x => x.StringTo,
                (x, value) => x.StringTo = value,
                true
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                GuidToStringConverter.FormatProperty,
                nameof(GuidToStringConverter.Format),
                x => x.Format,
                (x, value) => x.Format = value,
                "alternate-Format"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                GuidToStringConverter.PreferredCultureProperty,
                nameof(GuidToStringConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (GuidToStringConverter)configured;
        var actual = (GuidToStringConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.StringTo, actual.StringTo);
        ConverterAssertions.ConfigurationEqual(expected.Format, actual.Format);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
