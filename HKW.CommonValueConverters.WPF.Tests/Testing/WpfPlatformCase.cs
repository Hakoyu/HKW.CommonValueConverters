using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConverters.WPF.Tests;

public sealed record WpfPlatformCase(
    string Id,
    Type Type,
    Func<WpfScenario> Create,
    string Resource,
    object? NullResult,
    Action<ConverterBase, ConverterBase> AssertConfiguration,
    Action VerifyWiring
) : IConverterCase
{
    public override string ToString() => Id;
}
