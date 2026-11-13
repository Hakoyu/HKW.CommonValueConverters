using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class StringFormatMultiConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<string?>? GetReplaceUnsetValue { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new MultiConversionCase<Configuration>("StringFormatMultiConverter", factory =>
                    StringFormatMultiConverter(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "MultiConvertersHandleNullAndEmptyRequiredInputs",
                    factory => MultiConvertersHandleNullAndEmptyRequiredInputs(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "FormattingValidatesInputsAndPreservesReplacement",
                    factory => FormattingValidatesInputsAndPreservesReplacement(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "DefaultResultIsLazyAndUserExceptionsPropagate",
                    factory => DefaultResultIsLazyAndUserExceptionsPropagate(factory)
                ),
            ];
        }
    }

    private static void StringFormatMultiConverter(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual("Value: 1", c.Convert([1], null, "Value: {0}", null));
        Assert.AreEqual("Value: 1", c.Convert(["Value: {0}", 1], null, null, null));
    }

    private static void MultiConvertersHandleNullAndEmptyRequiredInputs(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert([], null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(null!, null, null, null));
    }

    private static void FormattingValidatesInputsAndPreservesReplacement(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        var unset = ConverterResults.UnsetValue;
        Assert.AreSame(unset, c.Convert([1], null, "{1}", null));
        Assert.AreSame(unset, c.Convert([1], null, "{", null));
        Assert.AreSame(unset, c.Convert([1], null, "{0:Q}", null));
        Assert.AreSame(unset, c.Convert([1, 2], null, null, null));
        Assert.AreSame(unset, c.Convert([unset], null, "{0}", null));
        c.Configuration.GetReplaceUnsetValue = () => "pending";
        Assert.AreEqual("pending", c.Convert([unset], null, "{0}", null));
        Assert.AreEqual("pending:", c.Convert(["{0}:{1}", unset, null], null, null, null));
        Assert.AreSame(unset, c.Convert([ConverterResults.DoNothing], null, "{0}", null));
        Assert.AreSame(unset, c.Convert([unset, 1], null, null, null));
    }

    private static void DefaultResultIsLazyAndUserExceptionsPropagate(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        Assert.ThrowsExactly<FormatException>(() =>
            factory(new Configuration()).Convert([new ThrowingValue()], null, "{0}", null)
        );
    }

    private sealed class ThrowingValue
    {
        public override string ToString() => throw new FormatException("user formatting");
    }
}

[TestClass]
public sealed class StringFormatMultiConverterTests
{
    public static IEnumerable<object[]> Cases => StringFormatMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(StringFormatMultiConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        MultiConversionCase<StringFormatMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static MultiConversionSubject<StringFormatMultiConverterSuite.Configuration> Create(
        StringFormatMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new StringFormatMultiConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetReplaceUnsetValue is not null)
                    converter.GetReplaceUnsetValue = settings.GetReplaceUnsetValue;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
