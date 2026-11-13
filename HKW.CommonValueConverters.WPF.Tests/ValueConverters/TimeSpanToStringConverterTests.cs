using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class TimeSpanToStringConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "TimeSpanToStringConverter",
                    typeof(TimeSpanToStringConverter),
                    () =>
                        S<TimeSpanToStringConverter>(
                            TimeSpan.FromMinutes(90),
                            TimeSpan.FromMinutes(15),
                            "01:30:00",
                            "00:15:00",
                            factory: () => new() { Format = "c" }
                        ),
                    "TimeSpanToStringConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (TimeSpanToStringConverter)expected;
                        var a = (TimeSpanToStringConverter)actual;
                        WpfAssert.AssertResult(e.Format, a.Format);
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
    [ConverterTest(typeof(TimeSpanToStringConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(TimeSpanToStringConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(TimeSpanToStringConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => TimeSpanToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(TimeSpanToStringConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<TimeSpanToStringConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new TimeSpanToStringConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(TimeSpanToStringConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetFormat is not null)
                            converter.Format = settings.GetFormat();
                        if (settings.GetDefaultResult is not null)
                            converter.DefaultResult = WpfConversionAdapter.ToPlatform(
                                settings.GetDefaultResult()
                            );
                    }
                );
            })
        );

    // 原平台回归场景按对象保留.
    [TestMethod]
    public void TimeSpanToStringConverter_TypedTextBoxRoundTrip() =>
        PlatformThread.Run(() =>
            WpfTwoWayBindingRunner.Verify(
                new TimeSpanToStringConverter { Format = "c" },
                new TextBox(),
                TextBox.TextProperty,
                TimeSpan.FromMinutes(90),
                "01:30:00",
                "00:15:00",
                TimeSpan.FromMinutes(15)
            )
        );

    [TestMethod]
    public void ConvertBack_UnsetAndDoNothingKeepTypedSource() =>
        PlatformThread.Run(() =>
        {
            var source = new TypedSource<TimeSpan>(TimeSpan.FromMinutes(90));
            var target = new TextBox();
            var converter = new TimeSpanToStringConverter
            {
                Format = "c",
                DefaultResult = Binding.DoNothing,
            };
            try
            {
                var expression = (BindingExpression)
                    BindingOperations.SetBinding(
                        target,
                        TextBox.TextProperty,
                        new Binding("Value")
                        {
                            Source = source,
                            Converter = converter,
                            ConverterCulture = CultureInfo.InvariantCulture,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                        }
                    );
                PlatformThread.Drain();
                target.SetCurrentValue(TextBox.TextProperty, "invalid");
                expression.UpdateSource();
                Assert.AreEqual(TimeSpan.FromMinutes(90), source.Value);
                converter.DefaultResult = DependencyProperty.UnsetValue;
                target.SetCurrentValue(TextBox.TextProperty, "still-invalid");
                expression.UpdateSource();
                Assert.AreEqual(TimeSpan.FromMinutes(90), source.Value);
                target.SetCurrentValue(TextBox.TextProperty, "00:15:00");
                expression.UpdateSource();
                Assert.AreEqual(TimeSpan.FromMinutes(15), source.Value);
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });

    [TestMethod]
    public void DatesTimesAndMargin_UseExplicitCultureAndFormatPriority_TimeSpanToStringConverter() =>
        PlatformThread.Run(() =>
        {
            var french = CultureInfo.GetCultureInfo("fr-FR");
            var time = new TimeSpanToStringConverter
            {
                Format = "g",
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            Assert.AreEqual(
                "0:00:01,5",
                time.Convert(TimeSpan.FromMilliseconds(1500), null, null, french)
            );
            Assert.AreEqual(
                TimeSpan.FromMilliseconds(1500),
                time.ConvertBack("0:00:01,5", null, null, french)
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new TimeSpanToStringConverter());
        WpfPropertyAssertions.Probe(
            () => new TimeSpanToStringConverter(),
            TimeSpanToStringConverter.FormatProperty,
            (c, value) => c.Format = value,
            c => c.Format,
            "c",
            "g",
            c => c.Convert(TimeSpan.FromMinutes(15), null, null, CultureInfo.InvariantCulture),
            "00:15:00",
            "0:15:00"
        );
    }
}
