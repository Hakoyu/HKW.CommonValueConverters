using System.Globalization;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class BoolToValueConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<string>? GetTrueValue { get; set; }
        public Func<string>? GetFalseValue { get; set; }
        public Func<bool>? GetIsNullable { get; set; }
        public Func<string>? GetNullValue { get; set; }
        public Func<bool>? GetIsInverted { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("BoolToValueConverter", factory =>
                    BoolToValueConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BoolToValueConverter_IsNullable",
                    factory => BoolToValueConverter_IsNullable(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BoolInversionPreservesNullBranch/0",
                    factory => BoolInversionPreservesNullBranch(factory, false, false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BoolInversionPreservesNullBranch/1",
                    factory => BoolInversionPreservesNullBranch(factory, false, true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BoolInversionPreservesNullBranch/2",
                    factory => BoolInversionPreservesNullBranch(factory, true, false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BoolInversionPreservesNullBranch/3",
                    factory => BoolInversionPreservesNullBranch(factory, true, true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "UnavailableInputsAreNotConvertedIntoOrdinaryValues",
                    factory => UnavailableInputsAreNotConvertedIntoOrdinaryValues(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BooleanNumericInputsUseEffectiveCulture",
                    factory => BooleanNumericInputsUseEffectiveCulture(factory)
                ),
            ];
        }
    }

    private static void BoolToValueConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(
            new Configuration
            {
                GetTrueValue = () => "True",
                GetFalseValue = () => "False",
                GetNullValue = () => "Null",
            }
        );

        Assert.AreEqual("True", c.Convert(true, null, null, null));
        Assert.AreEqual("False", c.Convert(false, null, null, null));
        Assert.AreEqual("False", c.Convert(null, null, null, null));
    }

    private static void BoolToValueConverter_IsNullable(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(
            new Configuration
            {
                GetTrueValue = () => "True",
                GetFalseValue = () => "False",
                GetNullValue = () => "Null",
                GetIsNullable = () => true,
            }
        );

        Assert.AreEqual("True", c.Convert(true, null, null, null));
        Assert.AreEqual("False", c.Convert(false, null, null, null));
        Assert.AreEqual("Null", c.Convert(null, null, null, null));
    }

    private static void BoolInversionPreservesNullBranch(
        Func<Configuration, SingleConversionSubject<Configuration>> factory,
        bool nullable,
        bool inverted
    )
    {
        var c = factory(
            new Configuration
            {
                GetIsNullable = () => nullable,
                GetIsInverted = () => inverted,
                GetTrueValue = () => "true",
                GetFalseValue = () => "false",
                GetNullValue = () => "null",
            }
        );
        Assert.AreEqual(inverted ? "false" : "true", c.Convert(true, null, null, null));
        Assert.AreEqual(inverted ? "true" : "false", c.Convert(false, null, null, null));
        var falseBranch = inverted ? "true" : "false";
        Assert.AreEqual(nullable ? "null" : falseBranch, c.Convert(null, null, null, null));
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert(ConverterResults.UnsetValue, null, null, null)
        );
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert(ConverterResults.DoNothing, null, null, null)
        );
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

    private static void BooleanNumericInputsUseEffectiveCulture(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.PositiveSign = "p";
        var c = factory(new Configuration { GetTrueValue = () => "yes" });
        Assert.AreEqual("yes", c.Convert("p1", null, null, culture));
    }
}

[TestClass]
[DoNotParallelize]
public sealed class BoolToValueConverterTests
{
    public static IEnumerable<object[]> Cases =>
        BoolToValueConverterSuite.Cases.Select(row =>
        {
            var scenario = (SingleConversionCase<BoolToValueConverterSuite.Configuration>)row[0];
            return new object[] { scenario with { Id = "string/" + scenario.Id } };
        });

    public static IEnumerable<object[]> ObjectCases =>
        BoolToValueConverterSuite.Cases.Select(row =>
        {
            var scenario = (SingleConversionCase<BoolToValueConverterSuite.Configuration>)row[0];
            return new object[] { scenario with { Id = "object/" + scenario.Id } };
        });

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(BoolToValueConverter<>), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<BoolToValueConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    [TestMethod]
    [DynamicData(nameof(ObjectCases))]
    [ConverterTest(typeof(BoolToValueConverter<>), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<BoolToValueConverterSuite.Configuration> scenario
    ) => scenario.Run(CreateObject);

    private static SingleConversionSubject<BoolToValueConverterSuite.Configuration> CreateObject(
        BoolToValueConverterSuite.Configuration configuration
    )
    {
        var converter = new BoolToValueConverter<object?>();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetTrueValue is not null)
                    converter.GetTrueValue = settings.GetTrueValue;
                if (settings.GetFalseValue is not null)
                    converter.GetFalseValue = settings.GetFalseValue;
                if (settings.GetIsNullable is not null)
                    converter.GetIsNullable = settings.GetIsNullable;
                if (settings.GetNullValue is not null)
                    converter.GetNullValue = settings.GetNullValue;
                if (settings.GetIsInverted is not null)
                    converter.GetIsInverted = settings.GetIsInverted;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }

    private static SingleConversionSubject<BoolToValueConverterSuite.Configuration> Create(
        BoolToValueConverterSuite.Configuration configuration
    )
    {
        var converter = new BoolToValueConverter<string>();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetTrueValue is not null)
                    converter.GetTrueValue = settings.GetTrueValue;
                if (settings.GetFalseValue is not null)
                    converter.GetFalseValue = settings.GetFalseValue;
                if (settings.GetIsNullable is not null)
                    converter.GetIsNullable = settings.GetIsNullable;
                if (settings.GetNullValue is not null)
                    converter.GetNullValue = settings.GetNullValue;
                if (settings.GetIsInverted is not null)
                    converter.GetIsInverted = settings.GetIsInverted;
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
        var exception = new FormatException("user configuration");
        var c = new BoolToValueConverter<string> { GetTrueValue = () => throw exception };
        Assert.AreSame(
            exception,
            Assert.ThrowsExactly<FormatException>(() => c.Convert(true, null, null, null))
        );
    }

    [TestMethod]
    public void ConfigurationIsReadOnEveryConversion()
    {
        var configured = "first";
        var converter = new BoolToValueConverter<string> { GetTrueValue = () => configured };
        Assert.AreEqual("first", converter.Convert(true, null, null, null));
        configured = "second";
        Assert.AreEqual("second", converter.Convert(true, null, null, null));
    }
}
