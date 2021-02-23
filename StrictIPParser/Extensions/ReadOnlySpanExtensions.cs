using System;

namespace StrictIPParser.Extensions {
    internal static class ReadOnlySpanExtensions {
        public static SplitEnumerator Split(this ReadOnlySpan<char> text, ReadOnlySpan<char> seperator) {
            return new SplitEnumerator(text, seperator);
        }

        public ref struct SplitEnumerator {
            private ReadOnlySpan<char> text;
            private readonly ReadOnlySpan<char> seperator;

            public ReadOnlySpan<char> Current { get; private set; }

            public SplitEnumerator(ReadOnlySpan<char> text, ReadOnlySpan<char> seperator) {
                this.text = text;
                this.seperator = seperator;
                Current = default;
            }

            public SplitEnumerator GetEnumerator() {
                return this;
            }

            public bool MoveNext() {
                if (text.Length == 0) {
                    return false;
                }

                int index = text.IndexOf(seperator);
                if (index == -1) {
                    Current = text;
                    text = ReadOnlySpan<char>.Empty;
                } else {
                    Current = text[0..index];
                    text = text[(index + 1)..];
                }
                return true;
            }
        }
    }
}
