using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InheritIL
{
    [MemoryDiagnoser]
    public class Benchmarker
    {
        private UtilGen<A> _gen;

        [GlobalSetup]
        public void Setup()
        {
            _gen = new UtilGen<A>();
        }
        [Benchmark]
        public void CreateWithReflection() => _gen.Direct();

        [Benchmark]
        public void CreateNormally() => new A() { Name = "Ciao" };
    }
}
