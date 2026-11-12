using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class CultureConversionTests
{
    [TestMethod]
    public Task NumericBinding_UsesExplicitCultureAndUpdates() =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrorScope();
            var saved = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var converter = new StringToNumberConverter
                {
                    NumberType = NumberType.Decimal,
                    PreferredCulture = Core.PreferredCulture.ConverterCulture,
                };
                var source = new TypedSource<string>("12,5");
                var target = new NumericUpDown();
                using var subscription = target.Bind(
                    NumericUpDown.ValueProperty,
                    new Binding("Value")
                    {
                        Source = source,
                        Converter = converter,
                        ConverterCulture = CultureInfo.GetCultureInfo("fr-FR"),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    }
                );
                Assert.AreEqual(12.5m, target.Value);
                source.Value = "23,5";
                PlatformThread.Drain();
                Assert.AreEqual(23.5m, target.Value);
                target.SetCurrentValue(NumericUpDown.ValueProperty, 34.5m);
                PlatformThread.Drain();
                Assert.AreEqual("34,5", source.Value);
            }
            finally
            {
                CultureInfo.CurrentCulture = saved;
            }
        });

    [TestMethod]
    public Task CulturePreference_TracksGlobalUntilExplicitOverrideAndClear() =>
        PlatformThread.Run(() =>
        {
            var savedGlobal = Core.ValueConvertersConfig.DefaultPreferredCulture;
            var savedCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                Core.ValueConvertersConfig.DefaultPreferredCulture =
                    Core.PreferredCulture.ConverterCulture;
                var converter = new StringCaseConverter();
                var turkish = CultureInfo.GetCultureInfo("tr-TR");
                Assert.AreEqual("İ", converter.Convert("i", null, "U", turkish));
                Core.ValueConvertersConfig.DefaultPreferredCulture =
                    Core.PreferredCulture.CurrentCulture;
                Assert.AreEqual("I", converter.Convert("i", null, "U", turkish));
                converter.PreferredCulture = Core.PreferredCulture.ConverterCulture;
                Assert.AreEqual("İ", converter.Convert("i", null, "U", turkish));
                converter.ClearValue(ConverterBase.PreferredCultureProperty);
                Assert.AreEqual("I", converter.Convert("i", null, "U", turkish));
            }
            finally
            {
                Core.ValueConvertersConfig.DefaultPreferredCulture = savedGlobal;
                CultureInfo.CurrentCulture = savedCulture;
            }
        });

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
            Assert.AreEqual(
                TimeSpan.FromMilliseconds(1250),
                new TimeSpanToStringConverter().ConvertBack("0:00:01,25", null, null, french)
            );
            Assert.AreEqual(
                "1,5/2,5",
                new StringFormatMultiConverter().Convert([1.5m, 2.5m], null, "{0}/{1}", french)
            );
        });

    [TestMethod]
    public Task BoolToSplitThicknessConverter_UsesEffectiveCulture() =>
        PlatformThread.Run(() =>
        {
            var saved = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                var converter = new BoolToSplitThicknessConverter { Separator = "|" };
                Assert.AreEqual(
                    new Thickness(1.5),
                    converter.Convert(true, null, "1.5|2.5", CultureInfo.GetCultureInfo("en-US"))
                );
                Assert.AreEqual(
                    new Thickness(1, 2, 3, 4),
                    converter.Convert(true, null, "1,2,3,4|0", CultureInfo.InvariantCulture)
                );
            }
            finally
            {
                CultureInfo.CurrentCulture = saved;
            }
        });

    [TestMethod]
    [DataRow("invalid")]
    [DataRow("NaN")]
    [DataRow("Infinity")]
    [DataRow("1e999")]
    public Task BoolToSplitThicknessConverter_InvalidValuesUseFallback(string text) =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var converter = new BoolToSplitThicknessConverter
            {
                Separator = "|",
                DefaultResult = marker,
            };
            Assert.AreSame(
                marker,
                converter.Convert(true, null, text + "|0", CultureInfo.InvariantCulture)
            );
        });

    [TestMethod]
    public Task MarginMultiConverter_RatioRejectsOverflowForUniformThickness() =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var converter = new MarginMultiConverter { HasRatio = true, DefaultResult = marker };
            Assert.AreEqual(
                new Thickness(3),
                converter.Convert([2, "1,5"], null, null, CultureInfo.GetCultureInfo("fr-FR"))
            );
            Assert.AreSame(
                marker,
                converter.Convert([double.MaxValue, 2], null, null, CultureInfo.InvariantCulture)
            );
            Assert.AreSame(
                marker,
                converter.Convert(
                    [double.MaxValue, 2, 3, 4, 5],
                    null,
                    null,
                    CultureInfo.InvariantCulture
                )
            );
        });
}
