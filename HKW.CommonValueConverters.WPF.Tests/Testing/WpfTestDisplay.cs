using System.Reflection;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConverters.WPF.Tests;

internal static class WpfTestDisplay
{
    public static string Name(MethodInfo method, object[] data) =>
        $"{method.DeclaringType!.FullName}/WPF/{method.Name}/{data.OfType<IConverterCase>().Single().Id}";
}
