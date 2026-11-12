using System.Reflection;
using System.Reflection.Emit;
using System.Windows;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class ConverterTestCoverageTests
{
    [TestMethod]
    public void SingleConverters_HaveNamedBehaviorTests() => Check(typeof(ValueConverterBase));

    [TestMethod]
    public void MultiConverters_HaveNamedBehaviorTests() => Check(typeof(MultiValueConverterBase));

    private static void Check(Type baseType) =>
        TestInventory.RequireTests(
            TestInventory.Discover(baseType.Assembly.GetTypes(), baseType),
            TestInventory
                .ActiveMethods(typeof(ConverterTestCoverageTests).Assembly.GetTypes())
                .Select(method => method.Name)
        );

    [TestMethod]
    public void Matrix_ReflectsEveryProductionTypeAndEveryExecutableLayer() =>
        PlatformThread.Run(() =>
        {
            var discovered = TestInventory.Discover(
                typeof(ConverterBase).Assembly.GetTypes(),
                typeof(ConverterBase)
            );
            var cases = ConverterTestCases.All;
            var methods = TestInventory.ActiveMethods(GetType().Assembly.GetTypes());
            var behaviorTypes = methods
                .Select(method => method.GetCustomAttribute<ConverterIdentityAttribute>())
                .Where(attribute => attribute is not null)
                .Select(attribute => attribute!.Type)
                .ToArray();
            CollectionAssert.AreEquivalent(discovered, behaviorTypes);
            CollectionAssert.AreEquivalent(
                discovered,
                cases.Select(item => item.Identity).ToArray()
            );
            Assert.AreEqual(
                cases.Count,
                cases.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count()
            );
            foreach (var item in cases.Where(item => item.IsMulti))
            {
                var changedSlots = new HashSet<int>();
                for (var stage = 1; stage < item.Inputs.Length; stage++)
                {
                    var changed = Enumerable
                        .Range(0, item.Inputs[stage].Length)
                        .Where(index =>
                            Equals(item.Inputs[stage - 1][index], item.Inputs[stage][index])
                                is false
                        )
                        .ToArray();
                    Assert.HasCount(
                        1,
                        changed,
                        $"Exactly one notified slot per stage: {item.Id}/{stage}"
                    );
                    changedSlots.Add(changed[0]);
                }
                Assert.AreEqual(
                    item.Inputs[0].Length,
                    changedSlots.Count,
                    $"All slots must notify: {item.Id}"
                );
            }
            var keys = cases.Select(item => item.Id).ToArray();
            CollectionAssert.AreEquivalent(
                keys,
                BindingIntegrationTests.Cases.Select(row => (string)row[0]).ToArray()
            );
            CollectionAssert.AreEquivalent(
                keys,
                XamlIntegrationTests.Cases.Select(row => (string)row[0]).ToArray()
            );
            Assert.HasCount(
                1,
                methods
                    .Where(method => method.DeclaringType == typeof(BindingIntegrationTests))
                    .ToArray()
            );
            Assert.HasCount(
                1,
                methods
                    .Where(method => method.DeclaringType == typeof(XamlIntegrationTests))
                    .ToArray()
            );
            var resources = new ConverterFixture();
            try
            {
                CollectionAssert.AreEquivalent(keys, resources.Keys.Cast<string>().ToArray());
                foreach (var key in keys)
                    Assert.IsInstanceOfType<DataTemplate>(resources[key]);
            }
            finally
            {
                resources.Clear();
            }
            Assert.HasCount(
                42,
                discovered.Where(type => type.IsSubclassOf(typeof(ValueConverterBase))).ToArray()
            );
            Assert.HasCount(
                17,
                discovered
                    .Where(type => type.IsSubclassOf(typeof(MultiValueConverterBase)))
                    .ToArray()
            );
        });

    [TestMethod]
    [DataRow("BoolToValueConverter", true)]
    [DataRow("BoolToValueConverter_True", true)]
    [DataRow("BoolToValueConverter_", false)]
    [DataRow("boolToValueConverter", false)]
    [DataRow("BoolToValueConverterExtra", false)]
    [DataRow("Other_BoolToValueConverter", false)]
    [DataRow("BoolToValueConverter`1", false)]
    public void Inventory_UsesOrdinalNamesAndNormalizesGenerics(string name, bool expected) =>
        Assert.AreEqual(expected, TestInventory.Matches(typeof(BoolToValueConverter<>), name));

    [TestMethod]
    public void Inventory_ReportsAllMissingTypesAndRejectsEmptyDiscovery()
    {
        var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
            TestInventory.RequireTests(
                [
                    typeof(BoolToValueConverter<>),
                    typeof(BoolInverterConverter),
                    typeof(BoolInverterConverter),
                ],
                ["BoolInverterConverter_"]
            )
        );
        Assert.AreEqual(
            $"Missing tests for 2 converters:{Environment.NewLine}{typeof(BoolInverterConverter).FullName}{Environment.NewLine}{typeof(BoolToValueConverter<>).FullName}",
            error.Message
        );
        Assert.ThrowsExactly<InvalidOperationException>(() => TestInventory.RequireTests([], []));
        TestInventory.RequireTests([typeof(BoolToValueConverter<>)], ["BoolToValueConverter_True"]);
    }

    [TestMethod]
    public void Inventory_DiscoversIndirectAndGenericTypesButNotAbstractBases()
    {
        CollectionAssert.AreEquivalent(
            new[] { typeof(BoolToValueConverter<>), typeof(BoolToVisibilityConverter) },
            TestInventory.Discover(
                [
                    typeof(ConverterBase),
                    typeof(ValueConverterBase),
                    typeof(BoolToValueConverter<>),
                    typeof(BoolToVisibilityConverter),
                    typeof(AllIsTrueMultiConverter),
                ],
                typeof(ValueConverterBase)
            )
        );
        Assert.IsEmpty(TestInventory.ActiveMethods([typeof(ConverterTestCoverageTests)]));
    }

    [TestMethod]
    public void Inventory_ExcludesIgnoredClassesMethodsAndOrdinaryHelpers()
    {
        var module = AssemblyBuilder
            .DefineDynamicAssembly(
                new AssemblyName(Guid.NewGuid().ToString("N")),
                AssemblyBuilderAccess.RunAndCollect
            )
            .DefineDynamicModule("Inventory");
        Type Make(string name, bool testClass, bool ignored, bool abstractType)
        {
            var builder = module.DefineType(
                name,
                TypeAttributes.Public | (abstractType ? TypeAttributes.Abstract : 0)
            );
            if (testClass)
                builder.SetCustomAttribute(Attribute<TestClassAttribute>());
            if (ignored)
                builder.SetCustomAttribute(Attribute<IgnoreAttribute>());
            foreach (var methodName in new[] { "Converter_Active", "Converter_Ignored", "Helper" })
            {
                var method = builder.DefineMethod(
                    methodName,
                    MethodAttributes.Public,
                    typeof(void),
                    Type.EmptyTypes
                );
                method.GetILGenerator().Emit(OpCodes.Ret);
                if (methodName != "Helper")
                    method.SetCustomAttribute(Attribute<TestMethodAttribute>());
                if (methodName == "Converter_Ignored")
                    method.SetCustomAttribute(Attribute<IgnoreAttribute>());
            }
            return builder.CreateType()!;
        }
        var methods = TestInventory.ActiveMethods([
            Make("Active", true, false, false),
            Make("Ignored", true, true, false),
            Make("Ordinary", false, false, false),
            Make("Abstract", true, false, true),
        ]);
        Assert.HasCount(1, methods);
        Assert.AreEqual("Active", methods[0].DeclaringType!.Name);
        Assert.AreEqual("Converter_Active", methods[0].Name);
    }

    private static CustomAttributeBuilder Attribute<T>()
        where T : Attribute
    {
        var constructor = typeof(T)
            .GetConstructors()
            .First(candidate => candidate.GetParameters().All(parameter => parameter.IsOptional));
        return new(
            constructor,
            constructor.GetParameters().Select(parameter => parameter.DefaultValue).ToArray()
        );
    }
}
