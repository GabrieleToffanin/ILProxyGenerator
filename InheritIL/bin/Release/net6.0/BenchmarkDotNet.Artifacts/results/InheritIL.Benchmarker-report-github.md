``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
Intel Core i5-8500 CPU 3.00GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK=6.0.300
  [Host]     : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT


```
|               Method |     Mean |     Error |    StdDev |  Gen 0 | Allocated |
|--------------------- |---------:|----------:|----------:|-------:|----------:|
| CreateWithReflection | 7.567 ns | 0.1698 ns | 0.1505 ns | 0.0085 |      40 B |
|       CreateNormally | 3.936 ns | 0.0893 ns | 0.0835 ns | 0.0051 |      24 B |
