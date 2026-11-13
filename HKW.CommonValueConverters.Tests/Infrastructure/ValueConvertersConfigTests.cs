using System.Globalization;
using HKW.CommonValueConverters;

namespace HKW.CommonValueConvertersTest;

[TestClass]
[DoNotParallelize]
public sealed class ValueConvertersConfigTests
{
    [TestMethod]
    public void CultureIsResolvedForAllEntryPoints()
    {
        var savedCulture = CultureInfo.CurrentCulture;
        var savedUiCulture = CultureInfo.CurrentUICulture;
        var savedPreference = ValueConvertersConfig.DefaultPreferredCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("tr-TR");
            var supplied = CultureInfo.GetCultureInfo("en-US");
            var single = new CultureProbe();
            var multi = new MultiCultureProbe();
            foreach (var preference in Enum.GetValues<PreferredCulture>())
            {
                ValueConvertersConfig.DefaultPreferredCulture = preference;
                var expected = preference switch
                {
                    PreferredCulture.CurrentCulture => CultureInfo.CurrentCulture,
                    PreferredCulture.CurrentUICulture => CultureInfo.CurrentUICulture,
                    _ => supplied,
                };
                Assert.AreSame(expected, single.Convert(null, null, null, supplied));
                Assert.AreSame(expected, single.ConvertBack(null, null, null, supplied));
                Assert.AreSame(expected, multi.Convert([], null, null, supplied));
                Assert.AreSame(expected, multi.ConvertBack(null, [], null, supplied)[0]);
            }
            single.PreferredCulture = PreferredCulture.ConverterCulture;
            ValueConvertersConfig.DefaultPreferredCulture = PreferredCulture.CurrentUICulture;
            Assert.AreSame(supplied, single.Convert(null, null, null, supplied));
            Assert.AreSame(CultureInfo.CurrentCulture, single.Convert(null, null, null, null));
        }
        finally
        {
            CultureInfo.CurrentCulture = savedCulture;
            CultureInfo.CurrentUICulture = savedUiCulture;
            ValueConvertersConfig.DefaultPreferredCulture = savedPreference;
        }
    }

    private sealed class CultureProbe : ValueConverterBase
    {
        protected override object ConvertCore(
            object? value,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => culture;

        protected override object ConvertBackCore(
            object? value,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => culture;
    }

    private sealed class MultiCultureProbe : MultiValueConverterBase
    {
        protected override object ConvertCore(
            IList<object?> values,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => culture;

        protected override object[] ConvertBackCore(
            object? value,
            IList<Type?> targetTypes,
            object? parameter,
            CultureInfo culture
        ) => [culture];
    }
}
