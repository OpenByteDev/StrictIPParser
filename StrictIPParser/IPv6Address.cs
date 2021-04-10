using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using StrictIPParser.Extensions;

namespace StrictIPParser {
    public readonly struct IPv6Address : IEquatable<IPv6Address>, IEquatable<IPAddress?> {
        private readonly long upper;
        private readonly long lower;

        public const int ByteCount = sizeof(long) * 2;
        public const int MaxBlockChars = 4;
        public const int BlockCount = 8;
        public const int MinAddressChars = 2;
        public const int MaxAddressChars = (BlockCount * MaxBlockChars) + (BlockCount-1);

        public static readonly IPv6Address None = new(IPAddress.IPv6None);
        public static readonly IPv6Address Any = new(IPAddress.IPv6Any);
        public static readonly IPv6Address Loopback = new(IPAddress.IPv6Loopback);

        public IPv6Address(long upper, long lower) {
            this.upper = upper;
            this.lower = lower;
        }
        public IPv6Address(ReadOnlySpan<byte> bytes) {
            if (bytes.Length < ByteCount) {
                throw new ArgumentException("The provided span has invalid length", nameof(bytes));
            }

            upper = BinaryPrimitives.ReadInt64BigEndian(bytes[..sizeof(long)]);
            lower = BinaryPrimitives.ReadInt64BigEndian(bytes[sizeof(long)..]);
        }
        public IPv6Address(IPAddress address) {
            if (address.AddressFamily != AddressFamily.InterNetworkV6) {
                throw new ArgumentException("Incompatible address family.", nameof(address));
            }

            Span<byte> bytes = stackalloc byte[ByteCount];
            bool success = address.TryWriteBytes(bytes, out int bytesWritten);
            Debug.Assert(success && bytesWritten == ByteCount);

            upper = BinaryPrimitives.ReadInt64BigEndian(bytes[..sizeof(long)]);
            lower = BinaryPrimitives.ReadInt64BigEndian(bytes[sizeof(long)..]);
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

            BinaryPrimitives.WriteInt64BigEndian(destination[..sizeof(long)], upper);
            BinaryPrimitives.WriteInt64BigEndian(destination[sizeof(long)..], lower);
        }

        public static IPv6Address Parse(ReadOnlySpan<char> text) {
            return TryParse(text, out IPv6Address address) ? address : throw new FormatException("An invalid IP address was specified.");
        }

        public static IPv6Address? TryParse(ReadOnlySpan<char> text) {
            return TryParse(text, out IPv6Address address) ? address : null;
        }

        public static bool TryParse(ReadOnlySpan<char> text, out IPv6Address address) {
            Span<byte> bytes = stackalloc byte[ByteCount];
            if (TryParseInto(text, bytes)) {
                address = new IPv6Address(bytes);
                return true;
            } else {
                address = default;
                return false;
            }
        }

        public static bool TryParseInto(ReadOnlySpan<char> text, Span<byte> span) {
            if (text.Length < MinAddressChars || text.Length > MaxAddressChars || span.Length < ByteCount) {
                return false;
            }

            ReadOnlySpan<char> sep = ":".AsSpan();

            int emptyBlockIndex = -1;
            int byteIndex = 0;
            int blockValue;

            foreach (ReadOnlySpan<char> block in text.Split(sep)) {
                if (byteIndex >= ByteCount) {
                    return false;
                }

                if (block.IsEmpty) {
                    if (byteIndex == 0 || byteIndex == ByteCount - 2) {
                        blockValue = 0;
                    } else if (emptyBlockIndex == -1) {
                        // first empty block
                        emptyBlockIndex = byteIndex;
                        continue;
                    } else {
                        // more empty blocks -> invalid
                        return false;
                    }
                } else {
                    if (!TryParseBlock(block, out blockValue)) {
                        return false;
                    }
                }

                BinaryPrimitives.WriteUInt16BigEndian(span[byteIndex..], (ushort)blockValue);
                byteIndex += 2;
            }

            if (byteIndex != ByteCount) {
                if (emptyBlockIndex == -1) {
                    return false; // not enough blocks
                }

                // TODO? could be avoided if we search for the empty block beforehand
                span[emptyBlockIndex..byteIndex].CopyTo(span[^(byteIndex - emptyBlockIndex)..]);
                span[emptyBlockIndex..^(byteIndex - emptyBlockIndex)].Fill(0);
            }

            return true;

            static bool TryParseBlock(ReadOnlySpan<char> block, out int value) {
                value = 0;

                if (block.Length == 0) {
                    return false;
                }

                foreach (char c in block) {
                    value *= 16;

                    switch (c) {
                        case >= '0' and <= '9':
                            value += c - '0';
                            break;
                        case >= 'a' and <= 'f':
                            value += c - 'a' + 10;
                            break;
                        case >= 'A' and <= 'F':
                            value += c - 'A' + 10;
                            break;
                        default:
                            return false;
                    }
                }

                return value <= ushort.MaxValue;
            }
        }

        public override bool Equals(object? obj) {
            return obj switch {
                IPv6Address a => Equals(a),
                IPAddress b => Equals(b),
                _ => false
            };
        }

        public bool Equals(IPv6Address other) {
            return upper == other.upper || lower == other.lower;
        }

        public bool Equals(IPAddress? other) {
            if (other is null) {
                return false;
            }

            if (other.AddressFamily != AddressFamily.InterNetworkV6) {
                return false;
            }

            Span<byte> bytes = stackalloc byte[ByteCount];
            if (!other.TryWriteBytes(bytes, out int _)) {
                return false;
            }

            return this == new IPv6Address(bytes);
        }

        public override int GetHashCode() {
            return HashCode.Combine(upper, lower);
        }

        public bool TryFormat(Span<char> destination, out int charsWritten) {
            return ((IPAddress)this).TryFormat(destination, out charsWritten);
        }

        public override string ToString() {
            return ((IPAddress)this).ToString();
        }

        public static bool operator ==(IPv6Address left, IPv6Address right) {
            return left.Equals(right);
        }

        public static bool operator !=(IPv6Address left, IPv6Address right) {
            return !(left == right);
        }

        public static bool operator ==(IPv6Address left, IPAddress right) {
            return left.Equals(right);
        }

        public static bool operator !=(IPv6Address left, IPAddress right) {
            return !(left == right);
        }

        public static implicit operator IPAddress(IPv6Address address) {
            Span<byte> value = stackalloc byte[ByteCount];
            address.WriteBytesInto(value);
            return new IPAddress(value);
        }
        public static explicit operator IPv6Address(IPAddress address) {
            return new(address);
        }
    }
}
