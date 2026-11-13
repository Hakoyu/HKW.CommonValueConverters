using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class MultiValueConverterBaseTests
{
    [TestMethod]
    public Task ForwardBridge_MapsOnlyTopLevelSentinelsAndPreservesCallerArray() =>
        PlatformThread.Run(() =>
        {
            var identity = new object();
            var nested = new object[] { AvaloniaProperty.UnsetValue };
            var values = new object?[]
            {
                AvaloniaProperty.UnsetValue,
                BindingOperations.DoNothing,
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
                BindingOperations.DoNothing,
                converter.Convert(
                    values,
                    typeof(string),
                    AvaloniaProperty.UnsetValue,
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
            Assert.AreSame(AvaloniaProperty.UnsetValue, nested[0]);
            Assert.AreSame(Core.ConverterResults.UnsetValue, core.Parameter);
            Assert.AreEqual(typeof(string), core.TargetType);
            Assert.AreSame(CultureInfo.InvariantCulture, core.Culture);
        });

    private sealed class Bridge(CaptureConverter core) : MultiValueConverterBase(core);

    private sealed class CaptureConverter : Core.MultiValueConverterBase
    {
        public IList<object?>? Values { get; private set; }
        public object? Parameter { get; private set; }
        public Type? TargetType { get; private set; }
        public CultureInfo? Culture { get; private set; }

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
    }
}
