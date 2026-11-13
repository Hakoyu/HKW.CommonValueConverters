using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConverters.Avalonia.Tests;

public enum SampleEnum
{
    First,
    Second,
}

public enum OtherEnum
{
    Other,
}

public sealed record ConversionStep(int Index, object? Value, object? Expected);

public sealed record ConverterCase(
    string Id,
    Type Type,
    Func<ConverterBase> Create,
    object?[] Initial,
    object? Expected,
    ConversionStep[] Steps,
    object? Parameter,
    bool Multi
);

public sealed record AvaloniaIntegrationCase(string Id, Func<ConverterCase> Create) : IConverterCase
{
    public override string ToString() => Id;
}
