using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class BindingIntegrationTests
{
    public static IEnumerable<object[]> Cases => ConverterTestCases.Rows;

    public static string DisplayName(MethodInfo method, object[] data) =>
        $"Binding::{ConverterTestCases.Find((string)data[0]).Type.FullName}";

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(nameof(Cases), DynamicDataDisplayName = nameof(DisplayName))]
    public void EveryConverter_InitialValueAndNotification(string id) =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrors();
            var item = ConverterTestCases.Find(id);
            var converter = item.Factory();
            Assert.AreEqual(item.Type, converter.GetType());
            var target = new TestTarget();
            var source = new TestViewModel();
            source.Apply(item.Inputs[0]);
            BindingBase binding;
            if (converter is IValueConverter single)
                binding = new Binding(nameof(TestViewModel.A))
                {
                    Source = source,
                    Converter = single,
                    ConverterParameter = item.Parameter,
                    ConverterCulture = ConverterTestCases.Culture,
                    Mode = BindingMode.OneWay,
                };
            else
            {
                var multi = new MultiBinding
                {
                    Converter = (IMultiValueConverter)converter,
                    ConverterParameter = item.Parameter,
                    ConverterCulture = ConverterTestCases.Culture,
                    Mode = BindingMode.OneWay,
                };
                for (var index = 0; index < item.Inputs[0].Length; index++)
                    multi.Bindings.Add(
                        new Binding(((char)('A' + index)).ToString()) { Source = source }
                    );
                binding = multi;
            }
            try
            {
                BindingOperations.SetBinding(target, TestTarget.ResultProperty, binding);
                IntegrationRunner.Verify(item, target, source);
                errors.AssertEmpty();
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });
}

internal static class IntegrationRunner
{
    public static void Verify(ConverterCase item, TestTarget target, TestViewModel source)
    {
        for (var index = 0; index < item.Inputs.Length; index++)
        {
            if (index > 0)
                source.Apply(item.Inputs[index]);
            PlatformThread.Drain();
            var expression = BindingOperations.GetBindingExpressionBase(
                target,
                TestTarget.ResultProperty
            );
            Assert.IsNotNull(expression);
            Assert.AreEqual(BindingStatus.Active, expression.Status);
            Assert.IsFalse(expression.HasError);
            ConverterTestCases.AssertResult(item.Expected[index], target.Result);
        }
    }
}

internal sealed class BindingErrors : TraceListener, IDisposable
{
    private readonly List<string> _messages = [];
    private readonly SourceLevels _previous = PresentationTraceSources
        .DataBindingSource
        .Switch
        .Level;

    public BindingErrors()
    {
        PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
        PresentationTraceSources.DataBindingSource.Listeners.Add(this);
    }

    public override void Write(string? message)
    {
        if (message is not null)
            _messages.Add(message);
    }

    public override void WriteLine(string? message) => Write(message);

    public void AssertEmpty() =>
        Assert.IsEmpty(_messages, string.Join(Environment.NewLine, _messages));

    protected override void Dispose(bool disposing)
    {
        PresentationTraceSources.DataBindingSource.Listeners.Remove(this);
        PresentationTraceSources.DataBindingSource.Switch.Level = _previous;
        base.Dispose(disposing);
    }
}
