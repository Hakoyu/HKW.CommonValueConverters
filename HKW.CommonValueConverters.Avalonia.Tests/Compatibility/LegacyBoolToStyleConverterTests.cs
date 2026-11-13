using System.Globalization;
using Avalonia;
using Avalonia.Data;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class LegacyBoolToStyleConverterTests
{
    public sealed record LegacyCase(string Id) : IConverterCase;

    public static IEnumerable<object[]> Cases =>
        [
            [new LegacyCase("legacy:core-delegates")],
        ];
    public static IEnumerable<ConverterTestException> Exceptions =>
        new[]
        {
            ConverterTestLayer.Properties,
            ConverterTestLayer.Binding,
            ConverterTestLayer.Xaml,
        }.Select(layer => new ConverterTestException(
            typeof(global::HKW.CommonValueConverters.WPF.BoolToStyleConverter),
            layer,
            "Legacy compatibility type derives from the core converter, not AvaloniaObject, and implements neither Avalonia converter interface."
        ));

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(
        typeof(global::HKW.CommonValueConverters.WPF.BoolToStyleConverter),
        ConverterTestLayer.Behavior
    )]
    public Task LegacyBoolToStyleConverter_PreservesFullyQualifiedCoreDelegateContract(
        LegacyCase scenario
    ) =>
        PlatformThread.Run(() =>
        {
            var trueStyle = new global::Avalonia.Styling.Style();
            var falseStyle = new global::Avalonia.Styling.Style();
            var nullStyle = new global::Avalonia.Styling.Style();
            var legacy = new global::HKW.CommonValueConverters.WPF.BoolToStyleConverter
            {
                GetTrueValue = () => trueStyle,
                GetFalseValue = () => falseStyle,
                GetNullValue = () => nullStyle,
                GetIsNullable = () => true,
            };
            Assert.AreEqual(
                "HKW.CommonValueConverters.WPF.BoolToStyleConverter",
                legacy.GetType().FullName
            );
            Assert.AreSame(typeof(ConverterBase).Assembly, legacy.GetType().Assembly);
            Assert.IsInstanceOfType<Core.BoolToValueConverter<global::Avalonia.Styling.Style>>(
                legacy
            );
            Assert.IsFalse(typeof(ConverterBase).IsAssignableFrom(legacy.GetType()));
            Assert.AreSame(trueStyle, legacy.GetTrueValue());
            Assert.AreSame(
                trueStyle,
                legacy.Convert(
                    true,
                    typeof(global::Avalonia.Styling.Style),
                    null,
                    CultureInfo.InvariantCulture
                )
            );
            Assert.AreSame(falseStyle, legacy.Convert(false, null, null, null));
            Assert.AreSame(nullStyle, legacy.Convert(null, null, null, null));
            legacy.GetTrueValue = () => nullStyle;
            Assert.AreSame(nullStyle, legacy.Convert(true, null, null, null));
            legacy.GetIsInverted = () => true;
            Assert.AreSame(falseStyle, legacy.Convert(true, null, null, null));
            Assert.ThrowsExactly<NotSupportedException>(() =>
                legacy.ConvertBack(trueStyle, null, null, null)
            );
        });
}
