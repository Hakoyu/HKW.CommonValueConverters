using System.Globalization;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class ValueConvertersConfigTests
{
    [TestMethod]
    public void CulturePreference_TracksGlobalUntilExplicitOverrideAndClear() =>
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
}
