using System.Collections;
using HKW.CommonValueConverters.Avalonia;
using StringCheckType = HKW.CommonValueConverters.StringCheckType;

namespace HKW.PlatformConverters.Avalonia.Tests;

#pragma warning disable S6562, S2701, MSTEST0037

[TestClass]
public sealed class MultiValueConverterTests
{
    [TestMethod]
    public void AllBoolToValueMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var c = new AllBoolToValueMultiConverter<string>()
            {
                TrueValue = "True",
                FalseValue = "False",
                NullValue = "Null",
                DefaultResult = "Default",
            };

            Assert.AreEqual("True", c.Convert([true, true], null, null, null));
            Assert.AreEqual("False", c.Convert([false, false], null, null, null));
            Assert.AreEqual("Null", c.Convert([null, null], null, null, null));
            Assert.AreEqual("Default", c.Convert([true, false], null, null, null));
        });

    [TestMethod]
    public void AllEqualsMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var c = new AllEqualsMultiConverter<string>() { Value = "Value" };

            Assert.AreEqual(true, c.Convert(["Value", "Value"], null, null, null));
            Assert.AreEqual(false, c.Convert(["Value", "Other"], null, null, null));
            Assert.AreEqual(true, c.Convert(["Parameter", "Parameter"], null, "Parameter", null));
        });

    [TestMethod]
    public void AnyEqualsMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var c = new AnyEqualsMultiConverter<string>() { Value = "Value" };

            Assert.AreEqual(true, c.Convert(["Other", "Value"], null, null, null));
            Assert.AreEqual(false, c.Convert(["Other", "Another"], null, null, null));
        });

    [TestMethod]
    public void CalculatorMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var c = new CalculatorMultiConverter();

            Assert.AreEqual(10, c.Convert([2, 3, 5], null, "++", null));
            Assert.AreEqual(15, c.Convert([2, "+", 3, "*", 3], null, null, null));
            Assert.AreSame(ConverterResults.UnsetValue, c.Convert([1, 2], null, null, null));
        });

    [TestMethod]
    public void EqualsCountMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var c = new EqualsCountMultiConverter<string>() { Value = "Value" };

            Assert.AreEqual(2, c.Convert(["Value", "Other", "Value"], null, null, null));
            Assert.AreEqual(1, c.Convert(["Parameter", "Other"], null, "Parameter", null));
        });

    [TestMethod]
    public void FirstBoolToValueMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var c = new FirstBoolToValueMultiConverter() { DefaultResult = "Default" };

            Assert.AreEqual("True", c.Convert([true, "True", "False", "Null"], null, null, null));
            Assert.AreEqual("False", c.Convert([false, "True", "False", "Null"], null, null, null));
            Assert.AreEqual("Null", c.Convert([null, "True", "False", "Null"], null, null, null));
            Assert.AreEqual("Default", c.Convert([true], null, null, null));
        });

    [TestMethod]
    public void FirstEqualsSecondMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var c = new FirstEqualsSecondMultiConverter();

            Assert.AreEqual(true, c.Convert(["Value", "Value"], null, null, null));
            Assert.AreEqual(false, c.Convert(["Value", "Other"], null, null, null));
            Assert.AreSame(ConverterResults.UnsetValue, c.Convert(["Value"], null, null, null));
        });

    [TestMethod]
    public void GetDictionaryValueMulitiConverter() =>
        PlatformThread.Run(() =>
        {
            var c = new GetDictionaryValueMultiConverter() { DefaultResult = "Default" };
            IDictionary dictionary = new Dictionary<string, string> { ["Key"] = "Value" };

            Assert.AreEqual("Value", c.Convert([dictionary, "Key"], null, null, null));
            Assert.AreEqual("Default", c.Convert([dictionary, "Missing"], null, null, null));
            Assert.AreEqual("Default", c.Convert([dictionary], null, null, null));
        });

    [TestMethod]
    public void StringFormatMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var c = new StringFormatMultiConverter();

            Assert.AreEqual("Value: 1", c.Convert([1], null, "Value: {0}", null));
            Assert.AreEqual("Value: 1", c.Convert(["Value: {0}", 1], null, null, null));
        });

    [TestMethod]
    public void FirstStringStateToOtherStringMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var c = new FirstStringStateToOtherStringMultiConverter();

            Assert.AreEqual("S2", c.Convert(["AAA", "S2", "S3", "S4", "S5"], null, null, null));
        });

    [TestMethod]
    public void FirstStringStateToOtherStringMultiConverter_Null() =>
        PlatformThread.Run(() =>
        {
            var c = new FirstStringStateToOtherStringMultiConverter();

            Assert.AreEqual("S3", c.Convert([null, "S2", "S3", "S4", "S5"], null, null, null));
        });

    [TestMethod]
    public void FirstStringStateToOtherStringMultiConverter_Empty() =>
        PlatformThread.Run(() =>
        {
            var c = new FirstStringStateToOtherStringMultiConverter();

            Assert.AreEqual("S4", c.Convert(["", "S2", "S3", "S4", "S5"], null, null, null));
        });

    [TestMethod]
    public void FirstStringStateToOtherStringMultiConverter_WhiteSpace() =>
        PlatformThread.Run(() =>
        {
            var c = new FirstStringStateToOtherStringMultiConverter();

            Assert.AreEqual("S5", c.Convert(["   ", "S2", "S3", "S4", "S5"], null, null, null));
        });
}
#pragma warning restore S6562,S2701,MSTEST0037
