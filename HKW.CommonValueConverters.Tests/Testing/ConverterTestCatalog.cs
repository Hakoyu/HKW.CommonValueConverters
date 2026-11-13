using System.Reflection;

namespace HKW.CommonValueConvertersTest.Testing;

public enum ConverterTestLayer
{
    Behavior,
    Properties,
    Binding,
    Xaml,
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ConverterTestAttribute(Type converterType, ConverterTestLayer layer) : Attribute
{
    public Type ConverterType { get; } = converterType;
    public ConverterTestLayer Layer { get; } = layer;
}

public sealed record ConverterTestException(
    Type ConverterType,
    ConverterTestLayer Layer,
    string Reason
);

public sealed record ConverterTestDescriptor(
    Type ConverterType,
    ConverterTestLayer Layer,
    string CaseId,
    bool Active = true
);

public static class ConverterTestCatalog
{
    public static Type Identity(Type type) =>
        type.IsGenericType ? type.GetGenericTypeDefinition() : type;

    public static Type[] DiscoverConverters(Assembly assembly, Func<Type, bool> predicate) =>
        assembly
            .GetExportedTypes()
            .Where(type => type.IsClass && type.IsAbstract is false && predicate(type))
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();

    public static IReadOnlyList<ConverterTestDescriptor> Discover(Assembly testAssembly)
    {
        var result = new List<ConverterTestDescriptor>();
        foreach (
            var type in testAssembly
                .GetTypes()
                .Where(type => type.IsDefined(typeof(TestClassAttribute), true))
        )
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            var associations = method.GetCustomAttributes<ConverterTestAttribute>().ToArray();
            if (associations.Length == 0)
                continue;
            var active =
                type.IsVisible
                && type.IsAbstract is false
                && type.ContainsGenericParameters is false
                && method.IsAbstract is false
                && method.ContainsGenericParameters is false
                && HasSupportedReturnType(method)
                && type.IsDefined(typeof(IgnoreAttribute), true) is false
                && method.IsDefined(typeof(TestMethodAttribute), true)
                && method.IsDefined(typeof(IgnoreAttribute), true) is false;
            result.AddRange(ReadCases(method, active));
        }
        return result;
    }

    internal static IReadOnlyList<ConverterTestDescriptor> ReadCases(MethodInfo method, bool active)
    {
        var result = new List<ConverterTestDescriptor>();
        var type = method.DeclaringType!;
        if (HasSupportedReturnType(method) is false)
            throw new InvalidOperationException(
                $"Unsupported test return type: {type.FullName}.{method.Name}. Expected void, Task or ValueTask; async void is not supported."
            );
        active =
            active
            && type.IsDefined(typeof(IgnoreAttribute), true) is false
            && method.IsDefined(typeof(IgnoreAttribute), true) is false;
        var associations = method.GetCustomAttributes<ConverterTestAttribute>().ToArray();
        var sources = method.GetCustomAttributes().OfType<ITestDataSource>().ToArray();
        if (sources.Length == 0)
            throw new InvalidOperationException(
                $"Associated entry must use executable data: {type.FullName}.{method.Name}."
            );
        foreach (var source in sources)
        {
            object?[][] rows;
            try
            {
                rows = source.GetData(method).ToArray();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Provider failed: {type.FullName}.{method.Name}.",
                    exception
                );
            }
            if (rows.Length == 0)
                throw new InvalidOperationException(
                    $"Empty provider: {type.FullName}.{method.Name}."
                );
            for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                var row = rows[rowIndex];
                ValidateRow(method, row, rowIndex);
                var scenario = row.OfType<IConverterCase>().SingleOrDefault();
                if (scenario is null || string.IsNullOrWhiteSpace(scenario.Id))
                    throw new InvalidOperationException(
                        $"Provider must expose one identified case: {type.FullName}.{method.Name}."
                    );
                foreach (var association in associations)
                    result.Add(
                        new(association.ConverterType, association.Layer, scenario.Id, active)
                    );
            }
        }
        return result;
    }

    private static bool HasSupportedReturnType(MethodInfo method)
    {
        if (method.ReturnType == typeof(void))
            return method.IsDefined(
                typeof(System.Runtime.CompilerServices.AsyncStateMachineAttribute),
                false
            )
                is false;
        return method.ReturnType == typeof(Task) || method.ReturnType == typeof(ValueTask);
    }

    private static void ValidateRow(MethodInfo method, object?[]? row, int rowIndex)
    {
        var location = $"{method.DeclaringType!.FullName}.{method.Name}, row {rowIndex}";
        var parameters = method.GetParameters();
        // 数据行必须精确匹配参数, 不推断可选值, 不展开 params, 不执行隐式转换.
        if (row is null || row.Length != parameters.Length)
            throw new InvalidOperationException(
                $"Data row argument count mismatch: {location}. Expected {parameters.Length} arguments."
            );
        for (var index = 0; index < parameters.Length; index++)
        {
            var parameterType = parameters[index].ParameterType;
            if (parameterType.IsByRef || parameterType.ContainsGenericParameters)
                throw new InvalidOperationException(
                    $"Unsupported data parameter: {location}, parameter {parameters[index].Name}."
                );
            var value = row[index];
            var compatible = value is null
                ? parameterType.IsValueType is false
                    || Nullable.GetUnderlyingType(parameterType) is not null
                : parameterType.IsInstanceOfType(value);
            if (compatible is false)
                throw new InvalidOperationException(
                    $"Data row argument type mismatch: {location}, parameter {parameters[index].Name}. Expected {parameterType.FullName}."
                );
        }
    }

    public static void Validate(
        IEnumerable<Type> converters,
        IEnumerable<ConverterTestDescriptor> descriptors,
        IEnumerable<ConverterTestLayer> requiredLayers,
        IEnumerable<ConverterTestException>? exceptions = null
    )
    {
        var types = converters.Select(Identity).Distinct().ToArray();
        var entries = descriptors.ToArray();
        var exclusions = (exceptions ?? []).ToArray();
        var errors = new List<string>();
        if (types.Length == 0)
            errors.Add("No converters were discovered.");
        foreach (var exclusion in exclusions)
        {
            if (string.IsNullOrWhiteSpace(exclusion.Reason))
                errors.Add(
                    $"Exception requires a reason: {exclusion.ConverterType.FullName}/{exclusion.Layer}."
                );
            if (types.Contains(Identity(exclusion.ConverterType)) is false)
                errors.Add(
                    $"Stale exception: {exclusion.ConverterType.FullName}/{exclusion.Layer}."
                );
        }
        foreach (var entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry.CaseId))
                errors.Add($"Empty case ID: {entry.ConverterType.FullName}/{entry.Layer}.");
            if (types.Contains(Identity(entry.ConverterType)) is false)
                errors.Add($"Unknown converter: {entry.ConverterType.FullName}/{entry.Layer}.");
            if (entry.Active is false)
                errors.Add(
                    $"Inactive entry: {entry.ConverterType.FullName}/{entry.Layer}/{entry.CaseId}."
                );
        }
        foreach (
            var duplicate in entries
                .GroupBy(entry => (Identity(entry.ConverterType), entry.Layer, entry.CaseId))
                .Where(group => group.Count() > 1)
        )
            errors.Add(
                $"Duplicate case: {duplicate.Key.Item1.FullName}/{duplicate.Key.Layer}/{duplicate.Key.CaseId}."
            );
        foreach (var type in types)
        foreach (var layer in requiredLayers.Distinct())
        {
            if (
                exclusions.Any(exclusion =>
                    Identity(exclusion.ConverterType) == type
                    && exclusion.Layer == layer
                    && string.IsNullOrWhiteSpace(exclusion.Reason) is false
                )
            )
                continue;
            if (
                entries.Any(entry =>
                    Identity(entry.ConverterType) == type
                    && entry.Layer == layer
                    && entry.Active
                    && string.IsNullOrWhiteSpace(entry.CaseId) is false
                )
                is false
            )
                errors.Add($"Missing active layer: {type.FullName}/{layer}.");
        }
        if (errors.Count > 0)
            throw new InvalidOperationException(
                string.Join(Environment.NewLine, errors.Order(StringComparer.Ordinal))
            );
    }
}
