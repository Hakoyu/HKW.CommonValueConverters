using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HKW.CommonValueConverters.WPF.Tests;

internal static class WpfBoundaryAssertions
{
    public static void Verify(WpfPlatformCase scenario, bool passThroughUnavailable = false) =>
        PlatformThread.Run(() =>
        {
            var item = scenario.Create();
            var converter = item.Factory();
            var fallback = new object();
            converter.DefaultResult = fallback;
            foreach (var sentinel in new[] { DependencyProperty.UnsetValue, Binding.DoNothing })
            {
                if (converter is ValueConverterBase single)
                {
                    var expected = passThroughUnavailable ? sentinel : fallback;
                    Assert.AreSame(
                        expected,
                        single.Convert(
                            sentinel,
                            typeof(object),
                            item.Parameter,
                            CultureInfo.InvariantCulture
                        )
                    );
                }
                else
                {
                    var multi = (MultiValueConverterBase)converter;
                    var values = item.Inputs[0].ToArray();
                    values[0] = sentinel;
                    var snapshot = values.ToArray();
                    Assert.AreSame(
                        fallback,
                        multi.Convert(
                            values,
                            typeof(object),
                            item.Parameter,
                            CultureInfo.InvariantCulture
                        )
                    );
                    CollectionAssert.AreEqual(snapshot, values);
                    Assert.AreSame(
                        fallback,
                        multi.Convert(
                            null!,
                            typeof(object),
                            item.Parameter,
                            CultureInfo.InvariantCulture
                        )
                    );
                }
            }
            if (converter is ValueConverterBase valueConverter)
            {
                converter.DefaultResult = "fallback";
                WpfAssert.AssertResult(
                    scenario.NullResult,
                    valueConverter.Convert(
                        null,
                        typeof(object),
                        item.Parameter,
                        CultureInfo.InvariantCulture
                    )
                );
            }
        });
}
