using System;
using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

namespace StrictIPParser {
    public readonly struct IPv4Address : IEquatable<IPv4Address>, IEquatable<IPAddress> {
        private readonly int value;

        public const int ByteCount = 4;
        public const int MaxAddressChars = 4 * 3 + 3;

        public static readonly IPv4Address None = (IPv4Address)IPAddress.None;
        public static readonly IPv4Address Any = (IPv4Address)IPAddress.Any;
        public static readonly IPv4Address Loopback = (IPv4Address)IPAddress.Loopback;
        public static readonly IPv4Address Broadcast = (IPv4Address)IPAddress.Broadcast;

        public IPv4Address(int value) {
            this.value = value;
        }
        public IPv4Address(ReadOnlySpan<byte> bytes) {
            if (bytes.Length < ByteCount) {
                throw new ArgumentException("The provided span has invalid length", nameof(bytes));
            }

            value = BinaryPrimitives.ReadInt32BigEndian(bytes);
        }
        public IPv4Address(IPAddress address) {
            if (address.AddressFamily != AddressFamily.InterNetwork) {
                throw new ArgumentException("Incompatible address family.", nameof(address));
            }

#pragma warning disable CS0618 // Type or member is obsolete
            // we use .Address because it is faster and we check the address family beforehand.
            value = (int)address.Address;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public byte[] GetAddressBytes() {
            byte[]? bytes = new byte[ByteCount];
            WriteBytesInto(bytes);
            return bytes;
        }

        public void WriteBytesInto(Span<byte> destination) {
            if (destination.Length < ByteCount) {
                throw new ArgumentException("Destination span is too short", nameof(destination));
            }

            BinaryPrimitives.WriteInt32BigEndian(destination, value);
        }

        public static IPv4Address Parse(ReadOnlySpan<char> text) {
            return TryParse(text, out IPv4Address address) ? address : throw new FormatException("An invalid IP address was specified.");
        }

        public static IPv4Address? TryParse(ReadOnlySpan<char> text) {
            return TryParse(text, out IPv4Address address) ? address : null;
        }

        public static bool TryParse(ReadOnlySpan<char> text, out IPv4Address address) {
            Span<byte> bytes = stackalloc byte[4];
            if (TryParseInto(text, bytes)) {
                address = new IPv4Address(bytes);
                return true;
            } else {
                address = default;
                return false;
            }
        }

        public static bool TryParseInto(ReadOnlySpan<char> text, Span<byte> span) {
            if (text.Length > 15 || span.Length < 4) {
                return false;
            }

            ReadOnlySpan<char> dot = ".".AsSpan();
            int nextDotIndex = -1;

            for (int blockIndex = 0; blockIndex < 4; blockIndex++) {
                text = text[(nextDotIndex + 1)..];

                if (blockIndex != 3) {
                    nextDotIndex = text.IndexOf(dot);

                    if (nextDotIndex == -1) {
                        return default;
                    }
                } else {
                    nextDotIndex = text.Length;
                }

                if (!TryParseBlock(text[..nextDotIndex], out int blockValue)) {
                    return default;
                }

                span[blockIndex] = (byte)blockValue;
            }

            return true;

            static bool TryParseBlock(ReadOnlySpan<char> block, out int value) {
                value = 0;

                if (block.Length == 0) {
                    return false;
                }

                foreach (char c in block) {
                    if (c < '0' || c > '9') {
                        return false;
                    }

                    value = value * 10 + c - '0';
                }

                return value <= 255;
            }
        }

        public override bool Equals(object? obj) {
            return obj switch {
                IPv4Address a => Equals(a),
                IPAddress b => Equals(b),
                _ => false
            };
        }

        public bool Equals(IPv4Address other) {
            return value == other.value;
        }

        public bool Equals(IPAddress? other) {
            if (other is null) {
                return false;
            }

            if (other.AddressFamily != AddressFamily.InterNetwork) {
                return false;
            }
#pragma warning disable CS0618 // Type or member is obsolete
            return value == other.Address;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public override int GetHashCode() {
            return value;
        }

        public override string ToString() {
            Span<char> addressString = stackalloc char[MaxAddressChars];
            int offset = 0;

            FormatBlock(value & 0xFF, addressString, ref offset);
            AddDot(addressString, ref offset);
            FormatBlock((value >> 8) & 0xFF, addressString, ref offset);
            AddDot(addressString, ref offset);
            FormatBlock((value >> 16) & 0xFF, addressString, ref offset);
            AddDot(addressString, ref offset);
            FormatBlock((value >> 24) & 0xFF, addressString, ref offset);

            return new string(addressString);

            static void FormatBlock(int block, Span<char> addressString, ref int offset) {
                offset += block >= 100 ? 3 : block >= 10 ? 2 : 1;

                int i = offset;
                do {
                    block = Math.DivRem(block, 10, out int rem);
                    addressString[--i] = (char)('0' + rem);
                } while (block != 0);
            }

            static void AddDot(Span<char> addressString, ref int offset) {
                addressString[offset] = '.';
                offset++;
            }
        }

        public static bool operator ==(IPv4Address left, IPv4Address right) {
            return left.Equals(right);
        }

        public static bool operator !=(IPv4Address left, IPv4Address right) {
            return !(left == right);
        }

        public static implicit operator IPAddress(IPv4Address address) {
            return new(address.value);
        }

        public static explicit operator IPv4Address(IPAddress address) {
            return new(address);
        }
    }
}
