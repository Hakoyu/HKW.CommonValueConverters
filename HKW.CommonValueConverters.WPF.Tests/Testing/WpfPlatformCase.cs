using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConverters.WPF.Tests;

public sealed record WpfPlatformCase(
    string Id,
    Type Type,
    Func<WpfScenario> Create,
    string Resource,
    object? NullResult,
    Action<ConverterBase, ConverterBase>? AssertConfiguration,
    Action VerifyWiring
) : IConverterCase
{
    public WpfPlatformCase(
        string Id,
        Type Type,
        Func<WpfScenario> Create,
        string Resource,
        object? NullResult,
        Action VerifyWiring
    )
        : this(Id, Type, Create, Resource, NullResult, null, VerifyWiring) { }

    public override string ToString() => Id;
}
