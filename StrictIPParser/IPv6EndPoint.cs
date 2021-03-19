using System;
using System.Net;
using System.Net.Sockets;

namespace StrictIPParser {
    public readonly struct IPv6EndPoint : IEquatable<IPv6EndPoint>, IEquatable<IPEndPoint> {
        private readonly IPv6Address Address { get; }
        private readonly Port Port { get; }

        public const int ByteCount = IPv6Address.ByteCount + Port.ByteCount;
        public const int MaxChars = IPv6Address.MaxAddressChars + Port.MaxChars;

        public IPv6EndPoint(IPv6Address address, Port port) {
            Address = address;
            Port = port;
        }
        public IPv6EndPoint(IPEndPoint endPoint) {
            if (endPoint.AddressFamily != AddressFamily.InterNetwork) {
                throw new ArgumentException("Incompatible address family.", nameof(endPoint));
            }

            Address = new IPv6Address(endPoint.Address);
            Port = new Port(endPoint.Port);
        }

        public static IPv6EndPoint Parse(ReadOnlySpan<char> text) {
            return TryParse(text, out IPv6EndPoint endPoint) ? endPoint : throw new FormatException("An invalid IP address or port was specified.");
        }

        public static IPv6EndPoint? TryParse(ReadOnlySpan<char> text) {
            return TryParse(text, out IPv6EndPoint endPoint) ? endPoint : null;
        }

        public static bool TryParse(ReadOnlySpan<char> text, out IPv6EndPoint endPoint) {
            if (text.Length > MaxChars || text.Length == 0) {
                endPoint = default;
                return false;
            }

            if (text[0] != '[') {
                endPoint = default;
                return false;
            }

            int separatorIndex = text.LastIndexOf(':');

            if (text[separatorIndex - 1] != ']') {
                endPoint = default;
                return false;
            }

            if (IPv6Address.TryParse(text[1..(separatorIndex - 1)], out IPv6Address address) &&
                Port.TryParse(text[(separatorIndex + 1)..], out Port port)) {
                endPoint = new IPv6EndPoint(address, port);
                return true;
            } else {
                endPoint = default;
                return false;
            }
        }

        public override bool Equals(object? obj) {
            return obj switch {
                IPv6EndPoint a => Equals(a),
                IPEndPoint b => Equals(b),
                _ => false
            };
        }

        public bool Equals(IPv6EndPoint other) {
            return Address == other.Address && Port == other.Port;
        }

        public bool Equals(IPEndPoint? other) {
            return other != null && Address == other.Address && Port == other.Port;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Address, Port);
        }

        public bool TryFormat(Span<char> destination, out int charsWritten) {
            charsWritten = 0;

            // [
            if (!WriteChar(destination, '[', ref charsWritten))
                return false;

            // address
            if (!Address.TryFormat(destination[charsWritten..], out int addressChars))
                return false;
            charsWritten += addressChars;

            // ]
            if (!WriteChar(destination, ']', ref charsWritten))
                return false;

            // :
            if (!WriteChar(destination, ':', ref charsWritten))
                return false;

            // port
            if (!Port.TryFormat(destination[charsWritten..], out int portChars))
                return false;
            charsWritten += portChars;

            return true;

            static bool WriteChar(Span<char> destination, char c, ref int charsWritten) {
                if (charsWritten + 1 >= destination.Length)
                    return false;
                destination[charsWritten] = c;
                charsWritten++;
                return true;
            }
        }

        public override string ToString() {
            Span<char> endPointString = stackalloc char[MaxChars];
            TryFormat(endPointString, out int charsWritten);
            return endPointString[..charsWritten].ToString();
        }

        public static bool operator ==(IPv6EndPoint left, IPv6EndPoint right) {
            return left.Equals(right);
        }

        public static bool operator !=(IPv6EndPoint left, IPv6EndPoint right) {
            return !(left == right);
        }

        public static bool operator ==(IPv6EndPoint left, IPEndPoint right) {
            return left.Equals(right);
        }

        public static bool operator !=(IPv6EndPoint left, IPEndPoint right) {
            return !(left == right);
        }

        public static implicit operator IPEndPoint(IPv6EndPoint endPoint) {
            return new(endPoint.Address, endPoint.Port);
        }

        public static explicit operator IPv6EndPoint(IPEndPoint endPoint) {
            return new(endPoint);
        }
    }
}
