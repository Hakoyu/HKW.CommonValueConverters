using System.Globalization;
using System.Windows;
using System.Windows.Data;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class CultureConversionTests
{
    [TestMethod]
    public void NumberBinding_UsesConverterCultureForInitialAndUpdatedValues() =>
        PlatformThread.Run(() =>
        {
            var source = new TestViewModel { A = "12,5" };
            var target = new TestTarget();
            var converter = new StringToNumberConverter
            {
                NumberType = NumberType.Decimal,
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            try
            {
                BindingOperations.SetBinding(
                    target,
                    TestTarget.ResultProperty,
                    new Binding("A")
                    {
                        Source = source,
                        Converter = converter,
                        ConverterCulture = CultureInfo.GetCultureInfo("fr-FR"),
                    }
                );
                PlatformThread.Drain();
                Assert.AreEqual(12.5m, target.Result);
                source.A = "13,75";
                PlatformThread.Drain();
                Assert.AreEqual(13.75m, target.Result);
                Assert.AreEqual(
                    "12,5",
                    converter.ConvertBack(12.5m, null, null, CultureInfo.GetCultureInfo("fr-FR"))
                );
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });

    [TestMethod]
    public void GlobalInstanceAndClearedCulturePreferences_AreLive() =>
        PlatformThread.Run(() =>
        {
            var saved = Core.ValueConvertersConfig.DefaultPreferredCulture;
            var ui = CultureInfo.CurrentUICulture;
            var current = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("tr-TR");
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var converter = new StringCaseConverter();
                Core.ValueConvertersConfig.DefaultPreferredCulture =
                    Core.PreferredCulture.CurrentUICulture;
                Assert.AreEqual(
                    "İ",
                    converter.Convert("i", null, "U", CultureInfo.InvariantCulture)
                );
                converter.PreferredCulture = Core.PreferredCulture.ConverterCulture;
                Assert.AreEqual(
                    "I",
                    converter.Convert("i", null, "U", CultureInfo.InvariantCulture)
                );
                converter.ClearValue(ConverterBase.PreferredCultureProperty);
                Assert.AreEqual(
                    "İ",
                    converter.Convert("i", null, "U", CultureInfo.InvariantCulture)
                );
                Core.ValueConvertersConfig.DefaultPreferredCulture =
                    Core.PreferredCulture.ConverterCulture;
                Assert.AreEqual(
                    "I",
                    converter.Convert("i", null, "U", CultureInfo.InvariantCulture)
                );
            }
            finally
            {
                Core.ValueConvertersConfig.DefaultPreferredCulture = saved;
                CultureInfo.CurrentUICulture = ui;
                CultureInfo.CurrentCulture = current;
            }
        });

    [TestMethod]
    public void SplitThicknessBinding_UsesEffectiveConverterCulture() =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrors();
            var saved = CultureInfo.CurrentCulture;
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NegativeSign = "~";
            var converter = new BoolToSplitThicknessConverter
            {
                Separator = "|",
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            var source = new TestViewModel { A = true };
            var target = new System.Windows.Controls.Border();
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                BindingOperations.SetBinding(
                    target,
                    FrameworkElement.MarginProperty,
                    new Binding("A")
                    {
                        Source = source,
                        Converter = converter,
                        ConverterCulture = culture,
                        ConverterParameter = "~1,~2,~3,~4|~5",
                        Mode = BindingMode.OneWay,
                    }
                );
                PlatformThread.Drain();
                Assert.AreEqual(new Thickness(-1, -2, -3, -4), target.Margin);
                source.A = false;
                PlatformThread.Drain();
                Assert.AreEqual(new Thickness(-5), target.Margin);
                converter.PreferredCulture = Core.PreferredCulture.CurrentCulture;
                CultureInfo.CurrentCulture = culture;
                Assert.AreEqual(
                    new Thickness(-2),
                    converter.Convert(true, null, "~2|0", CultureInfo.InvariantCulture)
                );
                errors.AssertEmpty();
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
                CultureInfo.CurrentCulture = saved;
            }
        });

    [TestMethod]
    public void DatesTimesAndMargin_UseExplicitCultureAndFormatPriority() =>
        PlatformThread.Run(() =>
        {
            var french = CultureInfo.GetCultureInfo("fr-FR");
            var date = new DateTimeToStringConverter
            {
                Format = "dd MMMM yyyy",
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            Assert.AreEqual(
                "04 septembre 2026",
                date.Convert(new DateTime(2026, 9, 4), null, null, french)
            );
            Assert.AreEqual(
                "2026/09",
                date.Convert(new DateTime(2026, 9, 4), null, "yyyy/MM", french)
            );
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
            var margin = new MarginMultiConverter
            {
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            Assert.AreEqual(new Thickness(1.5), margin.Convert(["1,5"], null, null, french));
            var number = new StringToNumberConverter { NumberStyles = NumberStyles.HexNumber };
            Assert.AreEqual(255, number.Convert("FF", null, null, CultureInfo.InvariantCulture));
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                number.Convert("not-hex", null, null, french)
            );
        });
}
