using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters.WPF.Tests;

public sealed record WpfScenario(
    Type Type,
    Func<ConverterBase> Factory,
    object?[][] Inputs,
    object?[] Expected,
    object? Parameter = null
)
{
    public static WpfScenario S<T>(
        object? first,
        object? next,
        object? expected,
        object? updated,
        object? parameter = null,
        Func<T>? factory = null
    )
        where T : ConverterBase, new() =>
        new(
            typeof(T),
            () => factory is null ? new T() : factory(),
            [
                [first],
                [next],
            ],
            [expected, updated],
            parameter
        );

    public static WpfScenario M<T>(
        object?[][] inputs,
        object?[] expected,
        object? parameter = null,
        Func<T>? factory = null
    )
        where T : ConverterBase, new() =>
        new(typeof(T), () => factory is null ? new T() : factory(), inputs, expected, parameter);

    public static Style Style(double opacity)
    {
        var style = new Style(typeof(FrameworkElement));
        style.Setters.Add(new Setter(UIElement.OpacityProperty, opacity));
        return style;
    }

    public string Id => Type.Name.Replace("`1", "_String");
    public Type Identity => Type.IsGenericType ? Type.GetGenericTypeDefinition() : Type;
    public bool IsMulti => typeof(MultiValueConverterBase).IsAssignableFrom(Type);
}

internal static class WpfAssert
{
    public static void AssertResult(object? expected, object? actual)
    {
        if (expected is SolidColorBrush expectedBrush && actual is SolidColorBrush actualBrush)
            Assert.AreEqual(expectedBrush.Color, actualBrush.Color);
        else if (expected is Style expectedStyle && actual is Style actualStyle)
        {
            Assert.AreEqual(expectedStyle.TargetType, actualStyle.TargetType);
            CollectionAssert.AreEqual(
                expectedStyle
                    .Setters.OfType<Setter>()
                    .Select(setter => (setter.Property, setter.Value))
                    .ToArray(),
                actualStyle
                    .Setters.OfType<Setter>()
                    .Select(setter => (setter.Property, setter.Value))
                    .ToArray()
            );
        }
        else if (expected is EnumExpectation enumValue)
        {
            var info = actual as IEnumInfo;
            Assert.IsNotNull(info);
            Assert.AreEqual(enumValue.Value, info.Value);
            Assert.AreEqual(enumValue.Value.GetType(), info.EnumType);
        }
        else if (expected is StyleExpectation styleValue)
        {
            var style = actual as Style;
            Assert.IsNotNull(style);
            Assert.AreEqual(typeof(FrameworkElement), style.TargetType);
            var setter = style.Setters.OfType<Setter>().Single();
            Assert.AreSame(UIElement.OpacityProperty, setter.Property);
            Assert.AreEqual(styleValue.Opacity, setter.Value);
        }
        else if (actual is SolidColorBrush brush && expected is Color color)
            Assert.AreEqual(color, brush.Color);
        else if (expected is IEnumerable sequence && expected is not string)
        {
            Assert.IsInstanceOfType<IEnumerable>(actual);
            var expectedItems = sequence.Cast<object?>().ToArray();
            var actualItems = ((IEnumerable)actual).Cast<object?>().ToArray();
            Assert.HasCount(expectedItems.Length, actualItems);
            for (var index = 0; index < expectedItems.Length; index++)
                AssertResult(expectedItems[index], actualItems[index]);
        }
        else
            Assert.AreEqual(expected, actual);
    }
}
