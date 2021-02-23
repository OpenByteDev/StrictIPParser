using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace StrictIPParser {
    public static class IPAddressParser {
        public static IPAddress? TryParseStrict(ReadOnlySpan<char> text) {
            return TryParseV4Strict(text) ?? TryParseV6Strict(text);
        }
        public static bool TryParseStrict(ReadOnlySpan<char> text, [NotNullWhen(true)] out IPAddress? address) {
            return (address = TryParseStrict(text)) != null;
        }
        public static IPAddress? TryParseV4Strict(ReadOnlySpan<char> text) {
            Span<byte> addressBytes = stackalloc byte[IPv4Address.ByteCount];
            if (IPv4Address.TryParseInto(text, addressBytes)) {
                return new IPAddress(addressBytes);
            } else {
                return null;
            }
        }
        public static bool TryParseV4Strict(ReadOnlySpan<char> text, [NotNullWhen(true)] out IPAddress? address) {
            return (address = TryParseV4Strict(text)) != null;
        }
        public static IPAddress? TryParseV6Strict(ReadOnlySpan<char> text) {
            Span<byte> addressBytes = stackalloc byte[IPv6Address.ByteCount];
            if (IPv6Address.TryParseInto(text, addressBytes)) {
                return new IPAddress(addressBytes);
            } else {
                return null;
            }
        }
        public static bool TryParseV6Strict(ReadOnlySpan<char> text, [NotNullWhen(true)] out IPAddress? address) {
            return (address = TryParseV6Strict(text)) != null;
        }
    }
}
