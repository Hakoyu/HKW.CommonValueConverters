using System.Collections;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ConverterCoverageAttribute(Type type, string layer, string id) : Attribute
{
    public Type Type { get; } = type;
    public string Layer { get; } = layer;
    public string Id { get; } = id;
}

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

    public static Task Behavior(string id) =>
        PlatformThread.Run(() =>
        {
            var c = ConverterTestCases.Get(id);
            var converter = c.Create();
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
        });

    public static Task Binding(string id) =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrorScope();
            var c = ConverterTestCases.Get(id);
            var vm = new TestViewModel(c.Initial);
            var converter = c.Create();
            var target = new TestTarget { DataContext = vm };
            BindingBase binding;
            if (c.Multi)
            {
                var multi = new MultiBinding
                {
                    Converter = (IMultiValueConverter)converter,
                    ConverterParameter = c.Parameter,
                };
                for (var i = 0; i < c.Initial.Length; i++)
                    multi.Bindings.Add(new Binding($"V{i}"));
                binding = multi;
            }
            else
                binding = new Binding("V0")
                {
                    Converter = (IValueConverter)converter,
                    ConverterParameter = c.Parameter,
                    Mode = BindingMode.OneWay,
                };
            using var subscription = target.Bind(TestTarget.ResultProperty, binding);
            VerifyUpdates(c, target, vm);
            target.DataContext = null;
        });

    public static Task Xaml(string id) =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrorScope();
            var c = ConverterTestCases.Get(id);
            var target = (TestTarget)
                AvaloniaXamlLoader.Load(
                    new Uri(
                        $"avares://HKW.CommonValueConverters.Avalonia.Tests/TestViews/{id}.axaml"
                    )
                );
            Assert.HasCount(1, target.Resources);
            var converter = (ConverterBase)target.Resources[c.Id]!;
            var actualType = converter.GetType();
            Assert.AreEqual(c.Type, c.Type.IsGenericType ? actualType.BaseType : actualType);
            Assert.IsTrue(c.Type.IsInstanceOfType(converter));
            var expectedConfiguration = c.Create();
            foreach (
                var property in c
                    .Type.GetProperties()
                    .Where(p =>
                        p.CanRead
                        && p.GetIndexParameters().Length == 0
                        && typeof(Delegate).IsAssignableFrom(p.PropertyType) is false
                        && p.DeclaringType?.Namespace == typeof(ConverterBase).Namespace
                    )
            )
            {
                var expected = property.GetValue(expectedConfiguration);
                if (expected is ISolidColorBrush brush)
                    expected = brush.Color;
                if (expected is Style style)
                    expected = ((Setter)style.Setters[0]).Value;
                Equal(expected, property.GetValue(converter));
            }
            var vm = new TestViewModel(c.Initial);
            target.DataContext = vm;
            try
            {
                VerifyUpdates(c, target, vm);
            }
            finally
            {
                target.ClearValue(TestTarget.ResultProperty);
                target.DataContext = null;
                target.Resources.Clear();
            }
        });

    private static void VerifyUpdates(ConverterCase c, TestTarget target, TestViewModel vm)
    {
        PlatformThread.Drain();
        Result(c, c.Expected, target.Result);
        foreach (var step in c.Steps)
        {
            vm.Set(step.Index, step.Value);
            PlatformThread.Drain();
            Result(c, step.Expected, target.Result);
        }
    }
}
