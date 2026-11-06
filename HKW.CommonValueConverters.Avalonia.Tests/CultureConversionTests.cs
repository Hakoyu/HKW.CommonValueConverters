using System.Globalization;
using HKW.CommonValueConverters.Avalonia;
using HKW.HKWUtils;
using PreferredCulture = HKW.CommonValueConverters.PreferredCulture;

namespace HKW.PlatformConverters.Avalonia.Tests;

[TestClass]
[DoNotParallelize]
public sealed class CultureConversionTests
{
    [TestMethod]
    [DataRow("fr-FR")]
    [DataRow("en-US")]
    public void DatesUseSuppliedCultureInBothDirections(string cultureName) =>
        PlatformThread.Run(() =>
        {
            var saved = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ja-JP");
                var culture = CultureInfo.GetCultureInfo(cultureName);
                var local = new DateTime(2026, 9, 14, 12, 34, 0, DateTimeKind.Local);
                var converter = new DateTimeToStringConverter { Format = "g" };
                var text = local.ToString("g", culture);
                Assert.AreEqual(text, converter.Convert(local, null, null, culture));
                Assert.AreEqual(
                    local.ToUniversalTime(),
                    converter.ConvertBack(text, null, null, culture)
                );
                var offset = new DateTimeOffset(local);
                var offsetConverter = new DateTimeOffsetToStringConverter { Format = "g" };
                Assert.AreEqual(text, offsetConverter.Convert(offset, null, null, culture));
                Assert.AreEqual(
                    offset.ToUniversalTime(),
                    offsetConverter.ConvertBack(text, null, null, culture)
                );
            }
            finally
            {
                CultureInfo.CurrentCulture = saved;
            }
        });

    [TestMethod]
    public void ExplicitDateFormatsTakePriorityAndFallbackUsesSameCulture() =>
        PlatformThread.Run(() =>
        {
            var culture = CultureInfo.GetCultureInfo("en-US");
            var expected = new DateTime(2026, 9, 4, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
            var converter = new DateTimeToStringConverter { Format = "MM/dd/yyyy" };
            Assert.AreEqual(
                expected,
                converter.ConvertBack("04/09/2026", null, "dd/MM/yyyy", culture)
            );
            converter.Format = "dd/MM/yyyy";
            Assert.AreEqual(expected, converter.ConvertBack("04/09/2026", null, null, culture));
            Assert.AreEqual(
                expected,
                converter.ConvertBack("September 4, 2026", null, "dd/MM/yyyy", culture)
            );
            var offset = new DateTimeOffset(2026, 9, 4, 12, 34, 0, TimeSpan.FromHours(3));
            var offsetConverter = new DateTimeOffsetToStringConverter();
            Assert.AreEqual(
                offset.ToUniversalTime(),
                offsetConverter.ConvertBack(offset.ToString("O", culture), null, "O", culture)
            );
            Assert.AreEqual(
                expected,
                converter.ConvertBack(expected.ToString("O", culture), null, "O", culture)
            );
        });

    [TestMethod]
    public void TimeSpanUsesCultureAndCustomFormat() =>
        PlatformThread.Run(() =>
        {
            var saved = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var culture = CultureInfo.GetCultureInfo("fr-FR");
                var value = new TimeSpan(1, 2, 3, 4, 567);
                var converter = new TimeSpanToStringConverter();
                var text = value.ToString("g", culture);
                Assert.AreEqual(text, converter.Convert(value, null, null, culture));
                Assert.AreEqual(value, converter.ConvertBack(text, null, null, culture));
                const string format = @"hh\|mm\|ss";
                var simple = new TimeSpan(2, 3, 4);
                Assert.AreEqual(simple, converter.ConvertBack("02|03|04", null, format, culture));
                converter.Format = format;
                Assert.AreEqual(simple, converter.ConvertBack("02|03|04", null, null, culture));
                Assert.AreEqual(simple, converter.ConvertBack("02:03:04", null, format, culture));
                Assert.AreEqual(simple, converter.ConvertBack(simple, null, null, culture));
            }
            finally
            {
                CultureInfo.CurrentCulture = saved;
            }
        });

    [TestMethod]
    public void NumbersUseCultureInBothDirections() =>
        PlatformThread.Run(() =>
        {
            var saved = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var culture = CultureInfo.GetCultureInfo("fr-FR");
                var converter = new StringToNumberConverter { NumberType = NumberType.Decimal };
                Assert.AreEqual(12.5m, converter.Convert("12,5", null, null, culture));
                Assert.AreEqual(12.5m, converter.Convert(12.5m, null, null, culture));
                Assert.AreEqual("12,5", converter.ConvertBack(12.5m, null, null, culture));
                Assert.IsNull(converter.ConvertBack(null, null, null, culture));
                Assert.AreEqual("other", converter.ConvertBack("other", null, null, culture));
                Assert.AreSame(
                    ConverterResults.UnsetValue,
                    converter.ConvertBack(ConverterResults.UnsetValue, null, null, culture)
                );
                Assert.AreSame(
                    ConverterResults.UnsetValue,
                    converter.Convert(ConverterResults.DoNothing, null, null, culture)
                );
                Assert.AreEqual(
                    0,
                    new NumberCompareConverter { NumberType = NumberType.Decimal }.Convert(
                        "12,5",
                        null,
                        12.5m,
                        culture
                    )
                );
            }
            finally
            {
                CultureInfo.CurrentCulture = saved;
            }
        });

    [TestMethod]
    public void BooleanNumericInputsUseEffectiveCulture() =>
        PlatformThread.Run(() =>
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.PositiveSign = "p";
            Assert.AreEqual(true, new ToBoolConverter().Convert("p1", null, null, culture));
            Assert.AreEqual(
                "yes",
                new BoolToSplitParameterConverter().Convert("p1", null, "yes,no", culture)
            );
            var converter = new BoolToValueConverter<string> { TrueValue = "yes" };
            Assert.AreEqual("yes", converter.Convert("p1", null, null, culture));
        });

    [TestMethod]
    public void InstancePreferenceAffectsRealConverters() =>
        PlatformThread.Run(() =>
        {
            var saved = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("tr-TR");
                var converter = new StringCaseConverter
                {
                    PreferredCulture = PreferredCulture.CurrentUICulture,
                };
                Assert.AreEqual(
                    "İ",
                    converter.Convert("i", null, "U", CultureInfo.InvariantCulture)
                );
            }
            finally
            {
                CultureInfo.CurrentUICulture = saved;
            }
        });

    [TestMethod]
    public void InvalidFormattingReturnsDefaultWithoutSwallowingUserDelegates() =>
        PlatformThread.Run(() =>
        {
            ValueConverterBase[] converters =
            [
                new DateTimeToStringConverter(),
                new DateTimeOffsetToStringConverter(),
                new TimeSpanToStringConverter(),
            ];
            object[] values = [DateTime.UnixEpoch, DateTimeOffset.UnixEpoch, TimeSpan.Zero];
            for (var i = 0; i < converters.Length; i++)
            {
                Assert.AreSame(
                    ConverterResults.UnsetValue,
                    converters[i].Convert(values[i], null, "%", CultureInfo.InvariantCulture)
                );
                Assert.AreSame(
                    ConverterResults.UnsetValue,
                    converters[i].ConvertBack("invalid", null, "%", CultureInfo.InvariantCulture)
                );
                Assert.AreSame(
                    ConverterResults.UnsetValue,
                    converters[i].Convert(values[i], null, ConverterResults.UnsetValue, null)
                );
            }
        });
}
