using System.Globalization;

namespace HKW.CommonValueConvertersTest.Testing;

public interface IConverterCase
{
    string Id { get; }
}

public sealed record SingleConversionCase<TConfiguration>(
    string Id,
    Action<Func<TConfiguration, SingleConversionSubject<TConfiguration>>> Execute
) : IConverterCase
{
    public void Run(Func<TConfiguration, SingleConversionSubject<TConfiguration>> factory) =>
        Execute(factory);

    public override string ToString() => Id;
}

public sealed record MultiConversionCase<TConfiguration>(
    string Id,
    Action<Func<TConfiguration, MultiConversionSubject<TConfiguration>>> Execute
) : IConverterCase
{
    public void Run(Func<TConfiguration, MultiConversionSubject<TConfiguration>> factory) =>
        Execute(factory);

    public override string ToString() => Id;
}

public sealed class SingleConversionSubject<TConfiguration>(
    TConfiguration configuration,
    object instance,
    Action<TConfiguration> apply,
    Func<object?, Type?, object?, CultureInfo?, object?> convert,
    Func<object?, Type?, object?, CultureInfo?, object?>? convertBack = null
)
{
    public TConfiguration Configuration { get; } = configuration;
    public object Instance { get; } = instance;

    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        apply(Configuration);
        return convert(value, targetType, parameter, culture);
    }

    public object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        apply(Configuration);
        if (convertBack is null)
            throw new NotSupportedException("This adapter does not support reverse conversion.");
        return convertBack(value, targetType, parameter, culture);
    }
}

public sealed class MultiConversionSubject<TConfiguration>(
    TConfiguration configuration,
    object instance,
    Action<TConfiguration> apply,
    Func<IList<object?>, Type?, object?, CultureInfo?, object?> convert,
    Func<object?, IList<Type?>, object?, CultureInfo?, object?[]>? convertBack = null
)
{
    public TConfiguration Configuration { get; } = configuration;
    public object Instance { get; } = instance;

    public object? Convert(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        apply(Configuration);
        var original = values?.ToArray();
        var result = convert(values!, targetType, parameter, culture);
        if (original is not null)
            CollectionAssert.AreEqual(
                original,
                values!.ToArray(),
                "Converters must not mutate caller inputs."
            );
        return result;
    }

    public object?[] ConvertBack(
        object? value,
        IList<Type?> targetTypes,
        object? parameter,
        CultureInfo? culture
    )
    {
        apply(Configuration);
        if (convertBack is null)
            throw new NotSupportedException("This adapter does not support reverse conversion.");
        return convertBack(value, targetTypes, parameter, culture);
    }
}
