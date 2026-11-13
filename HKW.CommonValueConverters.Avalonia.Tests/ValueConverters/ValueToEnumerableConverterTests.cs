using System.Collections;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class ValueToEnumerableConverterTests
{
    [TestMethod]
    public Task ValueToEnumerableConverter_KeepsArrayAsOneElementAndPreservesNull() =>
        PlatformThread.Run(() =>
        {
            var value = new[] { 1, 2 };
            var converter = new ValueToEnumerableConverter();
            var result = (object[])converter.Convert(value, typeof(IEnumerable), null, null)!;
            Assert.HasCount(1, result);
            Assert.AreSame(value, result[0]);
            Assert.IsNull(converter.Convert(null, null, null, null));
        });

    public static IEnumerable<object[]> SharedCases => ValueToEnumerableConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(ValueToEnumerableConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<ValueToEnumerableConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<ValueToEnumerableConverterSuite.Configuration> CreateShared(
        ValueToEnumerableConverterSuite.Configuration configuration
    )
    {
        var converter = new ValueToEnumerableConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(ValueToEnumerableConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:ValueToEnumerableConverter", () =>
                    new ConverterCase(
                        "ValueToEnumerableConverter",
                        typeof(ValueToEnumerableConverter),
                        () => new ValueToEnumerableConverter(),
                        ["before"],
                        new object[] { "before" },
                        [new(0, "after", new object[] { "after" })],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(ValueToEnumerableConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(ValueToEnumerableConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(ValueToEnumerableConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(ValueToEnumerableConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new ValueToEnumerableConverter());
            var converter = (ValueToEnumerableConverter)c.Create();
            var pristine = new ValueToEnumerableConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                ValueToEnumerableConverter.PreferredCultureProperty,
                nameof(ValueToEnumerableConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (ValueToEnumerableConverter)configured;
        var actual = (ValueToEnumerableConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
