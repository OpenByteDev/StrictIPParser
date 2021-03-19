using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace StrictIPParser {
    public static class StrictIPEndPointParser {
        public static IPEndPoint? TryParse(ReadOnlySpan<char> text) {
            return TryParseV4(text) ?? TryParseV6(text);
        }
        public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out IPEndPoint? endPoint) {
            return (endPoint = TryParse(text)) != null;
        }
        public static IPEndPoint? TryParseV4(ReadOnlySpan<char> text) {
            return IPv4EndPoint.TryParse(text);
        }
        public static bool TryParseV4(ReadOnlySpan<char> text, [NotNullWhen(true)] out IPEndPoint? endPoint) {
            return (endPoint = TryParseV4(text)) != null;
        }
        public static IPEndPoint? TryParseV6(ReadOnlySpan<char> text) {
            return IPv6EndPoint.TryParse(text);
        }
        public static bool TryParseV6(ReadOnlySpan<char> text, [NotNullWhen(true)] out IPEndPoint? endPoint) {
            return (endPoint = TryParseV6(text)) != null;
        }
    }
}
