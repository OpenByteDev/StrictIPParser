using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

namespace StrictIPParser.Benchmark {
    public static class Program {
        public static void Main(string[] args) {
            ManualConfig config = DefaultConfig.Instance
                .AddDiagnoser(MemoryDiagnoser.Default);
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }
}
