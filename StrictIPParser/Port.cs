using System;
using System.Globalization;

namespace StrictIPParser {
    public readonly struct Port : IEquatable<Port>, IEquatable<int>, IEquatable<ushort> {
        public readonly ushort Value { get; }

        public const int Max = ushort.MaxValue;
        public const int ByteCount = sizeof(ushort);
        public const int MinChars = 1;
        public const int MaxChars = 5;

        public Port(ushort value) {
            Value = value;
        }
        public Port(int value) {
            if (value > Max || value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} has to be non-negative and smaller than {Max}");

            Value = (ushort)value;
        }

        public static Port Parse(ReadOnlySpan<char> text) {
            return TryParse(text, out Port address) ? address : throw new FormatException("An invalid port was specified.");
        }

        public static Port? TryParse(ReadOnlySpan<char> text) {
            return TryParse(text, out Port address) ? address : null;
        }

        public static bool TryParse(ReadOnlySpan<char> text, out Port port) {
            if (text.Length > MaxChars || text.Length < MinChars) {
                port = default;
                return false;
            }

            if (ushort.TryParse(text, NumberStyles.None, null, out var result)) {
                port = new Port(result);
                return true;
            } else {
                port = default;
                return false;
            }
        }

        public override bool Equals(object? obj) {
            return obj switch {
                Port a => Equals(a),
                int b => Equals(b),
                ushort c => Equals(c),
                _ => false
            };
        }

        public bool Equals(Port other) {
            return Value == other.Value;
        }

        public bool Equals(int other) {
            return Value == other;
        }

        public bool Equals(ushort other) {
            return Value == other;
        }

        public override int GetHashCode() {
            return Value;
        }

        public bool TryFormat(Span<char> destination, out int charsWritten) {
            return Value.TryFormat(destination, out charsWritten);
        }

        public override string ToString() {
            return Value.ToString();
        }

        public static bool operator ==(Port left, Port right) {
            return left.Equals(right);
        }

        public static bool operator !=(Port left, Port right) {
            return !(left == right);
        }

        public static bool operator ==(Port left, ushort right) {
            return left.Equals(right);
        }

        public static bool operator !=(Port left, ushort right) {
            return !(left == right);
        }

        public static bool operator ==(Port left, int right) {
            return left.Equals(right);
        }

        public static bool operator !=(Port left, int right) {
            return !(left == right);
        }

        public static implicit operator ushort(Port port) {
            return port.Value;
        }

        public static implicit operator int(Port port) {
            return port.Value;
        }

        public static implicit operator Port(ushort port) {
            return new Port(port);
        }

        public static explicit operator Port(int port) {
            return new Port(port);
        }
    }
}
