using System.Collections;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters.Avalonia.Tests;

public static class ConverterAssertions
{
    public static void Equal(object? expected, object? actual)
    {
        if (actual is IEnumInfo info)
        {
            Assert.AreEqual(expected, info.Value);
            Assert.AreEqual(expected?.GetType(), info.EnumType);
        }
        else if (actual is ISolidColorBrush brush)
            Assert.AreEqual(expected, brush.Color);
        else if (actual is Style style)
        {
            Assert.HasCount(1, style.Setters);
            var setter = (Setter)style.Setters[0];
            Assert.AreSame(Control.TagProperty, setter.Property);
            Assert.AreEqual(expected, setter.Value);
        }
        else if (expected is IEnumerable sequence && expected is not string)
        {
            Assert.IsInstanceOfType<IEnumerable>(actual);
            var left = sequence.Cast<object?>().ToArray();
            var right = ((IEnumerable)actual).Cast<object?>().ToArray();
            Assert.HasCount(left.Length, right);
            for (var i = 0; i < left.Length; i++)
                Equal(left[i], right[i]);
        }
        else
            Assert.AreEqual(expected, actual);
    }

    private static void Result(ConverterCase c, object? expected, object? actual)
    {
        if (
            c.Type == typeof(MediaColorToSolidColorBrushConverter)
            || c.Type == typeof(BoolToSolidColorBrushConverter)
        )
            Assert.IsInstanceOfType<ISolidColorBrush>(actual);
        if (c.Type == typeof(BoolToStyleConverter))
            Assert.IsInstanceOfType<Style>(actual);
        if (c.Type == typeof(EnumToEnumInfoConverter))
            Assert.IsInstanceOfType<IEnumInfo>(actual);
        Equal(expected, actual);
    }

    public static void Behavior(ConverterCase c)
    {
        var converter = c.Create();
        Assert.AreEqual(c.Type, converter.GetType());
        var values = c.Initial.ToArray();
        object? Convert() =>
            c.Multi
                ? ((IMultiValueConverter)converter).Convert(
                    values,
                    typeof(object),
                    c.Parameter,
                    CultureInfo.InvariantCulture
                )
                : ((IValueConverter)converter).Convert(
                    values[0],
                    typeof(object),
                    c.Parameter,
                    CultureInfo.InvariantCulture
                );
        Result(c, c.Expected, Convert());
        foreach (var step in c.Steps)
        {
            values[step.Index] = step.Value;
            var snapshot = values.ToArray();
            Result(c, step.Expected, Convert());
            CollectionAssert.AreEqual(snapshot, values);
        }
    }

    internal static void VerifyUpdates(ConverterCase c, TestTarget target, TestViewModel vm)
    {
        Assert.IsNotEmpty(c.Steps);
        Assert.IsTrue(c.Steps.All(step => step.Index >= 0 && step.Index < c.Initial.Length));
        CollectionAssert.AreEquivalent(
            Enumerable.Range(0, c.Initial.Length).ToArray(),
            c.Steps.Select(step => step.Index).Distinct().ToArray()
        );
        PlatformThread.Drain();
        Result(c, c.Expected, target.Result);
        foreach (var step in c.Steps)
        {
            vm.Set(step.Index, step.Value);
            PlatformThread.Drain();
            Result(c, step.Expected, target.Result);
        }
    }

    internal static void ConfigurationEqual(object? expected, object? actual)
    {
        if (expected is ISolidColorBrush brush)
            expected = brush.Color;
        if (expected is Style style)
            expected = ((Setter)style.Setters[0]).Value;
        Equal(expected, actual);
    }

    internal static void Reverse(ConverterCase c, bool supported)
    {
        var converter = (IValueConverter)c.Create();
        if (supported is false)
        {
            Assert.ThrowsExactly<NotSupportedException>(
                () =>
                    converter.ConvertBack(
                        c.Expected,
                        typeof(object),
                        c.Parameter,
                        CultureInfo.InvariantCulture
                    ),
                c.Id
            );
            return;
        }
        var converted = converter.Convert(
            c.Initial[0],
            typeof(object),
            c.Parameter,
            CultureInfo.InvariantCulture
        );
        var original = c.Initial[0];
        if (original is DateTime date)
            original = date.ToUniversalTime();
        if (original is DateTimeOffset offset)
            original = offset.ToUniversalTime();
        if (original is ISolidColorBrush brush)
            original = brush.Color;
        Equal(
            original,
            converter.ConvertBack(
                converted,
                c.Initial[0]!.GetType(),
                c.Parameter,
                CultureInfo.InvariantCulture
            )
        );
    }
}
