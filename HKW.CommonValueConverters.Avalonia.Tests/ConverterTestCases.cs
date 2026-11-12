using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace HKW.CommonValueConverters.Avalonia.Tests;

public enum SampleEnum
{
    First,
    Second,
}

public enum OtherEnum
{
    Other,
}

public sealed record ConversionStep(int Index, object? Value, object? Expected);

public sealed record ConverterCase(
    string Id,
    Type Type,
    Func<ConverterBase> Create,
    object?[] Initial,
    object? Expected,
    ConversionStep[] Steps,
    object? Parameter,
    bool Multi
);

public static class ConverterTestCases
{
    private static Style Style(string tag) =>
        new() { Setters = { new Setter(Control.TagProperty, tag) } };

    public static ConverterCase Get(string id) => All().Single(c => c.Id == id);

    public static IEnumerable<ConverterCase> All()
    {
        yield return new(
            "BoolInverterConverter",
            typeof(BoolInverterConverter),
            () => new BoolInverterConverter(),
            [true],
            false,
            [new(0, false, true)],
            null,
            false
        );
        yield return new(
            "BoolToFontWeightConverter",
            typeof(BoolToFontWeightConverter),
            () => new BoolToFontWeightConverter(),
            [true],
            FontWeight.Bold,
            [new(0, false, FontWeight.Normal)],
            null,
            false
        );
        yield return new(
            "BoolToNumberConverter",
            typeof(BoolToNumberConverter),
            () => new BoolToNumberConverter { TrueValue = 7, FalseValue = 3 },
            [true],
            7,
            [new(0, false, 3)],
            null,
            false
        );
        yield return new(
            "BoolToSolidColorBrushConverter",
            typeof(BoolToSolidColorBrushConverter),
            () => new BoolToSolidColorBrushConverter(),
            [true],
            Colors.White,
            [new(0, false, Colors.Black)],
            null,
            false
        );
        yield return new(
            "BoolToSplitParameterConverter",
            typeof(BoolToSplitParameterConverter),
            () => new BoolToSplitParameterConverter(),
            [true],
            "yes",
            [new(0, false, "no")],
            "yes,no,nil",
            false
        );
        yield return new(
            "BoolToSplitThicknessConverter",
            typeof(BoolToSplitThicknessConverter),
            () => new BoolToSplitThicknessConverter { Separator = "|" },
            [true],
            new Thickness(2),
            [new(0, false, new Thickness(4))],
            "2|4|0",
            false
        );
        yield return new(
            "BoolToStyleConverter",
            typeof(BoolToStyleConverter),
            () =>
                new BoolToStyleConverter
                {
                    TrueValue = Style("yes-style"),
                    FalseValue = Style("no-style"),
                },
            [true],
            "yes-style",
            [new(0, false, "no-style")],
            null,
            false
        );
        yield return new(
            "BoolToThicknessConverter",
            typeof(BoolToThicknessConverter),
            () => new BoolToThicknessConverter(),
            [true],
            new Thickness(5),
            [new(0, false, new Thickness(0))],
            null,
            false
        );
        yield return new(
            "BoolToValueConverter",
            typeof(BoolToValueConverter),
            () =>
                new BoolToValueConverter
                {
                    TrueValue = "yes",
                    FalseValue = "no",
                    IsNullable = true,
                    NullValue = "nil",
                },
            [true],
            "yes",
            [new(0, false, "no")],
            null,
            false
        );
        yield return new(
            "BoolToValueConverter_String",
            typeof(BoolToValueConverter<string>),
            () =>
                new BoolToValueConverter<string>
                {
                    TrueValue = "yes",
                    FalseValue = "no",
                    IsNullable = true,
                    NullValue = "nil",
                },
            [true],
            "yes",
            [new(0, false, "no")],
            null,
            false
        );
        yield return new(
            "CalculatorConverter",
            typeof(CalculatorConverter),
            () => new CalculatorConverter(),
            [2],
            10,
            [new(0, 5, 13)],
            "+8",
            false
        );
        yield return new(
            "CollectionCountCompareByConverter",
            typeof(CollectionCountCompareByConverter),
            () => new CollectionCountCompareByConverter(),
            [new[] { 1, 2, 3 }],
            true,
            [new(0, new[] { 1 }, false)],
            ">2",
            false
        );
        yield return new(
            "CollectionCountCompareConverter",
            typeof(CollectionCountCompareConverter),
            () => new CollectionCountCompareConverter(),
            [new[] { 1, 2, 3 }],
            0,
            [new(0, new[] { 1 }, -1)],
            "3",
            false
        );
        yield return new(
            "DateTimeOffsetToStringConverter",
            typeof(DateTimeOffsetToStringConverter),
            () => new DateTimeOffsetToStringConverter { Format = "yyyy-MM-dd" },
            [new DateTimeOffset(new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local))],
            "2026-01-02",
            [
                new(
                    0,
                    new DateTimeOffset(new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Local)),
                    "2026-02-03"
                ),
            ],
            null,
            false
        );
        yield return new(
            "DateTimeToStringConverter",
            typeof(DateTimeToStringConverter),
            () => new DateTimeToStringConverter { Format = "yyyy-MM-dd" },
            [new DateTime(2026, 1, 2)],
            "2026-01-02",
            [new(0, new DateTime(2026, 2, 3), "2026-02-03")],
            null,
            false
        );
        yield return new(
            "DebugConverter",
            typeof(DebugConverter),
            () => new DebugConverter(),
            ["before"],
            "before",
            [new(0, "after", "after")],
            null,
            false
        );
        yield return new(
            "EnumEqualsConverter",
            typeof(EnumEqualsConverter),
            () => new EnumEqualsConverter(),
            [SampleEnum.First],
            true,
            [new(0, SampleEnum.Second, false)],
            "First",
            false
        );
        yield return new(
            "EnumToEnumInfoConverter",
            typeof(EnumToEnumInfoConverter),
            () => new EnumToEnumInfoConverter(),
            [SampleEnum.First],
            SampleEnum.First,
            [new(0, SampleEnum.Second, SampleEnum.Second)],
            null,
            false
        );
        yield return new(
            "EnumToEnumInfoDisplayConverter",
            typeof(EnumToEnumInfoDisplayConverter),
            () => new EnumToEnumInfoDisplayConverter(),
            [SampleEnum.First],
            "First",
            [new(0, SampleEnum.Second, "Second")],
            null,
            false
        );
        yield return new(
            "EnumsToEnumInfosConverter",
            typeof(EnumsToEnumInfosConverter),
            () => new EnumsToEnumInfosConverter(),
            [new[] { SampleEnum.First }],
            new object[] { SampleEnum.First, SampleEnum.Second },
            [new(0, new[] { OtherEnum.Other }, new object[] { OtherEnum.Other })],
            null,
            false
        );
        yield return new(
            "EqualsConverter",
            typeof(EqualsConverter),
            () => new EqualsConverter(),
            ["match"],
            true,
            [new(0, "other", false)],
            "match",
            false
        );
        yield return new(
            "EqualsToValueConverter",
            typeof(EqualsToValueConverter),
            () =>
                new EqualsToValueConverter
                {
                    Other = "match",
                    TrueValue = "yes",
                    FalseValue = "no",
                    NullValue = "nil",
                },
            ["match"],
            "yes",
            [new(0, "other", "no")],
            null,
            false
        );
        yield return new(
            "EqualsToValueConverter_String",
            typeof(EqualsToValueConverter<string>),
            () =>
                new EqualsToValueConverter<string>
                {
                    Other = "match",
                    TrueValue = "yes",
                    FalseValue = "no",
                    NullValue = "nil",
                },
            ["match"],
            "yes",
            [new(0, "other", "no")],
            null,
            false
        );
        yield return new(
            "FirstOrDefaultConverter",
            typeof(FirstOrDefaultConverter),
            () => new FirstOrDefaultConverter(),
            [new[] { 4, 5 }],
            4,
            [new(0, new[] { 8, 9 }, 8)],
            null,
            false
        );
        yield return new(
            "GetDictionaryValueConverter",
            typeof(GetDictionaryValueConverter),
            () => new GetDictionaryValueConverter(),
            [new Hashtable { ["key"] = "before" }],
            "before",
            [new(0, new Hashtable { ["key"] = "after" }, "after")],
            "key",
            false
        );
        yield return new(
            "GuidToStringConverter",
            typeof(GuidToStringConverter),
            () => new GuidToStringConverter(),
            [Guid.Parse("00000000-0000-0000-0000-000000000001")],
            "00000000-0000-0000-0000-000000000001",
            [
                new(
                    0,
                    Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    "00000000-0000-0000-0000-000000000002"
                ),
            ],
            null,
            false
        );
        yield return new(
            "MediaColorToSolidColorBrushConverter",
            typeof(MediaColorToSolidColorBrushConverter),
            () => new MediaColorToSolidColorBrushConverter(),
            [Colors.Red],
            Colors.Red,
            [new(0, Colors.Blue, Colors.Blue)],
            null,
            false
        );
        yield return new(
            "NullToTrueConverter",
            typeof(NullToTrueConverter),
            () => new NullToTrueConverter(),
            [null],
            true,
            [new(0, "value", false)],
            null,
            false
        );
        yield return new(
            "NumberClampConverter",
            typeof(NumberClampConverter),
            () => new NumberClampConverter(),
            [5],
            true,
            [new(0, 12, false)],
            "0,10",
            false
        );
        yield return new(
            "NumberCompareByConverter",
            typeof(NumberCompareByConverter),
            () => new NumberCompareByConverter(),
            [5],
            true,
            [new(0, 1, false)],
            ">3",
            false
        );
        yield return new(
            "NumberCompareConverter",
            typeof(NumberCompareConverter),
            () => new NumberCompareConverter(),
            [5],
            1,
            [new(0, 1, -1)],
            "3",
            false
        );
        yield return new(
            "SolidColorBrushToMediaColorConverter",
            typeof(SolidColorBrushToMediaColorConverter),
            () => new SolidColorBrushToMediaColorConverter(),
            [new SolidColorBrush(Colors.Red)],
            Colors.Red,
            [new(0, new SolidColorBrush(Colors.Blue), Colors.Blue)],
            null,
            false
        );
        yield return new(
            "StringCaseConverter",
            typeof(StringCaseConverter),
            () => new StringCaseConverter(),
            ["before"],
            "BEFORE",
            [new(0, "after", "AFTER")],
            "U",
            false
        );
        yield return new(
            "StringCheckConverter",
            typeof(StringCheckConverter),
            () => new StringCheckConverter(),
            [null],
            true,
            [new(0, "text", false)],
            null,
            false
        );
        yield return new(
            "StringToNumberConverter",
            typeof(StringToNumberConverter),
            () => new StringToNumberConverter(),
            ["12"],
            12,
            [new(0, "34", 34)],
            null,
            false
        );
        yield return new(
            "TimeSpanToStringConverter",
            typeof(TimeSpanToStringConverter),
            () => new TimeSpanToStringConverter { Format = "c" },
            [TimeSpan.FromHours(1)],
            "01:00:00",
            [new(0, TimeSpan.FromHours(2), "02:00:00")],
            null,
            false
        );
        yield return new(
            "ToBoolConverter",
            typeof(ToBoolConverter),
            () => new ToBoolConverter(),
            ["1"],
            true,
            [new(0, "0", false)],
            null,
            false
        );
        yield return new(
            "ValueToEnumerableConverter",
            typeof(ValueToEnumerableConverter),
            () => new ValueToEnumerableConverter(),
            ["before"],
            new object[] { "before" },
            [new(0, "after", new object[] { "after" })],
            null,
            false
        );
        yield return new(
            "VersionToStringConverter",
            typeof(VersionToStringConverter),
            () => new VersionToStringConverter(),
            [new Version(1, 2)],
            "1.2",
            [new(0, new Version(3, 4), "3.4")],
            null,
            false
        );
        yield return new(
            "AllBoolToValueMultiConverter",
            typeof(AllBoolToValueMultiConverter),
            () =>
                new AllBoolToValueMultiConverter
                {
                    TrueValue = "yes",
                    FalseValue = "no",
                    NullValue = "nil",
                    DefaultResult = "mixed",
                },
            [true, true],
            "yes",
            [new(0, false, "mixed"), new(1, false, "no")],
            null,
            true
        );
        yield return new(
            "AllEqualsMultiConverter",
            typeof(AllEqualsMultiConverter),
            () => new AllEqualsMultiConverter { Value = "match" },
            ["match", "match"],
            true,
            [new(0, "other", false), new(0, "match", true), new(1, "other", false)],
            null,
            true
        );
        yield return new(
            "AnyEqualsMultiConverter",
            typeof(AnyEqualsMultiConverter),
            () => new AnyEqualsMultiConverter { Value = "match" },
            ["match", "other"],
            true,
            [new(0, "other", false), new(1, "match", true)],
            null,
            true
        );
        yield return new(
            "EqualsCountMultiConverter",
            typeof(EqualsCountMultiConverter),
            () => new EqualsCountMultiConverter { Value = "match" },
            ["match", "other"],
            1,
            [new(0, "other", 0), new(1, "match", 1)],
            null,
            true
        );
        yield return new(
            "AllBoolToValueMultiConverter_String",
            typeof(AllBoolToValueMultiConverter<string>),
            () =>
                new AllBoolToValueMultiConverter<string>
                {
                    TrueValue = "yes",
                    FalseValue = "no",
                    NullValue = "nil",
                    DefaultResult = "mixed",
                },
            [true, true],
            "yes",
            [new(0, false, "mixed"), new(1, false, "no")],
            null,
            true
        );
        yield return new(
            "AllEqualsMultiConverter_String",
            typeof(AllEqualsMultiConverter<string>),
            () => new AllEqualsMultiConverter<string> { Value = "match" },
            ["match", "match"],
            true,
            [new(0, "other", false), new(0, "match", true), new(1, "other", false)],
            null,
            true
        );
        yield return new(
            "AnyEqualsMultiConverter_String",
            typeof(AnyEqualsMultiConverter<string>),
            () => new AnyEqualsMultiConverter<string> { Value = "match" },
            ["match", "other"],
            true,
            [new(0, "other", false), new(1, "match", true)],
            null,
            true
        );
        yield return new(
            "EqualsCountMultiConverter_String",
            typeof(EqualsCountMultiConverter<string>),
            () => new EqualsCountMultiConverter<string> { Value = "match" },
            ["match", "other"],
            1,
            [new(0, "other", 0), new(1, "match", 1)],
            null,
            true
        );
        yield return new(
            "AllIsTrueMultiConverter",
            typeof(AllIsTrueMultiConverter),
            () => new AllIsTrueMultiConverter(),
            [true, true],
            true,
            [new(0, false, false), new(0, true, true), new(1, false, false)],
            null,
            true
        );
        yield return new(
            "CalculatorMultiConverter",
            typeof(CalculatorMultiConverter),
            () => new CalculatorMultiConverter(),
            [2, 3],
            5,
            [new(0, 4, 7), new(1, 6, 10)],
            "+",
            true
        );
        yield return new(
            "FirstBoolToValueMultiConverter",
            typeof(FirstBoolToValueMultiConverter),
            () => new FirstBoolToValueMultiConverter(),
            [true, "yes", "no", "nil"],
            "yes",
            [
                new(0, false, "no"),
                new(2, "changed", "changed"),
                new(0, true, "yes"),
                new(1, "updated", "updated"),
                new(0, null, "nil"),
                new(3, "empty", "empty"),
            ],
            null,
            true
        );
        yield return new(
            "FirstEqualsSecondMultiConverter",
            typeof(FirstEqualsSecondMultiConverter),
            () => new FirstEqualsSecondMultiConverter(),
            ["one", "one"],
            true,
            [new(0, "two", false), new(1, "two", true)],
            null,
            true
        );
        yield return new(
            "FirstStringStateToOtherStringMultiConverter",
            typeof(FirstStringStateToOtherStringMultiConverter),
            () => new FirstStringStateToOtherStringMultiConverter(),
            ["text", "text-result", "null-result", "empty-result", "space-result"],
            "text-result",
            [
                new(1, "updated", "updated"),
                new(0, null, "null-result"),
                new(2, "nil", "nil"),
                new(0, "", "empty-result"),
                new(3, "empty", "empty"),
                new(0, " ", "space-result"),
                new(4, "space", "space"),
            ],
            null,
            true
        );
        yield return new(
            "GetDictionaryValueMultiConverter",
            typeof(GetDictionaryValueMultiConverter),
            () => new GetDictionaryValueMultiConverter(),
            [new Hashtable { ["a"] = "first", ["b"] = "second" }, "a"],
            "first",
            [new(1, "b", "second"), new(0, new Hashtable { ["b"] = "changed" }, "changed")],
            null,
            true
        );
        yield return new(
            "MarginMultiConverter",
            typeof(MarginMultiConverter),
            () => new MarginMultiConverter(),
            [1, 2, 3, 4],
            new Thickness(1, 2, 3, 4),
            [
                new(0, 5, new Thickness(5, 2, 3, 4)),
                new(1, 6, new Thickness(5, 6, 3, 4)),
                new(2, 7, new Thickness(5, 6, 7, 4)),
                new(3, 8, new Thickness(5, 6, 7, 8)),
            ],
            null,
            true
        );
        yield return new(
            "StringFormatMultiConverter",
            typeof(StringFormatMultiConverter),
            () => new StringFormatMultiConverter(),
            ["a", "b"],
            "a:b",
            [new(0, "c", "c:b"), new(1, "d", "c:d")],
            "{0}:{1}",
            true
        );
    }
}
