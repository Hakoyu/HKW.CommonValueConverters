namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class MultiValueConverterTests
{
    [TestMethod]
    [ConverterIdentity(typeof(AllBoolToValueMultiConverter<string>))]
    public void AllBoolToValueMultiConverter_String() =>
        BehaviorRunner.Verify("AllBoolToValueMultiConverter_String");

    [TestMethod]
    [ConverterIdentity(typeof(AllBoolToValueMultiConverter))]
    public void AllBoolToValueMultiConverter() =>
        BehaviorRunner.Verify("AllBoolToValueMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(AllBoolToVisibilityMultiConverter))]
    public void AllBoolToVisibilityMultiConverter() =>
        BehaviorRunner.Verify("AllBoolToVisibilityMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(AllEqualsMultiConverter<string>))]
    public void AllEqualsMultiConverter_String() =>
        BehaviorRunner.Verify("AllEqualsMultiConverter_String");

    [TestMethod]
    [ConverterIdentity(typeof(AllEqualsMultiConverter))]
    public void AllEqualsMultiConverter() => BehaviorRunner.Verify("AllEqualsMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(AllIsTrueMultiConverter))]
    public void AllIsTrueMultiConverter() => BehaviorRunner.Verify("AllIsTrueMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(AnyEqualsMultiConverter<string>))]
    public void AnyEqualsMultiConverter_String() =>
        BehaviorRunner.Verify("AnyEqualsMultiConverter_String");

    [TestMethod]
    [ConverterIdentity(typeof(AnyEqualsMultiConverter))]
    public void AnyEqualsMultiConverter() => BehaviorRunner.Verify("AnyEqualsMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(CalculatorMultiConverter))]
    public void CalculatorMultiConverter() => BehaviorRunner.Verify("CalculatorMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(EqualsCountMultiConverter<string>))]
    public void EqualsCountMultiConverter_String() =>
        BehaviorRunner.Verify("EqualsCountMultiConverter_String");

    [TestMethod]
    [ConverterIdentity(typeof(EqualsCountMultiConverter))]
    public void EqualsCountMultiConverter() => BehaviorRunner.Verify("EqualsCountMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(FirstBoolToValueMultiConverter))]
    public void FirstBoolToValueMultiConverter() =>
        BehaviorRunner.Verify("FirstBoolToValueMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(FirstEqualsSecondMultiConverter))]
    public void FirstEqualsSecondMultiConverter() =>
        BehaviorRunner.Verify("FirstEqualsSecondMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(FirstStringStateToOtherStringMultiConverter))]
    public void FirstStringStateToOtherStringMultiConverter() =>
        BehaviorRunner.Verify("FirstStringStateToOtherStringMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(GetDictionaryValueMultiConverter))]
    public void GetDictionaryValueMultiConverter() =>
        BehaviorRunner.Verify("GetDictionaryValueMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(MarginMultiConverter))]
    public void MarginMultiConverter() => BehaviorRunner.Verify("MarginMultiConverter");

    [TestMethod]
    [ConverterIdentity(typeof(StringFormatMultiConverter))]
    public void StringFormatMultiConverter() => BehaviorRunner.Verify("StringFormatMultiConverter");
}
