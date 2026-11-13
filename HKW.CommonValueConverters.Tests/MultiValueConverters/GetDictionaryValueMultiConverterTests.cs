using System.Collections;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class GetDictionaryValueMultiConverterSuite
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
                new MultiConversionCase<Configuration>(
                    "GetDictionaryValueMultiConverter",
                    factory => GetDictionaryValueMultiConverter(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "GetDictionaryValueMultiConverter_PreservesNullAndObjectValues",
                    factory =>
                        GetDictionaryValueMultiConverter_PreservesNullAndObjectValues(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "GetDictionaryValueMultiConverter_InvalidInputsUseDefault",
                    factory => GetDictionaryValueMultiConverter_InvalidInputsUseDefault(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "MultiConvertersHandleNullAndEmptyRequiredInputs",
                    factory => MultiConvertersHandleNullAndEmptyRequiredInputs(factory)
                ),
            ];
        }
    }

    private static void GetDictionaryValueMultiConverter(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetDefaultResult = () => "Default" });
        IDictionary dictionary = new Dictionary<string, string> { ["Key"] = "Value" };

        Assert.AreEqual("Value", c.Convert([dictionary, "Key"], null, null, null));
        Assert.AreEqual("Default", c.Convert([dictionary, "Missing"], null, null, null));
        Assert.AreEqual("Default", c.Convert([dictionary], null, null, null));
    }

    private static void GetDictionaryValueMultiConverter_PreservesNullAndObjectValues(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var value = new object();
        IDictionary dictionary = new Hashtable { ["Null"] = null, ["Value"] = value };
        var c = factory(new Configuration());

        Assert.IsNull(c.Convert([dictionary, "Null"], null, null, null));
        Assert.AreSame(value, c.Convert([dictionary, "Value"], null, null, null));
    }

    private static void GetDictionaryValueMultiConverter_InvalidInputsUseDefault(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });
        IDictionary dictionary = new Hashtable
        {
            ["Unset"] = ConverterResults.UnsetValue,
            ["DoNothing"] = ConverterResults.DoNothing,
        };
        IList<object?>[] inputs =
        [
            [],
            [dictionary],
            [dictionary, "Unset", "extra"],
            [dictionary, null],
            [dictionary, "Missing"],
            [new object(), "Key"],
            [null, "Key"],
            [dictionary, "Unset"],
            [dictionary, "DoNothing"],
            [ConverterResults.UnsetValue, "Key"],
            [ConverterResults.DoNothing, "Key"],
            [dictionary, ConverterResults.UnsetValue],
            [dictionary, ConverterResults.DoNothing],
        ];

        foreach (var input in inputs)
            Assert.AreSame(fallback, c.Convert(input, null, null, null));
        Assert.AreSame(fallback, c.Convert(null!, null, null, null));
    }

    private static void MultiConvertersHandleNullAndEmptyRequiredInputs(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert([], null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(null!, null, null, null));
    }
}

[TestClass]
public sealed class GetDictionaryValueMultiConverterTests
{
    public static IEnumerable<object[]> Cases => GetDictionaryValueMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(GetDictionaryValueMultiConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        MultiConversionCase<GetDictionaryValueMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static MultiConversionSubject<GetDictionaryValueMultiConverterSuite.Configuration> Create(
        GetDictionaryValueMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new GetDictionaryValueMultiConverter();
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
        var result = new GetDictionaryValueMultiConverter { GetDefaultResult = Fail }.Convert(
            [new Hashtable { ["key"] = 1 }, "key"],
            null,
            null,
            null
        );
        Assert.AreEqual(1, result);
    }
}
