using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class MultiValueConverterBaseTests
{
    [TestMethod]
    public void ForwardBridge_MapsOnlyTopLevelSentinelsAndPreservesCallerArray() =>
        PlatformThread.Run(() =>
        {
            var identity = new object();
            var nested = new object[] { DependencyProperty.UnsetValue };
            var values = new object?[]
            {
                DependencyProperty.UnsetValue,
                Binding.DoNothing,
                identity,
                null,
                nested,
            };
            var snapshot = values.ToArray();
            var core = new CaptureConverter();
            var converter = new Bridge(core)
            {
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            Assert.AreSame(
                Binding.DoNothing,
                converter.Convert(
                    values,
                    typeof(string),
                    DependencyProperty.UnsetValue,
                    CultureInfo.InvariantCulture
                )
            );
            CollectionAssert.AreEqual(snapshot, values);
            Assert.AreNotSame(values, core.Values);
            Assert.AreSame(Core.ConverterResults.UnsetValue, core.Values![0]);
            Assert.AreSame(Core.ConverterResults.DoNothing, core.Values[1]);
            Assert.AreSame(identity, core.Values[2]);
            Assert.IsNull(core.Values[3]);
            Assert.AreSame(nested, core.Values[4]);
            Assert.AreSame(DependencyProperty.UnsetValue, nested[0]);
            Assert.AreSame(Core.ConverterResults.UnsetValue, core.Parameter);
            Assert.AreEqual(typeof(string), core.TargetType);
            Assert.AreSame(CultureInfo.InvariantCulture, core.Culture);
        });

    [TestMethod]
    public void ReverseBridge_MapsEveryResultSlotWithoutRecursingIntoObjects() =>
        PlatformThread.Run(() =>
        {
            var identity = new object();
            var nested = new object[] { Core.ConverterResults.UnsetValue };
            var result = new object[]
            {
                Core.ConverterResults.UnsetValue,
                Core.ConverterResults.DoNothing,
                identity,
                null!,
                nested,
            };
            var core = new CaptureConverter { ReverseResult = result };
            var converter = new Bridge(core)
            {
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            var targets = new[]
            {
                typeof(string),
                typeof(int),
                typeof(object),
                typeof(object),
                typeof(object[]),
            };
            var actual = converter.ConvertBack(
                DependencyProperty.UnsetValue,
                targets,
                Binding.DoNothing,
                CultureInfo.InvariantCulture
            );
            Assert.AreSame(Core.ConverterResults.UnsetValue, core.Value);
            Assert.AreSame(Core.ConverterResults.DoNothing, core.Parameter);
            Assert.AreSame(targets, core.TargetTypes);
            Assert.AreSame(CultureInfo.InvariantCulture, core.Culture);
            Assert.AreNotSame(result, actual);
            Assert.AreSame(DependencyProperty.UnsetValue, actual[0]);
            Assert.AreSame(Binding.DoNothing, actual[1]);
            Assert.AreSame(identity, actual[2]);
            Assert.IsNull(actual[3]);
            Assert.AreSame(nested, actual[4]);
            Assert.AreSame(Core.ConverterResults.UnsetValue, nested[0]);
            Assert.AreSame(Core.ConverterResults.UnsetValue, result[0]);
            Assert.AreSame(Core.ConverterResults.DoNothing, result[1]);
        });

    private sealed class Bridge(CaptureConverter core) : MultiValueConverterBase(core);

    private sealed class CaptureConverter : Core.MultiValueConverterBase
    {
        public IList<object?>? Values { get; private set; }
        public object? Value { get; private set; }
        public object? Parameter { get; private set; }
        public Type? TargetType { get; private set; }
        public IList<Type?>? TargetTypes { get; private set; }
        public CultureInfo? Culture { get; private set; }
        public object[] ReverseResult { get; init; } = [];

        protected override object? ConvertCore(
            IList<object?> values,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            Values = values;
            Parameter = parameter;
            TargetType = targetType;
            Culture = culture;
            return Core.ConverterResults.DoNothing;
        }

        protected override object[] ConvertBackCore(
            object? value,
            IList<Type?> targetTypes,
            object? parameter,
            CultureInfo culture
        )
        {
            Value = value;
            Parameter = parameter;
            TargetTypes = targetTypes;
            Culture = culture;
            return ReverseResult;
        }
    }
}
