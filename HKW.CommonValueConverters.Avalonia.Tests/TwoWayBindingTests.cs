using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters.Avalonia.Tests;

public sealed class TypedSource<T>(T value) : INotifyPropertyChanged
{
    private T _value = value;
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class TypedTarget : Control
{
    public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<
        TypedTarget,
        Color
    >(nameof(Color));
    public static readonly StyledProperty<IEnumInfo?> InfoProperty = AvaloniaProperty.Register<
        TypedTarget,
        IEnumInfo?
    >(nameof(Info));
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    public IEnumInfo? Info
    {
        get => GetValue(InfoProperty);
        set => SetValue(InfoProperty, value);
    }
}

[TestClass]
public sealed class TwoWayBindingTests
{
    private static void Verify<TSource, TTarget>(
        TSource initial,
        TSource expectedSource,
        AvaloniaObject target,
        StyledProperty<TTarget> property,
        IValueConverter converter,
        object? expectedInitial,
        TTarget edit
    )
    {
        using var errors = new BindingErrorScope();
        var source = new TypedSource<TSource>(initial);
        using var subscription = target.Bind(
            property,
            new Binding(nameof(source.Value))
            {
                Source = source,
                Converter = converter,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ConverterCulture = CultureInfo.InvariantCulture,
            }
        );
        PlatformThread.Drain();
        ConverterAssertions.Equal(expectedInitial, target.GetValue(property));
        target.SetCurrentValue(property, edit);
        PlatformThread.Drain();
        if (expectedSource is ISolidColorBrush brush)
            ConverterAssertions.Equal(brush.Color, source.Value);
        else
            Assert.AreEqual(expectedSource, source.Value);
    }

    [TestMethod]
    public Task DateTimeToStringConverter_TwoWayTextBox() =>
        PlatformThread.Run(() =>
            Verify(
                new DateTime(2026, 1, 2),
                new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Local).ToUniversalTime(),
                new TextBox(),
                TextBox.TextProperty,
                new DateTimeToStringConverter { Format = "yyyy-MM-dd" },
                "2026-01-02",
                "2026-02-03"
            )
        );

    [TestMethod]
    public Task DateTimeOffsetToStringConverter_TwoWayTextBox() =>
        PlatformThread.Run(() =>
            Verify(
                new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 2, 3, 0, 0, 0, TimeSpan.Zero),
                new TextBox(),
                TextBox.TextProperty,
                new DateTimeOffsetToStringConverter { Format = "O" },
                new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)
                    .ToLocalTime()
                    .ToString("O", CultureInfo.InvariantCulture),
                "2026-02-03T00:00:00.0000000+00:00"
            )
        );

    [TestMethod]
    public Task GuidToStringConverter_TwoWayTextBox() =>
        PlatformThread.Run(() =>
            Verify(
                Guid.Empty,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                new TextBox(),
                TextBox.TextProperty,
                new GuidToStringConverter(),
                "00000000-0000-0000-0000-000000000000",
                "00000000-0000-0000-0000-000000000001"
            )
        );

    [TestMethod]
    public Task TimeSpanToStringConverter_TwoWayTextBox() =>
        PlatformThread.Run(() =>
            Verify(
                TimeSpan.FromHours(1),
                TimeSpan.FromHours(2),
                new TextBox(),
                TextBox.TextProperty,
                new TimeSpanToStringConverter { Format = "c" },
                "01:00:00",
                "02:00:00"
            )
        );

    [TestMethod]
    public Task DebugConverter_TwoWayTextBox() =>
        PlatformThread.Run(() =>
            Verify(
                "before",
                "after",
                new TextBox(),
                TextBox.TextProperty,
                new DebugConverter(),
                "before",
                "after"
            )
        );

    [TestMethod]
    public Task StringToNumberConverter_TwoWayNumericUpDown() =>
        PlatformThread.Run(() =>
            Verify(
                "12.5",
                "27.5",
                new NumericUpDown(),
                NumericUpDown.ValueProperty,
                new StringToNumberConverter { NumberType = NumberType.Decimal },
                12.5m,
                (decimal?)27.5m
            )
        );

    [TestMethod]
    public Task MediaColorToSolidColorBrushConverter_TwoWayBorder() =>
        PlatformThread.Run(() =>
            Verify(
                Colors.Red,
                Colors.Blue,
                new Border(),
                Border.BackgroundProperty,
                new MediaColorToSolidColorBrushConverter(),
                Colors.Red,
                (IBrush?)new SolidColorBrush(Colors.Blue)
            )
        );

    [TestMethod]
    public Task SolidColorBrushToMediaColorConverter_TwoWayColorProperty() =>
        PlatformThread.Run(() =>
            Verify(
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Blue),
                new TypedTarget(),
                TypedTarget.ColorProperty,
                new SolidColorBrushToMediaColorConverter(),
                Colors.Red,
                Colors.Blue
            )
        );

    [TestMethod]
    public Task EnumToEnumInfoConverter_TwoWayTypedInfoProperty() =>
        PlatformThread.Run(() =>
        {
            var info = (IEnumInfo)
                new EnumToEnumInfoConverter().Convert(
                    SampleEnum.Second,
                    typeof(IEnumInfo),
                    null,
                    null
                )!;
            Assert.AreEqual(SampleEnum.Second, info.Value);
            Verify(
                SampleEnum.First,
                SampleEnum.Second,
                new TypedTarget(),
                TypedTarget.InfoProperty,
                new EnumToEnumInfoConverter(),
                SampleEnum.First,
                info
            );
        });

    [TestMethod]
    public Task UnsetFallback_DoNothingPreservesTarget_NullRemainsNull() =>
        PlatformThread.Run(() =>
        {
            var source = new TypedSource<bool?>(true);
            var converter = new BoolToValueConverter
            {
                TrueValue = "initial",
                FalseValue = BindingOperations.DoNothing,
                IsNullable = true,
                NullValue = null,
            };
            var target = new TextBox();
            using var subscription = target.Bind(
                TextBox.TextProperty,
                new Binding(nameof(source.Value))
                {
                    Source = source,
                    Converter = converter,
                    Mode = BindingMode.OneWay,
                    FallbackValue = "fallback",
                }
            );
            Assert.AreEqual("initial", target.Text);
            source.Value = false;
            PlatformThread.Drain();
            Assert.AreEqual("initial", target.Text);
            source.Value = null;
            PlatformThread.Drain();
            Assert.IsNull(target.Text);
            converter.TrueValue = AvaloniaProperty.UnsetValue;
            source.Value = true;
            PlatformThread.Drain();
            Assert.AreEqual("fallback", target.Text);
        });

    [TestMethod]
    public Task ReverseDoNothing_PreservesTypedSource() =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrorScope();
            var source = new TypedSource<Guid>(Guid.Empty);
            var converter = new GuidToStringConverter
            {
                DefaultResult = BindingOperations.DoNothing,
            };
            var target = new TextBox();
            using var subscription = target.Bind(
                TextBox.TextProperty,
                new Binding(nameof(source.Value))
                {
                    Source = source,
                    Converter = converter,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                }
            );
            target.SetCurrentValue(TextBox.TextProperty, "invalid");
            PlatformThread.Drain();
            Assert.AreEqual(Guid.Empty, source.Value);
            Assert.AreEqual("invalid", target.Text);
        });
}
