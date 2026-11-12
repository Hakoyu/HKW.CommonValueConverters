using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class XamlIntegrationTests
{
    public static IEnumerable<object[]> Cases => ConverterTestCases.Rows;

    public static string DisplayName(MethodInfo method, object[] data) =>
        $"Xaml::{ConverterTestCases.Find((string)data[0]).Type.FullName}";

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(nameof(Cases), DynamicDataDisplayName = nameof(DisplayName))]
    public void EveryConverter_CompiledDeclarationAndNotification(string id) =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrors();
            var item = ConverterTestCases.Find(id);
            var resources = new ConverterFixture();
            var template = (DataTemplate)resources[id];
            var root = (Grid)template.LoadContent();
            var target = (TestTarget)root.Children[0];
            var converter = (ConverterBase)root.Resources["Converter"];
            if (item.Type.IsGenericType)
            {
                Assert.AreEqual(item.Type, converter.GetType().BaseType);
                Assert.IsEmpty(
                    converter
                        .GetType()
                        .GetMethods(
                            BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance
                        )
                );
            }
            else
                Assert.AreEqual(item.Type, converter.GetType());
            VerifyConfiguration(item, converter);
            var source = new TestViewModel();
            source.Apply(item.Inputs[0]);
            try
            {
                root.DataContext = source;
                IntegrationRunner.Verify(item, target, source);
                var binding = BindingOperations.GetBindingBase(target, TestTarget.ResultProperty);
                Assert.IsNotNull(binding);
                if (binding is Binding single)
                    Assert.AreSame<object>(converter, single.Converter);
                else
                    Assert.AreSame<object>(converter, ((MultiBinding)binding).Converter);
                errors.AssertEmpty();
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
                root.DataContext = null;
                root.Resources.Clear();
                resources.Clear();
            }
        });

    private static void VerifyConfiguration(ConverterCase item, ConverterBase actual)
    {
        var expected = item.Factory();
        var fields = item
            .Type.GetFields(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
            )
            .Where(field => field.Name.EndsWith("Property", StringComparison.Ordinal));
        foreach (var field in fields)
        {
            var property = item.Type.GetProperty(field.Name[..^"Property".Length]);
            if (property is null)
                continue;
            var configured = property.GetValue(expected);
            if (configured is System.Windows.Media.SolidColorBrush brush)
                ConverterTestCases.AssertResult(brush.Color, property.GetValue(actual));
            else if (configured is Style style)
                ConverterTestCases.AssertResult(
                    new StyleExpectation((double)style.Setters.OfType<Setter>().Single().Value),
                    property.GetValue(actual)
                );
            else
                Assert.AreEqual(
                    configured,
                    property.GetValue(actual),
                    $"XAML configuration: {item.Id}.{property.Name}"
                );
        }
    }
}
