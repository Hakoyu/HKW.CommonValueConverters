namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class BehaviorIntegrationTests
{
    [TestMethod(DisplayName = "BoolInverterConverter / Behavior")]
    [ConverterCoverage(typeof(BoolInverterConverter), "Behavior", "BoolInverterConverter")]
    public Task BoolInverterConverter_Behavior() =>
        ConverterAssertions.Behavior("BoolInverterConverter");

    [TestMethod(DisplayName = "BoolToFontWeightConverter / Behavior")]
    [ConverterCoverage(typeof(BoolToFontWeightConverter), "Behavior", "BoolToFontWeightConverter")]
    public Task BoolToFontWeightConverter_Behavior() =>
        ConverterAssertions.Behavior("BoolToFontWeightConverter");

    [TestMethod(DisplayName = "BoolToNumberConverter / Behavior")]
    [ConverterCoverage(typeof(BoolToNumberConverter), "Behavior", "BoolToNumberConverter")]
    public Task BoolToNumberConverter_Behavior() =>
        ConverterAssertions.Behavior("BoolToNumberConverter");

    [TestMethod(DisplayName = "BoolToSolidColorBrushConverter / Behavior")]
    [ConverterCoverage(
        typeof(BoolToSolidColorBrushConverter),
        "Behavior",
        "BoolToSolidColorBrushConverter"
    )]
    public Task BoolToSolidColorBrushConverter_Behavior() =>
        ConverterAssertions.Behavior("BoolToSolidColorBrushConverter");

    [TestMethod(DisplayName = "BoolToSplitParameterConverter / Behavior")]
    [ConverterCoverage(
        typeof(BoolToSplitParameterConverter),
        "Behavior",
        "BoolToSplitParameterConverter"
    )]
    public Task BoolToSplitParameterConverter_Behavior() =>
        ConverterAssertions.Behavior("BoolToSplitParameterConverter");

    [TestMethod(DisplayName = "BoolToSplitThicknessConverter / Behavior")]
    [ConverterCoverage(
        typeof(BoolToSplitThicknessConverter),
        "Behavior",
        "BoolToSplitThicknessConverter"
    )]
    public Task BoolToSplitThicknessConverter_Behavior() =>
        ConverterAssertions.Behavior("BoolToSplitThicknessConverter");

    [TestMethod(DisplayName = "BoolToStyleConverter / Behavior")]
    [ConverterCoverage(typeof(BoolToStyleConverter), "Behavior", "BoolToStyleConverter")]
    public Task BoolToStyleConverter_Behavior() =>
        ConverterAssertions.Behavior("BoolToStyleConverter");

    [TestMethod(DisplayName = "BoolToThicknessConverter / Behavior")]
    [ConverterCoverage(typeof(BoolToThicknessConverter), "Behavior", "BoolToThicknessConverter")]
    public Task BoolToThicknessConverter_Behavior() =>
        ConverterAssertions.Behavior("BoolToThicknessConverter");

    [TestMethod(DisplayName = "BoolToValueConverter / Behavior")]
    [ConverterCoverage(typeof(BoolToValueConverter), "Behavior", "BoolToValueConverter")]
    public Task BoolToValueConverter_Behavior() =>
        ConverterAssertions.Behavior("BoolToValueConverter");

    [TestMethod(DisplayName = "BoolToValueConverter<string> / Behavior")]
    [ConverterCoverage(
        typeof(BoolToValueConverter<string>),
        "Behavior",
        "BoolToValueConverter_String"
    )]
    public Task BoolToValueConverter_String_Behavior() =>
        ConverterAssertions.Behavior("BoolToValueConverter_String");

    [TestMethod(DisplayName = "CalculatorConverter / Behavior")]
    [ConverterCoverage(typeof(CalculatorConverter), "Behavior", "CalculatorConverter")]
    public Task CalculatorConverter_Behavior() =>
        ConverterAssertions.Behavior("CalculatorConverter");

    [TestMethod(DisplayName = "CollectionCountCompareByConverter / Behavior")]
    [ConverterCoverage(
        typeof(CollectionCountCompareByConverter),
        "Behavior",
        "CollectionCountCompareByConverter"
    )]
    public Task CollectionCountCompareByConverter_Behavior() =>
        ConverterAssertions.Behavior("CollectionCountCompareByConverter");

    [TestMethod(DisplayName = "CollectionCountCompareConverter / Behavior")]
    [ConverterCoverage(
        typeof(CollectionCountCompareConverter),
        "Behavior",
        "CollectionCountCompareConverter"
    )]
    public Task CollectionCountCompareConverter_Behavior() =>
        ConverterAssertions.Behavior("CollectionCountCompareConverter");

    [TestMethod(DisplayName = "DateTimeOffsetToStringConverter / Behavior")]
    [ConverterCoverage(
        typeof(DateTimeOffsetToStringConverter),
        "Behavior",
        "DateTimeOffsetToStringConverter"
    )]
    public Task DateTimeOffsetToStringConverter_Behavior() =>
        ConverterAssertions.Behavior("DateTimeOffsetToStringConverter");

    [TestMethod(DisplayName = "DateTimeToStringConverter / Behavior")]
    [ConverterCoverage(typeof(DateTimeToStringConverter), "Behavior", "DateTimeToStringConverter")]
    public Task DateTimeToStringConverter_Behavior() =>
        ConverterAssertions.Behavior("DateTimeToStringConverter");

    [TestMethod(DisplayName = "DebugConverter / Behavior")]
    [ConverterCoverage(typeof(DebugConverter), "Behavior", "DebugConverter")]
    public Task DebugConverter_Behavior() => ConverterAssertions.Behavior("DebugConverter");

    [TestMethod(DisplayName = "EnumEqualsConverter / Behavior")]
    [ConverterCoverage(typeof(EnumEqualsConverter), "Behavior", "EnumEqualsConverter")]
    public Task EnumEqualsConverter_Behavior() =>
        ConverterAssertions.Behavior("EnumEqualsConverter");

    [TestMethod(DisplayName = "EnumToEnumInfoConverter / Behavior")]
    [ConverterCoverage(typeof(EnumToEnumInfoConverter), "Behavior", "EnumToEnumInfoConverter")]
    public Task EnumToEnumInfoConverter_Behavior() =>
        ConverterAssertions.Behavior("EnumToEnumInfoConverter");

    [TestMethod(DisplayName = "EnumToEnumInfoDisplayConverter / Behavior")]
    [ConverterCoverage(
        typeof(EnumToEnumInfoDisplayConverter),
        "Behavior",
        "EnumToEnumInfoDisplayConverter"
    )]
    public Task EnumToEnumInfoDisplayConverter_Behavior() =>
        ConverterAssertions.Behavior("EnumToEnumInfoDisplayConverter");

    [TestMethod(DisplayName = "EnumsToEnumInfosConverter / Behavior")]
    [ConverterCoverage(typeof(EnumsToEnumInfosConverter), "Behavior", "EnumsToEnumInfosConverter")]
    public Task EnumsToEnumInfosConverter_Behavior() =>
        ConverterAssertions.Behavior("EnumsToEnumInfosConverter");

    [TestMethod(DisplayName = "EqualsConverter / Behavior")]
    [ConverterCoverage(typeof(EqualsConverter), "Behavior", "EqualsConverter")]
    public Task EqualsConverter_Behavior() => ConverterAssertions.Behavior("EqualsConverter");

    [TestMethod(DisplayName = "EqualsToValueConverter / Behavior")]
    [ConverterCoverage(typeof(EqualsToValueConverter), "Behavior", "EqualsToValueConverter")]
    public Task EqualsToValueConverter_Behavior() =>
        ConverterAssertions.Behavior("EqualsToValueConverter");

    [TestMethod(DisplayName = "EqualsToValueConverter<string> / Behavior")]
    [ConverterCoverage(
        typeof(EqualsToValueConverter<string>),
        "Behavior",
        "EqualsToValueConverter_String"
    )]
    public Task EqualsToValueConverter_String_Behavior() =>
        ConverterAssertions.Behavior("EqualsToValueConverter_String");

    [TestMethod(DisplayName = "FirstOrDefaultConverter / Behavior")]
    [ConverterCoverage(typeof(FirstOrDefaultConverter), "Behavior", "FirstOrDefaultConverter")]
    public Task FirstOrDefaultConverter_Behavior() =>
        ConverterAssertions.Behavior("FirstOrDefaultConverter");

    [TestMethod(DisplayName = "GetDictionaryValueConverter / Behavior")]
    [ConverterCoverage(
        typeof(GetDictionaryValueConverter),
        "Behavior",
        "GetDictionaryValueConverter"
    )]
    public Task GetDictionaryValueConverter_Behavior() =>
        ConverterAssertions.Behavior("GetDictionaryValueConverter");

    [TestMethod(DisplayName = "GuidToStringConverter / Behavior")]
    [ConverterCoverage(typeof(GuidToStringConverter), "Behavior", "GuidToStringConverter")]
    public Task GuidToStringConverter_Behavior() =>
        ConverterAssertions.Behavior("GuidToStringConverter");

    [TestMethod(DisplayName = "MediaColorToSolidColorBrushConverter / Behavior")]
    [ConverterCoverage(
        typeof(MediaColorToSolidColorBrushConverter),
        "Behavior",
        "MediaColorToSolidColorBrushConverter"
    )]
    public Task MediaColorToSolidColorBrushConverter_Behavior() =>
        ConverterAssertions.Behavior("MediaColorToSolidColorBrushConverter");

    [TestMethod(DisplayName = "NullToTrueConverter / Behavior")]
    [ConverterCoverage(typeof(NullToTrueConverter), "Behavior", "NullToTrueConverter")]
    public Task NullToTrueConverter_Behavior() =>
        ConverterAssertions.Behavior("NullToTrueConverter");

    [TestMethod(DisplayName = "NumberClampConverter / Behavior")]
    [ConverterCoverage(typeof(NumberClampConverter), "Behavior", "NumberClampConverter")]
    public Task NumberClampConverter_Behavior() =>
        ConverterAssertions.Behavior("NumberClampConverter");

    [TestMethod(DisplayName = "NumberCompareByConverter / Behavior")]
    [ConverterCoverage(typeof(NumberCompareByConverter), "Behavior", "NumberCompareByConverter")]
    public Task NumberCompareByConverter_Behavior() =>
        ConverterAssertions.Behavior("NumberCompareByConverter");

    [TestMethod(DisplayName = "NumberCompareConverter / Behavior")]
    [ConverterCoverage(typeof(NumberCompareConverter), "Behavior", "NumberCompareConverter")]
    public Task NumberCompareConverter_Behavior() =>
        ConverterAssertions.Behavior("NumberCompareConverter");

    [TestMethod(DisplayName = "SolidColorBrushToMediaColorConverter / Behavior")]
    [ConverterCoverage(
        typeof(SolidColorBrushToMediaColorConverter),
        "Behavior",
        "SolidColorBrushToMediaColorConverter"
    )]
    public Task SolidColorBrushToMediaColorConverter_Behavior() =>
        ConverterAssertions.Behavior("SolidColorBrushToMediaColorConverter");

    [TestMethod(DisplayName = "StringCaseConverter / Behavior")]
    [ConverterCoverage(typeof(StringCaseConverter), "Behavior", "StringCaseConverter")]
    public Task StringCaseConverter_Behavior() =>
        ConverterAssertions.Behavior("StringCaseConverter");

    [TestMethod(DisplayName = "StringCheckConverter / Behavior")]
    [ConverterCoverage(typeof(StringCheckConverter), "Behavior", "StringCheckConverter")]
    public Task StringCheckConverter_Behavior() =>
        ConverterAssertions.Behavior("StringCheckConverter");

    [TestMethod(DisplayName = "StringToNumberConverter / Behavior")]
    [ConverterCoverage(typeof(StringToNumberConverter), "Behavior", "StringToNumberConverter")]
    public Task StringToNumberConverter_Behavior() =>
        ConverterAssertions.Behavior("StringToNumberConverter");

    [TestMethod(DisplayName = "TimeSpanToStringConverter / Behavior")]
    [ConverterCoverage(typeof(TimeSpanToStringConverter), "Behavior", "TimeSpanToStringConverter")]
    public Task TimeSpanToStringConverter_Behavior() =>
        ConverterAssertions.Behavior("TimeSpanToStringConverter");

    [TestMethod(DisplayName = "ToBoolConverter / Behavior")]
    [ConverterCoverage(typeof(ToBoolConverter), "Behavior", "ToBoolConverter")]
    public Task ToBoolConverter_Behavior() => ConverterAssertions.Behavior("ToBoolConverter");

    [TestMethod(DisplayName = "ValueToEnumerableConverter / Behavior")]
    [ConverterCoverage(
        typeof(ValueToEnumerableConverter),
        "Behavior",
        "ValueToEnumerableConverter"
    )]
    public Task ValueToEnumerableConverter_Behavior() =>
        ConverterAssertions.Behavior("ValueToEnumerableConverter");

    [TestMethod(DisplayName = "VersionToStringConverter / Behavior")]
    [ConverterCoverage(typeof(VersionToStringConverter), "Behavior", "VersionToStringConverter")]
    public Task VersionToStringConverter_Behavior() =>
        ConverterAssertions.Behavior("VersionToStringConverter");

    [TestMethod(DisplayName = "AllBoolToValueMultiConverter / Behavior")]
    [ConverterCoverage(
        typeof(AllBoolToValueMultiConverter),
        "Behavior",
        "AllBoolToValueMultiConverter"
    )]
    public Task AllBoolToValueMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("AllBoolToValueMultiConverter");

    [TestMethod(DisplayName = "AllEqualsMultiConverter / Behavior")]
    [ConverterCoverage(typeof(AllEqualsMultiConverter), "Behavior", "AllEqualsMultiConverter")]
    public Task AllEqualsMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("AllEqualsMultiConverter");

    [TestMethod(DisplayName = "AnyEqualsMultiConverter / Behavior")]
    [ConverterCoverage(typeof(AnyEqualsMultiConverter), "Behavior", "AnyEqualsMultiConverter")]
    public Task AnyEqualsMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("AnyEqualsMultiConverter");

    [TestMethod(DisplayName = "EqualsCountMultiConverter / Behavior")]
    [ConverterCoverage(typeof(EqualsCountMultiConverter), "Behavior", "EqualsCountMultiConverter")]
    public Task EqualsCountMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("EqualsCountMultiConverter");

    [TestMethod(DisplayName = "AllBoolToValueMultiConverter<string> / Behavior")]
    [ConverterCoverage(
        typeof(AllBoolToValueMultiConverter<string>),
        "Behavior",
        "AllBoolToValueMultiConverter_String"
    )]
    public Task AllBoolToValueMultiConverter_String_Behavior() =>
        ConverterAssertions.Behavior("AllBoolToValueMultiConverter_String");

    [TestMethod(DisplayName = "AllEqualsMultiConverter<string> / Behavior")]
    [ConverterCoverage(
        typeof(AllEqualsMultiConverter<string>),
        "Behavior",
        "AllEqualsMultiConverter_String"
    )]
    public Task AllEqualsMultiConverter_String_Behavior() =>
        ConverterAssertions.Behavior("AllEqualsMultiConverter_String");

    [TestMethod(DisplayName = "AnyEqualsMultiConverter<string> / Behavior")]
    [ConverterCoverage(
        typeof(AnyEqualsMultiConverter<string>),
        "Behavior",
        "AnyEqualsMultiConverter_String"
    )]
    public Task AnyEqualsMultiConverter_String_Behavior() =>
        ConverterAssertions.Behavior("AnyEqualsMultiConverter_String");

    [TestMethod(DisplayName = "EqualsCountMultiConverter<string> / Behavior")]
    [ConverterCoverage(
        typeof(EqualsCountMultiConverter<string>),
        "Behavior",
        "EqualsCountMultiConverter_String"
    )]
    public Task EqualsCountMultiConverter_String_Behavior() =>
        ConverterAssertions.Behavior("EqualsCountMultiConverter_String");

    [TestMethod(DisplayName = "AllIsTrueMultiConverter / Behavior")]
    [ConverterCoverage(typeof(AllIsTrueMultiConverter), "Behavior", "AllIsTrueMultiConverter")]
    public Task AllIsTrueMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("AllIsTrueMultiConverter");

    [TestMethod(DisplayName = "CalculatorMultiConverter / Behavior")]
    [ConverterCoverage(typeof(CalculatorMultiConverter), "Behavior", "CalculatorMultiConverter")]
    public Task CalculatorMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("CalculatorMultiConverter");

    [TestMethod(DisplayName = "FirstBoolToValueMultiConverter / Behavior")]
    [ConverterCoverage(
        typeof(FirstBoolToValueMultiConverter),
        "Behavior",
        "FirstBoolToValueMultiConverter"
    )]
    public Task FirstBoolToValueMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("FirstBoolToValueMultiConverter");

    [TestMethod(DisplayName = "FirstEqualsSecondMultiConverter / Behavior")]
    [ConverterCoverage(
        typeof(FirstEqualsSecondMultiConverter),
        "Behavior",
        "FirstEqualsSecondMultiConverter"
    )]
    public Task FirstEqualsSecondMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("FirstEqualsSecondMultiConverter");

    [TestMethod(DisplayName = "FirstStringStateToOtherStringMultiConverter / Behavior")]
    [ConverterCoverage(
        typeof(FirstStringStateToOtherStringMultiConverter),
        "Behavior",
        "FirstStringStateToOtherStringMultiConverter"
    )]
    public Task FirstStringStateToOtherStringMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("FirstStringStateToOtherStringMultiConverter");

    [TestMethod(DisplayName = "GetDictionaryValueMultiConverter / Behavior")]
    [ConverterCoverage(
        typeof(GetDictionaryValueMultiConverter),
        "Behavior",
        "GetDictionaryValueMultiConverter"
    )]
    public Task GetDictionaryValueMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("GetDictionaryValueMultiConverter");

    [TestMethod(DisplayName = "MarginMultiConverter / Behavior")]
    [ConverterCoverage(typeof(MarginMultiConverter), "Behavior", "MarginMultiConverter")]
    public Task MarginMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("MarginMultiConverter");

    [TestMethod(DisplayName = "StringFormatMultiConverter / Behavior")]
    [ConverterCoverage(
        typeof(StringFormatMultiConverter),
        "Behavior",
        "StringFormatMultiConverter"
    )]
    public Task StringFormatMultiConverter_Behavior() =>
        ConverterAssertions.Behavior("StringFormatMultiConverter");
}
