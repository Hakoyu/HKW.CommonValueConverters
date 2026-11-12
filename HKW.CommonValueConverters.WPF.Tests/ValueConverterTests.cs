namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class ValueConverterTests
{
    [TestMethod]
    [ConverterIdentity(typeof(BoolInverterConverter))]
    public void BoolInverterConverter() => BehaviorRunner.Verify("BoolInverterConverter");

    [TestMethod]
    [ConverterIdentity(typeof(BoolToFontWeightConverter))]
    public void BoolToFontWeightConverter() => BehaviorRunner.Verify("BoolToFontWeightConverter");

    [TestMethod]
    [ConverterIdentity(typeof(BoolToNumberConverter))]
    public void BoolToNumberConverter() => BehaviorRunner.Verify("BoolToNumberConverter");

    [TestMethod]
    [ConverterIdentity(typeof(BoolToSolidColorBrushConverter))]
    public void BoolToSolidColorBrushConverter() =>
        BehaviorRunner.Verify("BoolToSolidColorBrushConverter");

    [TestMethod]
    [ConverterIdentity(typeof(BoolToSplitParameterConverter))]
    public void BoolToSplitParameterConverter() =>
        BehaviorRunner.Verify("BoolToSplitParameterConverter");

    [TestMethod]
    [ConverterIdentity(typeof(BoolToSplitThicknessConverter))]
    public void BoolToSplitThicknessConverter() =>
        BehaviorRunner.Verify("BoolToSplitThicknessConverter");

    [TestMethod]
    [ConverterIdentity(typeof(BoolToStyleConverter))]
    public void BoolToStyleConverter() => BehaviorRunner.Verify("BoolToStyleConverter");

    [TestMethod]
    [ConverterIdentity(typeof(BoolToThicknessConverter))]
    public void BoolToThicknessConverter() => BehaviorRunner.Verify("BoolToThicknessConverter");

    [TestMethod]
    [ConverterIdentity(typeof(BoolToValueConverter<string>))]
    public void BoolToValueConverter_String() =>
        BehaviorRunner.Verify("BoolToValueConverter_String");

    [TestMethod]
    [ConverterIdentity(typeof(BoolToValueConverter))]
    public void BoolToValueConverter() => BehaviorRunner.Verify("BoolToValueConverter");

    [TestMethod]
    [ConverterIdentity(typeof(BoolToVisibilityConverter))]
    public void BoolToVisibilityConverter() => BehaviorRunner.Verify("BoolToVisibilityConverter");

    [TestMethod]
    [ConverterIdentity(typeof(CalculatorConverter))]
    public void CalculatorConverter() => BehaviorRunner.Verify("CalculatorConverter");

    [TestMethod]
    [ConverterIdentity(typeof(CollectionCountCompareByConverter))]
    public void CollectionCountCompareByConverter() =>
        BehaviorRunner.Verify("CollectionCountCompareByConverter");

    [TestMethod]
    [ConverterIdentity(typeof(CollectionCountCompareConverter))]
    public void CollectionCountCompareConverter() =>
        BehaviorRunner.Verify("CollectionCountCompareConverter");

    [TestMethod]
    [ConverterIdentity(typeof(DateTimeOffsetToStringConverter))]
    public void DateTimeOffsetToStringConverter() =>
        BehaviorRunner.Verify("DateTimeOffsetToStringConverter");

    [TestMethod]
    [ConverterIdentity(typeof(DateTimeToStringConverter))]
    public void DateTimeToStringConverter() => BehaviorRunner.Verify("DateTimeToStringConverter");

    [TestMethod]
    [ConverterIdentity(typeof(DebugConverter))]
    public void DebugConverter() => BehaviorRunner.Verify("DebugConverter");

    [TestMethod]
    [ConverterIdentity(typeof(EnumEqualsConverter))]
    public void EnumEqualsConverter() => BehaviorRunner.Verify("EnumEqualsConverter");

    [TestMethod]
    [ConverterIdentity(typeof(EnumToEnumInfoConverter))]
    public void EnumToEnumInfoConverter() => BehaviorRunner.Verify("EnumToEnumInfoConverter");

    [TestMethod]
    [ConverterIdentity(typeof(EnumToEnumInfoDisplayConverter))]
    public void EnumToEnumInfoDisplayConverter() =>
        BehaviorRunner.Verify("EnumToEnumInfoDisplayConverter");

    [TestMethod]
    [ConverterIdentity(typeof(EnumsToEnumInfosConverter))]
    public void EnumsToEnumInfosConverter() => BehaviorRunner.Verify("EnumsToEnumInfosConverter");

    [TestMethod]
    [ConverterIdentity(typeof(EqualsConverter))]
    public void EqualsConverter() => BehaviorRunner.Verify("EqualsConverter");

    [TestMethod]
    [ConverterIdentity(typeof(EqualsToValueConverter<string>))]
    public void EqualsToValueConverter_String() =>
        BehaviorRunner.Verify("EqualsToValueConverter_String");

    [TestMethod]
    [ConverterIdentity(typeof(EqualsToValueConverter))]
    public void EqualsToValueConverter() => BehaviorRunner.Verify("EqualsToValueConverter");

    [TestMethod]
    [ConverterIdentity(typeof(EqualsToVisibilityConverter))]
    public void EqualsToVisibilityConverter() =>
        BehaviorRunner.Verify("EqualsToVisibilityConverter");

    [TestMethod]
    [ConverterIdentity(typeof(FirstOrDefaultConverter))]
    public void FirstOrDefaultConverter() => BehaviorRunner.Verify("FirstOrDefaultConverter");

    [TestMethod]
    [ConverterIdentity(typeof(GetDictionaryValueConverter))]
    public void GetDictionaryValueConverter() =>
        BehaviorRunner.Verify("GetDictionaryValueConverter");

    [TestMethod]
    [ConverterIdentity(typeof(GuidToStringConverter))]
    public void GuidToStringConverter() => BehaviorRunner.Verify("GuidToStringConverter");

    [TestMethod]
    [ConverterIdentity(typeof(MediaColorToSolidColorBrushConverter))]
    public void MediaColorToSolidColorBrushConverter() =>
        BehaviorRunner.Verify("MediaColorToSolidColorBrushConverter");

    [TestMethod]
    [ConverterIdentity(typeof(NullToTrueConverter))]
    public void NullToTrueConverter() => BehaviorRunner.Verify("NullToTrueConverter");

    [TestMethod]
    [ConverterIdentity(typeof(NumberClampConverter))]
    public void NumberClampConverter() => BehaviorRunner.Verify("NumberClampConverter");

    [TestMethod]
    [ConverterIdentity(typeof(NumberCompareByConverter))]
    public void NumberCompareByConverter() => BehaviorRunner.Verify("NumberCompareByConverter");

    [TestMethod]
    [ConverterIdentity(typeof(NumberCompareConverter))]
    public void NumberCompareConverter() => BehaviorRunner.Verify("NumberCompareConverter");

    [TestMethod]
    [ConverterIdentity(typeof(SolidColorBrushToMediaColorConverter))]
    public void SolidColorBrushToMediaColorConverter() =>
        BehaviorRunner.Verify("SolidColorBrushToMediaColorConverter");

    [TestMethod]
    [ConverterIdentity(typeof(StringCaseConverter))]
    public void StringCaseConverter() => BehaviorRunner.Verify("StringCaseConverter");

    [TestMethod]
    [ConverterIdentity(typeof(StringCheckConverter))]
    public void StringCheckConverter() => BehaviorRunner.Verify("StringCheckConverter");

    [TestMethod]
    [ConverterIdentity(typeof(StringToNumberConverter))]
    public void StringToNumberConverter() => BehaviorRunner.Verify("StringToNumberConverter");

    [TestMethod]
    [ConverterIdentity(typeof(TimeSpanToStringConverter))]
    public void TimeSpanToStringConverter() => BehaviorRunner.Verify("TimeSpanToStringConverter");

    [TestMethod]
    [ConverterIdentity(typeof(ToBoolConverter))]
    public void ToBoolConverter() => BehaviorRunner.Verify("ToBoolConverter");

    [TestMethod]
    [ConverterIdentity(typeof(ValueToEnumerableConverter))]
    public void ValueToEnumerableConverter() => BehaviorRunner.Verify("ValueToEnumerableConverter");

    [TestMethod]
    [ConverterIdentity(typeof(VersionToStringConverter))]
    public void VersionToStringConverter() => BehaviorRunner.Verify("VersionToStringConverter");
}
