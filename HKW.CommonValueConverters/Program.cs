namespace HKW.CommonValueConverters;

internal class Program
{
    static void Main(string[] args)
    {
        var c = new NumberCompareXConverter<int>();
        var r = c.Convert(10, null, ">10", null);
        //Console.WriteLine("Hello, World!");
    }
}
