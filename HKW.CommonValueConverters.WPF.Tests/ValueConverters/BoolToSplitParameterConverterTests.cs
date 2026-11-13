using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class BoolToSplitParameterConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "BoolToSplitParameterConverter",
                    typeof(BoolToSplitParameterConverter),
                    () =>
                        S<BoolToSplitParameterConverter>(
                            true,
                            false,
                            "yes",
                            "no",
                            "yes|no|null",
                            () => new() { Separator = "|" }
                        ),
                    "BoolToSplitParameterConverter.xaml",
                    "null",
                    (expected, actual) =>
                    {
                        var e = (BoolToSplitParameterConverter)expected;
                        var a = (BoolToSplitParameterConverter)actual;
                        WpfAssert.AssertResult(e.Separator, a.Separator);
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
    [ConverterTest(typeof(BoolToSplitParameterConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToSplitParameterConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToSplitParameterConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => BoolToSplitParameterConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToSplitParameterConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<BoolToSplitParameterConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new BoolToSplitParameterConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(BoolToSplitParameterConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetSeparator is not null)
                            converter.Separator = settings.GetSeparator();
                        if (settings.GetDefaultResult is not null)
                            converter.DefaultResult = WpfConversionAdapter.ToPlatform(
                                settings.GetDefaultResult()
                            );
                        if (settings.ConvertSplitValue is not null)
                            converter.ConvertSplitValue = settings.ConvertSplitValue;
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

    // 原平台回归场景按对象保留.
    [TestMethod]
    public void BoolToSplitThicknessConverter_UsesLiveDelegateAndTypedThicknessTarget_BoolToSplitParameterConverter() =>
        PlatformThread.Run(() =>
        {
            var split = new BoolToSplitParameterConverter();
            split.ConvertSplitValue = (value, _) => value.Length;
            Assert.AreEqual(3, split.Convert(true, null, "yes,no", null));
            split.ConvertSplitValue = (_, _) =>
                throw new InvalidOperationException("Delegate failure.");
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                split.Convert(true, null, "yes,no", null)
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new BoolToSplitParameterConverter());
        WpfPropertyAssertions.Probe(
            () => new BoolToSplitParameterConverter(),
            BoolToSplitParameterConverter.SeparatorProperty,
            (c, value) => c.Separator = value,
            c => c.Separator,
            "|",
            ",",
            c => c.Convert(true, null, "one|two,no", null),
            "one",
            "one|two"
        );
    }
}
