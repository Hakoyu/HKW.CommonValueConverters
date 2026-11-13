using System.Globalization;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class TimeSpanToStringConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<string>? GetFormat { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("TimeSpanToStringConverter", factory =>
                    TimeSpanToStringConverter(factory)
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
                    "TimeSpanUsesCultureAndCustomFormat",
                    factory => TimeSpanUsesCultureAndCustomFormat(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "InvalidFormattingReturnsDefaultWithoutSwallowingUserDelegates",
                    factory =>
                        InvalidFormattingReturnsDefaultWithoutSwallowingUserDelegates(factory)
                ),
            ];
        }
    }

    private static void TimeSpanToStringConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetFormat = () => "c" });
        var value = TimeSpan.FromHours(1.5);

        Assert.AreEqual("01:30:00", c.Convert(value, null, null, CultureInfo.InvariantCulture));
        Assert.AreEqual(value, c.ConvertBack("01:30:00", null, null, CultureInfo.InvariantCulture));
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

    private static void TimeSpanUsesCultureAndCustomFormat(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var saved = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            var culture = CultureInfo.GetCultureInfo("fr-FR");
            var value = new TimeSpan(1, 2, 3, 4, 567);
            var converter = factory(new Configuration());
            var text = value.ToString("g", culture);
            Assert.AreEqual(text, converter.Convert(value, null, null, culture));
            Assert.AreEqual(value, converter.ConvertBack(text, null, null, culture));
            const string format = @"hh\|mm\|ss";
            var simple = new TimeSpan(2, 3, 4);
            Assert.AreEqual(simple, converter.ConvertBack("02|03|04", null, format, culture));
            converter.Configuration.GetFormat = () => format;
            Assert.AreEqual(simple, converter.ConvertBack("02|03|04", null, null, culture));
            Assert.AreEqual(simple, converter.ConvertBack("02:03:04", null, format, culture));
            Assert.AreEqual(simple, converter.ConvertBack(simple, null, null, culture));
        }
        finally
        {
            CultureInfo.CurrentCulture = saved;
        }
    }

    private static void InvalidFormattingReturnsDefaultWithoutSwallowingUserDelegates(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert(TimeSpan.Zero, null, "%", CultureInfo.InvariantCulture)
        );
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.ConvertBack("invalid", null, "%", CultureInfo.InvariantCulture)
        );
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert(TimeSpan.Zero, null, ConverterResults.UnsetValue, null)
        );
    }
}

[TestClass]
[DoNotParallelize]
public sealed class TimeSpanToStringConverterTests
{
    public static IEnumerable<object[]> Cases => TimeSpanToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(TimeSpanToStringConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<TimeSpanToStringConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<TimeSpanToStringConverterSuite.Configuration> Create(
        TimeSpanToStringConverterSuite.Configuration configuration
    )
    {
        var converter = new TimeSpanToStringConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetFormat is not null)
                    converter.GetFormat = settings.GetFormat;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
