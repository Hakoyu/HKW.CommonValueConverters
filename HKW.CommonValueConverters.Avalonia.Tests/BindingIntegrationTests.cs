namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class BindingIntegrationTests
{
    [TestMethod(DisplayName = "BoolInverterConverter / Binding")]
    [ConverterCoverage(typeof(BoolInverterConverter), "Binding", "BoolInverterConverter")]
    public Task BoolInverterConverter_Binding() =>
        ConverterAssertions.Binding("BoolInverterConverter");

    [TestMethod(DisplayName = "BoolToFontWeightConverter / Binding")]
    [ConverterCoverage(typeof(BoolToFontWeightConverter), "Binding", "BoolToFontWeightConverter")]
    public Task BoolToFontWeightConverter_Binding() =>
        ConverterAssertions.Binding("BoolToFontWeightConverter");

    [TestMethod(DisplayName = "BoolToNumberConverter / Binding")]
    [ConverterCoverage(typeof(BoolToNumberConverter), "Binding", "BoolToNumberConverter")]
    public Task BoolToNumberConverter_Binding() =>
        ConverterAssertions.Binding("BoolToNumberConverter");

    [TestMethod(DisplayName = "BoolToSolidColorBrushConverter / Binding")]
    [ConverterCoverage(
        typeof(BoolToSolidColorBrushConverter),
        "Binding",
        "BoolToSolidColorBrushConverter"
    )]
    public Task BoolToSolidColorBrushConverter_Binding() =>
        ConverterAssertions.Binding("BoolToSolidColorBrushConverter");

    [TestMethod(DisplayName = "BoolToSplitParameterConverter / Binding")]
    [ConverterCoverage(
        typeof(BoolToSplitParameterConverter),
        "Binding",
        "BoolToSplitParameterConverter"
    )]
    public Task BoolToSplitParameterConverter_Binding() =>
        ConverterAssertions.Binding("BoolToSplitParameterConverter");

    [TestMethod(DisplayName = "BoolToSplitThicknessConverter / Binding")]
    [ConverterCoverage(
        typeof(BoolToSplitThicknessConverter),
        "Binding",
        "BoolToSplitThicknessConverter"
    )]
    public Task BoolToSplitThicknessConverter_Binding() =>
        ConverterAssertions.Binding("BoolToSplitThicknessConverter");

    [TestMethod(DisplayName = "BoolToStyleConverter / Binding")]
    [ConverterCoverage(typeof(BoolToStyleConverter), "Binding", "BoolToStyleConverter")]
    public Task BoolToStyleConverter_Binding() =>
        ConverterAssertions.Binding("BoolToStyleConverter");

    [TestMethod(DisplayName = "BoolToThicknessConverter / Binding")]
    [ConverterCoverage(typeof(BoolToThicknessConverter), "Binding", "BoolToThicknessConverter")]
    public Task BoolToThicknessConverter_Binding() =>
        ConverterAssertions.Binding("BoolToThicknessConverter");

    [TestMethod(DisplayName = "BoolToValueConverter / Binding")]
    [ConverterCoverage(typeof(BoolToValueConverter), "Binding", "BoolToValueConverter")]
    public Task BoolToValueConverter_Binding() =>
        ConverterAssertions.Binding("BoolToValueConverter");

    [TestMethod(DisplayName = "BoolToValueConverter<string> / Binding")]
    [ConverterCoverage(
        typeof(BoolToValueConverter<string>),
        "Binding",
        "BoolToValueConverter_String"
    )]
    public Task BoolToValueConverter_String_Binding() =>
        ConverterAssertions.Binding("BoolToValueConverter_String");

    [TestMethod(DisplayName = "CalculatorConverter / Binding")]
    [ConverterCoverage(typeof(CalculatorConverter), "Binding", "CalculatorConverter")]
    public Task CalculatorConverter_Binding() => ConverterAssertions.Binding("CalculatorConverter");

    [TestMethod(DisplayName = "CollectionCountCompareByConverter / Binding")]
    [ConverterCoverage(
        typeof(CollectionCountCompareByConverter),
        "Binding",
        "CollectionCountCompareByConverter"
    )]
    public Task CollectionCountCompareByConverter_Binding() =>
        ConverterAssertions.Binding("CollectionCountCompareByConverter");

    [TestMethod(DisplayName = "CollectionCountCompareConverter / Binding")]
    [ConverterCoverage(
        typeof(CollectionCountCompareConverter),
        "Binding",
        "CollectionCountCompareConverter"
    )]
    public Task CollectionCountCompareConverter_Binding() =>
        ConverterAssertions.Binding("CollectionCountCompareConverter");

    [TestMethod(DisplayName = "DateTimeOffsetToStringConverter / Binding")]
    [ConverterCoverage(
        typeof(DateTimeOffsetToStringConverter),
        "Binding",
        "DateTimeOffsetToStringConverter"
    )]
    public Task DateTimeOffsetToStringConverter_Binding() =>
        ConverterAssertions.Binding("DateTimeOffsetToStringConverter");

    [TestMethod(DisplayName = "DateTimeToStringConverter / Binding")]
    [ConverterCoverage(typeof(DateTimeToStringConverter), "Binding", "DateTimeToStringConverter")]
    public Task DateTimeToStringConverter_Binding() =>
        ConverterAssertions.Binding("DateTimeToStringConverter");

    [TestMethod(DisplayName = "DebugConverter / Binding")]
    [ConverterCoverage(typeof(DebugConverter), "Binding", "DebugConverter")]
    public Task DebugConverter_Binding() => ConverterAssertions.Binding("DebugConverter");

    [TestMethod(DisplayName = "EnumEqualsConverter / Binding")]
    [ConverterCoverage(typeof(EnumEqualsConverter), "Binding", "EnumEqualsConverter")]
    public Task EnumEqualsConverter_Binding() => ConverterAssertions.Binding("EnumEqualsConverter");

    [TestMethod(DisplayName = "EnumToEnumInfoConverter / Binding")]
    [ConverterCoverage(typeof(EnumToEnumInfoConverter), "Binding", "EnumToEnumInfoConverter")]
    public Task EnumToEnumInfoConverter_Binding() =>
        ConverterAssertions.Binding("EnumToEnumInfoConverter");

    [TestMethod(DisplayName = "EnumToEnumInfoDisplayConverter / Binding")]
    [ConverterCoverage(
        typeof(EnumToEnumInfoDisplayConverter),
        "Binding",
        "EnumToEnumInfoDisplayConverter"
    )]
    public Task EnumToEnumInfoDisplayConverter_Binding() =>
        ConverterAssertions.Binding("EnumToEnumInfoDisplayConverter");

    [TestMethod(DisplayName = "EnumsToEnumInfosConverter / Binding")]
    [ConverterCoverage(typeof(EnumsToEnumInfosConverter), "Binding", "EnumsToEnumInfosConverter")]
    public Task EnumsToEnumInfosConverter_Binding() =>
        ConverterAssertions.Binding("EnumsToEnumInfosConverter");

    [TestMethod(DisplayName = "EqualsConverter / Binding")]
    [ConverterCoverage(typeof(EqualsConverter), "Binding", "EqualsConverter")]
    public Task EqualsConverter_Binding() => ConverterAssertions.Binding("EqualsConverter");

    [TestMethod(DisplayName = "EqualsToValueConverter / Binding")]
    [ConverterCoverage(typeof(EqualsToValueConverter), "Binding", "EqualsToValueConverter")]
    public Task EqualsToValueConverter_Binding() =>
        ConverterAssertions.Binding("EqualsToValueConverter");

    [TestMethod(DisplayName = "EqualsToValueConverter<string> / Binding")]
    [ConverterCoverage(
        typeof(EqualsToValueConverter<string>),
        "Binding",
        "EqualsToValueConverter_String"
    )]
    public Task EqualsToValueConverter_String_Binding() =>
        ConverterAssertions.Binding("EqualsToValueConverter_String");

    [TestMethod(DisplayName = "FirstOrDefaultConverter / Binding")]
    [ConverterCoverage(typeof(FirstOrDefaultConverter), "Binding", "FirstOrDefaultConverter")]
    public Task FirstOrDefaultConverter_Binding() =>
        ConverterAssertions.Binding("FirstOrDefaultConverter");

    [TestMethod(DisplayName = "GetDictionaryValueConverter / Binding")]
    [ConverterCoverage(
        typeof(GetDictionaryValueConverter),
        "Binding",
        "GetDictionaryValueConverter"
    )]
    public Task GetDictionaryValueConverter_Binding() =>
        ConverterAssertions.Binding("GetDictionaryValueConverter");

    [TestMethod(DisplayName = "GuidToStringConverter / Binding")]
    [ConverterCoverage(typeof(GuidToStringConverter), "Binding", "GuidToStringConverter")]
    public Task GuidToStringConverter_Binding() =>
        ConverterAssertions.Binding("GuidToStringConverter");

    [TestMethod(DisplayName = "MediaColorToSolidColorBrushConverter / Binding")]
    [ConverterCoverage(
        typeof(MediaColorToSolidColorBrushConverter),
        "Binding",
        "MediaColorToSolidColorBrushConverter"
    )]
    public Task MediaColorToSolidColorBrushConverter_Binding() =>
        ConverterAssertions.Binding("MediaColorToSolidColorBrushConverter");

    [TestMethod(DisplayName = "NullToTrueConverter / Binding")]
    [ConverterCoverage(typeof(NullToTrueConverter), "Binding", "NullToTrueConverter")]
    public Task NullToTrueConverter_Binding() => ConverterAssertions.Binding("NullToTrueConverter");

    [TestMethod(DisplayName = "NumberClampConverter / Binding")]
    [ConverterCoverage(typeof(NumberClampConverter), "Binding", "NumberClampConverter")]
    public Task NumberClampConverter_Binding() =>
        ConverterAssertions.Binding("NumberClampConverter");

    [TestMethod(DisplayName = "NumberCompareByConverter / Binding")]
    [ConverterCoverage(typeof(NumberCompareByConverter), "Binding", "NumberCompareByConverter")]
    public Task NumberCompareByConverter_Binding() =>
        ConverterAssertions.Binding("NumberCompareByConverter");

    [TestMethod(DisplayName = "NumberCompareConverter / Binding")]
    [ConverterCoverage(typeof(NumberCompareConverter), "Binding", "NumberCompareConverter")]
    public Task NumberCompareConverter_Binding() =>
        ConverterAssertions.Binding("NumberCompareConverter");

    [TestMethod(DisplayName = "SolidColorBrushToMediaColorConverter / Binding")]
    [ConverterCoverage(
        typeof(SolidColorBrushToMediaColorConverter),
        "Binding",
        "SolidColorBrushToMediaColorConverter"
    )]
    public Task SolidColorBrushToMediaColorConverter_Binding() =>
        ConverterAssertions.Binding("SolidColorBrushToMediaColorConverter");

    [TestMethod(DisplayName = "StringCaseConverter / Binding")]
    [ConverterCoverage(typeof(StringCaseConverter), "Binding", "StringCaseConverter")]
    public Task StringCaseConverter_Binding() => ConverterAssertions.Binding("StringCaseConverter");

    [TestMethod(DisplayName = "StringCheckConverter / Binding")]
    [ConverterCoverage(typeof(StringCheckConverter), "Binding", "StringCheckConverter")]
    public Task StringCheckConverter_Binding() =>
        ConverterAssertions.Binding("StringCheckConverter");

    [TestMethod(DisplayName = "StringToNumberConverter / Binding")]
    [ConverterCoverage(typeof(StringToNumberConverter), "Binding", "StringToNumberConverter")]
    public Task StringToNumberConverter_Binding() =>
        ConverterAssertions.Binding("StringToNumberConverter");

    [TestMethod(DisplayName = "TimeSpanToStringConverter / Binding")]
    [ConverterCoverage(typeof(TimeSpanToStringConverter), "Binding", "TimeSpanToStringConverter")]
    public Task TimeSpanToStringConverter_Binding() =>
        ConverterAssertions.Binding("TimeSpanToStringConverter");

    [TestMethod(DisplayName = "ToBoolConverter / Binding")]
    [ConverterCoverage(typeof(ToBoolConverter), "Binding", "ToBoolConverter")]
    public Task ToBoolConverter_Binding() => ConverterAssertions.Binding("ToBoolConverter");

    [TestMethod(DisplayName = "ValueToEnumerableConverter / Binding")]
    [ConverterCoverage(typeof(ValueToEnumerableConverter), "Binding", "ValueToEnumerableConverter")]
    public Task ValueToEnumerableConverter_Binding() =>
        ConverterAssertions.Binding("ValueToEnumerableConverter");

    [TestMethod(DisplayName = "VersionToStringConverter / Binding")]
    [ConverterCoverage(typeof(VersionToStringConverter), "Binding", "VersionToStringConverter")]
    public Task VersionToStringConverter_Binding() =>
        ConverterAssertions.Binding("VersionToStringConverter");

    [TestMethod(DisplayName = "AllBoolToValueMultiConverter / Binding")]
    [ConverterCoverage(
        typeof(AllBoolToValueMultiConverter),
        "Binding",
        "AllBoolToValueMultiConverter"
    )]
    public Task AllBoolToValueMultiConverter_Binding() =>
        ConverterAssertions.Binding("AllBoolToValueMultiConverter");

    [TestMethod(DisplayName = "AllEqualsMultiConverter / Binding")]
    [ConverterCoverage(typeof(AllEqualsMultiConverter), "Binding", "AllEqualsMultiConverter")]
    public Task AllEqualsMultiConverter_Binding() =>
        ConverterAssertions.Binding("AllEqualsMultiConverter");

    [TestMethod(DisplayName = "AnyEqualsMultiConverter / Binding")]
    [ConverterCoverage(typeof(AnyEqualsMultiConverter), "Binding", "AnyEqualsMultiConverter")]
    public Task AnyEqualsMultiConverter_Binding() =>
        ConverterAssertions.Binding("AnyEqualsMultiConverter");

    [TestMethod(DisplayName = "EqualsCountMultiConverter / Binding")]
    [ConverterCoverage(typeof(EqualsCountMultiConverter), "Binding", "EqualsCountMultiConverter")]
    public Task EqualsCountMultiConverter_Binding() =>
        ConverterAssertions.Binding("EqualsCountMultiConverter");

    [TestMethod(DisplayName = "AllBoolToValueMultiConverter<string> / Binding")]
    [ConverterCoverage(
        typeof(AllBoolToValueMultiConverter<string>),
        "Binding",
        "AllBoolToValueMultiConverter_String"
    )]
    public Task AllBoolToValueMultiConverter_String_Binding() =>
        ConverterAssertions.Binding("AllBoolToValueMultiConverter_String");

    [TestMethod(DisplayName = "AllEqualsMultiConverter<string> / Binding")]
    [ConverterCoverage(
        typeof(AllEqualsMultiConverter<string>),
        "Binding",
        "AllEqualsMultiConverter_String"
    )]
    public Task AllEqualsMultiConverter_String_Binding() =>
        ConverterAssertions.Binding("AllEqualsMultiConverter_String");

    [TestMethod(DisplayName = "AnyEqualsMultiConverter<string> / Binding")]
    [ConverterCoverage(
        typeof(AnyEqualsMultiConverter<string>),
        "Binding",
        "AnyEqualsMultiConverter_String"
    )]
    public Task AnyEqualsMultiConverter_String_Binding() =>
        ConverterAssertions.Binding("AnyEqualsMultiConverter_String");

    [TestMethod(DisplayName = "EqualsCountMultiConverter<string> / Binding")]
    [ConverterCoverage(
        typeof(EqualsCountMultiConverter<string>),
        "Binding",
        "EqualsCountMultiConverter_String"
    )]
    public Task EqualsCountMultiConverter_String_Binding() =>
        ConverterAssertions.Binding("EqualsCountMultiConverter_String");

    [TestMethod(DisplayName = "AllIsTrueMultiConverter / Binding")]
    [ConverterCoverage(typeof(AllIsTrueMultiConverter), "Binding", "AllIsTrueMultiConverter")]
    public Task AllIsTrueMultiConverter_Binding() =>
        ConverterAssertions.Binding("AllIsTrueMultiConverter");

    [TestMethod(DisplayName = "CalculatorMultiConverter / Binding")]
    [ConverterCoverage(typeof(CalculatorMultiConverter), "Binding", "CalculatorMultiConverter")]
    public Task CalculatorMultiConverter_Binding() =>
        ConverterAssertions.Binding("CalculatorMultiConverter");

    [TestMethod(DisplayName = "FirstBoolToValueMultiConverter / Binding")]
    [ConverterCoverage(
        typeof(FirstBoolToValueMultiConverter),
        "Binding",
        "FirstBoolToValueMultiConverter"
    )]
    public Task FirstBoolToValueMultiConverter_Binding() =>
        ConverterAssertions.Binding("FirstBoolToValueMultiConverter");

    [TestMethod(DisplayName = "FirstEqualsSecondMultiConverter / Binding")]
    [ConverterCoverage(
        typeof(FirstEqualsSecondMultiConverter),
        "Binding",
        "FirstEqualsSecondMultiConverter"
    )]
    public Task FirstEqualsSecondMultiConverter_Binding() =>
        ConverterAssertions.Binding("FirstEqualsSecondMultiConverter");

    [TestMethod(DisplayName = "FirstStringStateToOtherStringMultiConverter / Binding")]
    [ConverterCoverage(
        typeof(FirstStringStateToOtherStringMultiConverter),
        "Binding",
        "FirstStringStateToOtherStringMultiConverter"
    )]
    public Task FirstStringStateToOtherStringMultiConverter_Binding() =>
        ConverterAssertions.Binding("FirstStringStateToOtherStringMultiConverter");

    [TestMethod(DisplayName = "GetDictionaryValueMultiConverter / Binding")]
    [ConverterCoverage(
        typeof(GetDictionaryValueMultiConverter),
        "Binding",
        "GetDictionaryValueMultiConverter"
    )]
    public Task GetDictionaryValueMultiConverter_Binding() =>
        ConverterAssertions.Binding("GetDictionaryValueMultiConverter");

    [TestMethod(DisplayName = "MarginMultiConverter / Binding")]
    [ConverterCoverage(typeof(MarginMultiConverter), "Binding", "MarginMultiConverter")]
    public Task MarginMultiConverter_Binding() =>
        ConverterAssertions.Binding("MarginMultiConverter");

    [TestMethod(DisplayName = "StringFormatMultiConverter / Binding")]
    [ConverterCoverage(typeof(StringFormatMultiConverter), "Binding", "StringFormatMultiConverter")]
    public Task StringFormatMultiConverter_Binding() =>
        ConverterAssertions.Binding("StringFormatMultiConverter");
}
