using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class EnumEqualsConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("EnumEqualsConverter", factory =>
                    EnumEqualsConverter(factory)
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

    private static void EnumEqualsConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(true, c.Convert(TestEnumType.First, null, "First", null));
        Assert.AreEqual(false, c.Convert(TestEnumType.First, null, "Second", null));
        Assert.AreEqual(true, c.Convert(TestEnumType.First, null, TestEnumType.First, null));
        Assert.AreEqual(false, c.Convert(null, null, "First", null));
    }

    private static void EnumGuidVersionAndDictionaryValidateOrdinaryFailures(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        Assert.AreEqual(false, factory(new Configuration()).Convert(1, null, "Monday", null));
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
public sealed class EnumEqualsConverterTests
{
    public static IEnumerable<object[]> Cases => EnumEqualsConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(EnumEqualsConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<EnumEqualsConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<EnumEqualsConverterSuite.Configuration> Create(
        EnumEqualsConverterSuite.Configuration configuration
    )
    {
        var converter = new EnumEqualsConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
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
        var result = new EnumEqualsConverter { GetDefaultResult = Fail }.Convert(
            DayOfWeek.Monday,
            null,
            "Monday",
            null
        );
        Assert.AreEqual(true, result);
    }
}
