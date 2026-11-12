namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class XamlIntegrationTests
{
    [TestMethod(DisplayName = "BoolInverterConverter / Xaml")]
    [ConverterCoverage(typeof(BoolInverterConverter), "Xaml", "BoolInverterConverter")]
    public Task BoolInverterConverter_Xaml() => ConverterAssertions.Xaml("BoolInverterConverter");

    [TestMethod(DisplayName = "BoolToFontWeightConverter / Xaml")]
    [ConverterCoverage(typeof(BoolToFontWeightConverter), "Xaml", "BoolToFontWeightConverter")]
    public Task BoolToFontWeightConverter_Xaml() =>
        ConverterAssertions.Xaml("BoolToFontWeightConverter");

    [TestMethod(DisplayName = "BoolToNumberConverter / Xaml")]
    [ConverterCoverage(typeof(BoolToNumberConverter), "Xaml", "BoolToNumberConverter")]
    public Task BoolToNumberConverter_Xaml() => ConverterAssertions.Xaml("BoolToNumberConverter");

    [TestMethod(DisplayName = "BoolToSolidColorBrushConverter / Xaml")]
    [ConverterCoverage(
        typeof(BoolToSolidColorBrushConverter),
        "Xaml",
        "BoolToSolidColorBrushConverter"
    )]
    public Task BoolToSolidColorBrushConverter_Xaml() =>
        ConverterAssertions.Xaml("BoolToSolidColorBrushConverter");

    [TestMethod(DisplayName = "BoolToSplitParameterConverter / Xaml")]
    [ConverterCoverage(
        typeof(BoolToSplitParameterConverter),
        "Xaml",
        "BoolToSplitParameterConverter"
    )]
    public Task BoolToSplitParameterConverter_Xaml() =>
        ConverterAssertions.Xaml("BoolToSplitParameterConverter");

    [TestMethod(DisplayName = "BoolToSplitThicknessConverter / Xaml")]
    [ConverterCoverage(
        typeof(BoolToSplitThicknessConverter),
        "Xaml",
        "BoolToSplitThicknessConverter"
    )]
    public Task BoolToSplitThicknessConverter_Xaml() =>
        ConverterAssertions.Xaml("BoolToSplitThicknessConverter");

    [TestMethod(DisplayName = "BoolToStyleConverter / Xaml")]
    [ConverterCoverage(typeof(BoolToStyleConverter), "Xaml", "BoolToStyleConverter")]
    public Task BoolToStyleConverter_Xaml() => ConverterAssertions.Xaml("BoolToStyleConverter");

    [TestMethod(DisplayName = "BoolToThicknessConverter / Xaml")]
    [ConverterCoverage(typeof(BoolToThicknessConverter), "Xaml", "BoolToThicknessConverter")]
    public Task BoolToThicknessConverter_Xaml() =>
        ConverterAssertions.Xaml("BoolToThicknessConverter");

    [TestMethod(DisplayName = "BoolToValueConverter / Xaml")]
    [ConverterCoverage(typeof(BoolToValueConverter), "Xaml", "BoolToValueConverter")]
    public Task BoolToValueConverter_Xaml() => ConverterAssertions.Xaml("BoolToValueConverter");

    [TestMethod(DisplayName = "BoolToValueConverter<string> / Xaml")]
    [ConverterCoverage(typeof(BoolToValueConverter<string>), "Xaml", "BoolToValueConverter_String")]
    public Task BoolToValueConverter_String_Xaml() =>
        ConverterAssertions.Xaml("BoolToValueConverter_String");

    [TestMethod(DisplayName = "CalculatorConverter / Xaml")]
    [ConverterCoverage(typeof(CalculatorConverter), "Xaml", "CalculatorConverter")]
    public Task CalculatorConverter_Xaml() => ConverterAssertions.Xaml("CalculatorConverter");

    [TestMethod(DisplayName = "CollectionCountCompareByConverter / Xaml")]
    [ConverterCoverage(
        typeof(CollectionCountCompareByConverter),
        "Xaml",
        "CollectionCountCompareByConverter"
    )]
    public Task CollectionCountCompareByConverter_Xaml() =>
        ConverterAssertions.Xaml("CollectionCountCompareByConverter");

    [TestMethod(DisplayName = "CollectionCountCompareConverter / Xaml")]
    [ConverterCoverage(
        typeof(CollectionCountCompareConverter),
        "Xaml",
        "CollectionCountCompareConverter"
    )]
    public Task CollectionCountCompareConverter_Xaml() =>
        ConverterAssertions.Xaml("CollectionCountCompareConverter");

    [TestMethod(DisplayName = "DateTimeOffsetToStringConverter / Xaml")]
    [ConverterCoverage(
        typeof(DateTimeOffsetToStringConverter),
        "Xaml",
        "DateTimeOffsetToStringConverter"
    )]
    public Task DateTimeOffsetToStringConverter_Xaml() =>
        ConverterAssertions.Xaml("DateTimeOffsetToStringConverter");

    [TestMethod(DisplayName = "DateTimeToStringConverter / Xaml")]
    [ConverterCoverage(typeof(DateTimeToStringConverter), "Xaml", "DateTimeToStringConverter")]
    public Task DateTimeToStringConverter_Xaml() =>
        ConverterAssertions.Xaml("DateTimeToStringConverter");

    [TestMethod(DisplayName = "DebugConverter / Xaml")]
    [ConverterCoverage(typeof(DebugConverter), "Xaml", "DebugConverter")]
    public Task DebugConverter_Xaml() => ConverterAssertions.Xaml("DebugConverter");

    [TestMethod(DisplayName = "EnumEqualsConverter / Xaml")]
    [ConverterCoverage(typeof(EnumEqualsConverter), "Xaml", "EnumEqualsConverter")]
    public Task EnumEqualsConverter_Xaml() => ConverterAssertions.Xaml("EnumEqualsConverter");

    [TestMethod(DisplayName = "EnumToEnumInfoConverter / Xaml")]
    [ConverterCoverage(typeof(EnumToEnumInfoConverter), "Xaml", "EnumToEnumInfoConverter")]
    public Task EnumToEnumInfoConverter_Xaml() =>
        ConverterAssertions.Xaml("EnumToEnumInfoConverter");

    [TestMethod(DisplayName = "EnumToEnumInfoDisplayConverter / Xaml")]
    [ConverterCoverage(
        typeof(EnumToEnumInfoDisplayConverter),
        "Xaml",
        "EnumToEnumInfoDisplayConverter"
    )]
    public Task EnumToEnumInfoDisplayConverter_Xaml() =>
        ConverterAssertions.Xaml("EnumToEnumInfoDisplayConverter");

    [TestMethod(DisplayName = "EnumsToEnumInfosConverter / Xaml")]
    [ConverterCoverage(typeof(EnumsToEnumInfosConverter), "Xaml", "EnumsToEnumInfosConverter")]
    public Task EnumsToEnumInfosConverter_Xaml() =>
        ConverterAssertions.Xaml("EnumsToEnumInfosConverter");

    [TestMethod(DisplayName = "EqualsConverter / Xaml")]
    [ConverterCoverage(typeof(EqualsConverter), "Xaml", "EqualsConverter")]
    public Task EqualsConverter_Xaml() => ConverterAssertions.Xaml("EqualsConverter");

    [TestMethod(DisplayName = "EqualsToValueConverter / Xaml")]
    [ConverterCoverage(typeof(EqualsToValueConverter), "Xaml", "EqualsToValueConverter")]
    public Task EqualsToValueConverter_Xaml() => ConverterAssertions.Xaml("EqualsToValueConverter");

    [TestMethod(DisplayName = "EqualsToValueConverter<string> / Xaml")]
    [ConverterCoverage(
        typeof(EqualsToValueConverter<string>),
        "Xaml",
        "EqualsToValueConverter_String"
    )]
    public Task EqualsToValueConverter_String_Xaml() =>
        ConverterAssertions.Xaml("EqualsToValueConverter_String");

    [TestMethod(DisplayName = "FirstOrDefaultConverter / Xaml")]
    [ConverterCoverage(typeof(FirstOrDefaultConverter), "Xaml", "FirstOrDefaultConverter")]
    public Task FirstOrDefaultConverter_Xaml() =>
        ConverterAssertions.Xaml("FirstOrDefaultConverter");

    [TestMethod(DisplayName = "GetDictionaryValueConverter / Xaml")]
    [ConverterCoverage(typeof(GetDictionaryValueConverter), "Xaml", "GetDictionaryValueConverter")]
    public Task GetDictionaryValueConverter_Xaml() =>
        ConverterAssertions.Xaml("GetDictionaryValueConverter");

    [TestMethod(DisplayName = "GuidToStringConverter / Xaml")]
    [ConverterCoverage(typeof(GuidToStringConverter), "Xaml", "GuidToStringConverter")]
    public Task GuidToStringConverter_Xaml() => ConverterAssertions.Xaml("GuidToStringConverter");

    [TestMethod(DisplayName = "MediaColorToSolidColorBrushConverter / Xaml")]
    [ConverterCoverage(
        typeof(MediaColorToSolidColorBrushConverter),
        "Xaml",
        "MediaColorToSolidColorBrushConverter"
    )]
    public Task MediaColorToSolidColorBrushConverter_Xaml() =>
        ConverterAssertions.Xaml("MediaColorToSolidColorBrushConverter");

    [TestMethod(DisplayName = "NullToTrueConverter / Xaml")]
    [ConverterCoverage(typeof(NullToTrueConverter), "Xaml", "NullToTrueConverter")]
    public Task NullToTrueConverter_Xaml() => ConverterAssertions.Xaml("NullToTrueConverter");

    [TestMethod(DisplayName = "NumberClampConverter / Xaml")]
    [ConverterCoverage(typeof(NumberClampConverter), "Xaml", "NumberClampConverter")]
    public Task NumberClampConverter_Xaml() => ConverterAssertions.Xaml("NumberClampConverter");

    [TestMethod(DisplayName = "NumberCompareByConverter / Xaml")]
    [ConverterCoverage(typeof(NumberCompareByConverter), "Xaml", "NumberCompareByConverter")]
    public Task NumberCompareByConverter_Xaml() =>
        ConverterAssertions.Xaml("NumberCompareByConverter");

    [TestMethod(DisplayName = "NumberCompareConverter / Xaml")]
    [ConverterCoverage(typeof(NumberCompareConverter), "Xaml", "NumberCompareConverter")]
    public Task NumberCompareConverter_Xaml() => ConverterAssertions.Xaml("NumberCompareConverter");

    [TestMethod(DisplayName = "SolidColorBrushToMediaColorConverter / Xaml")]
    [ConverterCoverage(
        typeof(SolidColorBrushToMediaColorConverter),
        "Xaml",
        "SolidColorBrushToMediaColorConverter"
    )]
    public Task SolidColorBrushToMediaColorConverter_Xaml() =>
        ConverterAssertions.Xaml("SolidColorBrushToMediaColorConverter");

    [TestMethod(DisplayName = "StringCaseConverter / Xaml")]
    [ConverterCoverage(typeof(StringCaseConverter), "Xaml", "StringCaseConverter")]
    public Task StringCaseConverter_Xaml() => ConverterAssertions.Xaml("StringCaseConverter");

    [TestMethod(DisplayName = "StringCheckConverter / Xaml")]
    [ConverterCoverage(typeof(StringCheckConverter), "Xaml", "StringCheckConverter")]
    public Task StringCheckConverter_Xaml() => ConverterAssertions.Xaml("StringCheckConverter");

    [TestMethod(DisplayName = "StringToNumberConverter / Xaml")]
    [ConverterCoverage(typeof(StringToNumberConverter), "Xaml", "StringToNumberConverter")]
    public Task StringToNumberConverter_Xaml() =>
        ConverterAssertions.Xaml("StringToNumberConverter");

    [TestMethod(DisplayName = "TimeSpanToStringConverter / Xaml")]
    [ConverterCoverage(typeof(TimeSpanToStringConverter), "Xaml", "TimeSpanToStringConverter")]
    public Task TimeSpanToStringConverter_Xaml() =>
        ConverterAssertions.Xaml("TimeSpanToStringConverter");

    [TestMethod(DisplayName = "ToBoolConverter / Xaml")]
    [ConverterCoverage(typeof(ToBoolConverter), "Xaml", "ToBoolConverter")]
    public Task ToBoolConverter_Xaml() => ConverterAssertions.Xaml("ToBoolConverter");

    [TestMethod(DisplayName = "ValueToEnumerableConverter / Xaml")]
    [ConverterCoverage(typeof(ValueToEnumerableConverter), "Xaml", "ValueToEnumerableConverter")]
    public Task ValueToEnumerableConverter_Xaml() =>
        ConverterAssertions.Xaml("ValueToEnumerableConverter");

    [TestMethod(DisplayName = "VersionToStringConverter / Xaml")]
    [ConverterCoverage(typeof(VersionToStringConverter), "Xaml", "VersionToStringConverter")]
    public Task VersionToStringConverter_Xaml() =>
        ConverterAssertions.Xaml("VersionToStringConverter");

    [TestMethod(DisplayName = "AllBoolToValueMultiConverter / Xaml")]
    [ConverterCoverage(
        typeof(AllBoolToValueMultiConverter),
        "Xaml",
        "AllBoolToValueMultiConverter"
    )]
    public Task AllBoolToValueMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("AllBoolToValueMultiConverter");

    [TestMethod(DisplayName = "AllEqualsMultiConverter / Xaml")]
    [ConverterCoverage(typeof(AllEqualsMultiConverter), "Xaml", "AllEqualsMultiConverter")]
    public Task AllEqualsMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("AllEqualsMultiConverter");

    [TestMethod(DisplayName = "AnyEqualsMultiConverter / Xaml")]
    [ConverterCoverage(typeof(AnyEqualsMultiConverter), "Xaml", "AnyEqualsMultiConverter")]
    public Task AnyEqualsMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("AnyEqualsMultiConverter");

    [TestMethod(DisplayName = "EqualsCountMultiConverter / Xaml")]
    [ConverterCoverage(typeof(EqualsCountMultiConverter), "Xaml", "EqualsCountMultiConverter")]
    public Task EqualsCountMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("EqualsCountMultiConverter");

    [TestMethod(DisplayName = "AllBoolToValueMultiConverter<string> / Xaml")]
    [ConverterCoverage(
        typeof(AllBoolToValueMultiConverter<string>),
        "Xaml",
        "AllBoolToValueMultiConverter_String"
    )]
    public Task AllBoolToValueMultiConverter_String_Xaml() =>
        ConverterAssertions.Xaml("AllBoolToValueMultiConverter_String");

    [TestMethod(DisplayName = "AllEqualsMultiConverter<string> / Xaml")]
    [ConverterCoverage(
        typeof(AllEqualsMultiConverter<string>),
        "Xaml",
        "AllEqualsMultiConverter_String"
    )]
    public Task AllEqualsMultiConverter_String_Xaml() =>
        ConverterAssertions.Xaml("AllEqualsMultiConverter_String");

    [TestMethod(DisplayName = "AnyEqualsMultiConverter<string> / Xaml")]
    [ConverterCoverage(
        typeof(AnyEqualsMultiConverter<string>),
        "Xaml",
        "AnyEqualsMultiConverter_String"
    )]
    public Task AnyEqualsMultiConverter_String_Xaml() =>
        ConverterAssertions.Xaml("AnyEqualsMultiConverter_String");

    [TestMethod(DisplayName = "EqualsCountMultiConverter<string> / Xaml")]
    [ConverterCoverage(
        typeof(EqualsCountMultiConverter<string>),
        "Xaml",
        "EqualsCountMultiConverter_String"
    )]
    public Task EqualsCountMultiConverter_String_Xaml() =>
        ConverterAssertions.Xaml("EqualsCountMultiConverter_String");

    [TestMethod(DisplayName = "AllIsTrueMultiConverter / Xaml")]
    [ConverterCoverage(typeof(AllIsTrueMultiConverter), "Xaml", "AllIsTrueMultiConverter")]
    public Task AllIsTrueMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("AllIsTrueMultiConverter");

    [TestMethod(DisplayName = "CalculatorMultiConverter / Xaml")]
    [ConverterCoverage(typeof(CalculatorMultiConverter), "Xaml", "CalculatorMultiConverter")]
    public Task CalculatorMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("CalculatorMultiConverter");

    [TestMethod(DisplayName = "FirstBoolToValueMultiConverter / Xaml")]
    [ConverterCoverage(
        typeof(FirstBoolToValueMultiConverter),
        "Xaml",
        "FirstBoolToValueMultiConverter"
    )]
    public Task FirstBoolToValueMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("FirstBoolToValueMultiConverter");

    [TestMethod(DisplayName = "FirstEqualsSecondMultiConverter / Xaml")]
    [ConverterCoverage(
        typeof(FirstEqualsSecondMultiConverter),
        "Xaml",
        "FirstEqualsSecondMultiConverter"
    )]
    public Task FirstEqualsSecondMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("FirstEqualsSecondMultiConverter");

    [TestMethod(DisplayName = "FirstStringStateToOtherStringMultiConverter / Xaml")]
    [ConverterCoverage(
        typeof(FirstStringStateToOtherStringMultiConverter),
        "Xaml",
        "FirstStringStateToOtherStringMultiConverter"
    )]
    public Task FirstStringStateToOtherStringMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("FirstStringStateToOtherStringMultiConverter");

    [TestMethod(DisplayName = "GetDictionaryValueMultiConverter / Xaml")]
    [ConverterCoverage(
        typeof(GetDictionaryValueMultiConverter),
        "Xaml",
        "GetDictionaryValueMultiConverter"
    )]
    public Task GetDictionaryValueMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("GetDictionaryValueMultiConverter");

    [TestMethod(DisplayName = "MarginMultiConverter / Xaml")]
    [ConverterCoverage(typeof(MarginMultiConverter), "Xaml", "MarginMultiConverter")]
    public Task MarginMultiConverter_Xaml() => ConverterAssertions.Xaml("MarginMultiConverter");

    [TestMethod(DisplayName = "StringFormatMultiConverter / Xaml")]
    [ConverterCoverage(typeof(StringFormatMultiConverter), "Xaml", "StringFormatMultiConverter")]
    public Task StringFormatMultiConverter_Xaml() =>
        ConverterAssertions.Xaml("StringFormatMultiConverter");
}
