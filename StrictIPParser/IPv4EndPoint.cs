using System;
using System.Net;
using System.Net.Sockets;

namespace StrictIPParser {
    public readonly struct IPv4EndPoint : IEquatable<IPv4EndPoint>, IEquatable<IPEndPoint> {
        private readonly IPv4Address Address { get; }
        private readonly Port Port { get; }

        public const int ByteCount = IPv4Address.ByteCount + Port.ByteCount;
        public const int MinChars = IPv4Address.MinAddressChars + Port.MinChars + 1;
        public const int MaxChars = IPv4Address.MaxAddressChars + Port.MaxChars + 1;

        public IPv4EndPoint(IPv4Address address, Port port) {
            Address = address;
            Port = port;
        }
        public IPv4EndPoint(IPEndPoint endPoint) {
            if (endPoint.AddressFamily != AddressFamily.InterNetwork) {
                throw new ArgumentException("Incompatible address family.", nameof(endPoint));
            }

            Address = new IPv4Address(endPoint.Address);
            Port = new Port(endPoint.Port);
        }

        public static IPv4EndPoint Parse(ReadOnlySpan<char> text) {
            return TryParse(text, out IPv4EndPoint endPoint) ? endPoint : throw new FormatException("An invalid IP address or port was specified.");
        }

        public static IPv4EndPoint? TryParse(ReadOnlySpan<char> text) {
            return TryParse(text, out IPv4EndPoint endPoint) ? endPoint : null;
        }

        public static bool TryParse(ReadOnlySpan<char> text, out IPv4EndPoint endPoint) {
            if (text.Length < MinChars || text.Length > MaxChars) {
                endPoint = default;
                return false;
            }

            int separatorIndex = text.LastIndexOf(':');
            if (IPv4Address.TryParse(text[..separatorIndex], out IPv4Address address) &&
                Port.TryParse(text[(separatorIndex + 1)..], out Port port)) {
                endPoint = new IPv4EndPoint(address, port);
                return true;
            } else {
                endPoint = default;
                return false;
            }
        }

        public override bool Equals(object? obj) {
            return obj switch {
                IPv4EndPoint a => Equals(a),
                IPEndPoint b => Equals(b),
                _ => false
            };
        }

        public bool Equals(IPv4EndPoint other) {
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

            // address
            if (!Address.TryFormat(destination[charsWritten..], out int addressChars))
                return false;
            charsWritten += addressChars;

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

        public static bool operator ==(IPv4EndPoint left, IPv4EndPoint right) {
            return left.Equals(right);
        }

        public static bool operator !=(IPv4EndPoint left, IPv4EndPoint right) {
            return !(left == right);
        }

        public static bool operator ==(IPv4EndPoint left, IPEndPoint right) {
            return left.Equals(right);
        }

        public static bool operator !=(IPv4EndPoint left, IPEndPoint right) {
            return !(left == right);
        }

        public static implicit operator IPEndPoint(IPv4EndPoint endPoint) {
            return new(endPoint.Address, endPoint.Port);
        }

        public static explicit operator IPv4EndPoint(IPEndPoint endPoint) {
            return new(endPoint);
        }
    }
}
