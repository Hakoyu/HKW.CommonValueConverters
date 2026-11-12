using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class TypedTargetBindingTests
{
    [TestMethod]
    [DataRow(nameof(BoolToFontWeightConverter))]
    [DataRow(nameof(BoolToNumberConverter))]
    [DataRow(nameof(BoolToSolidColorBrushConverter))]
    [DataRow(nameof(BoolToSplitThicknessConverter))]
    [DataRow(nameof(BoolToStyleConverter))]
    [DataRow(nameof(BoolToThicknessConverter))]
    [DataRow(nameof(BoolToVisibilityConverter))]
    [DataRow(nameof(EqualsToVisibilityConverter))]
    [DataRow(nameof(MediaColorToSolidColorBrushConverter))]
    [DataRow(nameof(SolidColorBrushToMediaColorConverter))]
    [DataRow(nameof(AllBoolToVisibilityMultiConverter))]
    [DataRow(nameof(MarginMultiConverter))]
    public void PlatformConverters_UseStronglyTypedDependencyProperties(string id) =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrors();
            var item = ConverterTestCases.Find(id);
            var converter = item.Factory();
            var source = new TestViewModel();
            source.Apply(item.Inputs[0]);
            FrameworkElement target;
            DependencyProperty property;
            switch (id)
            {
                case nameof(BoolToFontWeightConverter):
                    target = new TextBlock();
                    property = TextBlock.FontWeightProperty;
                    break;
                case nameof(BoolToNumberConverter):
                    target = new Slider { Maximum = 100 };
                    property = Slider.ValueProperty;
                    break;
                case nameof(BoolToStyleConverter):
                    target = new Border();
                    property = FrameworkElement.StyleProperty;
                    break;
                case nameof(BoolToVisibilityConverter):
                case nameof(EqualsToVisibilityConverter):
                case nameof(AllBoolToVisibilityMultiConverter):
                    target = new Border();
                    property = UIElement.VisibilityProperty;
                    break;
                case nameof(SolidColorBrushToMediaColorConverter):
                    target = new ColorTarget();
                    property = ColorTarget.ColorProperty;
                    break;
                case nameof(BoolToSolidColorBrushConverter):
                case nameof(MediaColorToSolidColorBrushConverter):
                    target = new Border();
                    property = Border.BackgroundProperty;
                    break;
                default:
                    target = new Border();
                    property = FrameworkElement.MarginProperty;
                    break;
            }
            BindingBase binding;
            if (converter is IValueConverter single)
                binding = new Binding("A")
                {
                    Source = source,
                    Converter = single,
                    ConverterParameter = item.Parameter,
                    Mode = BindingMode.OneWay,
                };
            else
            {
                var multi = new MultiBinding
                {
                    Converter = (IMultiValueConverter)converter,
                    ConverterParameter = item.Parameter,
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
                var expression = BindingOperations.SetBinding(target, property, binding);
                for (var index = 0; index < item.Inputs.Length; index++)
                {
                    if (index > 0)
                        source.Apply(item.Inputs[index]);
                    PlatformThread.Drain();
                    Assert.AreEqual(BindingStatus.Active, expression.Status);
                    Assert.IsFalse(expression.HasError);
                    var expected = item.Expected[index];
                    if (id == nameof(BoolToNumberConverter))
                        expected = System.Convert.ToDouble(expected);
                    ConverterTestCases.AssertResult(expected, target.GetValue(property));
                }
                errors.AssertEmpty();
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });
}
