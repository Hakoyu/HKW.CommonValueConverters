using System.Reflection;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class CoverageMatrixTests
{
    private static Type Identity(Type type) =>
        type.IsGenericType ? type.GetGenericTypeDefinition() : type;

    [TestMethod]
    public Task EveryProductionType_HasExactlyOneExecutableCaseInEveryLayer() =>
        PlatformThread.Run(() =>
        {
            var production = typeof(ConverterBase)
                .Assembly.GetTypes()
                .Where(t =>
                    t.IsClass
                    && t.IsAbstract is false
                    && (
                        t.IsSubclassOf(typeof(ValueConverterBase))
                        || t.IsSubclassOf(typeof(MultiValueConverterBase))
                    )
                )
                .ToArray();
            var cases = ConverterTestCases.All().ToArray();
            CollectionAssert.AreEquivalent(
                production,
                cases.Select(c => Identity(c.Type)).ToArray()
            );
            Assert.HasCount(
                39,
                production.Where(t => t.IsSubclassOf(typeof(ValueConverterBase))).ToArray()
            );
            Assert.HasCount(
                16,
                production.Where(t => t.IsSubclassOf(typeof(MultiValueConverterBase))).ToArray()
            );
            Assert.HasCount(6, production.Where(t => t.IsGenericTypeDefinition).ToArray());
            Assert.HasCount(cases.Length, cases.Select(c => c.Id).Distinct().ToArray());
            var methods = typeof(CoverageMatrixTests)
                .Assembly.GetTypes()
                .Where(t =>
                    t.IsDefined(typeof(TestClassAttribute))
                    && t.IsDefined(typeof(IgnoreAttribute)) is false
                )
                .SelectMany(t => t.GetMethods())
                .Where(m =>
                    m.IsDefined(typeof(TestMethodAttribute))
                    && m.IsDefined(typeof(IgnoreAttribute)) is false
                )
                .Select(m =>
                    (Method: m, Coverage: m.GetCustomAttribute<ConverterCoverageAttribute>())
                )
                .Where(x => x.Coverage is not null)
                .ToArray();
            Assert.HasCount(cases.Length * 3, methods);
            foreach (var layer in new[] { "Behavior", "Binding", "Xaml" })
            {
                var entries = methods.Where(m => m.Coverage!.Layer == layer).ToArray();
                CollectionAssert.AreEquivalent(
                    production,
                    entries.Select(e => Identity(e.Coverage!.Type)).ToArray()
                );
                foreach (var c in cases)
                {
                    var entry = entries.Single(e => e.Coverage!.Type == c.Type);
                    Assert.AreEqual(c.Id, entry.Coverage!.Id);
                    Assert.AreEqual(typeof(Task), entry.Method.ReturnType);
                    Assert.AreEqual(
                        $"{c.Type.Name.Split('`')[0]}{(c.Type.IsGenericType ? "<string>" : "")} / {layer}",
                        entry.Method.GetCustomAttribute<TestMethodAttribute>()!.DisplayName
                    );
                    Assert.IsTrue(c.Steps.Length > 0);
                    Assert.IsTrue(c.Steps.All(s => s.Index >= 0 && s.Index < c.Initial.Length));
                    Assert.HasCount(
                        c.Initial.Length,
                        c.Steps.Select(s => s.Index).Distinct().ToArray()
                    );
                    Assert.AreEqual(c.Type, c.Create().GetType());
                }
            }
        });

    [TestMethod]
    public void GenericXamlAdapters_InheritWithoutOverridingProductionBehavior()
    {
        Type[] adapters =
        [
            typeof(StringBoolToValueConverter),
            typeof(StringEqualsToValueConverter),
            typeof(StringAllBoolToValueMultiConverter),
            typeof(StringAllEqualsMultiConverter),
            typeof(StringAnyEqualsMultiConverter),
            typeof(StringEqualsCountMultiConverter),
        ];
        foreach (var adapter in adapters)
        {
            Assert.IsTrue(adapter.BaseType!.IsConstructedGenericType);
            Assert.IsEmpty(
                adapter.GetMethods(
                    BindingFlags.Public
                        | BindingFlags.NonPublic
                        | BindingFlags.Instance
                        | BindingFlags.DeclaredOnly
                )
            );
        }
    }
}
