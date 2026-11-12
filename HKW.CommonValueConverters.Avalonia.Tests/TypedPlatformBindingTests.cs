using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class TypedPlatformBindingTests
{
    private static void Verify<T>(
        AvaloniaObject target,
        StyledProperty<T> property,
        IValueConverter converter,
        object expectedTrue,
        object expectedFalse,
        object? parameter = null
    )
    {
        using var errors = new BindingErrorScope();
        var source = new TypedSource<bool>(true);
        using var subscription = target.Bind(
            property,
            new Binding("Value")
            {
                Source = source,
                Converter = converter,
                ConverterParameter = parameter,
                Mode = BindingMode.OneWay,
            }
        );
        ConverterAssertions.Equal(expectedTrue, target.GetValue(property));
        source.Value = false;
        PlatformThread.Drain();
        ConverterAssertions.Equal(expectedFalse, target.GetValue(property));
    }

    [TestMethod]
    public Task BoolToFontWeightConverter_TextBlockFontWeight() =>
        PlatformThread.Run(() =>
            Verify(
                new TextBlock(),
                TextBlock.FontWeightProperty,
                new BoolToFontWeightConverter(),
                FontWeight.Bold,
                FontWeight.Normal
            )
        );

    [TestMethod]
    public Task BoolToThicknessConverter_BorderPadding() =>
        PlatformThread.Run(() =>
            Verify(
                new Border(),
                Border.PaddingProperty,
                new BoolToThicknessConverter(),
                new Thickness(5),
                new Thickness()
            )
        );

    [TestMethod]
    public Task BoolToSolidColorBrushConverter_BorderBackground() =>
        PlatformThread.Run(() =>
            Verify(
                new Border(),
                Border.BackgroundProperty,
                new BoolToSolidColorBrushConverter(),
                Colors.White,
                Colors.Black
            )
        );

    [TestMethod]
    public Task BoolToSplitThicknessConverter_BorderMargin() =>
        PlatformThread.Run(() =>
            Verify(
                new Border(),
                Border.MarginProperty,
                new BoolToSplitThicknessConverter { Separator = "|" },
                new Thickness(2),
                new Thickness(4),
                "2|4"
            )
        );

    [TestMethod]
    public Task MarginMultiConverter_BorderMarginAndEachSlotNotification() =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrorScope();
            var source = new TestViewModel(1, 2, 3, 4);
            var target = new Border { DataContext = source };
            var binding = new MultiBinding { Converter = new MarginMultiConverter() };
            for (var index = 0; index < 4; index++)
                binding.Bindings.Add(new Binding($"V{index}"));
            using var subscription = target.Bind(Border.MarginProperty, binding);
            Assert.AreEqual(new Thickness(1, 2, 3, 4), target.Margin);
            source.V1 = 7;
            PlatformThread.Drain();
            Assert.AreEqual(new Thickness(1, 7, 3, 4), target.Margin);
            source.V3 = 8;
            PlatformThread.Drain();
            Assert.AreEqual(new Thickness(1, 7, 3, 8), target.Margin);
        });

    [TestMethod]
    public Task MultiBinding_DefaultTargetNullValueUsesFallback() =>
        PlatformThread.Run(() =>
        {
            var source = new TestViewModel(null, null);
            var converter = new AllBoolToValueMultiConverter { NullValue = null };
            Assert.IsNull(converter.Convert([null, null], typeof(string), null, null));
            var target = new TextBlock { DataContext = source };
            var binding = new MultiBinding { Converter = converter, FallbackValue = "fallback" };
            binding.Bindings.Add(new Binding("V0"));
            binding.Bindings.Add(new Binding("V1"));
            using var subscription = target.Bind(TextBlock.TextProperty, binding);
            // Avalonia 12 的默认 TargetNullValue 为 UnsetValue, 因此 null 继续采用 FallbackValue.
            Assert.AreEqual("fallback", target.Text);
        });

    [TestMethod]
    public Task MultiBinding_UnsetFallbackDoNothingAndNullAreDistinct() =>
        PlatformThread.Run(() =>
        {
            var source = new TestViewModel(true, true);
            var converter = new AllBoolToValueMultiConverter
            {
                TrueValue = "yes",
                FalseValue = "no",
                NullValue = null,
            };
            var target = new TextBlock { DataContext = source };
            var binding = new MultiBinding
            {
                Converter = converter,
                FallbackValue = "fallback",
                TargetNullValue = null!,
            };
            binding.Bindings.Add(new Binding("V0"));
            binding.Bindings.Add(new Binding("V1"));
            using var subscription = target.Bind(TextBlock.TextProperty, binding);
            Assert.AreEqual("yes", target.Text);
            source.V0 = false;
            PlatformThread.Drain();
            Assert.AreEqual("fallback", target.Text);
            source.V0 = true;
            PlatformThread.Drain();
            Assert.AreEqual("yes", target.Text);
            converter.DefaultResult = BindingOperations.DoNothing;
            source.V0 = false;
            PlatformThread.Drain();
            Assert.AreEqual("yes", target.Text);
            source.V0 = null;
            source.V1 = null;
            PlatformThread.Drain();
            Assert.IsNull(target.Text);
        });
}
