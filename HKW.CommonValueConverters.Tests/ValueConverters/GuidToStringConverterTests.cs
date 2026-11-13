using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class GuidToStringConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<bool?>? GetStringTo { get; set; }
        public Func<string>? GetFormat { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("GuidToStringConverter", factory =>
                    GuidToStringConverter(factory)
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

    private static void GuidToStringConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetFormat = () => "N", GetStringTo = () => true });
        var value = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");

        Assert.AreEqual("0123456789ABCDEF0123456789ABCDEF", c.Convert(value, null, null, null));
        Assert.AreEqual(value, c.ConvertBack(value.ToString(), null, null, null));
    }

    private static void EnumGuidVersionAndDictionaryValidateOrdinaryFailures(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreSame(ConverterResults.UnsetValue, c.ConvertBack("invalid", null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(Guid.Empty, null, "bad", null));
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
public sealed class GuidToStringConverterTests
{
    public static IEnumerable<object[]> Cases => GuidToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(GuidToStringConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<GuidToStringConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<GuidToStringConverterSuite.Configuration> Create(
        GuidToStringConverterSuite.Configuration configuration
    )
    {
        var converter = new GuidToStringConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetStringTo is not null)
                    converter.GetStringTo = settings.GetStringTo;
                if (settings.GetFormat is not null)
                    converter.GetFormat = settings.GetFormat;
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
        var result = new GuidToStringConverter { GetDefaultResult = Fail }.ConvertBack(
            Guid.Empty.ToString(),
            null,
            null,
            null
        );
        Assert.AreEqual(Guid.Empty, result);
    }
}
