using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class EnumsToEnumInfosConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "EnumsToEnumInfosConverter",
                    typeof(EnumsToEnumInfosConverter),
                    () =>
                        S<EnumsToEnumInfosConverter>(
                            new[] { DayOfWeek.Monday },
                            new[] { SampleEnum.First },
                            new[]
                            {
                                new EnumExpectation(DayOfWeek.Sunday),
                                new EnumExpectation(DayOfWeek.Monday),
                                new EnumExpectation(DayOfWeek.Tuesday),
                                new EnumExpectation(DayOfWeek.Wednesday),
                                new EnumExpectation(DayOfWeek.Thursday),
                                new EnumExpectation(DayOfWeek.Friday),
                                new EnumExpectation(DayOfWeek.Saturday),
                            },
                            new[]
                            {
                                new EnumExpectation(SampleEnum.First),
                                new EnumExpectation(SampleEnum.Second),
                            }
                        ),
                    "EnumsToEnumInfosConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (EnumsToEnumInfosConverter)expected;
                        var a = (EnumsToEnumInfosConverter)actual;
                        WpfAssert.AssertResult(e.OnlyValid, a.OnlyValid);
                        WpfAssert.AssertResult(e.DefaultResult, a.DefaultResult);
                        WpfAssert.AssertResult(e.PreferredCulture, a.PreferredCulture);
                    },
                    VerifyPropertyWiring
                ),
            ],
        ];

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EnumsToEnumInfosConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EnumsToEnumInfosConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EnumsToEnumInfosConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => EnumsToEnumInfosConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EnumsToEnumInfosConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<EnumsToEnumInfosConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new EnumsToEnumInfosConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(EnumsToEnumInfosConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetOnlyValid is not null)
                            converter.OnlyValid = settings.GetOnlyValid();
                        if (settings.GetDefaultResult is not null)
                            converter.DefaultResult = WpfConversionAdapter.ToPlatform(
                                settings.GetDefaultResult()
                            );
                    }
                );
            })
        );

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectUnsupportedReverse(WpfPlatformCase scenario) =>
        WpfPlatformBehavior.VerifyUnsupportedReverse(scenario);

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new EnumsToEnumInfosConverter());
        var info = HKW.HKWUtils.Extensions.EnumExtensions.GetInfo(FlagsValue.First);
        var valid = info
            .ValidInfoDictionary.Values.Select(value => new EnumExpectation(value.Value))
            .ToArray();
        var all = info
            .InfoDictionary.Values.Select(value => new EnumExpectation(value.Value))
            .ToArray();
        WpfPropertyAssertions.Probe(
            () => new EnumsToEnumInfosConverter(),
            EnumsToEnumInfosConverter.OnlyValidProperty,
            (c, value) => c.OnlyValid = value,
            c => c.OnlyValid,
            true,
            false,
            c => c.Convert(new[] { FlagsValue.First }, null, null, null),
            valid,
            all
        );
    }

    [Flags]
    private enum FlagsValue
    {
        None = 0,
        First = 1,
        Second = 2,
        Both = First | Second,
    }
}
