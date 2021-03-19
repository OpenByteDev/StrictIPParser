using System;
using BenchmarkDotNet.Attributes;

namespace StrictIPParser.Benchmark {
    public class IPv6ParseBenchmark {
        public string[] Addresses = new string[1000];

        [GlobalSetup]
        public void Setup() {
            Random random = new();
            for (int i = 0; i < Addresses.Length; i++) {
                Addresses[i] = new IPv6Address(random.NextLong(), random.NextLong()).ToString();
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

    internal static class RandomExtensions {
        public static long NextLong(this Random random) {
            return random.Next() << 4 + random.Next();
        }
    }
}
