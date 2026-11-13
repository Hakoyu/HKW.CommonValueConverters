using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HKW.CommonValueConverters.WPF.Tests;

internal static class WpfBindingRunner
{
    public static void Verify(WpfPlatformCase scenario) =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrors();
            var item = scenario.Create();
            var converter = item.Factory();
            Assert.AreEqual(scenario.Type, converter.GetType());
            var target = new TestTarget();
            var source = new TestViewModel();
            source.Apply(item.Inputs[0]);
            try
            {
                BindingOperations.SetBinding(
                    target,
                    TestTarget.ResultProperty,
                    CreateBinding(item, converter, source)
                );
                VerifyUpdates(item, target, TestTarget.ResultProperty, source);
                errors.AssertEmpty();
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });

    public static BindingBase CreateBinding(
        WpfScenario item,
        ConverterBase converter,
        TestViewModel source
    )
    {
        if (converter is IValueConverter single)
            return new Binding(nameof(TestViewModel.A))
            {
                Source = source,
                Converter = single,
                ConverterParameter = item.Parameter,
                ConverterCulture = CultureInfo.InvariantCulture,
                Mode = BindingMode.OneWay,
            };
        var multi = new MultiBinding
        {
            Converter = (IMultiValueConverter)converter,
            ConverterParameter = item.Parameter,
            ConverterCulture = CultureInfo.InvariantCulture,
            Mode = BindingMode.OneWay,
        };
        for (var index = 0; index < item.Inputs[0].Length; index++)
            multi.Bindings.Add(new Binding(((char)('A' + index)).ToString()) { Source = source });
        return multi;
    }

    public static void VerifyUpdates(
        WpfScenario item,
        FrameworkElement target,
        DependencyProperty property,
        TestViewModel source,
        Func<object?, object?>? expectedValue = null
    )
    {
        var notifiedSlots = new HashSet<int>();
        for (var stage = 0; stage < item.Inputs.Length; stage++)
        {
            if (stage > 0)
            {
                if (item.IsMulti)
                {
                    var changed = Enumerable
                        .Range(0, item.Inputs[stage].Length)
                        .Where(index =>
                            Equals(item.Inputs[stage - 1][index], item.Inputs[stage][index])
                                is false
                        )
                        .ToArray();
                    Assert.HasCount(1, changed, $"Exactly one notified slot: {item.Id}/{stage}");
                    notifiedSlots.Add(changed[0]);
                }
                source.Apply(item.Inputs[stage]);
            }
            PlatformThread.Drain();
            var expression = BindingOperations.GetBindingExpressionBase(target, property);
            Assert.IsNotNull(expression);
            Assert.AreEqual(BindingStatus.Active, expression.Status);
            Assert.IsFalse(expression.HasError);
            var expected = item.Expected[stage];
            WpfAssert.AssertResult(
                expectedValue is null ? expected : expectedValue(expected),
                target.GetValue(property)
            );
        }
        if (item.IsMulti)
            Assert.AreEqual(
                item.Inputs[0].Length,
                notifiedSlots.Count,
                $"All slots must notify: {item.Id}"
            );
    }

    public static void VerifyTyped(
        WpfPlatformCase scenario,
        Func<FrameworkElement> createTarget,
        DependencyProperty property,
        Func<object?, object?>? expectedValue = null
    ) =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrors();
            var item = scenario.Create();
            var source = new TestViewModel();
            source.Apply(item.Inputs[0]);
            var target = createTarget();
            try
            {
                BindingOperations.SetBinding(
                    target,
                    property,
                    CreateBinding(item, item.Factory(), source)
                );
                VerifyUpdates(item, target, property, source, expectedValue);
                errors.AssertEmpty();
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });
}
