using System;

namespace HKW.CommonValueConverters.WPF.Tests;

internal sealed record EnumExpectation(Enum Value);

internal enum SampleEnum
{
    First,
    Second,
}

internal sealed record StyleExpectation(double Opacity);
