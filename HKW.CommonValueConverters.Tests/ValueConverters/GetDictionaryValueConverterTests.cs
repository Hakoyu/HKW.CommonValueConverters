using System.Collections;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class GetDictionaryValueConverterSuite
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
                new SingleConversionCase<Configuration>("GetDictionaryValueConverter", factory =>
                    GetDictionaryValueConverter(factory)
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

    private static void GetDictionaryValueConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetDefaultResult = () => "Default" });
        IDictionary dictionary = new Dictionary<string, string> { ["Key"] = "Value" };

        Assert.AreEqual("Value", c.Convert(dictionary, null, "Key", null));
        Assert.AreEqual("Default", c.Convert(dictionary, null, "Missing", null));
        Assert.AreEqual("Default", c.Convert(null, null, "Key", null));
    }

    private static void EnumGuidVersionAndDictionaryValidateOrdinaryFailures(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var unset = ConverterResults.UnsetValue;
        IDictionary dictionary = new Dictionary<string, object?>
        {
            ["null"] = null,
            ["pending"] = unset,
        };
        var c = factory(new Configuration());
        Assert.IsNull(c.Convert(dictionary, null, "null", null));
        Assert.AreSame(unset, c.Convert(dictionary, null, 1, null));
        Assert.AreSame(unset, c.Convert(dictionary, null, unset, null));
        Assert.AreSame(unset, c.Convert(dictionary, null, "pending", null));
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
public sealed class GetDictionaryValueConverterTests
{
    public static IEnumerable<object[]> Cases => GetDictionaryValueConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(GetDictionaryValueConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<GetDictionaryValueConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<GetDictionaryValueConverterSuite.Configuration> Create(
        GetDictionaryValueConverterSuite.Configuration configuration
    )
    {
        var converter = new GetDictionaryValueConverter();
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
        var result = new GetDictionaryValueConverter { GetDefaultResult = Fail }.Convert(
            new Hashtable { ["key"] = 1 },
            null,
            "key",
            null
        );
        Assert.AreEqual(1, result);
    }
}
