using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters.WPF.Tests;

internal sealed record ConverterCase(
    Type Type,
    Func<ConverterBase> Factory,
    object?[][] Inputs,
    object?[] Expected,
    object? Parameter = null
)
{
    public string Id => Type.Name.Replace("`1", "_String");
    public Type Identity => Type.IsGenericType ? Type.GetGenericTypeDefinition() : Type;
    public bool IsMulti => typeof(MultiValueConverterBase).IsAssignableFrom(Type);
}

internal sealed record EnumExpectation(Enum Value);

internal enum SampleEnum
{
    First,
    Second,
}

internal sealed record StyleExpectation(double Opacity);

internal static class ConverterTestCases
{
    public static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
    public static IEnumerable<object[]> Rows => All.Select(c => new object[] { c.Id });

    public static ConverterCase Find(string id) => All.Single(c => c.Id == id);

    private static ConverterCase S<T>(
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

    private static ConverterCase M<T>(
        object?[][] inputs,
        object?[] expected,
        object? parameter = null,
        Func<T>? factory = null
    )
        where T : ConverterBase, new() =>
        new(typeof(T), () => factory is null ? new T() : factory(), inputs, expected, parameter);

    internal static Style Style(double opacity)
    {
        var style = new Style(typeof(FrameworkElement));
        style.Setters.Add(new Setter(UIElement.OpacityProperty, opacity));
        return style;
    }

    public static IReadOnlyList<ConverterCase> All =>
        [
            S<BoolInverterConverter>(true, false, false, true),
            S<BoolToFontWeightConverter>(true, false, FontWeights.Bold, FontWeights.Normal),
            S<BoolToNumberConverter>(
                true,
                false,
                7,
                2,
                factory: () => new() { TrueValue = 7, FalseValue = 2 }
            ),
            S<BoolToSolidColorBrushConverter>(true, false, Colors.White, Colors.Black),
            S<BoolToSplitParameterConverter>(
                true,
                false,
                "yes",
                "no",
                "yes|no|null",
                () => new() { Separator = "|" }
            ),
            S<BoolToSplitThicknessConverter>(
                true,
                false,
                new Thickness(1, 2, 3, 4),
                new Thickness(5),
                "1,2,3,4|5|0",
                () => new() { Separator = "|" }
            ),
            S<BoolToStyleConverter>(
                true,
                false,
                new StyleExpectation(0.25),
                new StyleExpectation(0.75),
                factory: () => new() { TrueValue = Style(0.25), FalseValue = Style(0.75) }
            ),
            S<BoolToThicknessConverter>(true, false, new Thickness(5), new Thickness(0)),
            S<BoolToValueConverter<string>>(
                true,
                false,
                "yes",
                "no",
                factory: () =>
                    new()
                    {
                        TrueValue = "yes",
                        FalseValue = "no",
                        NullValue = "null",
                        IsNullable = true,
                    }
            ),
            S<BoolToValueConverter>(
                true,
                false,
                "yes",
                "no",
                factory: () =>
                    new()
                    {
                        TrueValue = "yes",
                        FalseValue = "no",
                        NullValue = "null",
                        IsNullable = true,
                    }
            ),
            S<BoolToVisibilityConverter>(true, false, Visibility.Visible, Visibility.Collapsed),
            S<CalculatorConverter>(2, 5, 5, 8, "+3"),
            S<CollectionCountCompareByConverter>(new[] { 1, 2, 3 }, new[] { 1 }, true, false, ">2"),
            S<CollectionCountCompareConverter>(new[] { 1, 2 }, new[] { 1, 2, 3 }, 0, 1, "2"),
            S<DateTimeOffsetToStringConverter>(
                new DateTimeOffset(new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local)),
                new DateTimeOffset(new DateTime(2027, 3, 4, 0, 0, 0, DateTimeKind.Local)),
                "2026-01-02",
                "2027-03-04",
                factory: () => new() { Format = "yyyy-MM-dd" }
            ),
            S<DateTimeToStringConverter>(
                new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local),
                new DateTime(2027, 3, 4, 0, 0, 0, DateTimeKind.Local),
                "2026-01-02",
                "2027-03-04",
                factory: () => new() { Format = "yyyy-MM-dd" }
            ),
            S<DebugConverter>("before", "after", "before", "after"),
            S<EnumEqualsConverter>(DayOfWeek.Monday, DayOfWeek.Tuesday, true, false, "Monday"),
            S<EnumToEnumInfoConverter>(
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                new EnumExpectation(DayOfWeek.Monday),
                new EnumExpectation(DayOfWeek.Tuesday)
            ),
            S<EnumToEnumInfoDisplayConverter>(
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                "Monday",
                "Tuesday"
            ),
            S<EnumsToEnumInfosConverter>(
                new[] { DayOfWeek.Monday },
                new[] { SampleEnum.First },
                new[]
                {
                    new EnumExpectation(DayOfWeek.Sunday),
                    new EnumExpectation(DayOfWeek.Monday),
                    new EnumExpectation(DayOfWeek.Tuesday),
                    new EnumExpectation(DayOfWeek.Wednesday),
                    new EnumExpectation(DayOfWeek.Thursday),
                    new EnumExpectation(DayOfWeek.Friday),
                    new EnumExpectation(DayOfWeek.Saturday),
                },
                new[]
                {
                    new EnumExpectation(SampleEnum.First),
                    new EnumExpectation(SampleEnum.Second),
                }
            ),
            S<EqualsConverter>(
                "match",
                "other",
                true,
                false,
                factory: () => new() { Other = "match" }
            ),
            S<EqualsToValueConverter<string>>(
                "match",
                "other",
                "yes",
                "no",
                factory: () =>
                    new()
                    {
                        Other = "match",
                        TrueValue = "yes",
                        FalseValue = "no",
                        NullValue = "null",
                    }
            ),
            S<EqualsToValueConverter>(
                "match",
                "other",
                "yes",
                "no",
                factory: () =>
                    new()
                    {
                        Other = "match",
                        TrueValue = "yes",
                        FalseValue = "no",
                        NullValue = "null",
                    }
            ),
            S<EqualsToVisibilityConverter>(
                "match",
                "other",
                Visibility.Visible,
                Visibility.Collapsed,
                factory: () => new() { Other = "match" }
            ),
            S<FirstOrDefaultConverter>(new[] { "one", "two" }, new[] { "next" }, "one", "next"),
            S<GetDictionaryValueConverter>(
                new Hashtable { ["key"] = "one" },
                new Hashtable { ["key"] = "two" },
                "one",
                "two",
                "key"
            ),
            S<GuidToStringConverter>(
                Guid.Parse("01234567-89ab-cdef-0123-456789abcdef"),
                Guid.Empty,
                "0123456789abcdef0123456789abcdef",
                "00000000000000000000000000000000",
                factory: () => new() { Format = "N", StringTo = false }
            ),
            S<MediaColorToSolidColorBrushConverter>(
                Colors.Red,
                Colors.Blue,
                Colors.Red,
                Colors.Blue
            ),
            S<NullToTrueConverter>(null, "value", true, false),
            S<NumberClampConverter>(5, 9, true, false, "2,8"),
            S<NumberCompareByConverter>(6, 4, true, false, ">5"),
            S<NumberCompareConverter>(5, 4, 0, -1, "5"),
            S<SolidColorBrushToMediaColorConverter>(
                Brushes.Red,
                Brushes.Blue,
                Colors.Red,
                Colors.Blue
            ),
            S<StringCaseConverter>("first", "next", "FIRST", "NEXT", "U"),
            S<StringCheckConverter>(null, "value", true, false),
            S<StringToNumberConverter>("12", "34", 12, 34),
            S<TimeSpanToStringConverter>(
                TimeSpan.FromMinutes(90),
                TimeSpan.FromMinutes(15),
                "01:30:00",
                "00:15:00",
                factory: () => new() { Format = "c" }
            ),
            S<ToBoolConverter>("true", "false", true, false),
            S<ValueToEnumerableConverter>(
                "one",
                "two",
                new object[] { "one" },
                new object[] { "two" }
            ),
            S<VersionToStringConverter>(new Version(1, 2), new Version(3, 4), "1.2", "3.4"),
            M<AllBoolToValueMultiConverter<string>>(
                [
                    [true, true],
                    [false, true],
                    [false, false],
                    [true, false],
                    [true, true],
                ],
                ["yes", "mixed", "no", "mixed", "yes"],
                factory: () =>
                    new()
                    {
                        TrueValue = "yes",
                        FalseValue = "no",
                        NullValue = "null",
                        DefaultResult = "mixed",
                    }
            ),
            M<AllBoolToValueMultiConverter>(
                [
                    [true, true],
                    [false, true],
                    [false, false],
                    [true, false],
                    [true, true],
                ],
                ["yes", "mixed", "no", "mixed", "yes"],
                factory: () =>
                    new()
                    {
                        TrueValue = "yes",
                        FalseValue = "no",
                        NullValue = "null",
                        DefaultResult = "mixed",
                    }
            ),
            M<AllBoolToVisibilityMultiConverter>(
                [
                    [true, true],
                    [false, true],
                    [false, false],
                    [true, false],
                    [true, true],
                ],
                [
                    Visibility.Visible,
                    Visibility.Collapsed,
                    Visibility.Collapsed,
                    Visibility.Collapsed,
                    Visibility.Visible,
                ]
            ),
            M<AllEqualsMultiConverter<string>>(
                [
                    ["match", "match"],
                    ["other", "match"],
                    ["other", "other"],
                    ["match", "other"],
                    ["match", "match"],
                ],
                [true, false, false, false, true],
                factory: () => new() { Value = "match" }
            ),
            M<AllEqualsMultiConverter>(
                [
                    ["match", "match"],
                    ["other", "match"],
                    ["other", "other"],
                    ["match", "other"],
                    ["match", "match"],
                ],
                [true, false, false, false, true],
                factory: () => new() { Value = "match" }
            ),
            M<AllIsTrueMultiConverter>(
                [
                    [true, true],
                    [false, true],
                    [false, false],
                    [true, false],
                    [true, true],
                ],
                [true, false, false, false, true]
            ),
            M<AnyEqualsMultiConverter<string>>(
                [
                    ["match", "other"],
                    ["other", "other"],
                    ["other", "match"],
                ],
                [true, false, true],
                factory: () => new() { Value = "match" }
            ),
            M<AnyEqualsMultiConverter>(
                [
                    ["match", "other"],
                    ["other", "other"],
                    ["other", "match"],
                ],
                [true, false, true],
                factory: () => new() { Value = "match" }
            ),
            M<CalculatorMultiConverter>(
                [
                    [2, 3],
                    [4, 3],
                    [4, 5],
                ],
                [5, 7, 9],
                "+"
            ),
            M<EqualsCountMultiConverter<string>>(
                [
                    ["match", "other"],
                    ["other", "other"],
                    ["other", "match"],
                ],
                [1, 0, 1],
                factory: () => new() { Value = "match" }
            ),
            M<EqualsCountMultiConverter>(
                [
                    ["match", "other"],
                    ["other", "other"],
                    ["other", "match"],
                ],
                [1, 0, 1],
                factory: () => new() { Value = "match" }
            ),
            M<FirstBoolToValueMultiConverter>(
                [
                    [true, "yes", "no", "null"],
                    [false, "yes", "no", "null"],
                    [false, "yes", "changed", "null"],
                    [true, "yes", "changed", "null"],
                    [true, "updated", "changed", "null"],
                    [null, "updated", "changed", "null"],
                    [null, "updated", "changed", "nil"],
                ],
                ["yes", "no", "changed", "yes", "updated", "null", "nil"]
            ),
            M<FirstEqualsSecondMultiConverter>(
                [
                    ["one", "one"],
                    ["two", "one"],
                    ["two", "two"],
                ],
                [true, false, true]
            ),
            M<FirstStringStateToOtherStringMultiConverter>(
                [
                    ["text", "value", "nil", "empty", "space"],
                    [null, "value", "nil", "empty", "space"],
                    [null, "value", "null", "empty", "space"],
                    ["", "value", "null", "empty", "space"],
                    ["", "value", "null", "blank", "space"],
                    [" ", "value", "null", "blank", "space"],
                    [" ", "value", "null", "blank", "white"],
                    ["text", "value", "null", "blank", "white"],
                    ["text", "new", "null", "blank", "white"],
                ],
                ["value", "nil", "null", "empty", "blank", "space", "white", "value", "new"]
            ),
            DictionaryCase(),
            M<MarginMultiConverter>(
                [
                    [1d, 2d, 3d, 4d],
                    [5d, 2d, 3d, 4d],
                    [5d, 6d, 3d, 4d],
                    [5d, 6d, 7d, 4d],
                    [5d, 6d, 7d, 8d],
                ],
                [
                    new Thickness(1, 2, 3, 4),
                    new Thickness(5, 2, 3, 4),
                    new Thickness(5, 6, 3, 4),
                    new Thickness(5, 6, 7, 4),
                    new Thickness(5, 6, 7, 8),
                ]
            ),
            M<StringFormatMultiConverter>(
                [
                    ["one", 1],
                    ["two", 1],
                    ["two", 2],
                ],
                ["one:1", "two:1", "two:2"],
                "{0}:{1}"
            ),
        ];

    private static ConverterCase DictionaryCase()
    {
        var first = new Hashtable { ["one"] = "a", ["two"] = "b" };
        var next = new Hashtable { ["one"] = "c", ["two"] = "d" };
        return M<GetDictionaryValueMultiConverter>(
            [
                [first, "one"],
                [next, "one"],
                [next, "two"],
            ],
            ["a", "c", "d"]
        );
    }

    public static void AssertResult(object? expected, object? actual)
    {
        if (expected is EnumExpectation enumValue)
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
