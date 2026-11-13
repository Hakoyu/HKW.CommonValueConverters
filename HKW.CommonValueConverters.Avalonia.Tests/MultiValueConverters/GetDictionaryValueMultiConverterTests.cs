using System.Collections;
using Avalonia;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class GetDictionaryValueMultiConverterTests
{
    [TestMethod]
    public Task SingleAndMultiBoundaries_RejectSentinelsWithoutMutatingInputs() =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var dictionary = new Hashtable
            {
                ["nil"] = null,
                ["array"] = new[] { 1, 2 },
                ["unset"] = AvaloniaProperty.UnsetValue,
            };
            var multi = new GetDictionaryValueMultiConverter { DefaultResult = marker };
            foreach (
                var invalid in new object?[][]
                {
                    [],
                    [dictionary],
                    [dictionary, "missing"],
                    [dictionary, null],
                    [AvaloniaProperty.UnsetValue, "nil"],
                }
            )
            {
                var snapshot = invalid.ToArray();
                Assert.AreSame(marker, multi.Convert(invalid, null, null, null));
                CollectionAssert.AreEqual(snapshot, invalid);
            }
            Assert.IsNull(multi.Convert([dictionary, "nil"], null, null, null));
            Assert.AreSame(
                dictionary["array"],
                multi.Convert([dictionary, "array"], null, null, null)
            );
        });

    public static IEnumerable<object[]> SharedCases => GetDictionaryValueMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(GetDictionaryValueMultiConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        MultiConversionCase<GetDictionaryValueMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static MultiConversionSubject<GetDictionaryValueMultiConverterSuite.Configuration> CreateShared(
        GetDictionaryValueMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new GetDictionaryValueMultiConverter();
        var subject = AvaloniaConversionAdapter.Multi(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(GetDictionaryValueMultiConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:GetDictionaryValueMultiConverter", () =>
                    new ConverterCase(
                        "GetDictionaryValueMultiConverter",
                        typeof(GetDictionaryValueMultiConverter),
                        () => new GetDictionaryValueMultiConverter(),
                        [new Hashtable { ["a"] = "first", ["b"] = "second" }, "a"],
                        "first",
                        [
                            new(1, "b", "second"),
                            new(0, new Hashtable { ["b"] = "changed" }, "changed"),
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
    [ConverterTest(typeof(GetDictionaryValueMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(GetDictionaryValueMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(GetDictionaryValueMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(GetDictionaryValueMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new GetDictionaryValueMultiConverter());
            var converter = (GetDictionaryValueMultiConverter)c.Create();
            var pristine = new GetDictionaryValueMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                GetDictionaryValueMultiConverter.PreferredCultureProperty,
                nameof(GetDictionaryValueMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (GetDictionaryValueMultiConverter)configured;
        var actual = (GetDictionaryValueMultiConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
