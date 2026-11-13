using System.Windows;
using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

internal static class WpfConversionAdapter
{
    public static object? ToPlatform(object? value)
    {
        if (ReferenceEquals(value, Core.ConverterResults.UnsetValue))
            return DependencyProperty.UnsetValue;
        if (ReferenceEquals(value, Core.ConverterResults.DoNothing))
            return Binding.DoNothing;
        return value;
    }

    private static object? ToCore(object? value)
    {
        if (ReferenceEquals(value, DependencyProperty.UnsetValue))
            return Core.ConverterResults.UnsetValue;
        if (ReferenceEquals(value, Binding.DoNothing))
            return Core.ConverterResults.DoNothing;
        return value;
    }

    public static SingleConversionSubject<T> Single<T>(
        T configuration,
        ValueConverterBase converter,
        Type expectedType,
        Action<T> apply
    )
    {
        Assert.AreEqual(expectedType, converter.GetType());
        var subject = new SingleConversionSubject<T>(
            configuration,
            converter,
            apply,
            (value, target, parameter, culture) =>
                ToCore(
                    converter.Convert(ToPlatform(value), target, ToPlatform(parameter), culture)
                ),
            (value, target, parameter, culture) =>
                ToCore(
                    converter.ConvertBack(ToPlatform(value), target, ToPlatform(parameter), culture)
                )
        );
        Assert.AreSame(converter, subject.Instance);
        return subject;
    }

    public static MultiConversionSubject<T> Multi<T>(
        T configuration,
        MultiValueConverterBase converter,
        Type expectedType,
        Action<T> apply
    )
    {
        Assert.AreEqual(expectedType, converter.GetType());
        var subject = new MultiConversionSubject<T>(
            configuration,
            converter,
            apply,
            (values, target, parameter, culture) =>
                ToCore(
                    converter.Convert(
                        values?.Select(ToPlatform).ToArray()!,
                        target,
                        ToPlatform(parameter),
                        culture
                    )
                ),
            (value, targets, parameter, culture) =>
                converter
                    .ConvertBack(
                        ToPlatform(value),
                        targets.ToArray()!,
                        ToPlatform(parameter),
                        culture
                    )
                    .Select(ToCore)
                    .ToArray()
        );
        Assert.AreSame(converter, subject.Instance);
        return subject;
    }
}
