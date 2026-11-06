using System.Windows;
using HKW.CommonValueConverters.WPF;
using Core = HKW.CommonValueConverters;
using PropertyFactory = HKW.CommonValueConverters.WPF.CommonDependencyProperty;

namespace HKW.PlatformConverters.WPF.Tests;

[TestClass]
public sealed class CommonDependencyPropertyTests
{
    [TestMethod]
    public void RegistrationPreservesNativeIdentityTypesOwnerAndDefaults() =>
        PlatformThread.Run(() =>
        {
            var value = PropertyFactory.Register<Probe, int>("ImplicitInteger");
            var reference = PropertyFactory.Register<Probe, string?>("ImplicitReference");
            var nullable = PropertyFactory.Register<Probe, int?>("ImplicitNullable");
            var marker = new object();
            var explicitValue = PropertyFactory.Register<Probe, object>(
                "ExplicitReference",
                marker
            );
            var explicitNull = PropertyFactory.Register<Probe, string?>("ExplicitNull", null);
            var native = (DependencyProperty)value.Value;
            Assert.AreEqual("ImplicitInteger", native.Name);
            Assert.AreEqual(typeof(int), native.PropertyType);
            Assert.AreEqual(typeof(Probe), native.OwnerType);
            Assert.AreSame(native, new Core.CommonDependencyProperty<int>(native).Value);
            var probe = new Probe();
            Assert.AreEqual(0, probe.GetValue(value));
            Assert.IsNull(probe.GetValue(reference));
            Assert.IsNull(probe.GetValue(nullable));
            Assert.AreSame(marker, probe.GetValue(explicitValue));
            Assert.IsNull(probe.GetValue(explicitNull));
        });

    [TestMethod]
    public void TypedAndNativeOperationsShareValuesAndResetToMetadata() =>
        PlatformThread.Run(() =>
        {
            var property = PropertyFactory.Register<Probe, int>("SharedInteger", 7);
            var native = (DependencyProperty)property.Value;
            var probe = new Probe();
            probe.SetValue(property, 9);
            Assert.AreEqual(9, probe.GetValue(native));
            probe.SetValue(native, 11);
            Assert.AreEqual(11, probe.GetValue(property));
            probe.SetCurrentValue(property, 13);
            Assert.AreEqual(13, probe.GetValue(native));
            Assert.IsTrue(DependencyPropertyHelper.GetValueSource(probe, native).IsCurrent);
            probe.ClearValue(property);
            Assert.AreEqual(7, probe.GetValue(property));
            Assert.AreSame(DependencyProperty.UnsetValue, probe.ReadLocalValue(native));
            probe.SetCurrentValue(native, 15);
            Assert.AreEqual(15, probe.GetValue(property));
            probe.ClearValue(native);
            Assert.AreEqual(7, probe.GetValue(property));
        });

    [TestMethod]
    public void NullAndNullableValuesUseNativeStorage() =>
        PlatformThread.Run(() =>
        {
            var reference = PropertyFactory.Register<Probe, string?>("NullableText", "initial");
            var nullable = PropertyFactory.Register<Probe, int?>("NullableNumber", 3);
            var probe = new Probe();
            probe.SetValue(reference, null);
            probe.SetCurrentValue(nullable, null);
            Assert.IsNull(probe.GetValue(reference));
            Assert.IsNull(probe.GetValue(nullable));
            probe.SetValue(nullable, 5);
            Assert.AreEqual(5, probe.GetValue(nullable));
            probe.ClearValue(reference);
            probe.ClearValue(nullable);
            Assert.AreEqual("initial", probe.GetValue(reference));
            Assert.AreEqual(3, probe.GetValue(nullable));
        });

    [TestMethod]
    public void EveryTypedOperationRejectsNullHandles() =>
        PlatformThread.Run(() =>
        {
            var probe = new Probe();
            Core.CommonDependencyProperty<int> property = null!;
            Assert.AreEqual(
                "property",
                Assert
                    .ThrowsExactly<ArgumentNullException>(() => probe.GetValue(property))
                    .ParamName
            );
            Assert.AreEqual(
                "property",
                Assert
                    .ThrowsExactly<ArgumentNullException>(() => probe.SetValue(property, 1))
                    .ParamName
            );
            Assert.AreEqual(
                "property",
                Assert
                    .ThrowsExactly<ArgumentNullException>(() => probe.ClearValue(property))
                    .ParamName
            );
            Assert.AreEqual(
                "property",
                Assert
                    .ThrowsExactly<ArgumentNullException>(() => probe.SetCurrentValue(property, 1))
                    .ParamName
            );
        });

    [TestMethod]
    public void EveryTypedOperationRejectsForeignAndMismatchedHandles() =>
        PlatformThread.Run(() =>
        {
            var native = DependencyProperty.Register(
                "MismatchedText",
                typeof(string),
                typeof(Probe)
            );
            var probe = new Probe();
            foreach (
                var property in new[]
                {
                    new Core.CommonDependencyProperty<object>(new object()),
                    new Core.CommonDependencyProperty<object>(native),
                }
            )
            {
                Assert.AreEqual(
                    "property",
                    Assert
                        .ThrowsExactly<ArgumentException>(() => probe.GetValue(property))
                        .ParamName
                );
                Assert.AreEqual(
                    "property",
                    Assert
                        .ThrowsExactly<ArgumentException>(() => probe.SetValue(property, "text"))
                        .ParamName
                );
                Assert.AreEqual(
                    "property",
                    Assert
                        .ThrowsExactly<ArgumentException>(() => probe.ClearValue(property))
                        .ParamName
                );
                Assert.AreEqual(
                    "property",
                    Assert
                        .ThrowsExactly<ArgumentException>(() =>
                            probe.SetCurrentValue(property, "text")
                        )
                        .ParamName
                );
            }
            var wrongValueType = new Core.CommonDependencyProperty<int>(native);
            Assert.ThrowsExactly<ArgumentException>(() => probe.GetValue(wrongValueType));
            Assert.ThrowsExactly<ArgumentException>(() => probe.SetValue(wrongValueType, 1));
            Assert.ThrowsExactly<ArgumentException>(() => probe.ClearValue(wrongValueType));
            Assert.ThrowsExactly<ArgumentException>(() => probe.SetCurrentValue(wrongValueType, 1));
        });

    [TestMethod]
    public void WrappedExistingPropertyPreservesValidationAndCoercion() =>
        PlatformThread.Run(() =>
        {
            var native = DependencyProperty.Register(
                "ValidatedInteger",
                typeof(int),
                typeof(Probe),
                new PropertyMetadata(2, null, (_, value) => Math.Min((int)value, 10)),
                value => (int)value >= 0
            );
            var property = new Core.CommonDependencyProperty<int>(native);
            var probe = new Probe();
            probe.SetValue(property, 20);
            Assert.AreEqual(10, probe.GetValue(property));
            Assert.ThrowsExactly<ArgumentException>(() => probe.SetValue(property, -1));
            Assert.ThrowsExactly<ArgumentException>(() => probe.SetCurrentValue(property, -1));
            probe.ClearValue(property);
            Assert.AreEqual(2, probe.GetValue(property));
        });

    [TestMethod]
    public void DirectTypedAccessDoesNotRemapNativeSentinels() =>
        PlatformThread.Run(() =>
        {
            var property = PropertyFactory.Register<Probe, object?>("NativeSentinels", "fallback");
            var probe = new Probe();
            probe.SetValue(property, System.Windows.Data.Binding.DoNothing);
            Assert.AreSame(System.Windows.Data.Binding.DoNothing, probe.GetValue(property));
            probe.SetValue(property, DependencyProperty.UnsetValue);
            Assert.AreEqual("fallback", probe.GetValue(property));
            probe.SetValue(property, Core.ConverterResults.UnsetValue);
            Assert.AreSame(Core.ConverterResults.UnsetValue, probe.GetValue(property));
        });

    [TestMethod]
    public void ClosedGenericConvertersRegisterIndependentNativeProperties() =>
        PlatformThread.Run(() =>
        {
            var integer = (DependencyProperty)BoolToValueConverter<int>.TrueValueProperty.Value;
            var text = (DependencyProperty)BoolToValueConverter<string>.TrueValueProperty.Value;
            Assert.AreNotSame(integer, text);
            Assert.AreEqual(typeof(int), integer.PropertyType);
            Assert.AreEqual(typeof(string), text.PropertyType);
            Assert.AreEqual(typeof(BoolToValueConverter<int>), integer.OwnerType);
            Assert.AreEqual(typeof(BoolToValueConverter<string>), text.OwnerType);
            Assert.AreEqual(0, new BoolToValueConverter<int>().TrueValue);
            Assert.IsNull(new BoolToValueConverter<string>().TrueValue);
        });

    [TestMethod]
    public void RegistrationRetainsNativeNameAndDefaultValidation() =>
        PlatformThread.Run(() =>
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                PropertyFactory.Register<Probe, int>(null!)
            );
            Assert.ThrowsExactly<ArgumentException>(() =>
                PropertyFactory.Register<Probe, int>(string.Empty)
            );
            Assert.ThrowsExactly<ArgumentException>(() =>
                PropertyFactory.Register<Probe, object>(
                    "InvalidDefault",
                    DependencyProperty.UnsetValue
                )
            );
        });

    [TestMethod]
    public void TypedOperationsRetainDispatcherThreadChecks() =>
        PlatformThread.Run(() =>
        {
            var property = PropertyFactory.Register<Probe, int>("ThreadInteger");
            var probe = new Probe();
            Task.Run(() =>
                {
                    Assert.ThrowsExactly<InvalidOperationException>(() => probe.GetValue(property));
                    Assert.ThrowsExactly<InvalidOperationException>(() =>
                        probe.SetValue(property, 1)
                    );
                    Assert.ThrowsExactly<InvalidOperationException>(() =>
                        probe.ClearValue(property)
                    );
                    Assert.ThrowsExactly<InvalidOperationException>(() =>
                        probe.SetCurrentValue(property, 1)
                    );
                })
                .GetAwaiter()
                .GetResult();
        });

    private sealed class Probe() : ValueConverterBase(new Core.StringCaseConverter());
}
