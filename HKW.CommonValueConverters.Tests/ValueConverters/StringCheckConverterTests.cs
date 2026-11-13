using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class StringCheckConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<StringCheckType>? GetStringCheckType { get; set; }
        public Func<bool>? GetIsInverted { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("StringCheckConverter", factory =>
                    StringCheckConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "StringCheckConverter_ConfiguredCheckAndInversion/0",
                    factory =>
                        StringCheckConverter_ConfiguredCheckAndInversion(
                            factory,
                            StringCheckType.Null,
                            null,
                            true
                        )
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "StringCheckConverter_ConfiguredCheckAndInversion/1",
                    factory =>
                        StringCheckConverter_ConfiguredCheckAndInversion(
                            factory,
                            StringCheckType.Null,
                            "",
                            false
                        )
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "StringCheckConverter_ConfiguredCheckAndInversion/2",
                    factory =>
                        StringCheckConverter_ConfiguredCheckAndInversion(
                            factory,
                            StringCheckType.NullOrEmpty,
                            "",
                            true
                        )
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "StringCheckConverter_ConfiguredCheckAndInversion/3",
                    factory =>
                        StringCheckConverter_ConfiguredCheckAndInversion(
                            factory,
                            StringCheckType.NullOrEmpty,
                            " ",
                            false
                        )
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "StringCheckConverter_ConfiguredCheckAndInversion/4",
                    factory =>
                        StringCheckConverter_ConfiguredCheckAndInversion(
                            factory,
                            StringCheckType.NullOrWhiteSpace,
                            " \t",
                            true
                        )
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "StringCheckConverter_ConfiguredCheckAndInversion/5",
                    factory =>
                        StringCheckConverter_ConfiguredCheckAndInversion(
                            factory,
                            StringCheckType.NullOrWhiteSpace,
                            "value",
                            false
                        )
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "StringCheckConverter_ParameterOverridesConfigurationAndInvalidValuesUseDefault",
                    factory =>
                        StringCheckConverter_ParameterOverridesConfigurationAndInvalidValuesUseDefault(
                            factory
                        )
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "UnavailableInputsAreNotConvertedIntoOrdinaryValues",
                    factory => UnavailableInputsAreNotConvertedIntoOrdinaryValues(factory)
                ),
            ];
        }
    }

    private static void StringCheckConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(true, c.Convert(null, null, StringCheckType.Null, null));
        Assert.AreEqual(true, c.Convert("", null, StringCheckType.NullOrEmpty, null));
        Assert.AreEqual(true, c.Convert(" ", null, StringCheckType.NullOrWhiteSpace, null));
        Assert.AreEqual(false, c.Convert("Value", null, StringCheckType.NullOrWhiteSpace, null));
    }

    private static void StringCheckConverter_ConfiguredCheckAndInversion(
        Func<Configuration, SingleConversionSubject<Configuration>> factory,
        StringCheckType checkType,
        string? value,
        bool expected
    )
    {
        var c = factory(new Configuration { GetStringCheckType = () => checkType });

        Assert.AreEqual(expected, c.Convert(value, null, null, null));
        Assert.AreEqual(expected, c.Convert(value, null, "invalid", null));
        c.Configuration.GetIsInverted = () => true;
        Assert.AreEqual(expected is false, c.Convert(value, null, null, null));
    }

    private static void StringCheckConverter_ParameterOverridesConfigurationAndInvalidValuesUseDefault(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });

        Assert.AreEqual(true, c.Convert("", null, "NullOrEmpty", null));
        Assert.AreSame(fallback, c.Convert("value", null, (StringCheckType)int.MaxValue, null));
        foreach (
            var unavailable in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing }
        )
        {
            Assert.AreSame(fallback, c.Convert(unavailable, null, null, null));
            Assert.AreSame(fallback, c.Convert("value", null, unavailable, null));
        }
    }

    private static void UnavailableInputsAreNotConvertedIntoOrdinaryValues(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });
        foreach (var sentinel in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing })
            Assert.AreSame(fallback, c.Convert(sentinel, null, null, null));
    }
}

[TestClass]
public sealed class StringCheckConverterTests
{
    public static IEnumerable<object[]> Cases => StringCheckConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(StringCheckConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<StringCheckConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<StringCheckConverterSuite.Configuration> Create(
        StringCheckConverterSuite.Configuration configuration
    )
    {
        var converter = new StringCheckConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetStringCheckType is not null)
                    converter.GetStringCheckType = settings.GetStringCheckType;
                if (settings.GetIsInverted is not null)
                    converter.GetIsInverted = settings.GetIsInverted;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
