using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class EnumsToEnumInfosConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<bool>? GetOnlyValid { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("EnumsToEnumInfosConverter", factory =>
                    EnumsToEnumInfosConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "EnumGuidVersionAndDictionaryValidateOrdinaryFailures",
                    factory => EnumGuidVersionAndDictionaryValidateOrdinaryFailures(factory)
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

    private static void EnumsToEnumInfosConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.IsNotNull(
            c.Convert(new[] { TestEnumType.First, TestEnumType.Second }, null, null, null)
        );
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(null, null, null, null));
    }

    private static void EnumGuidVersionAndDictionaryValidateOrdinaryFailures(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert(Array.Empty<DayOfWeek>(), null, null, null)
        );
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(new[] { 1 }, null, null, null));
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

    private enum TestEnumType
    {
        First,
        Second,
    }
}

[TestClass]
public sealed class EnumsToEnumInfosConverterTests
{
    public static IEnumerable<object[]> Cases => EnumsToEnumInfosConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(EnumsToEnumInfosConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<EnumsToEnumInfosConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<EnumsToEnumInfosConverterSuite.Configuration> Create(
        EnumsToEnumInfosConverterSuite.Configuration configuration
    )
    {
        var converter = new EnumsToEnumInfosConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetOnlyValid is not null)
                    converter.GetOnlyValid = settings.GetOnlyValid;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }

    [TestMethod]
    public void DefaultResultIsLazyAndUserExceptionsPropagate()
    {
        object? Fail() => throw new InvalidOperationException("default called on success");
        var result = new EnumsToEnumInfosConverter { GetDefaultResult = Fail }.Convert(
            new[] { DayOfWeek.Monday },
            null,
            null,
            null
        );
        Assert.IsNotNull(result);
    }
}
