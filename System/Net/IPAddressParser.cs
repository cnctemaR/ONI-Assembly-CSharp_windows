using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Net
{
	internal class IPAddressParser
	{
		internal unsafe static IPAddress Parse(ReadOnlySpan<char> ipSpan, bool tryParse)
		{
			long num2;
			if (ipSpan.Contains(':'))
			{
				ushort* ptr = stackalloc ushort[(UIntPtr)16];
				new Span<ushort>((void*)ptr, 8).Clear();
				uint num;
				if (IPAddressParser.Ipv6StringToAddress(ipSpan, ptr, 8, out num))
				{
					return new IPAddress(ptr, 8, num);
				}
			}
			else if (IPAddressParser.Ipv4StringToAddress(ipSpan, out num2))
			{
				return new IPAddress(num2);
			}
			if (tryParse)
			{
				return null;
			}
			throw new FormatException("An invalid IP address was specified.", new SocketException(SocketError.InvalidArgument));
		}

		internal unsafe static string IPv4AddressToString(uint address)
		{
			char* ptr = stackalloc char[(UIntPtr)30];
			int num = IPAddressParser.IPv4AddressToStringHelper(address, ptr);
			return new string(ptr, 0, num);
		}

		internal unsafe static void IPv4AddressToString(uint address, StringBuilder destination)
		{
			char* ptr = stackalloc char[(UIntPtr)30];
			int num = IPAddressParser.IPv4AddressToStringHelper(address, ptr);
			destination.Append(ptr, num);
		}

		internal unsafe static bool IPv4AddressToString(uint address, Span<char> formatted, out int charsWritten)
		{
			if (formatted.Length < 15)
			{
				charsWritten = 0;
				return false;
			}
			fixed (char* reference = MemoryMarshal.GetReference<char>(formatted))
			{
				char* ptr = reference;
				charsWritten = IPAddressParser.IPv4AddressToStringHelper(address, ptr);
			}
			return true;
		}

		private unsafe static int IPv4AddressToStringHelper(uint address, char* addressString)
		{
			int num = 0;
			IPAddressParser.FormatIPv4AddressNumber((int)(address & 255U), addressString, ref num);
			addressString[num++] = '.';
			IPAddressParser.FormatIPv4AddressNumber((int)((address >> 8) & 255U), addressString, ref num);
			addressString[num++] = '.';
			IPAddressParser.FormatIPv4AddressNumber((int)((address >> 16) & 255U), addressString, ref num);
			addressString[num++] = '.';
			IPAddressParser.FormatIPv4AddressNumber((int)((address >> 24) & 255U), addressString, ref num);
			return num;
		}

		internal static string IPv6AddressToString(ushort[] address, uint scopeId)
		{
			return StringBuilderCache.GetStringAndRelease(IPAddressParser.IPv6AddressToStringHelper(address, scopeId));
		}

		internal static bool IPv6AddressToString(ushort[] address, uint scopeId, Span<char> destination, out int charsWritten)
		{
			StringBuilder stringBuilder = IPAddressParser.IPv6AddressToStringHelper(address, scopeId);
			if (destination.Length < stringBuilder.Length)
			{
				StringBuilderCache.Release(stringBuilder);
				charsWritten = 0;
				return false;
			}
			stringBuilder.CopyTo(0, destination, stringBuilder.Length);
			charsWritten = stringBuilder.Length;
			StringBuilderCache.Release(stringBuilder);
			return true;
		}

		internal static StringBuilder IPv6AddressToStringHelper(ushort[] address, uint scopeId)
		{
			StringBuilder stringBuilder = StringBuilderCache.Acquire(65);
			if (IPv6AddressHelper.ShouldHaveIpv4Embedded(address))
			{
				IPAddressParser.AppendSections(address, 0, 6, stringBuilder);
				if (stringBuilder[stringBuilder.Length - 1] != ':')
				{
					stringBuilder.Append(':');
				}
				IPAddressParser.IPv4AddressToString(IPAddressParser.ExtractIPv4Address(address), stringBuilder);
			}
			else
			{
				IPAddressParser.AppendSections(address, 0, 8, stringBuilder);
			}
			if (scopeId != 0U)
			{
				stringBuilder.Append('%').Append(scopeId);
			}
			return stringBuilder;
		}

		private unsafe static void FormatIPv4AddressNumber(int number, char* addressString, ref int offset)
		{
			offset += ((number > 99) ? 3 : ((number > 9) ? 2 : 1));
			int num = offset;
			do
			{
				int num2;
				number = Math.DivRem(number, 10, out num2);
				addressString[--num] = (char)(48 + num2);
			}
			while (number != 0);
		}

		public unsafe static bool Ipv4StringToAddress(ReadOnlySpan<char> ipSpan, out long address)
		{
			int length = ipSpan.Length;
			long num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(ipSpan))
			{
				num = IPv4AddressHelper.ParseNonCanonical(reference, 0, ref length, true);
			}
			if (num != -1L && length == ipSpan.Length)
			{
				address = (long)((((ulong)(-16777216) & (ulong)num) >> 24) | (ulong)((16711680L & num) >> 8) | (ulong)((ulong)(65280L & num) << 8) | (ulong)((ulong)(255L & num) << 24));
				return true;
			}
			address = 0L;
			return false;
		}

		public unsafe static bool Ipv6StringToAddress(ReadOnlySpan<char> ipSpan, ushort* numbers, int numbersLength, out uint scope)
		{
			int length = ipSpan.Length;
			bool flag;
			fixed (char* reference = MemoryMarshal.GetReference<char>(ipSpan))
			{
				flag = IPv6AddressHelper.IsValidStrict(reference, 0, ref length);
			}
			if (flag || length != ipSpan.Length)
			{
				string text = null;
				IPv6AddressHelper.Parse(ipSpan, numbers, 0, ref text);
				long num = 0L;
				if (!string.IsNullOrEmpty(text))
				{
					if (text.Length < 2)
					{
						scope = 0U;
						return false;
					}
					for (int i = 1; i < text.Length; i++)
					{
						char c = text[i];
						if (c < '0' || c > '9')
						{
							scope = 0U;
							return true;
						}
						num = num * 10L + (long)(c - '0');
						if (num > (long)((ulong)(-1)))
						{
							scope = 0U;
							return false;
						}
					}
				}
				scope = (uint)num;
				return true;
			}
			scope = 0U;
			return false;
		}

		private static void AppendSections(ushort[] address, int fromInclusive, int toExclusive, StringBuilder buffer)
		{
			ValueTuple<int, int> valueTuple = IPv6AddressHelper.FindCompressionRange(new ReadOnlySpan<ushort>(address, fromInclusive, toExclusive - fromInclusive));
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			bool flag = false;
			for (int i = fromInclusive; i < item; i++)
			{
				if (flag)
				{
					buffer.Append(':');
				}
				flag = true;
				IPAddressParser.AppendHex(address[i], buffer);
			}
			if (item >= 0)
			{
				buffer.Append("::");
				flag = false;
				fromInclusive = item2;
			}
			for (int j = fromInclusive; j < toExclusive; j++)
			{
				if (flag)
				{
					buffer.Append(':');
				}
				flag = true;
				IPAddressParser.AppendHex(address[j], buffer);
			}
		}

		private unsafe static void AppendHex(ushort value, StringBuilder buffer)
		{
			char* ptr = stackalloc char[(UIntPtr)8];
			int num = 4;
			do
			{
				int num2 = (int)(value % 16);
				value /= 16;
				ptr[(IntPtr)(--num) * 2] = ((num2 < 10) ? ((char)(48 + num2)) : ((char)(97 + (num2 - 10))));
			}
			while (value != 0);
			buffer.Append(ptr + num, 4 - num);
		}

		private static uint ExtractIPv4Address(ushort[] address)
		{
			return (uint)(((int)IPAddressParser.Reverse(address[7]) << 16) | (int)IPAddressParser.Reverse(address[6]));
		}

		private static ushort Reverse(ushort number)
		{
			return (ushort)(((number >> 8) & 255) | (((int)number << 8) & 65280));
		}

		private const int MaxIPv4StringLength = 15;
	}
}
