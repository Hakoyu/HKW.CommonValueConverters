using System.Globalization;

namespace HKW.CommonValueConverters.WPF.Tests;

internal static class WpfPlatformBehavior
{
    public static void Verify(WpfPlatformCase scenario) =>
        PlatformThread.Run(() =>
        {
            var item = scenario.Create();
            var converter = item.Factory();
            Assert.AreEqual(scenario.Type, converter.GetType());
            for (var index = 0; index < item.Inputs.Length; index++)
            {
                var values = item.Inputs[index];
                var snapshot = values.ToArray();
                var actual = converter is ValueConverterBase single
                    ? single.Convert(
                        values[0],
                        typeof(object),
                        item.Parameter,
                        CultureInfo.InvariantCulture
                    )
                    : ((MultiValueConverterBase)converter).Convert(
                        values,
                        typeof(object),
                        item.Parameter,
                        CultureInfo.InvariantCulture
                    );
                WpfAssert.AssertResult(item.Expected[index], actual);
                CollectionAssert.AreEqual(snapshot, values);
            }
        });

    public static void VerifyUnsupportedReverse(WpfPlatformCase scenario) =>
        PlatformThread.Run(() =>
        {
            var converter = scenario.Create().Factory();
            if (converter is MultiValueConverterBase multi)
                Assert.ThrowsExactly<NotSupportedException>(() =>
                    multi.ConvertBack(null, [typeof(object)], null, CultureInfo.InvariantCulture)
                );
            else
                Assert.ThrowsExactly<NotSupportedException>(() =>
                    ((ValueConverterBase)converter).ConvertBack(
                        null,
                        typeof(object),
                        null,
                        CultureInfo.InvariantCulture
                    )
                );
        });
}
