using System.Globalization;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class DateTimeOffsetToStringConverterSuite
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
                new SingleConversionCase<Configuration>(
                    "DateTimeOffsetToStringConverter",
                    factory => DateTimeOffsetToStringConverter(factory)
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
                    "DatesUseSuppliedCultureInBothDirections/0",
                    factory => DatesUseSuppliedCultureInBothDirections(factory, "fr-FR")
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "DatesUseSuppliedCultureInBothDirections/1",
                    factory => DatesUseSuppliedCultureInBothDirections(factory, "en-US")
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "ExplicitDateFormatsTakePriorityAndFallbackUsesSameCulture",
                    factory => ExplicitDateFormatsTakePriorityAndFallbackUsesSameCulture(factory)
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

    private static void DateTimeOffsetToStringConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetFormat = () => "O" });
        var value = new DateTimeOffset(2026, 9, 4, 12, 0, 0, TimeSpan.Zero);

        Assert.IsNotNull(c.Convert(value, null, "O", CultureInfo.InvariantCulture));
        Assert.IsInstanceOfType(
            c.ConvertBack(value.ToString("O", CultureInfo.InvariantCulture), null, null, null),
            typeof(DateTimeOffset)
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

    private static void DatesUseSuppliedCultureInBothDirections(
        Func<Configuration, SingleConversionSubject<Configuration>> factory,
        string cultureName
    )
    {
        var saved = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ja-JP");
            var culture = CultureInfo.GetCultureInfo(cultureName);
            var local = new DateTime(2026, 9, 14, 12, 34, 0, DateTimeKind.Local);
            var text = local.ToString("g", culture);
            var offset = new DateTimeOffset(local);
            var offsetConverter = factory(new Configuration { GetFormat = () => "g" });
            Assert.AreEqual(text, offsetConverter.Convert(offset, null, null, culture));
            Assert.AreEqual(
                offset.ToUniversalTime(),
                offsetConverter.ConvertBack(text, null, null, culture)
            );
        }
        finally
        {
            CultureInfo.CurrentCulture = saved;
        }
    }

    private static void ExplicitDateFormatsTakePriorityAndFallbackUsesSameCulture(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var offset = new DateTimeOffset(2026, 9, 4, 12, 34, 0, TimeSpan.FromHours(3));
        var offsetConverter = factory(new Configuration());
        Assert.AreEqual(
            offset.ToUniversalTime(),
            offsetConverter.ConvertBack(offset.ToString("O", culture), null, "O", culture)
        );
    }

    private static void InvalidFormattingReturnsDefaultWithoutSwallowingUserDelegates(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert(DateTimeOffset.UnixEpoch, null, "%", CultureInfo.InvariantCulture)
        );
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.ConvertBack("invalid", null, "%", CultureInfo.InvariantCulture)
        );
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert(DateTimeOffset.UnixEpoch, null, ConverterResults.UnsetValue, null)
        );
    }
}

[TestClass]
[DoNotParallelize]
public sealed class DateTimeOffsetToStringConverterTests
{
    public static IEnumerable<object[]> Cases => DateTimeOffsetToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(DateTimeOffsetToStringConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<DateTimeOffsetToStringConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<DateTimeOffsetToStringConverterSuite.Configuration> Create(
        DateTimeOffsetToStringConverterSuite.Configuration configuration
    )
    {
        var converter = new DateTimeOffsetToStringConverter();
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
