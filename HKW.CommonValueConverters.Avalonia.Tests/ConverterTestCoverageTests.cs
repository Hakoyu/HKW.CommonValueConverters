using System.Reflection;
using System.Reflection.Emit;
using HKW.CommonValueConverters.Avalonia;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class ConverterTestCoverageTests
{
    [TestMethod]
    public void SingleValueConverters_HaveTests() => CheckCoverage(typeof(ValueConverterBase));

    [TestMethod]
    public void MultiValueConverters_HaveTests() => CheckCoverage(typeof(MultiValueConverterBase));

    [TestMethod]
    [DataRow("BoolToValueConverter", true)]
    [DataRow("BoolToValueConverter_Null", true)]
    [DataRow("BoolToValueConverter_Convert_Back", true)]
    [DataRow("boolToValueConverter", false)]
    [DataRow("BoolToValueConverter_", false)]
    [DataRow("BoolToValueConverterExtra", false)]
    [DataRow("Other_BoolToValueConverter", false)]
    [DataRow("BoolToValueConverter`1", false)]
    public void Matching_UsesExactNameOrNonEmptySuffix(string methodName, bool expected)
    {
        Assert.AreEqual(expected, MatchesConverter(typeof(BoolToValueConverter<>), methodName));
    }

    [TestMethod]
    public void Matching_AcceptsMultipleTestsAndNonGenericConverters()
    {
        string[] names = ["BoolInverterConverter", "BoolInverterConverter_Null", "Other"];
        Assert.AreEqual(
            2,
            names.Count(name => MatchesConverter(typeof(BoolInverterConverter), name))
        );
    }

    [TestMethod]
    public void Discovery_IncludesGenericAndIndirectSubclassesAndSeparatesGroups()
    {
        Type[] types =
        [
            typeof(ValueConverterBase),
            typeof(InvertibleValueConverterBase),
            typeof(BoolToValueConverter<>),
            typeof(BoolInverterConverter),
            typeof(MultiValueConverterBase),
            typeof(AllBoolToValueMultiConverter<>),
            typeof(string),
        ];

        CollectionAssert.AreEquivalent(
            new[] { typeof(BoolToValueConverter<>), typeof(BoolInverterConverter) },
            GetConverters(types, typeof(ValueConverterBase))
        );
        CollectionAssert.AreEqual(
            new[] { typeof(AllBoolToValueMultiConverter<>) },
            GetConverters(types, typeof(MultiValueConverterBase))
        );
    }

    [TestMethod]
    public void Discovery_OnlyIncludesActiveTestMethodsInTestClasses()
    {
        // 动态样例不会被 MSTest 当作本程序集的真实测试执行.
        var module = AssemblyBuilder
            .DefineDynamicAssembly(
                new AssemblyName(Guid.NewGuid().ToString("N")),
                AssemblyBuilderAccess.RunAndCollect
            )
            .DefineDynamicModule("Fixtures");
        Type[] types =
        [
            CreateTestClass(module, "Active", isTestClass: true),
            CreateTestClass(module, "Ignored", isTestClass: true, ignored: true),
            CreateTestClass(module, "Ordinary", isTestClass: false),
            CreateTestClass(module, "Abstract", isTestClass: true, isAbstract: true),
        ];

        var methods = GetTestMethods(types);
        Assert.HasCount(1, methods);
        Assert.AreEqual("Active", methods[0].DeclaringType?.Name);
        Assert.AreEqual("Converter_Test", methods[0].Name);
    }

    [TestMethod]
    public void Discovery_ExcludesCoverageChecks()
    {
        Assert.IsEmpty(GetTestMethods([typeof(ConverterTestCoverageTests)]));
    }

    [TestMethod]
    public void Coverage_DoesNotThrowWhenAllConvertersHaveTests()
    {
        RequireTests(
            [typeof(BoolInverterConverter), typeof(BoolToValueConverter<>)],
            ["BoolInverterConverter", "BoolToValueConverter_True", "BoolToValueConverter_False"],
            "single-value"
        );
    }

    [TestMethod]
    public void Coverage_ReportsAllMissingTypesInStableOrder()
    {
        var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
            RequireTests(
                [
                    typeof(BoolToValueConverter<>),
                    typeof(BoolInverterConverter),
                    typeof(BoolInverterConverter),
                ],
                ["BoolInverterConverter_", "BoolToValueConverterExtra"],
                "single-value"
            )
        );

        Assert.AreEqual(
            $"Missing tests for 2 single-value converters:{Environment.NewLine}"
                + $"{typeof(BoolInverterConverter).FullName}{Environment.NewLine}"
                + typeof(BoolToValueConverter<>).FullName,
            exception.Message
        );
    }

    [TestMethod]
    public void Coverage_RejectsEmptyConverterDiscovery()
    {
        var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
            RequireTests([], [], "multi-value")
        );
        Assert.AreEqual("No multi-value converters were discovered.", exception.Message);
    }

    private static void CheckCoverage(Type baseType)
    {
        var converters = GetConverters(baseType.Assembly.GetTypes(), baseType);
        var methods = GetTestMethods(typeof(ConverterTestCoverageTests).Assembly.GetTypes());
        RequireTests(
            converters,
            methods.Select(method => method.Name),
            baseType == typeof(ValueConverterBase) ? "single-value" : "multi-value"
        );
    }

    private static Type[] GetConverters(IEnumerable<Type> types, Type baseType) =>
        types
            .Where(type => type.IsClass && type.IsAbstract is false && type.IsSubclassOf(baseType))
            .ToArray();

    private static MethodInfo[] GetTestMethods(IEnumerable<Type> types) =>
        types
            .Where(type =>
                type != typeof(ConverterTestCoverageTests)
                && type.IsClass
                && type.IsVisible
                && type.IsAbstract is false
                && type.ContainsGenericParameters is false
                && type.IsDefined(typeof(TestClassAttribute), inherit: true)
                && type.IsDefined(typeof(IgnoreAttribute), inherit: true) is false
            )
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .Where(method =>
                method.IsAbstract is false
                && method.ContainsGenericParameters is false
                && method.IsDefined(typeof(TestMethodAttribute), inherit: true)
                && method.IsDefined(typeof(IgnoreAttribute), inherit: true) is false
                && (
                    method.ReturnType == typeof(void)
                    || method.ReturnType == typeof(Task)
                    || method.ReturnType == typeof(ValueTask)
                )
            )
            .ToArray();

    private static bool MatchesConverter(Type converter, string methodName)
    {
        var name = converter.Name;
        var arityIndex = name.IndexOf('`');
        if (arityIndex >= 0)
            name = name[..arityIndex];

        return string.Equals(methodName, name, StringComparison.Ordinal)
            || (
                methodName.Length > name.Length + 1
                && methodName.StartsWith(name + "_", StringComparison.Ordinal)
            );
    }

    private static void RequireTests(
        Type[] converters,
        IEnumerable<string> methodNames,
        string group
    )
    {
        if (converters.Length == 0)
            throw new InvalidOperationException($"No {group} converters were discovered.");

        var names = methodNames.Distinct(StringComparer.Ordinal).ToArray();
        var missing = converters
            .Where(converter => names.Any(name => MatchesConverter(converter, name)) is false)
            .Select(converter => converter.FullName ?? converter.Name)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        if (missing.Length > 0)
        {
            throw new InvalidOperationException(
                $"Missing tests for {missing.Length} {group} converters:{Environment.NewLine}"
                    + string.Join(Environment.NewLine, missing)
            );
        }
    }

    private static Type CreateTestClass(
        ModuleBuilder module,
        string name,
        bool isTestClass,
        bool ignored = false,
        bool isAbstract = false
    )
    {
        var attributes = TypeAttributes.Public;
        if (isAbstract)
            attributes |= TypeAttributes.Abstract;
        var type = module.DefineType(name, attributes);
        if (isTestClass)
            type.SetCustomAttribute(CreateAttribute<TestClassAttribute>());
        if (ignored)
            type.SetCustomAttribute(CreateAttribute<IgnoreAttribute>());

        foreach (var methodName in new[] { "Converter_Test", "Converter_Ignored", "Helper" })
        {
            var method = type.DefineMethod(
                methodName,
                MethodAttributes.Public,
                typeof(void),
                Type.EmptyTypes
            );
            method.GetILGenerator().Emit(OpCodes.Ret);
            if (methodName != "Helper")
                method.SetCustomAttribute(CreateAttribute<TestMethodAttribute>());
            if (methodName == "Converter_Ignored")
                method.SetCustomAttribute(CreateAttribute<IgnoreAttribute>());
        }
        return type.CreateType()!;
    }

    private static CustomAttributeBuilder CreateAttribute<T>()
        where T : Attribute
    {
        var constructor = typeof(T)
            .GetConstructors()
            .First(candidate => candidate.GetParameters().All(parameter => parameter.IsOptional));
        return new CustomAttributeBuilder(
            constructor,
            constructor.GetParameters().Select(parameter => parameter.DefaultValue).ToArray()
        );
    }
}
