using global::Avalonia;
using global::Avalonia.Data;
using HKW.CommonValueConverters.Avalonia;
using Core = HKW.CommonValueConverters;

namespace HKW.PlatformConverters.Avalonia.Tests;

[TestClass]
public sealed class CommonDependencyPropertyTests
{
    [TestMethod]
    public void RegistrationPreservesNativeIdentityTypesOwnersAndDefaults() =>
        PlatformThread.Run(() =>
        {
            var converter = new Probe();
            var number = CommonDependencyProperty.Register<Probe, int>("Number");
            var text = CommonDependencyProperty.Register<Probe, string?>("Text");
            var marker = new object();
            var reference = CommonDependencyProperty.Register<Probe, object?>("Reference", marker);
            var configured = CommonDependencyProperty.Register<Probe, int>("Configured", 42);
            Assert.AreEqual(0, converter.GetValue(number));
            Assert.IsNull(converter.GetValue(text));
            Assert.AreSame(marker, converter.GetValue(reference));
            Assert.AreEqual(42, converter.GetValue(configured));
            var native = (StyledProperty<int>)configured.Value;
            Assert.AreEqual("Configured", native.Name);
            Assert.AreEqual(typeof(int), native.PropertyType);
            Assert.AreEqual(typeof(Probe), native.OwnerType);
            Assert.AreEqual(42, native.GetDefaultValue(converter));
            Assert.IsTrue(
                AvaloniaPropertyRegistry
                    .Instance.GetRegistered(typeof(Probe))
                    .Any(p => ReferenceEquals(p, native))
            );
            Assert.AreSame(native, new Core.CommonDependencyProperty<int>(native).Value);
        });

    [TestMethod]
    public void FactoryPreservesNativeNameValidation() =>
        PlatformThread.Run(() =>
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                CommonDependencyProperty.Register<Probe, int>(null!)
            );
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                CommonDependencyProperty.Register<Probe, int>(null!, 1)
            );
            Assert.ThrowsExactly<ArgumentException>(() =>
                CommonDependencyProperty.Register<Probe, int>("Invalid.Name")
            );
            Assert.ThrowsExactly<ArgumentException>(() =>
                CommonDependencyProperty.Register<Probe, int>("Invalid.Name", 1)
            );
        });

    [TestMethod]
    public void TypedAndNativeOperationsShareStateAndClearRestoresDefault() =>
        PlatformThread.Run(() =>
        {
            var converter = new Probe();
            var property = CommonDependencyProperty.Register<Probe, int>("SharedNumber", 7);
            var native = (StyledProperty<int>)property.Value;
            Assert.IsNull(converter.SetValue(property, 9));
            Assert.AreEqual(9, converter.GetValue(native));
            converter.SetValue(native, 12);
            Assert.AreEqual(12, converter.GetValue(property));
            converter.ClearValue(property);
            Assert.AreEqual(7, converter.GetValue(native));
            converter.SetValue(property, 21);
            converter.ClearValue(native);
            Assert.AreEqual(7, converter.GetValue(property));
            var reference = CommonDependencyProperty.Register<Probe, object?>(
                "NullableReference",
                new object()
            );
            converter.SetValue(reference, null);
            Assert.IsNull(converter.GetValue(reference));
            converter.ClearValue(reference);
            Assert.IsNotNull(converter.GetValue(reference));
        });

    [TestMethod]
    public void StylePriorityAndDisposableRestoreUnderlyingValueSources() =>
        PlatformThread.Run(() =>
        {
            var converter = new Probe();
            var property = CommonDependencyProperty.Register<Probe, int>("Prioritized", 5);
            var native = (StyledProperty<int>)property.Value;
            var style = converter.SetValue(property, 10, BindingPriority.Style);
            Assert.IsNotNull(style);
            Assert.AreEqual(10, converter.GetValue(native));
            converter.SetValue(property, 20);
            Assert.AreEqual(20, converter.GetValue(property));
            converter.ClearValue(property);
            Assert.AreEqual(10, converter.GetValue(property));
            style.Dispose();
            Assert.AreEqual(5, converter.GetValue(property));
            var hiddenStyle = converter.SetValue(property, 30, BindingPriority.Style);
            Assert.IsNotNull(hiddenStyle);
            converter.SetValue(native, 40);
            hiddenStyle.Dispose();
            Assert.AreEqual(40, converter.GetValue(property));
            converter.ClearValue(property);
            Assert.AreEqual(5, converter.GetValue(property));
        });

    [TestMethod]
    public void NullAndInvalidNativeHandlesAreRejectedByEveryTypedOperation() =>
        PlatformThread.Run(() =>
        {
            var converter = new Probe();
            Core.CommonDependencyProperty<int> missing = null!;
            Assert.AreEqual(
                "property",
                Assert
                    .ThrowsExactly<ArgumentNullException>(() => converter.GetValue(missing))
                    .ParamName
            );
            Assert.AreEqual(
                "property",
                Assert
                    .ThrowsExactly<ArgumentNullException>(() => converter.SetValue(missing, 1))
                    .ParamName
            );
            Assert.AreEqual(
                "property",
                Assert
                    .ThrowsExactly<ArgumentNullException>(() => converter.ClearValue(missing))
                    .ParamName
            );
            var wrongType = CommonDependencyProperty.Register<Probe, string>("WrongType", "value");
            foreach (
                var invalid in new[]
                {
                    new Core.CommonDependencyProperty<int>(new object()),
                    new Core.CommonDependencyProperty<int>(wrongType.Value),
                }
            )
            {
                Assert.AreEqual(
                    "property",
                    Assert
                        .ThrowsExactly<ArgumentException>(() => converter.GetValue(invalid))
                        .ParamName
                );
                Assert.AreEqual(
                    "property",
                    Assert
                        .ThrowsExactly<ArgumentException>(() => converter.SetValue(invalid, 1))
                        .ParamName
                );
                Assert.AreEqual(
                    "property",
                    Assert
                        .ThrowsExactly<ArgumentException>(() => converter.ClearValue(invalid))
                        .ParamName
                );
            }
            Assert.AreEqual("value", converter.GetValue(wrongType));
        });

    [TestMethod]
    public void ClosedGenericConvertersRegisterSeparateNativeProperties() =>
        PlatformThread.Run(() =>
        {
            var numbers = new BoolToValueConverter<int>();
            var strings = new BoolToValueConverter<string?>();
            var number = BoolToValueConverter<int>.TrueValueProperty;
            var text = BoolToValueConverter<string?>.TrueValueProperty;
            Assert.AreNotSame(number.Value, text.Value);
            Assert.AreEqual(
                typeof(BoolToValueConverter<int>),
                ((StyledProperty<int>)number.Value).OwnerType
            );
            Assert.AreEqual(
                typeof(BoolToValueConverter<string?>),
                ((StyledProperty<string?>)text.Value).OwnerType
            );
            Assert.AreEqual(0, numbers.GetValue(number));
            Assert.IsNull(strings.GetValue(text));
            numbers.SetValue(number, 123);
            strings.SetValue(text, "text");
            Assert.AreEqual(123, numbers.Convert(true, null, null, null));
            Assert.AreEqual("text", strings.Convert(true, null, null, null));
        });

    [TestMethod]
    public void DirectTypedAccessPreservesNativeSentinelCommands() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter();
            var property = BoolToValueConverter<object?>.TrueValueProperty;
            var native = (StyledProperty<object?>)property.Value;
            converter.SetValue(property, "local");
            converter.SetValue(property, BindingOperations.DoNothing);
            Assert.AreEqual("local", converter.GetValue(native));
            converter.SetValue(property, AvaloniaProperty.UnsetValue);
            Assert.IsNull(converter.GetValue(property));
            converter.TrueValue = AvaloniaProperty.UnsetValue;
            Assert.AreSame(Core.ConverterResults.UnsetValue, converter.GetValue(property));
            Assert.AreSame(AvaloniaProperty.UnsetValue, converter.TrueValue);
        });

    private sealed class Probe() : ConverterBase(new Core.StringCaseConverter());
}
