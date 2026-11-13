using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

[TestClass]
public sealed class ConverterTestCoverageTests
{
    [TestMethod]
    public void PublicConvertersHaveActiveBehaviorCases()
    {
        var converters = ConverterTestCatalog.DiscoverConverters(
            typeof(CommonConverterBase).Assembly,
            type =>
                type.IsSubclassOf(typeof(ValueConverterBase))
                || type.IsSubclassOf(typeof(MultiValueConverterBase))
        );
        ConverterTestCatalog.Validate(
            converters,
            ConverterTestCatalog.Discover(typeof(ConverterTestCoverageTests).Assembly),
            [ConverterTestLayer.Behavior]
        );
    }
}
