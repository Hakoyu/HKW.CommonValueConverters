using System.Globalization;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class DateTimeToStringConverterSuite
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
                new SingleConversionCase<Configuration>("DateTimeToStringConverter", factory =>
                    DateTimeToStringConverter(factory)
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

    private static void DateTimeToStringConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetFormat = () => "yyyy-MM-dd" });

        var value = new DateTime(2026, 9, 4);

        Assert.IsNotNull(c.Convert(value, null, "yyyy-MM-dd", CultureInfo.InvariantCulture));
        Assert.IsInstanceOfType(c.ConvertBack("2026-09-04", null, null, null), typeof(DateTime));
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
            var converter = factory(new Configuration { GetFormat = () => "g" });
            var text = local.ToString("g", culture);
            Assert.AreEqual(text, converter.Convert(local, null, null, culture));
            Assert.AreEqual(
                local.ToUniversalTime(),
                converter.ConvertBack(text, null, null, culture)
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
        var expected = new DateTime(2026, 9, 4, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
        var converter = factory(new Configuration { GetFormat = () => "MM/dd/yyyy" });
        Assert.AreEqual(expected, converter.ConvertBack("04/09/2026", null, "dd/MM/yyyy", culture));
        converter.Configuration.GetFormat = () => "dd/MM/yyyy";
        Assert.AreEqual(expected, converter.ConvertBack("04/09/2026", null, null, culture));
        Assert.AreEqual(
            expected,
            converter.ConvertBack("September 4, 2026", null, "dd/MM/yyyy", culture)
        );
        Assert.AreEqual(
            expected,
            converter.ConvertBack(expected.ToString("O", culture), null, "O", culture)
        );
    }

    private static void InvalidFormattingReturnsDefaultWithoutSwallowingUserDelegates(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert(DateTime.UnixEpoch, null, "%", CultureInfo.InvariantCulture)
        );
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.ConvertBack("invalid", null, "%", CultureInfo.InvariantCulture)
        );
        Assert.AreSame(
            ConverterResults.UnsetValue,
            c.Convert(DateTime.UnixEpoch, null, ConverterResults.UnsetValue, null)
        );
    }
}

[TestClass]
[DoNotParallelize]
public sealed class DateTimeToStringConverterTests
{
    public static IEnumerable<object[]> Cases => DateTimeToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(DateTimeToStringConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<DateTimeToStringConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<DateTimeToStringConverterSuite.Configuration> Create(
        DateTimeToStringConverterSuite.Configuration configuration
    )
    {
        var converter = new DateTimeToStringConverter();
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

    [TestMethod]
    public void InvalidFormattingReturnsDefaultWithoutSwallowingUserDelegates_Delegates()
    {
        var c = new DateTimeToStringConverter
        {
            GetFormat = () => throw new FormatException("user format"),
        };
        Assert.ThrowsExactly<FormatException>(() =>
            c.Convert(DateTime.UnixEpoch, null, null, null)
        );
        Assert.ThrowsExactly<FormatException>(() => c.ConvertBack("2026-01-01", null, null, null));
    }
}
