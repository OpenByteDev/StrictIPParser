using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace StrictIPParser {
    public static class StrictIPAddressParser {
        public static IPAddress? TryParse(ReadOnlySpan<char> text) {
            return TryParseV4(text) ?? TryParseV6(text);
        }
        public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out IPAddress? address) {
            return (address = TryParse(text)) != null;
        }
        public static IPAddress? TryParseV4(ReadOnlySpan<char> text) {
            Span<byte> addressBytes = stackalloc byte[IPv4Address.ByteCount];
            if (IPv4Address.TryParseInto(text, addressBytes)) {
                return new IPAddress(addressBytes);
            } else {
                return null;
            }
        }
        public static bool TryParseV4(ReadOnlySpan<char> text, [NotNullWhen(true)] out IPAddress? address) {
            return (address = TryParseV4(text)) != null;
        }
        public static IPAddress? TryParseV6(ReadOnlySpan<char> text) {
            Span<byte> addressBytes = stackalloc byte[IPv6Address.ByteCount];
            if (IPv6Address.TryParseInto(text, addressBytes)) {
                return new IPAddress(addressBytes);
            } else {
                return null;
            }
        }
        public static bool TryParseV6(ReadOnlySpan<char> text, [NotNullWhen(true)] out IPAddress? address) {
            return (address = TryParseV6(text)) != null;
        }
    }
}
