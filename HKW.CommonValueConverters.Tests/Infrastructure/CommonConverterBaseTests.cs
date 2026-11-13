using HKW.CommonValueConverters;

namespace HKW.CommonValueConvertersTest;

[TestClass]
[DoNotParallelize]
public sealed class CommonConverterBaseTests
{
    [TestMethod]
    public void GlobalDefaultIsReadDynamically()
    {
        var saved = CommonConverterBase.GlobalDefaultResult;
        try
        {
            var converter = new StringCaseConverter();
            var fallback = new object();
            CommonConverterBase.GlobalDefaultResult = fallback;
            Assert.AreSame(fallback, converter.Convert(1, null, null, null));
        }
        finally
        {
            CommonConverterBase.GlobalDefaultResult = saved;
        }
    }
}
