using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class CommonDependencyPropertyTests
{
    public static IEnumerable<object[]> Cases => ConverterTestCases.Rows;

    public static string DisplayName(MethodInfo method, object[] data) =>
        $"Properties::{ConverterTestCases.Find((string)data[0]).Type.FullName}";

    [TestMethod]
    [DynamicData(nameof(Cases), DynamicDataDisplayName = nameof(DisplayName))]
    public void EveryConverter_RegistersExactNativeTypesAndIsolatesInstances(string id) =>
        PlatformThread.Run(() =>
        {
            var item = ConverterTestCases.Find(id);
            var first = item.Factory();
            var second = item.Factory();
            Assert.AreNotSame(first, second);
            var fields = item
                .Type.GetFields(
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                )
                .Where(field =>
                    field.FieldType.IsGenericType
                    && field.FieldType.GetGenericTypeDefinition()
                        == typeof(Core.CommonDependencyProperty<>)
                )
                .ToArray();
            Assert.IsTrue(fields.Length >= 2);
            foreach (var field in fields)
            {
                var handle = field.GetValue(null)!;
                var native = (DependencyProperty)
                    field.FieldType.GetProperty("Value")!.GetValue(handle)!;
                Assert.AreEqual(field.FieldType.GenericTypeArguments[0], native.PropertyType);
                Assert.AreEqual(field.Name[..^"Property".Length], native.Name);
                Assert.IsTrue(native.OwnerType.IsAssignableFrom(item.Type));
                Assert.IsNotNull(item.Type.GetProperty(native.Name));
            }
            first.DefaultResult = "first";
            second.DefaultResult = "second";
            Assert.AreEqual("first", first.DefaultResult);
            Assert.AreEqual("second", second.DefaultResult);
            first.ClearValue(ConverterBase.DefaultResultProperty);
            Assert.AreEqual("second", second.DefaultResult);
        });

    [TestMethod]
    public void NativeSetClearAndCurrentValue_UpdateCoreConfiguration() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter<int>();
            var property = (DependencyProperty)BoolToValueConverter<int>.TrueValueProperty.Value;
            Assert.AreEqual(typeof(int), property.PropertyType);
            Assert.AreEqual(0, property.DefaultMetadata.DefaultValue);
            converter.SetValue(property, 7);
            Assert.AreEqual(7, converter.TrueValue);
            Assert.AreEqual(7, converter.Convert(true, null, null, null));
            converter.SetCurrentValue(BoolToValueConverter<int>.TrueValueProperty, 9);
            Assert.AreEqual(9, converter.Convert(true, null, null, null));
            converter.ClearValue(property);
            Assert.AreEqual(0, converter.Convert(true, null, null, null));
            converter.TrueValue = 2;
            Assert.AreEqual(2, converter.GetValue(property));
            Assert.ThrowsExactly<ArgumentException>(() => converter.SetValue(property, "invalid"));
        });

    [TestMethod]
    public void MappedClrSetterAndNativeUnsetHaveDifferentSemantics() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter();
            var property = (DependencyProperty)BoolToValueConverter<object>.TrueValueProperty.Value;
            converter.TrueValue = DependencyProperty.UnsetValue;
            Assert.AreSame(Core.ConverterResults.UnsetValue, converter.ReadLocalValue(property));
            Assert.AreSame(DependencyProperty.UnsetValue, converter.TrueValue);
            converter.SetValue(property, DependencyProperty.UnsetValue);
            Assert.AreSame(DependencyProperty.UnsetValue, converter.ReadLocalValue(property));
            Assert.IsNull(converter.TrueValue);
            converter.TrueValue = Binding.DoNothing;
            Assert.AreSame(Core.ConverterResults.DoNothing, converter.ReadLocalValue(property));
            converter.ClearValue(property);
            Assert.IsNull(converter.TrueValue);
        });

    [TestMethod]
    public void HandleValidation_RejectsForeignWrongTypedAndNullHandles() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter<int>();
            var foreign = new Core.CommonDependencyProperty<int>(new object());
            var wrong = new Core.CommonDependencyProperty<string>(
                BoolToValueConverter<int>.TrueValueProperty.Value
            );
            Assert.ThrowsExactly<ArgumentException>(() => converter.GetValue(foreign));
            Assert.ThrowsExactly<ArgumentException>(() => converter.SetValue(foreign, 1));
            Assert.ThrowsExactly<ArgumentException>(() => converter.ClearValue(foreign));
            Assert.ThrowsExactly<ArgumentException>(() => converter.SetCurrentValue(foreign, 1));
            Assert.ThrowsExactly<ArgumentException>(() => converter.GetValue(wrong));
            Assert.ThrowsExactly<ArgumentNullException>(() => converter.GetValue<int>(null!));
        });

    [TestMethod]
    public void Conversion_RejectsForeignThreadAccess()
    {
        BoolInverterConverter? converter = null;
        PlatformThread.Run(() => converter = new());
        Assert.IsNotNull(converter);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            converter.Convert(true, null, null, null)
        );
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            converter.ConvertBack(true, null, null, null)
        );
    }
}
