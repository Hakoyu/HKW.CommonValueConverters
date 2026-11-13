using System.ComponentModel.DataAnnotations;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;

namespace HKW.CommonValueConvertersTest;

public static class EnumToEnumInfoDisplayConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<EnumInfoDisplayTarget>? GetEnumInfoDisplayTarget { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "EnumToEnumInfoDisplayConverter_DefaultName",
                    factory => EnumToEnumInfoDisplayConverter_DefaultName(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "EnumToEnumInfoDisplayConverter_DisplayMetadata/0",
                    factory =>
                        EnumToEnumInfoDisplayConverter_DisplayMetadata(
                            factory,
                            EnumInfoDisplayTarget.Name,
                            "First name"
                        )
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "EnumToEnumInfoDisplayConverter_DisplayMetadata/1",
                    factory =>
                        EnumToEnumInfoDisplayConverter_DisplayMetadata(
                            factory,
                            EnumInfoDisplayTarget.ShortName,
                            "First short name"
                        )
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "EnumToEnumInfoDisplayConverter_DisplayMetadata/2",
                    factory =>
                        EnumToEnumInfoDisplayConverter_DisplayMetadata(
                            factory,
                            EnumInfoDisplayTarget.Description,
                            "First description"
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

    private static void EnumToEnumInfoDisplayConverter_DefaultName(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual("First", c.Convert(TestEnumType.First, null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert("First", null, null, null));
    }

    private static void EnumToEnumInfoDisplayConverter_DisplayMetadata(
        Func<Configuration, SingleConversionSubject<Configuration>> factory,
        EnumInfoDisplayTarget target,
        string expected
    )
    {
        var c = factory(new Configuration { GetEnumInfoDisplayTarget = () => target });

        Assert.AreEqual(expected, c.Convert(DisplayEnumType.First, null, null, null));
        var fallback = new object();
        c.Configuration.GetDefaultResult = () => fallback;
        Assert.AreSame(fallback, c.Convert(null, null, null, null));
        Assert.AreSame(fallback, c.Convert(0, null, null, null));
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
public sealed class EnumToEnumInfoDisplayConverterTests
{
    public static IEnumerable<object[]> Cases => EnumToEnumInfoDisplayConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(EnumToEnumInfoDisplayConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<EnumToEnumInfoDisplayConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<EnumToEnumInfoDisplayConverterSuite.Configuration> Create(
        EnumToEnumInfoDisplayConverterSuite.Configuration configuration
    )
    {
        var converter = new EnumToEnumInfoDisplayConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetEnumInfoDisplayTarget is not null)
                    converter.GetEnumInfoDisplayTarget = settings.GetEnumInfoDisplayTarget;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
