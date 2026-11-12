using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class ConverterContractTests
{
    [TestMethod]
    public void Defaults_UnconfiguredAndClearedResultsUseCorePolicy() =>
        PlatformThread.Run(() =>
        {
            var converter = new NumberCompareConverter();
            Assert.AreSame(DependencyProperty.UnsetValue, converter.DefaultResult);
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                converter.Convert("invalid", null, null, null)
            );
            converter.DefaultResult = "fallback";
            Assert.AreEqual("fallback", converter.Convert("invalid", null, null, null));
            converter.DefaultResult = null;
            Assert.IsNull(converter.Convert("invalid", null, null, null));
            converter.ClearValue(ConverterBase.DefaultResultProperty);
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                converter.Convert("invalid", null, null, null)
            );
            var margin = new MarginMultiConverter();
            Assert.AreEqual(new Thickness(), margin.Convert(["invalid"], null, null, null));
            margin.DefaultResult = "fallback";
            margin.ClearValue(ConverterBase.DefaultResultProperty);
            Assert.AreEqual(new Thickness(), margin.Convert([double.NaN], null, null, null));
        });

    [TestMethod]
    public void BoolToSplitThicknessConverter_UsesLiveDelegateAndTypedThicknessTarget() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToSplitThicknessConverter { Separator = "|" };
            Assert.AreEqual(
                new Thickness(1, 2, 3, 4),
                converter.Convert(true, typeof(Thickness), "1,2,3,4|5|0", null)
            );
            var source = new TestViewModel { A = true };
            var border = new Border();
            try
            {
                BindingOperations.SetBinding(
                    border,
                    FrameworkElement.MarginProperty,
                    new Binding("A")
                    {
                        Source = source,
                        Converter = converter,
                        ConverterParameter = "1,2,3,4|5|0",
                    }
                );
                PlatformThread.Drain();
                Assert.AreEqual(new Thickness(1, 2, 3, 4), border.Margin);
                source.A = false;
                PlatformThread.Drain();
                Assert.AreEqual(new Thickness(5), border.Margin);
                converter.ConvertSplitValue = (_, _) => new Thickness(9);
                source.A = true;
                PlatformThread.Drain();
                Assert.AreEqual(new Thickness(9), border.Margin);
            }
            finally
            {
                BindingOperations.ClearAllBindings(border);
            }
            var split = new BoolToSplitParameterConverter();
            split.ConvertSplitValue = (value, _) => value.Length;
            Assert.AreEqual(3, split.Convert(true, null, "yes,no", null));
            split.ConvertSplitValue = (_, _) =>
                throw new InvalidOperationException("Delegate failure.");
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                split.Convert(true, null, "yes,no", null)
            );
        });

    [TestMethod]
    public void SentinelMappings_PreserveNullAndObjectIdentityAndDoNotMutateInput() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter
            {
                TrueValue = DependencyProperty.UnsetValue,
                FalseValue = Binding.DoNothing,
                NullValue = null,
                IsNullable = true,
            };
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                converter.Convert(true, null, null, null)
            );
            Assert.AreSame(Binding.DoNothing, converter.Convert(false, null, null, null));
            Assert.IsNull(converter.Convert(null, null, null, null));
            Assert.AreSame(
                Core.ConverterResults.UnsetValue,
                converter.GetValue(BoolToValueConverter<object>.TrueValueProperty)
            );
            Assert.AreSame(
                Core.ConverterResults.DoNothing,
                converter.GetValue(BoolToValueConverter<object>.FalseValueProperty)
            );
            converter.DefaultResult = "unavailable";
            foreach (var sentinel in new[] { DependencyProperty.UnsetValue, Binding.DoNothing })
            {
                Assert.AreEqual("unavailable", converter.Convert(sentinel, null, null, null));
                var values = new object?[] { sentinel, true };
                var snapshot = values.ToArray();
                var multi = new AllBoolToValueMultiConverter { DefaultResult = "unavailable" };
                Assert.AreEqual("unavailable", multi.Convert(values, null, null, null));
                CollectionAssert.AreEqual(snapshot, values);
            }
            var objectValue = new object();
            converter.TrueValue = objectValue;
            Assert.AreSame(objectValue, converter.Convert(true, null, null, null));
            var array = new[] { 1, 2 };
            var enumerable = new ValueToEnumerableConverter();
            var wrapped = (object[])enumerable.Convert(array, null, null, null)!;
            Assert.HasCount(1, wrapped);
            Assert.AreSame(array, wrapped[0]);
            Assert.IsNull(enumerable.Convert(null, null, null, null));
        });

    [TestMethod]
    public void Binding_FallbackDoNothingAndNullAreDistinct() =>
        PlatformThread.Run(() =>
        {
            var source = new TestViewModel { A = true };
            var target = new TestTarget();
            var converter = new BoolToValueConverter
            {
                TrueValue = "value",
                FalseValue = Binding.DoNothing,
                NullValue = null,
                IsNullable = true,
            };
            try
            {
                BindingOperations.SetBinding(
                    target,
                    TestTarget.ResultProperty,
                    new Binding("A")
                    {
                        Source = source,
                        Converter = converter,
                        FallbackValue = "fallback",
                        TargetNullValue = "null-target",
                        Mode = BindingMode.OneWay,
                    }
                );
                PlatformThread.Drain();
                Assert.AreEqual("value", target.Result);
                source.A = false;
                PlatformThread.Drain();
                Assert.AreEqual("value", target.Result);
                source.A = null;
                PlatformThread.Drain();
                Assert.AreEqual("null-target", target.Result);
                converter.TrueValue = DependencyProperty.UnsetValue;
                source.A = true;
                PlatformThread.Drain();
                Assert.AreEqual("fallback", target.Result);
                converter.TrueValue = "restored";
                source.A = true;
                PlatformThread.Drain();
                Assert.AreEqual("restored", target.Result);
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });

    [TestMethod]
    public void MultiBinding_FallbackDoNothingAndNullAreDistinct() =>
        PlatformThread.Run(() =>
        {
            var source = new TestViewModel { A = true, B = true };
            var target = new TestTarget();
            var converter = new AllBoolToValueMultiConverter
            {
                TrueValue = "value",
                FalseValue = Binding.DoNothing,
                NullValue = null,
                DefaultResult = Binding.DoNothing,
            };
            var binding = new MultiBinding
            {
                Converter = converter,
                FallbackValue = "fallback",
                TargetNullValue = "null-target",
                Mode = BindingMode.OneWay,
            };
            binding.Bindings.Add(new Binding("A") { Source = source });
            binding.Bindings.Add(new Binding("B") { Source = source });
            try
            {
                BindingOperations.SetBinding(target, TestTarget.ResultProperty, binding);
                PlatformThread.Drain();
                Assert.AreEqual("value", target.Result);
                source.Apply([false, false]);
                PlatformThread.Drain();
                Assert.AreEqual("value", target.Result);
                source.Apply([null, null]);
                PlatformThread.Drain();
                Assert.AreEqual("null-target", target.Result);
                converter.DefaultResult = DependencyProperty.UnsetValue;
                source.Apply([true, false]);
                PlatformThread.Drain();
                Assert.AreEqual("fallback", target.Result);
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });

    [TestMethod]
    public void PlatformDefaultsAndInvalidInputs_AreExplicit() =>
        PlatformThread.Run(() =>
        {
            Assert.AreEqual(FontWeights.Bold, new BoolToFontWeightConverter().TrueValue);
            Assert.AreEqual(FontWeights.Normal, new BoolToFontWeightConverter().FalseValue);
            Assert.AreEqual(0, new BoolToNumberConverter().TrueValue);
            Assert.AreSame(Brushes.White, new BoolToSolidColorBrushConverter().TrueValue);
            Assert.IsNull(new BoolToStyleConverter().TrueValue);
            Assert.AreEqual(
                new Thickness(0),
                new BoolToThicknessConverter().Convert(new object(), null, null, null)
            );
            Assert.AreEqual(
                Visibility.Collapsed,
                new BoolToVisibilityConverter().Convert(null, null, null, null)
            );
            Assert.AreEqual(
                Visibility.Collapsed,
                new EqualsToVisibilityConverter().Convert(null, null, null, null)
            );
            var color = new MediaColorToSolidColorBrushConverter();
            Assert.AreSame(DependencyProperty.UnsetValue, color.Convert(null, null, null, null));
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                color.ConvertBack(Colors.Red, null, null, null)
            );
            var first = (SolidColorBrush)color.Convert(Colors.Red, null, null, null)!;
            var second = (SolidColorBrush)color.Convert(Colors.Red, null, null, null)!;
            Assert.AreNotSame(first, second);
            Assert.IsFalse(first.IsFrozen);
            first.Color = Colors.Blue;
            Assert.AreEqual(Colors.Red, second.Color);
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                new SolidColorBrushToMediaColorConverter().Convert(
                    new LinearGradientBrush(),
                    null,
                    null,
                    null
                )
            );
            var margin = new MarginMultiConverter { HasRatio = true };
            Assert.AreEqual(
                new Thickness(2, 4, 6, 8),
                margin.Convert([2, 1, 2, 3, 4], null, null, null)
            );
            Assert.AreEqual(new Thickness(), margin.Convert([], null, null, null));
            Assert.AreEqual(new Thickness(), margin.Convert([1, 2, 3, 4, 5, 6], null, null, null));
            Assert.AreEqual(
                new Thickness(),
                margin.Convert([double.PositiveInfinity, 1], null, null, null)
            );
            Assert.AreEqual(new Thickness(), margin.Convert([1, double.NaN], null, null, null));
        });

    [TestMethod]
    public void Binding_UnconfiguredAndClearedDefaultActivatesTypedFallback() =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrors();
            var source = new TestViewModel { A = "12" };
            var target = new Slider { Maximum = 100 };
            var converter = new StringToNumberConverter
            {
                NumberType = HKW.HKWUtils.NumberType.Double,
            };
            try
            {
                BindingOperations.SetBinding(
                    target,
                    Slider.ValueProperty,
                    new Binding("A")
                    {
                        Source = source,
                        Converter = converter,
                        Mode = BindingMode.OneWay,
                        FallbackValue = 3d,
                        TargetNullValue = 5d,
                    }
                );
                PlatformThread.Drain();
                Assert.AreEqual(12d, target.Value);
                source.A = "invalid";
                PlatformThread.Drain();
                Assert.AreEqual(3d, target.Value);
                converter.DefaultResult = null;
                source.A = "still-invalid";
                PlatformThread.Drain();
                Assert.AreEqual(5d, target.Value);
                converter.ClearValue(ConverterBase.DefaultResultProperty);
                source.A = "cleared-invalid";
                PlatformThread.Drain();
                Assert.AreEqual(3d, target.Value);
                errors.AssertEmpty();
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });

    [TestMethod]
    public void NullableAndInversionConfiguration_RemainsLive() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter<int>
            {
                TrueValue = 7,
                FalseValue = 3,
                NullValue = 5,
            };
            Assert.AreEqual(3, converter.Convert(null, null, null, null));
            converter.IsNullable = true;
            Assert.AreEqual(5, converter.Convert(null, null, null, null));
            converter.IsInverted = true;
            Assert.AreEqual(3, converter.Convert(true, null, null, null));
            var equals = new EqualsToValueConverter<int>
            {
                Other = "2",
                TrueValue = 7,
                FalseValue = 3,
                NullValue = 5,
            };
            Assert.AreEqual(3, equals.Convert(2, null, null, null));
            equals.IsStringEquals = true;
            Assert.AreEqual(7, equals.Convert(2, null, null, null));
            Assert.AreEqual(5, equals.Convert(null, null, null, null));
            var all = new AllEqualsMultiConverter<int> { Value = 2 };
            Assert.AreEqual(true, all.Convert([2, 2], null, null, null));
            all.IsInverted = true;
            Assert.AreEqual(false, all.Convert([2, 2], null, null, null));
        });
}
