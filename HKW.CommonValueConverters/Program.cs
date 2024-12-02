namespace HKW.CommonValueConverters;

internal class Program
{
    static void Main(string[] args)
    {
        var c = new StringFormatMultiConverter() { GetDefaultResult = () => 0 };
        var r = c.GetDefaultResult();
        //var r = c.Convert(new List<object>([111, 222, 333]).ToArray(), null, "{0} {1} {2}", null);

        //Console.WriteLine("Hello, World!");
    }
}
