using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class InputBoundaryTests
{
    public static IEnumerable<object[]> Cases => ConverterTestCases.Rows;
    public static IEnumerable<object[]> Singles =>
        ConverterTestCases
            .All.Where(item => item.IsMulti is false)
            .Select(item => new object[] { item.Id });

    public static string DisplayName(MethodInfo method, object[] data) =>
        $"{method.Name}::{ConverterTestCases.Find((string)data[0]).Type.FullName}";

    [TestMethod]
    [DynamicData(nameof(Cases), DynamicDataDisplayName = nameof(DisplayName))]
    public void UnavailableInput_UsesConfiguredFallbackWithoutMutatingSource(string id) =>
        PlatformThread.Run(() =>
        {
            var item = ConverterTestCases.Find(id);
            var converter = item.Factory();
            var fallback = new object();
            converter.DefaultResult = fallback;
            foreach (var sentinel in new[] { DependencyProperty.UnsetValue, Binding.DoNothing })
            {
                if (converter is ValueConverterBase single)
                {
                    var expected = converter is DebugConverter ? sentinel : fallback;
                    Assert.AreSame(
                        expected,
                        single.Convert(
                            sentinel,
                            typeof(object),
                            item.Parameter,
                            ConverterTestCases.Culture
                        )
                    );
                }
                else
                {
                    var multi = (MultiValueConverterBase)converter;
                    var values = item.Inputs[0].ToArray();
                    values[0] = sentinel;
                    var copy = values.ToArray();
                    Assert.AreSame(
                        fallback,
                        multi.Convert(
                            values,
                            typeof(object),
                            item.Parameter,
                            ConverterTestCases.Culture
                        )
                    );
                    CollectionAssert.AreEqual(copy, values);
                    Assert.AreSame(
                        fallback,
                        multi.Convert(
                            null!,
                            typeof(object),
                            item.Parameter,
                            ConverterTestCases.Culture
                        )
                    );
                }
            }
        });

    [TestMethod]
    [DynamicData(nameof(Singles), DynamicDataDisplayName = nameof(DisplayName))]
    public void NullInput_IsNotAnUnavailableSentinel(string id) =>
        PlatformThread.Run(() =>
        {
            var item = ConverterTestCases.Find(id);
            var converter = (ValueConverterBase)item.Factory();
            converter.DefaultResult = "fallback";
            object? expected = id switch
            {
                nameof(BoolInverterConverter)
                or nameof(EqualsConverter)
                or nameof(ToBoolConverter) => false,
                nameof(BoolToFontWeightConverter) => FontWeights.Normal,
                nameof(BoolToNumberConverter) => 2,
                nameof(BoolToSolidColorBrushConverter) => Colors.Black,
                nameof(BoolToSplitParameterConverter)
                or nameof(BoolToValueConverter)
                or "BoolToValueConverter_String"
                or nameof(EqualsToValueConverter)
                or "EqualsToValueConverter_String" => "null",
                nameof(BoolToSplitThicknessConverter) or nameof(BoolToThicknessConverter) =>
                    new Thickness(),
                nameof(BoolToStyleConverter) => new StyleExpectation(0.75),
                nameof(BoolToVisibilityConverter) or nameof(EqualsToVisibilityConverter) =>
                    Visibility.Collapsed,
                nameof(DebugConverter) or nameof(ValueToEnumerableConverter) => null,
                nameof(NullToTrueConverter)
                or nameof(NullToTrueConverter)
                or nameof(StringCheckConverter) => true,
                _ => "fallback",
            };
            ConverterTestCases.AssertResult(
                expected,
                converter.Convert(null, typeof(object), item.Parameter, ConverterTestCases.Culture)
            );
        });

    [TestMethod]
    public void MarginMultiConverter_RejectsUniformOverflowLikePerSideOverflow() =>
        PlatformThread.Run(() =>
        {
            var converter = new MarginMultiConverter { HasRatio = true, DefaultResult = "invalid" };
            Assert.AreEqual("invalid", converter.Convert([2d, double.MaxValue], null, null, null));
            Assert.AreEqual(
                "invalid",
                converter.Convert([2d, double.MaxValue, 1d, 1d, 1d], null, null, null)
            );
        });

    [TestMethod]
    public void BoolToSplitThicknessConverter_InvalidAndNonFiniteComponentsUseFallback() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToSplitThicknessConverter
            {
                Separator = "|",
                DefaultResult = "invalid",
            };
            foreach (var invalid in new[] { "bad", "NaN", "Infinity", "1,2,3,bad", "1,2,3,1E999" })
                Assert.AreEqual(
                    "invalid",
                    converter.Convert(
                        true,
                        null,
                        invalid + "|0",
                        System.Globalization.CultureInfo.InvariantCulture
                    )
                );
            converter.DefaultResult = null;
            Assert.IsNull(converter.Convert(true, null, "bad|0", null));
            Assert.AreEqual(new Thickness(), converter.Convert(true, null, "1,2|0", null));
        });

    [TestMethod]
    public void MarginMultiConverter_TwoSidesPreserveBothSlotsAndDefaultMissingSides() =>
        PlatformThread.Run(() =>
        {
            var converter = new MarginMultiConverter();
            Assert.AreEqual(new Thickness(1, 2, 0, 0), converter.Convert([1, 2], null, null, null));
            Assert.AreEqual(
                new Thickness(1, 2, 3, 0),
                converter.Convert([1, 2, 3], null, null, null)
            );
            Assert.AreEqual(
                new Thickness(0, 2, 0, 0),
                converter.Convert([null, 2], null, null, null)
            );
            converter.HasRatio = true;
            Assert.AreEqual(
                new Thickness(2, 4, 0, 0),
                converter.Convert([2, 1, 2], null, null, null)
            );
            Assert.AreEqual(new Thickness(2), converter.Convert([2, 1], null, null, null));
        });

    [TestMethod]
    public void MultiValueBoundaries_EmptyShortAndSelectedBranchInputs() =>
        PlatformThread.Run(() =>
        {
            Assert.AreEqual(
                "yes",
                new AllBoolToValueMultiConverter<string> { TrueValue = "yes" }.Convert(
                    [],
                    null,
                    null,
                    null
                )
            );
            Assert.AreEqual(
                true,
                new AllEqualsMultiConverter<string>().Convert([], null, null, null)
            );
            Assert.AreEqual(
                false,
                new AnyEqualsMultiConverter<string>().Convert([], null, null, null)
            );
            Assert.AreEqual(
                0,
                new EqualsCountMultiConverter<string>().Convert([], null, null, null)
            );
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                new FirstEqualsSecondMultiConverter().Convert([1], null, null, null)
            );
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                new FirstBoolToValueMultiConverter().Convert([true], null, null, null)
            );
            Assert.AreEqual(
                "selected",
                new FirstBoolToValueMultiConverter().Convert(
                    [true, "selected", DependencyProperty.UnsetValue],
                    null,
                    null,
                    null
                )
            );
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                new GetDictionaryValueMultiConverter().Convert([], null, null, null)
            );
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                new CalculatorMultiConverter().Convert([1, 2], null, null, null)
            );
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                new StringFormatMultiConverter().Convert([1], null, "{9}", null)
            );
            Assert.AreEqual(
                "selected",
                new FirstStringStateToOtherStringMultiConverter().Convert(
                    ["text", "selected", Binding.DoNothing],
                    null,
                    null,
                    null
                )
            );
        });
}
