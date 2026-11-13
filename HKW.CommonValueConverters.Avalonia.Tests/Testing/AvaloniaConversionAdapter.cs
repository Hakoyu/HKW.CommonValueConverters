using Avalonia;
using Avalonia.Data;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

internal static class AvaloniaConversionAdapter
{
    internal static object? ToPlatform(object? value)
    {
        if (ReferenceEquals(value, Core.ConverterResults.UnsetValue))
            return AvaloniaProperty.UnsetValue;
        if (ReferenceEquals(value, Core.ConverterResults.DoNothing))
            return BindingOperations.DoNothing;
        return value;
    }

    private static object? ToCore(object? value)
    {
        if (ReferenceEquals(value, AvaloniaProperty.UnsetValue))
            return Core.ConverterResults.UnsetValue;
        if (ReferenceEquals(value, BindingOperations.DoNothing))
            return Core.ConverterResults.DoNothing;
        return value;
    }

    internal static SingleConversionSubject<TConfiguration> Single<TConfiguration, TConverter>(
        TConfiguration configuration,
        TConverter converter,
        Action<TConfiguration> apply
    )
        where TConverter : ValueConverterBase
    {
        Assert.AreEqual(typeof(TConverter), converter.GetType());
        return new(
            configuration,
            converter,
            apply,
            (value, targetType, parameter, culture) =>
                ToCore(
                    converter.Convert(ToPlatform(value), targetType, ToPlatform(parameter), culture)
                ),
            (value, targetType, parameter, culture) =>
                ToCore(
                    converter.ConvertBack(
                        ToPlatform(value),
                        targetType,
                        ToPlatform(parameter),
                        culture
                    )
                )
        );
    }

    internal static MultiConversionSubject<TConfiguration> Multi<TConfiguration, TConverter>(
        TConfiguration configuration,
        TConverter converter,
        Action<TConfiguration> apply
    )
        where TConverter : MultiValueConverterBase
    {
        Assert.AreEqual(typeof(TConverter), converter.GetType());
        return new(configuration, converter, apply, (values, targetType, parameter, culture) =>
        {
            var mapped = values?.Select(ToPlatform).ToArray();
            var snapshot = mapped?.ToArray();
            var result = converter.Convert(mapped!, targetType, ToPlatform(parameter), culture);
            if (snapshot is not null)
                CollectionAssert.AreEqual(
                    snapshot,
                    mapped!,
                    "Converters must not mutate platform inputs."
                );
            return ToCore(result);
        });
    }
}
