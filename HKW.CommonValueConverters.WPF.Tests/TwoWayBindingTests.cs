using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters.WPF.Tests;

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

public sealed class ColorTarget : FrameworkElement
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color),
        typeof(Color),
        typeof(ColorTarget)
    );
    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
}

public sealed class EnumInfoTarget : FrameworkElement
{
    public static readonly DependencyProperty InfoProperty = DependencyProperty.Register(
        nameof(Info),
        typeof(IEnumInfo),
        typeof(EnumInfoTarget)
    );
    public IEnumInfo? Info
    {
        get => (IEnumInfo?)GetValue(InfoProperty);
        set => SetValue(InfoProperty, value);
    }
}

[TestClass]
public sealed class TwoWayBindingTests
{
    [TestMethod]
    public void DateTimeToStringConverter_TypedTextBoxRoundTrip() =>
        PlatformThread.Run(() =>
        {
            var first = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local);
            var next = new DateTime(2027, 3, 4, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
            Verify(
                new DateTimeToStringConverter { Format = "yyyy-MM-dd" },
                new TextBox(),
                TextBox.TextProperty,
                first,
                "2026-01-02",
                "2027-03-04",
                next
            );
        });

    [TestMethod]
    public void DateTimeOffsetToStringConverter_TypedTextBoxRoundTrip() =>
        PlatformThread.Run(() =>
            Verify(
                new DateTimeOffsetToStringConverter { Format = "yyyy-MM-dd" },
                new TextBox(),
                TextBox.TextProperty,
                new DateTimeOffset(new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local)),
                "2026-01-02",
                "2027-03-04",
                new DateTimeOffset(
                    new DateTime(2027, 3, 4, 0, 0, 0, DateTimeKind.Local)
                ).ToUniversalTime()
            )
        );

    [TestMethod]
    public void DebugConverter_TypedTextBoxRoundTrip() =>
        PlatformThread.Run(() =>
            Verify(
                new DebugConverter(),
                new TextBox(),
                TextBox.TextProperty,
                "first",
                "first",
                "next",
                "next"
            )
        );

    [TestMethod]
    public void GuidToStringConverter_TypedTextBoxRoundTrip() =>
        PlatformThread.Run(() =>
            Verify(
                new GuidToStringConverter { Format = "D", StringTo = false },
                new TextBox(),
                TextBox.TextProperty,
                Guid.Empty,
                "00000000-0000-0000-0000-000000000000",
                "01234567-89ab-cdef-0123-456789abcdef",
                Guid.Parse("01234567-89ab-cdef-0123-456789abcdef")
            )
        );

    [TestMethod]
    public void TimeSpanToStringConverter_TypedTextBoxRoundTrip() =>
        PlatformThread.Run(() =>
            Verify(
                new TimeSpanToStringConverter { Format = "c" },
                new TextBox(),
                TextBox.TextProperty,
                TimeSpan.FromMinutes(90),
                "01:30:00",
                "00:15:00",
                TimeSpan.FromMinutes(15)
            )
        );

    [TestMethod]
    public void StringToNumberConverter_TypedSliderRoundTrip() =>
        PlatformThread.Run(() =>
            Verify(
                new StringToNumberConverter { NumberType = NumberType.Double },
                new Slider { Maximum = 100 },
                Slider.ValueProperty,
                "12",
                12d,
                34d,
                "34"
            )
        );

    [TestMethod]
    public void MediaColorToSolidColorBrushConverter_TypedBrushRoundTrip() =>
        PlatformThread.Run(() =>
            Verify(
                new MediaColorToSolidColorBrushConverter(),
                new Border(),
                Border.BackgroundProperty,
                Colors.Red,
                Colors.Red,
                Brushes.Blue,
                Colors.Blue
            )
        );

    [TestMethod]
    public void SolidColorBrushToMediaColorConverter_TypedColorRoundTrip() =>
        PlatformThread.Run(() =>
            Verify(
                new SolidColorBrushToMediaColorConverter(),
                new ColorTarget(),
                ColorTarget.ColorProperty,
                Brushes.Red,
                Colors.Red,
                Colors.Blue,
                Brushes.Blue
            )
        );

    [TestMethod]
    public void EnumToEnumInfoConverter_TypedInfoRoundTrip() =>
        PlatformThread.Run(() =>
        {
            var converter = new EnumToEnumInfoConverter();
            // 反向输入由公开枚举元数据生成, 预期值独立指定.
            var nextInfo = HKW.HKWUtils.Extensions.EnumExtensions.GetInfo(DayOfWeek.Tuesday);
            Verify(
                converter,
                new EnumInfoTarget(),
                EnumInfoTarget.InfoProperty,
                DayOfWeek.Monday,
                new EnumExpectation(DayOfWeek.Monday),
                nextInfo,
                DayOfWeek.Tuesday
            );
        });

    private static void Verify<T>(
        IValueConverter converter,
        FrameworkElement target,
        DependencyProperty property,
        T initial,
        object expectedTarget,
        object updatedTarget,
        T expectedSource
    )
    {
        using var errors = new BindingErrors();
        var source = new TypedSource<T>(initial);
        try
        {
            var expression = (BindingExpression)
                BindingOperations.SetBinding(
                    target,
                    property,
                    new Binding("Value")
                    {
                        Source = source,
                        Converter = converter,
                        ConverterCulture = CultureInfo.InvariantCulture,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                    }
                );
            PlatformThread.Drain();
            ConverterTestCases.AssertResult(expectedTarget, target.GetValue(property));
            target.SetCurrentValue(property, updatedTarget);
            expression.UpdateSource();
            PlatformThread.Drain();
            Assert.AreEqual(BindingStatus.Active, expression.Status);
            Assert.IsFalse(expression.HasError);
            if (expectedSource is SolidColorBrush brush)
                ConverterTestCases.AssertResult(brush.Color, source.Value);
            else
                Assert.AreEqual(expectedSource, source.Value);
            source.Value = initial;
            PlatformThread.Drain();
            ConverterTestCases.AssertResult(expectedTarget, target.GetValue(property));
            errors.AssertEmpty();
        }
        finally
        {
            BindingOperations.ClearAllBindings(target);
        }
    }

    [TestMethod]
    public void ConvertBack_UnsetAndDoNothingKeepTypedSource() =>
        PlatformThread.Run(() =>
        {
            var source = new TypedSource<TimeSpan>(TimeSpan.FromMinutes(90));
            var target = new TextBox();
            var converter = new TimeSpanToStringConverter
            {
                Format = "c",
                DefaultResult = Binding.DoNothing,
            };
            try
            {
                var expression = (BindingExpression)
                    BindingOperations.SetBinding(
                        target,
                        TextBox.TextProperty,
                        new Binding("Value")
                        {
                            Source = source,
                            Converter = converter,
                            ConverterCulture = CultureInfo.InvariantCulture,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                        }
                    );
                PlatformThread.Drain();
                target.SetCurrentValue(TextBox.TextProperty, "invalid");
                expression.UpdateSource();
                Assert.AreEqual(TimeSpan.FromMinutes(90), source.Value);
                converter.DefaultResult = DependencyProperty.UnsetValue;
                target.SetCurrentValue(TextBox.TextProperty, "still-invalid");
                expression.UpdateSource();
                Assert.AreEqual(TimeSpan.FromMinutes(90), source.Value);
                target.SetCurrentValue(TextBox.TextProperty, "00:15:00");
                expression.UpdateSource();
                Assert.AreEqual(TimeSpan.FromMinutes(15), source.Value);
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });
}
