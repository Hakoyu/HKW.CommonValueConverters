using System.Globalization;
using HKW.CommonValueConverters;

namespace HKW.CommonValueConvertersTest;

[TestClass]
[DoNotParallelize]
public sealed class ConverterContractTests
{
    [TestMethod]
    public void ResultsAreDistinctAndDefaultCanBeOverridden()
    {
        Assert.IsNotNull(ConverterResults.UnsetValue);
        Assert.IsNotNull(ConverterResults.DoNothing);
        Assert.AreNotSame(ConverterResults.UnsetValue, ConverterResults.DoNothing);
        var converter = new StringCaseConverter();
        Assert.AreSame(ConverterResults.UnsetValue, converter.Convert(1, null, null, null));
        converter.GetDefaultResult = () => null;
        Assert.IsNull(converter.Convert(1, null, null, null));
        converter.GetDefaultResult = () => ConverterResults.DoNothing;
        Assert.AreSame(ConverterResults.DoNothing, converter.Convert(1, null, null, null));
    }

    [TestMethod]
    public void GlobalDefaultIsReadDynamically()
    {
        var saved = CommonConverterBase.GlobalDefaultResult;
        try
        {
            var converter = new StringCaseConverter();
            var fallback = new object();
            CommonConverterBase.GlobalDefaultResult = fallback;
            Assert.AreSame(fallback, converter.Convert(1, null, null, null));
        }
        finally
        {
            CommonConverterBase.GlobalDefaultResult = saved;
        }
    }

    [TestMethod]
    public void CultureIsResolvedForAllEntryPoints()
    {
        var savedCulture = CultureInfo.CurrentCulture;
        var savedUiCulture = CultureInfo.CurrentUICulture;
        var savedPreference = ValueConvertersConfig.DefaultPreferredCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("tr-TR");
            var supplied = CultureInfo.GetCultureInfo("en-US");
            var single = new CultureProbe();
            var multi = new MultiCultureProbe();
            foreach (var preference in Enum.GetValues<PreferredCulture>())
            {
                ValueConvertersConfig.DefaultPreferredCulture = preference;
                var expected = preference switch
                {
                    PreferredCulture.CurrentCulture => CultureInfo.CurrentCulture,
                    PreferredCulture.CurrentUICulture => CultureInfo.CurrentUICulture,
                    _ => supplied,
                };
                Assert.AreSame(expected, single.Convert(null, null, null, supplied));
                Assert.AreSame(expected, single.ConvertBack(null, null, null, supplied));
                Assert.AreSame(expected, multi.Convert([], null, null, supplied));
                Assert.AreSame(expected, multi.ConvertBack(null, [], null, supplied)[0]);
            }
            single.PreferredCulture = PreferredCulture.ConverterCulture;
            ValueConvertersConfig.DefaultPreferredCulture = PreferredCulture.CurrentUICulture;
            Assert.AreSame(supplied, single.Convert(null, null, null, supplied));
            Assert.AreSame(CultureInfo.CurrentCulture, single.Convert(null, null, null, null));
        }
        finally
        {
            CultureInfo.CurrentCulture = savedCulture;
            CultureInfo.CurrentUICulture = savedUiCulture;
            ValueConvertersConfig.DefaultPreferredCulture = savedPreference;
        }
    }

    [TestMethod]
    public void UnsupportedReverseConversionRemainsExplicit()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
            new NullToTrueConverter().ConvertBack(null, null, null, null)
        );
        Assert.ThrowsExactly<NotSupportedException>(() =>
            new AllEqualsMultiConverter<object>().ConvertBack(null, [], null, null)
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

    [TestMethod]
    public void PlatformMappingsDoNotShareMutableSentinels()
    {
        var a = new PlatformMapping();
        var b = new PlatformMapping();
        Assert.AreNotSame(a.Unset, b.Unset);
        Assert.AreSame(ConverterResults.UnsetValue, a.ToCore(a.Unset));
        Assert.AreSame(ConverterResults.UnsetValue, b.ToCore(b.Unset));
        Assert.AreSame(a.Unset, a.FromCore(ConverterResults.UnsetValue));
        Assert.AreSame(b.Unset, b.FromCore(ConverterResults.UnsetValue));
        Assert.AreSame(a.DoNothing, a.FromCore(ConverterResults.DoNothing));
        Assert.AreSame(ConverterResults.DoNothing, b.ToCore(b.DoNothing));
        Assert.IsNull(a.FromCore(a.ToCore(null)));
        var ordinary = new object();
        Assert.AreSame(ordinary, b.FromCore(b.ToCore(ordinary)));
    }

    private sealed class CultureProbe : ValueConverterBase
    {
        protected override object ConvertCore(
            object? value,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => culture;

        protected override object ConvertBackCore(
            object? value,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => culture;
    }

    private sealed class MultiCultureProbe : MultiValueConverterBase
    {
        protected override object ConvertCore(
            IList<object?> values,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => culture;

        protected override object[] ConvertBackCore(
            object? value,
            IList<Type?> targetTypes,
            object? parameter,
            CultureInfo culture
        ) => [culture];
    }

    private sealed class PlatformMapping
    {
        public object Unset { get; } = new();
        public object DoNothing { get; } = new();

        public object? ToCore(object? value) =>
            ReferenceEquals(value, Unset) ? ConverterResults.UnsetValue
            : ReferenceEquals(value, DoNothing) ? ConverterResults.DoNothing
            : value;

        public object? FromCore(object? value) =>
            ReferenceEquals(value, ConverterResults.UnsetValue) ? Unset
            : ReferenceEquals(value, ConverterResults.DoNothing) ? DoNothing
            : value;
    }
}
