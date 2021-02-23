using System;
using BenchmarkDotNet.Attributes;

namespace StrictIPParser.Benchmark {
    public class IPv4ParseBenchmark {
        public string[] Addresses = new string[1000];

        [GlobalSetup]
        public void Setup() {
            Random random = new Random();
            for (int i = 0; i < Addresses.Length; i++) {
                Addresses[i] = new IPv4Address(random.Next()).ToString();
            }
        }

        [Benchmark]
        public void TryParse() {
            foreach (string str in Addresses) {
                IPv4Address.TryParse(str);
            }
        }

        private readonly byte[] buffer = new byte[IPv4Address.ByteCount];
        [Benchmark]
        public void TryParseInto() {
            foreach (string str in Addresses) {
                IPv4Address.TryParseInto(str, buffer);
            }
        }
    }
}
