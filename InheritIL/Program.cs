using BenchmarkDotNet.Running;
using InheritIL;


//List<A> myItems = new List<A>();

//A myClass = new A() { Name = "Ciao" };

//UtilGen<A> util = new UtilGen<A>();


//myItems.Add((A)util.Direct());



//foreach (var item in myItems)
//{
//    Console.WriteLine(((A)item).Name);
//}



var summary = BenchmarkRunner.Run<Benchmarker>();

public class A
{

    public string Name { get; set; }
}

