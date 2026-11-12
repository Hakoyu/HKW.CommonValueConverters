using System.Reflection;

namespace HKW.CommonValueConverters.WPF.Tests;

internal static class TestInventory
{
    public static Type[] Discover(IEnumerable<Type> types, Type baseType) =>
        types
            .Where(type => type.IsClass && type.IsAbstract is false && type.IsSubclassOf(baseType))
            .ToArray();

    public static MethodInfo[] ActiveMethods(IEnumerable<Type> types) =>
        types
            .Where(type =>
                type != typeof(ConverterTestCoverageTests)
                && type.IsClass
                && type.IsVisible
                && type.IsAbstract is false
                && type.ContainsGenericParameters is false
                && type.IsDefined(typeof(TestClassAttribute), true)
                && type.IsDefined(typeof(IgnoreAttribute), true) is false
            )
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            .Where(method =>
                method.IsAbstract is false
                && method.ContainsGenericParameters is false
                && method.IsDefined(typeof(TestMethodAttribute), true)
                && method.IsDefined(typeof(IgnoreAttribute), true) is false
                && (
                    method.ReturnType == typeof(void)
                    || method.ReturnType == typeof(Task)
                    || method.ReturnType == typeof(ValueTask)
                )
            )
            .ToArray();

    public static bool Matches(Type converter, string method)
    {
        var name = converter.Name.Split('`')[0];
        return string.Equals(name, method, StringComparison.Ordinal)
            || (
                method.Length > name.Length + 1
                && method.StartsWith(name + "_", StringComparison.Ordinal)
            );
    }

    public static void RequireTests(Type[] converters, IEnumerable<string> methods)
    {
        if (converters.Length == 0)
            throw new InvalidOperationException("No converters were discovered.");
        var names = methods.ToArray();
        var missing = converters
            .Where(type => names.Any(name => Matches(type, name)) is false)
            .Select(type => type.FullName!)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        if (missing.Length > 0)
            throw new InvalidOperationException(
                $"Missing tests for {missing.Length} converters:{Environment.NewLine}"
                    + string.Join(Environment.NewLine, missing)
            );
    }
}
