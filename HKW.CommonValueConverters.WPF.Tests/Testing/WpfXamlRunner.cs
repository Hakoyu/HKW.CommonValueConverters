using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HKW.CommonValueConverters.WPF.Tests;

internal static class WpfXamlRunner
{
    public static void Verify(WpfPlatformCase scenario) =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrors();
            var item = scenario.Create();
            var resources = new ResourceDictionary();
            Grid? root = null;
            TestTarget? target = null;
            try
            {
                resources.Source = new Uri(
                    $"/HKW.CommonValueConverters.WPF.Tests;component/TestViews/{scenario.Resource}",
                    UriKind.Relative
                );
                var template = (DataTemplate)resources[scenario.Id];
                root = (Grid)template.LoadContent();
                target = (TestTarget)root.Children[0];
                var converter = (ConverterBase)root.Resources["Converter"];
                if (scenario.Type.IsGenericType)
                {
                    Assert.AreEqual(scenario.Type, converter.GetType().BaseType);
                    Assert.IsEmpty(
                        converter
                            .GetType()
                            .GetMethods(
                                BindingFlags.DeclaredOnly
                                    | BindingFlags.Public
                                    | BindingFlags.Instance
                            )
                    );
                }
                else
                    Assert.AreEqual(scenario.Type, converter.GetType());
                scenario.AssertConfiguration(item.Factory(), converter);
                var source = new TestViewModel();
                source.Apply(item.Inputs[0]);
                root.DataContext = source;
                WpfBindingRunner.VerifyUpdates(item, target, TestTarget.ResultProperty, source);
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
                if (target is not null)
                    BindingOperations.ClearAllBindings(target);
                if (root is not null)
                {
                    root.DataContext = null;
                    root.Resources.Clear();
                }
                resources.Clear();
            }
        });
}
