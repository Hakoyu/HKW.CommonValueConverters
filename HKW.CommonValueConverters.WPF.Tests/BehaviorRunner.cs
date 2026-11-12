namespace HKW.CommonValueConverters.WPF.Tests;

[AttributeUsage(AttributeTargets.Method)]
internal sealed class ConverterIdentityAttribute(Type type) : Attribute
{
    public Type Type { get; } = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
}

internal static class BehaviorRunner
{
    private static readonly Type[] Reversible =
    [
        typeof(DateTimeToStringConverter),
        typeof(DateTimeOffsetToStringConverter),
        typeof(DebugConverter),
        typeof(EnumToEnumInfoConverter),
        typeof(GuidToStringConverter),
        typeof(StringToNumberConverter),
        typeof(TimeSpanToStringConverter),
        typeof(MediaColorToSolidColorBrushConverter),
        typeof(SolidColorBrushToMediaColorConverter),
    ];

    public static void Verify(string id) =>
        PlatformThread.Run(() =>
        {
            var item = ConverterTestCases.Find(id);
            var converter = item.Factory();
            Assert.AreEqual(item.Type, converter.GetType());
            for (var index = 0; index < item.Inputs.Length; index++)
            {
                var input = item.Inputs[index];
                var snapshot = input.ToArray();
                var actual = converter is ValueConverterBase single
                    ? single.Convert(
                        input[0],
                        typeof(object),
                        item.Parameter,
                        ConverterTestCases.Culture
                    )
                    : ((MultiValueConverterBase)converter).Convert(
                        input,
                        typeof(object),
                        item.Parameter,
                        ConverterTestCases.Culture
                    );
                ConverterTestCases.AssertResult(item.Expected[index], actual);
                CollectionAssert.AreEqual(snapshot, input);
            }
            if (converter is MultiValueConverterBase multi)
                Assert.ThrowsExactly<NotSupportedException>(() =>
                    multi.ConvertBack(null, [typeof(object)], null, ConverterTestCases.Culture)
                );
            else if (Reversible.Contains(item.Type) is false)
                Assert.ThrowsExactly<NotSupportedException>(() =>
                    ((ValueConverterBase)converter).ConvertBack(
                        null,
                        typeof(object),
                        null,
                        ConverterTestCases.Culture
                    )
                );
        });
}
