using System.ComponentModel.DataAnnotations;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;

namespace HKW.CommonValueConvertersTest;

public static class EnumToEnumInfoConverterSuite
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
                new SingleConversionCase<Configuration>(
                    "EnumToEnumInfoConverter_RoundTrip",
                    factory => EnumToEnumInfoConverter_RoundTrip(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "EnumToEnumInfoConverter_InvalidInputsUseDefault",
                    factory => EnumToEnumInfoConverter_InvalidInputsUseDefault(factory)
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

    private static void EnumToEnumInfoConverter_RoundTrip(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        var info = c.Convert(DisplayEnumType.First, null, null, null) as IEnumInfo;

        Assert.IsNotNull(info);
        Assert.AreEqual(DisplayEnumType.First, info.Value);
        Assert.AreEqual(typeof(DisplayEnumType), info.EnumType);
        Assert.AreEqual("First name", info.DisplayName);
        Assert.AreEqual(DisplayEnumType.First, c.ConvertBack(info, null, null, null));
    }

    private static void EnumToEnumInfoConverter_InvalidInputsUseDefault(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });

        foreach (
            var value in new object?[]
            {
                null,
                "First",
                0,
                ConverterResults.UnsetValue,
                ConverterResults.DoNothing,
            }
        )
        {
            Assert.AreSame(fallback, c.Convert(value, null, null, null));
            Assert.AreSame(fallback, c.ConvertBack(value, null, null, null));
        }
        Assert.AreSame(fallback, c.ConvertBack(TestEnumType.First, null, null, null));
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

    private enum DisplayEnumType
    {
        [Display(
            Name = "First name",
            ShortName = "First short name",
            Description = "First description"
        )]
        First,
    }
}

[TestClass]
public sealed class EnumToEnumInfoConverterTests
{
    public static IEnumerable<object[]> Cases => EnumToEnumInfoConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(EnumToEnumInfoConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<EnumToEnumInfoConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<EnumToEnumInfoConverterSuite.Configuration> Create(
        EnumToEnumInfoConverterSuite.Configuration configuration
    )
    {
        var converter = new EnumToEnumInfoConverter();
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
        var result = new EnumToEnumInfoConverter { GetDefaultResult = Fail }.Convert(
            DayOfWeek.Monday,
            null,
            null,
            null
        );
        Assert.IsNotNull(result);
    }
}
