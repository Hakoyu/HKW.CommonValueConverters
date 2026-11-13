using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class EqualsToValueConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<object?>? GetOther { get; set; }
        public Func<string>? GetTrueValue { get; set; }
        public Func<string>? GetFalseValue { get; set; }
        public Func<string>? GetNullValue { get; set; }
        public Func<bool>? GetIsStringEquals { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("EqualsToValueConverter", factory =>
                    EqualsToValueConverter(factory)
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

    private static void EqualsToValueConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(
            new Configuration
            {
                GetOther = () => "Target",
                GetTrueValue = () => "True",
                GetFalseValue = () => "False",
                GetNullValue = () => "Null",
            }
        );

        Assert.AreEqual("True", c.Convert("Target", null, null, null));
        Assert.AreEqual("False", c.Convert("Other", null, null, null));
        Assert.AreEqual("Null", c.Convert(null, null, null, null));
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
public sealed class EqualsToValueConverterTests
{
    public static IEnumerable<object[]> Cases =>
        EqualsToValueConverterSuite.Cases.Select(row =>
        {
            var scenario = (SingleConversionCase<EqualsToValueConverterSuite.Configuration>)row[0];
            return new object[] { scenario with { Id = "string/" + scenario.Id } };
        });

    public static IEnumerable<object[]> ObjectCases =>
        EqualsToValueConverterSuite.Cases.Select(row =>
        {
            var scenario = (SingleConversionCase<EqualsToValueConverterSuite.Configuration>)row[0];
            return new object[] { scenario with { Id = "object/" + scenario.Id } };
        });

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(EqualsToValueConverter<>), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<EqualsToValueConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    [TestMethod]
    [DynamicData(nameof(ObjectCases))]
    [ConverterTest(typeof(EqualsToValueConverter<>), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<EqualsToValueConverterSuite.Configuration> scenario
    ) => scenario.Run(CreateObject);

    private static SingleConversionSubject<EqualsToValueConverterSuite.Configuration> CreateObject(
        EqualsToValueConverterSuite.Configuration configuration
    )
    {
        var converter = new EqualsToValueConverter<object?>();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetOther is not null)
                    converter.GetOther = settings.GetOther;
                if (settings.GetTrueValue is not null)
                    converter.GetTrueValue = settings.GetTrueValue;
                if (settings.GetFalseValue is not null)
                    converter.GetFalseValue = settings.GetFalseValue;
                if (settings.GetNullValue is not null)
                    converter.GetNullValue = settings.GetNullValue;
                if (settings.GetIsStringEquals is not null)
                    converter.GetIsStringEquals = settings.GetIsStringEquals;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }

    private static SingleConversionSubject<EqualsToValueConverterSuite.Configuration> Create(
        EqualsToValueConverterSuite.Configuration configuration
    )
    {
        var converter = new EqualsToValueConverter<string>();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetOther is not null)
                    converter.GetOther = settings.GetOther;
                if (settings.GetTrueValue is not null)
                    converter.GetTrueValue = settings.GetTrueValue;
                if (settings.GetFalseValue is not null)
                    converter.GetFalseValue = settings.GetFalseValue;
                if (settings.GetNullValue is not null)
                    converter.GetNullValue = settings.GetNullValue;
                if (settings.GetIsStringEquals is not null)
                    converter.GetIsStringEquals = settings.GetIsStringEquals;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
