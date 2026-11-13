using System.Collections;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class FirstOrDefaultConverterSuite
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
                new SingleConversionCase<Configuration>("FirstOrDefaultConverter", factory =>
                    FirstOrDefaultConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "FirstOrDefaultConverter_PreservesFirstValueAndRejectsUnavailableValues",
                    factory =>
                        FirstOrDefaultConverter_PreservesFirstValueAndRejectsUnavailableValues(
                            factory
                        )
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "FirstOrDefaultDisposesEmptyEnumerator",
                    factory => FirstOrDefaultDisposesEmptyEnumerator(factory)
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

    private static void FirstOrDefaultConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetDefaultResult = () => "Default" });
        var array = new[] { 1, 2, 3 };
        Assert.AreEqual(1, c.Convert(array, null, null, null));
        Assert.AreEqual("Default", c.Convert(Array.Empty<int>(), null, null, null));
        Assert.AreEqual("Default", c.Convert(null, null, null, null));
    }

    private static void FirstOrDefaultConverter_PreservesFirstValueAndRejectsUnavailableValues(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });
        var first = new object();

        Assert.AreSame(first, c.Convert(new[] { first, new object() }, null, null, null));
        Assert.IsNull(c.Convert(new object?[] { null, first }, null, null, null));
        Assert.AreEqual('a', c.Convert("abc", null, null, null));
        Assert.AreSame(fallback, c.Convert("", null, null, null));
        Assert.AreSame(fallback, c.Convert(42, null, null, null));
        foreach (
            var unavailable in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing }
        )
            Assert.AreSame(fallback, c.Convert(new[] { unavailable, first }, null, null, null));
    }

    private static void FirstOrDefaultDisposesEmptyEnumerator(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var source = new EmptyEnumerable();
        Assert.AreSame(
            ConverterResults.UnsetValue,
            factory(new Configuration()).Convert(source, null, null, null)
        );
        Assert.IsTrue(source.Disposed);
        Assert.IsNull(
            factory(new Configuration()).Convert(new object?[] { null }, null, null, null)
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

    private sealed class EmptyEnumerable : IEnumerable, IEnumerator, IDisposable
    {
        public bool Disposed { get; private set; }
        public object Current => throw new InvalidOperationException();

        public IEnumerator GetEnumerator() => this;

        public bool MoveNext() => false;

        public void Reset() => throw new NotSupportedException();

        public void Dispose() => Disposed = true;
    }
}

[TestClass]
public sealed class FirstOrDefaultConverterTests
{
    public static IEnumerable<object[]> Cases => FirstOrDefaultConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(FirstOrDefaultConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<FirstOrDefaultConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<FirstOrDefaultConverterSuite.Configuration> Create(
        FirstOrDefaultConverterSuite.Configuration configuration
    )
    {
        var converter = new FirstOrDefaultConverter();
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
        var result = new FirstOrDefaultConverter { GetDefaultResult = Fail }.Convert(
            new[] { 1 },
            null,
            null,
            null
        );
        Assert.AreEqual(1, result);
    }
}
