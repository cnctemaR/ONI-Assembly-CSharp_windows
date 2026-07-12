using System;
using System.Buffers.Text;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
	internal static class Number
	{
		public unsafe static string FormatDecimal(decimal value, ReadOnlySpan<char> format, NumberFormatInfo info)
		{
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			global::System.Number.DecimalToNumber(value, ref numberBuffer);
			char* ptr = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)ptr, 32));
			if (c != '\0')
			{
				global::System.Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, info, true);
			}
			else
			{
				global::System.Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, info);
			}
			return valueStringBuilder.ToString();
		}

		public unsafe static bool TryFormatDecimal(decimal value, ReadOnlySpan<char> format, NumberFormatInfo info, Span<char> destination, out int charsWritten)
		{
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			global::System.Number.DecimalToNumber(value, ref numberBuffer);
			char* ptr = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)ptr, 32));
			if (c != '\0')
			{
				global::System.Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, info, true);
			}
			else
			{
				global::System.Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, info);
			}
			return valueStringBuilder.TryCopyTo(destination, out charsWritten);
		}

		private unsafe static void DecimalToNumber(decimal value, ref global::System.Number.NumberBuffer number)
		{
			decimal num = value;
			char* digits = number.digits;
			number.precision = 29;
			number.sign = num.IsNegative;
			char* ptr = digits + 29;
			while ((num.Mid | num.High) != 0U)
			{
				ptr = global::System.Number.UInt32ToDecChars(ptr, decimal.DecDivMod1E9(ref num), 9);
			}
			ptr = global::System.Number.UInt32ToDecChars(ptr, num.Low, 0);
			int num2 = (int)((long)(digits + 29 - ptr));
			number.scale = num2 - num.Scale;
			char* digits2 = number.digits;
			while (--num2 >= 0)
			{
				*(digits2++) = *(ptr++);
			}
			*digits2 = '\0';
		}

		public unsafe static string FormatDouble(double value, string format, NumberFormatInfo info)
		{
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)64], 32);
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(span);
			return global::System.Number.FormatDouble(ref valueStringBuilder, value, format, info) ?? valueStringBuilder.ToString();
		}

		public unsafe static bool TryFormatDouble(double value, ReadOnlySpan<char> format, NumberFormatInfo info, Span<char> destination, out int charsWritten)
		{
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)64], 32);
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(span);
			string text = global::System.Number.FormatDouble(ref valueStringBuilder, value, format, info);
			if (text == null)
			{
				return valueStringBuilder.TryCopyTo(destination, out charsWritten);
			}
			return global::System.Number.TryCopyTo(text, destination, out charsWritten);
		}

		private static string FormatDouble(ref ValueStringBuilder sb, double value, ReadOnlySpan<char> format, NumberFormatInfo info)
		{
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			int num2 = 15;
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			if (c <= 'R')
			{
				if (c == 'E')
				{
					goto IL_00C1;
				}
				if (c == 'G')
				{
					goto IL_00CB;
				}
				if (c != 'R')
				{
					goto IL_00D3;
				}
			}
			else
			{
				if (c == 'e')
				{
					goto IL_00C1;
				}
				if (c == 'g')
				{
					goto IL_00CB;
				}
				if (c != 'r')
				{
					goto IL_00D3;
				}
			}
			global::System.Number.DoubleToNumber(value, 15, ref numberBuffer);
			if (numberBuffer.scale == -2147483648)
			{
				return info.NaNSymbol;
			}
			if (numberBuffer.scale != 2147483647)
			{
				if (global::System.Number.NumberToDouble(ref numberBuffer) == value)
				{
					global::System.Number.NumberToString(ref sb, ref numberBuffer, 'G', 15, info, false);
				}
				else
				{
					global::System.Number.DoubleToNumber(value, 17, ref numberBuffer);
					global::System.Number.NumberToString(ref sb, ref numberBuffer, 'G', 17, info, false);
				}
				return null;
			}
			if (!numberBuffer.sign)
			{
				return info.PositiveInfinitySymbol;
			}
			return info.NegativeInfinitySymbol;
			IL_00C1:
			if (num > 14)
			{
				num2 = 17;
				goto IL_00D3;
			}
			goto IL_00D3;
			IL_00CB:
			if (num > 15)
			{
				num2 = 17;
			}
			IL_00D3:
			global::System.Number.DoubleToNumber(value, num2, ref numberBuffer);
			if (numberBuffer.scale == -2147483648)
			{
				return info.NaNSymbol;
			}
			if (numberBuffer.scale != 2147483647)
			{
				if (c != '\0')
				{
					global::System.Number.NumberToString(ref sb, ref numberBuffer, c, num, info, false);
				}
				else
				{
					global::System.Number.NumberToStringFormat(ref sb, ref numberBuffer, format, info);
				}
				return null;
			}
			if (!numberBuffer.sign)
			{
				return info.PositiveInfinitySymbol;
			}
			return info.NegativeInfinitySymbol;
		}

		public unsafe static string FormatSingle(float value, string format, NumberFormatInfo info)
		{
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)64], 32);
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(span);
			return global::System.Number.FormatSingle(ref valueStringBuilder, value, format, info) ?? valueStringBuilder.ToString();
		}

		public unsafe static bool TryFormatSingle(float value, ReadOnlySpan<char> format, NumberFormatInfo info, Span<char> destination, out int charsWritten)
		{
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)64], 32);
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(span);
			string text = global::System.Number.FormatSingle(ref valueStringBuilder, value, format, info);
			if (text == null)
			{
				return valueStringBuilder.TryCopyTo(destination, out charsWritten);
			}
			return global::System.Number.TryCopyTo(text, destination, out charsWritten);
		}

		private static string FormatSingle(ref ValueStringBuilder sb, float value, ReadOnlySpan<char> format, NumberFormatInfo info)
		{
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			int num2 = 7;
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			if (c <= 'R')
			{
				if (c == 'E')
				{
					goto IL_00C1;
				}
				if (c == 'G')
				{
					goto IL_00CA;
				}
				if (c != 'R')
				{
					goto IL_00D1;
				}
			}
			else
			{
				if (c == 'e')
				{
					goto IL_00C1;
				}
				if (c == 'g')
				{
					goto IL_00CA;
				}
				if (c != 'r')
				{
					goto IL_00D1;
				}
			}
			global::System.Number.DoubleToNumber((double)value, 7, ref numberBuffer);
			if (numberBuffer.scale == -2147483648)
			{
				return info.NaNSymbol;
			}
			if (numberBuffer.scale != 2147483647)
			{
				if ((float)global::System.Number.NumberToDouble(ref numberBuffer) == value)
				{
					global::System.Number.NumberToString(ref sb, ref numberBuffer, 'G', 7, info, false);
				}
				else
				{
					global::System.Number.DoubleToNumber((double)value, 9, ref numberBuffer);
					global::System.Number.NumberToString(ref sb, ref numberBuffer, 'G', 9, info, false);
				}
				return null;
			}
			if (!numberBuffer.sign)
			{
				return info.PositiveInfinitySymbol;
			}
			return info.NegativeInfinitySymbol;
			IL_00C1:
			if (num > 6)
			{
				num2 = 9;
				goto IL_00D1;
			}
			goto IL_00D1;
			IL_00CA:
			if (num > 7)
			{
				num2 = 9;
			}
			IL_00D1:
			global::System.Number.DoubleToNumber((double)value, num2, ref numberBuffer);
			if (numberBuffer.scale == -2147483648)
			{
				return info.NaNSymbol;
			}
			if (numberBuffer.scale != 2147483647)
			{
				if (c != '\0')
				{
					global::System.Number.NumberToString(ref sb, ref numberBuffer, c, num, info, false);
				}
				else
				{
					global::System.Number.NumberToStringFormat(ref sb, ref numberBuffer, format, info);
				}
				return null;
			}
			if (!numberBuffer.sign)
			{
				return info.PositiveInfinitySymbol;
			}
			return info.NegativeInfinitySymbol;
		}

		private static bool TryCopyTo(string source, Span<char> destination, out int charsWritten)
		{
			if (source.AsSpan().TryCopyTo(destination))
			{
				charsWritten = source.Length;
				return true;
			}
			charsWritten = 0;
			return false;
		}

		public unsafe static string FormatInt32(int value, ReadOnlySpan<char> format, IFormatProvider provider)
		{
			if (value >= 0 && format.Length == 0)
			{
				return global::System.Number.UInt32ToDecStr((uint)value, -1);
			}
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			char c2 = c & '\uffdf';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				if (value < 0)
				{
					return global::System.Number.NegativeInt32ToDecStr(value, num, instance.NegativeSign);
				}
				return global::System.Number.UInt32ToDecStr((uint)value, num);
			}
			else
			{
				if (c2 == 'X')
				{
					return global::System.Number.Int32ToHexStr(value, c - '!', num);
				}
				global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
				global::System.Number.Int32ToNumber(value, ref numberBuffer);
				char* ptr = stackalloc char[(UIntPtr)64];
				ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)ptr, 32));
				if (c != '\0')
				{
					global::System.Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance, false);
				}
				else
				{
					global::System.Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
				}
				return valueStringBuilder.ToString();
			}
		}

		public unsafe static bool TryFormatInt32(int value, ReadOnlySpan<char> format, IFormatProvider provider, Span<char> destination, out int charsWritten)
		{
			if (value >= 0 && format.Length == 0)
			{
				return global::System.Number.TryUInt32ToDecStr((uint)value, -1, destination, out charsWritten);
			}
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			char c2 = c & '\uffdf';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				if (value < 0)
				{
					return global::System.Number.TryNegativeInt32ToDecStr(value, num, instance.NegativeSign, destination, out charsWritten);
				}
				return global::System.Number.TryUInt32ToDecStr((uint)value, num, destination, out charsWritten);
			}
			else
			{
				if (c2 == 'X')
				{
					return global::System.Number.TryInt32ToHexStr(value, c - '!', num, destination, out charsWritten);
				}
				global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
				global::System.Number.Int32ToNumber(value, ref numberBuffer);
				char* ptr = stackalloc char[(UIntPtr)64];
				ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)ptr, 32));
				if (c != '\0')
				{
					global::System.Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance, false);
				}
				else
				{
					global::System.Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
				}
				return valueStringBuilder.TryCopyTo(destination, out charsWritten);
			}
		}

		public unsafe static string FormatUInt32(uint value, ReadOnlySpan<char> format, IFormatProvider provider)
		{
			if (format.Length == 0)
			{
				return global::System.Number.UInt32ToDecStr(value, -1);
			}
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			char c2 = c & '\uffdf';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				return global::System.Number.UInt32ToDecStr(value, num);
			}
			if (c2 == 'X')
			{
				return global::System.Number.Int32ToHexStr((int)value, c - '!', num);
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			global::System.Number.UInt32ToNumber(value, ref numberBuffer);
			char* ptr = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)ptr, 32));
			if (c != '\0')
			{
				global::System.Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance, false);
			}
			else
			{
				global::System.Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
			}
			return valueStringBuilder.ToString();
		}

		public unsafe static bool TryFormatUInt32(uint value, ReadOnlySpan<char> format, IFormatProvider provider, Span<char> destination, out int charsWritten)
		{
			if (format.Length == 0)
			{
				return global::System.Number.TryUInt32ToDecStr(value, -1, destination, out charsWritten);
			}
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			char c2 = c & '\uffdf';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				return global::System.Number.TryUInt32ToDecStr(value, num, destination, out charsWritten);
			}
			if (c2 == 'X')
			{
				return global::System.Number.TryInt32ToHexStr((int)value, c - '!', num, destination, out charsWritten);
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			global::System.Number.UInt32ToNumber(value, ref numberBuffer);
			char* ptr = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)ptr, 32));
			if (c != '\0')
			{
				global::System.Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance, false);
			}
			else
			{
				global::System.Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
			}
			return valueStringBuilder.TryCopyTo(destination, out charsWritten);
		}

		public unsafe static string FormatInt64(long value, ReadOnlySpan<char> format, IFormatProvider provider)
		{
			if (value >= 0L && format.Length == 0)
			{
				return global::System.Number.UInt64ToDecStr((ulong)value, -1);
			}
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			char c2 = c & '\uffdf';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				if (value < 0L)
				{
					return global::System.Number.NegativeInt64ToDecStr(value, num, instance.NegativeSign);
				}
				return global::System.Number.UInt64ToDecStr((ulong)value, num);
			}
			else
			{
				if (c2 == 'X')
				{
					return global::System.Number.Int64ToHexStr(value, c - '!', num);
				}
				global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
				global::System.Number.Int64ToNumber(value, ref numberBuffer);
				char* ptr = stackalloc char[(UIntPtr)64];
				ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)ptr, 32));
				if (c != '\0')
				{
					global::System.Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance, false);
				}
				else
				{
					global::System.Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
				}
				return valueStringBuilder.ToString();
			}
		}

		public unsafe static bool TryFormatInt64(long value, ReadOnlySpan<char> format, IFormatProvider provider, Span<char> destination, out int charsWritten)
		{
			if (value >= 0L && format.Length == 0)
			{
				return global::System.Number.TryUInt64ToDecStr((ulong)value, -1, destination, out charsWritten);
			}
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			char c2 = c & '\uffdf';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				if (value < 0L)
				{
					return global::System.Number.TryNegativeInt64ToDecStr(value, num, instance.NegativeSign, destination, out charsWritten);
				}
				return global::System.Number.TryUInt64ToDecStr((ulong)value, num, destination, out charsWritten);
			}
			else
			{
				if (c2 == 'X')
				{
					return global::System.Number.TryInt64ToHexStr(value, c - '!', num, destination, out charsWritten);
				}
				global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
				global::System.Number.Int64ToNumber(value, ref numberBuffer);
				char* ptr = stackalloc char[(UIntPtr)64];
				ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)ptr, 32));
				if (c != '\0')
				{
					global::System.Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance, false);
				}
				else
				{
					global::System.Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
				}
				return valueStringBuilder.TryCopyTo(destination, out charsWritten);
			}
		}

		public unsafe static string FormatUInt64(ulong value, ReadOnlySpan<char> format, IFormatProvider provider)
		{
			if (format.Length == 0)
			{
				return global::System.Number.UInt64ToDecStr(value, -1);
			}
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			char c2 = c & '\uffdf';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				return global::System.Number.UInt64ToDecStr(value, num);
			}
			if (c2 == 'X')
			{
				return global::System.Number.Int64ToHexStr((long)value, c - '!', num);
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			global::System.Number.UInt64ToNumber(value, ref numberBuffer);
			char* ptr = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)ptr, 32));
			if (c != '\0')
			{
				global::System.Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance, false);
			}
			else
			{
				global::System.Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
			}
			return valueStringBuilder.ToString();
		}

		public unsafe static bool TryFormatUInt64(ulong value, ReadOnlySpan<char> format, IFormatProvider provider, Span<char> destination, out int charsWritten)
		{
			if (format.Length == 0)
			{
				return global::System.Number.TryUInt64ToDecStr(value, -1, destination, out charsWritten);
			}
			int num;
			char c = global::System.Number.ParseFormatSpecifier(format, out num);
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			char c2 = c & '\uffdf';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				return global::System.Number.TryUInt64ToDecStr(value, num, destination, out charsWritten);
			}
			if (c2 == 'X')
			{
				return global::System.Number.TryInt64ToHexStr((long)value, c - '!', num, destination, out charsWritten);
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			global::System.Number.UInt64ToNumber(value, ref numberBuffer);
			char* ptr = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)ptr, 32));
			if (c != '\0')
			{
				global::System.Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance, false);
			}
			else
			{
				global::System.Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
			}
			return valueStringBuilder.TryCopyTo(destination, out charsWritten);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void Int32ToNumber(int value, ref global::System.Number.NumberBuffer number)
		{
			number.precision = 10;
			if (value >= 0)
			{
				number.sign = false;
			}
			else
			{
				number.sign = true;
				value = -value;
			}
			char* digits = number.digits;
			char* ptr = global::System.Number.UInt32ToDecChars(digits + 10, (uint)value, 0);
			int num = (int)((long)(digits + 10 - ptr));
			number.scale = num;
			char* digits2 = number.digits;
			while (--num >= 0)
			{
				*(digits2++) = *(ptr++);
			}
			*digits2 = '\0';
		}

		private unsafe static string NegativeInt32ToDecStr(int value, int digits, string sNegative)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountDigits((uint)(-(uint)value))) + sNegative.Length;
			string text = string.FastAllocateString(num);
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = global::System.Number.UInt32ToDecChars(ptr + num, (uint)(-(uint)value), digits);
				for (int i = sNegative.Length - 1; i >= 0; i--)
				{
					*(--ptr2) = sNegative[i];
				}
			}
			return text;
		}

		private unsafe static bool TryNegativeInt32ToDecStr(int value, int digits, string sNegative, Span<char> destination, out int charsWritten)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountDigits((uint)(-(uint)value))) + sNegative.Length;
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* ptr = global::System.Number.UInt32ToDecChars(reference + num, (uint)(-(uint)value), digits);
				for (int i = sNegative.Length - 1; i >= 0; i--)
				{
					*(--ptr) = sNegative[i];
				}
			}
			return true;
		}

		private unsafe static string Int32ToHexStr(int value, char hexBase, int digits)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong)value));
			string text2;
			string text = (text2 = string.FastAllocateString(num));
			char* ptr = text2;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			global::System.Number.Int32ToHexChars(ptr + num, (uint)value, (int)hexBase, digits);
			text2 = null;
			return text;
		}

		private unsafe static bool TryInt32ToHexStr(int value, char hexBase, int digits, Span<char> destination, out int charsWritten)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong)value));
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				global::System.Number.Int32ToHexChars(reference + num, (uint)value, (int)hexBase, digits);
			}
			return true;
		}

		private unsafe static char* Int32ToHexChars(char* buffer, uint value, int hexBase, int digits)
		{
			while (--digits >= 0 || value != 0U)
			{
				byte b = (byte)(value & 15U);
				*(--buffer) = (char)((int)b + ((b < 10) ? 48 : hexBase));
				value >>= 4;
			}
			return buffer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void UInt32ToNumber(uint value, ref global::System.Number.NumberBuffer number)
		{
			number.precision = 10;
			number.sign = false;
			char* digits = number.digits;
			char* ptr = global::System.Number.UInt32ToDecChars(digits + 10, value, 0);
			int num = (int)((long)(digits + 10 - ptr));
			number.scale = num;
			char* digits2 = number.digits;
			while (--num >= 0)
			{
				*(digits2++) = *(ptr++);
			}
			*digits2 = '\0';
		}

		internal unsafe static char* UInt32ToDecChars(char* bufferEnd, uint value, int digits)
		{
			while (--digits >= 0 || value != 0U)
			{
				uint num = value / 10U;
				*(--bufferEnd) = (char)(value - num * 10U + 48U);
				value = num;
			}
			return bufferEnd;
		}

		private unsafe static string UInt32ToDecStr(uint value, int digits)
		{
			int num = Math.Max(digits, FormattingHelpers.CountDigits(value));
			string text = string.FastAllocateString(num);
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr + num;
				if (digits <= 1)
				{
					do
					{
						uint num2 = value / 10U;
						*(--ptr2) = (char)(48U + value - num2 * 10U);
						value = num2;
					}
					while (value != 0U);
				}
				else
				{
					ptr2 = global::System.Number.UInt32ToDecChars(ptr2, value, digits);
				}
			}
			return text;
		}

		private unsafe static bool TryUInt32ToDecStr(uint value, int digits, Span<char> destination, out int charsWritten)
		{
			int num = Math.Max(digits, FormattingHelpers.CountDigits(value));
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* ptr = reference + num;
				if (digits <= 1)
				{
					do
					{
						uint num2 = value / 10U;
						*(--ptr) = (char)(48U + value - num2 * 10U);
						value = num2;
					}
					while (value != 0U);
				}
				else
				{
					ptr = global::System.Number.UInt32ToDecChars(ptr, value, digits);
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static bool TryCopyTo(char* src, int length, Span<char> destination, out int charsWritten)
		{
			if (new ReadOnlySpan<char>((void*)src, length).TryCopyTo(destination))
			{
				charsWritten = length;
				return true;
			}
			charsWritten = 0;
			return false;
		}

		private unsafe static void Int64ToNumber(long input, ref global::System.Number.NumberBuffer number)
		{
			ulong num = (ulong)input;
			number.sign = input < 0L;
			number.precision = 19;
			if (number.sign)
			{
				num = (ulong)(-(ulong)input);
			}
			char* digits = number.digits;
			char* ptr = digits + 19;
			while (global::System.Number.High32(num) != 0U)
			{
				ptr = global::System.Number.UInt32ToDecChars(ptr, global::System.Number.Int64DivMod1E9(ref num), 9);
			}
			ptr = global::System.Number.UInt32ToDecChars(ptr, global::System.Number.Low32(num), 0);
			int num2 = (int)((long)(digits + 19 - ptr));
			number.scale = num2;
			char* digits2 = number.digits;
			while (--num2 >= 0)
			{
				*(digits2++) = *(ptr++);
			}
			*digits2 = '\0';
		}

		private unsafe static string NegativeInt64ToDecStr(long input, int digits, string sNegative)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			ulong num = (ulong)(-(ulong)input);
			int num2 = Math.Max(digits, FormattingHelpers.CountDigits(num)) + sNegative.Length;
			string text = string.FastAllocateString(num2);
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr + num2;
				while (global::System.Number.High32(num) != 0U)
				{
					ptr2 = global::System.Number.UInt32ToDecChars(ptr2, global::System.Number.Int64DivMod1E9(ref num), 9);
					digits -= 9;
				}
				ptr2 = global::System.Number.UInt32ToDecChars(ptr2, global::System.Number.Low32(num), digits);
				for (int i = sNegative.Length - 1; i >= 0; i--)
				{
					*(--ptr2) = sNegative[i];
				}
			}
			return text;
		}

		private unsafe static bool TryNegativeInt64ToDecStr(long input, int digits, string sNegative, Span<char> destination, out int charsWritten)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			ulong num = (ulong)(-(ulong)input);
			int num2 = Math.Max(digits, FormattingHelpers.CountDigits((ulong)(-(ulong)input))) + sNegative.Length;
			if (num2 > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num2;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* ptr = reference + num2;
				while (global::System.Number.High32(num) != 0U)
				{
					ptr = global::System.Number.UInt32ToDecChars(ptr, global::System.Number.Int64DivMod1E9(ref num), 9);
					digits -= 9;
				}
				ptr = global::System.Number.UInt32ToDecChars(ptr, global::System.Number.Low32(num), digits);
				for (int i = sNegative.Length - 1; i >= 0; i--)
				{
					*(--ptr) = sNegative[i];
				}
			}
			return true;
		}

		private unsafe static string Int64ToHexStr(long value, char hexBase, int digits)
		{
			int num = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong)value));
			string text2;
			string text = (text2 = string.FastAllocateString(num));
			char* ptr = text2;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = ptr + num;
			if (global::System.Number.High32((ulong)value) != 0U)
			{
				ptr2 = global::System.Number.Int32ToHexChars(ptr2, global::System.Number.Low32((ulong)value), (int)hexBase, 8);
				ptr2 = global::System.Number.Int32ToHexChars(ptr2, global::System.Number.High32((ulong)value), (int)hexBase, digits - 8);
			}
			else
			{
				ptr2 = global::System.Number.Int32ToHexChars(ptr2, global::System.Number.Low32((ulong)value), (int)hexBase, Math.Max(digits, 1));
			}
			text2 = null;
			return text;
		}

		private unsafe static bool TryInt64ToHexStr(long value, char hexBase, int digits, Span<char> destination, out int charsWritten)
		{
			int num = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong)value));
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* ptr = reference + num;
				if (global::System.Number.High32((ulong)value) != 0U)
				{
					ptr = global::System.Number.Int32ToHexChars(ptr, global::System.Number.Low32((ulong)value), (int)hexBase, 8);
					ptr = global::System.Number.Int32ToHexChars(ptr, global::System.Number.High32((ulong)value), (int)hexBase, digits - 8);
				}
				else
				{
					ptr = global::System.Number.Int32ToHexChars(ptr, global::System.Number.Low32((ulong)value), (int)hexBase, Math.Max(digits, 1));
				}
			}
			return true;
		}

		private unsafe static void UInt64ToNumber(ulong value, ref global::System.Number.NumberBuffer number)
		{
			number.precision = 20;
			number.sign = false;
			char* digits = number.digits;
			char* ptr = digits + 20;
			while (global::System.Number.High32(value) != 0U)
			{
				ptr = global::System.Number.UInt32ToDecChars(ptr, global::System.Number.Int64DivMod1E9(ref value), 9);
			}
			ptr = global::System.Number.UInt32ToDecChars(ptr, global::System.Number.Low32(value), 0);
			int num = (int)((long)(digits + 20 - ptr));
			number.scale = num;
			char* digits2 = number.digits;
			while (--num >= 0)
			{
				*(digits2++) = *(ptr++);
			}
			*digits2 = '\0';
		}

		private unsafe static string UInt64ToDecStr(ulong value, int digits)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountDigits(value));
			string text = string.FastAllocateString(num);
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr + num;
				while (global::System.Number.High32(value) != 0U)
				{
					ptr2 = global::System.Number.UInt32ToDecChars(ptr2, global::System.Number.Int64DivMod1E9(ref value), 9);
					digits -= 9;
				}
				ptr2 = global::System.Number.UInt32ToDecChars(ptr2, global::System.Number.Low32(value), digits);
			}
			return text;
		}

		private unsafe static bool TryUInt64ToDecStr(ulong value, int digits, Span<char> destination, out int charsWritten)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountDigits(value));
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* ptr = reference + num;
				while (global::System.Number.High32(value) != 0U)
				{
					ptr = global::System.Number.UInt32ToDecChars(ptr, global::System.Number.Int64DivMod1E9(ref value), 9);
					digits -= 9;
				}
				ptr = global::System.Number.UInt32ToDecChars(ptr, global::System.Number.Low32(value), digits);
			}
			return true;
		}

		internal unsafe static char ParseFormatSpecifier(ReadOnlySpan<char> format, out int digits)
		{
			char c = '\0';
			if (format.Length > 0)
			{
				c = (char)(*format[0]);
				if (c - 'A' <= '\u0019' || c - 'a' <= '\u0019')
				{
					if (format.Length == 1)
					{
						digits = -1;
						return c;
					}
					if (format.Length == 2)
					{
						int num = (int)(*format[1] - 48);
						if (num < 10)
						{
							digits = num;
							return c;
						}
					}
					else if (format.Length == 3)
					{
						int num2 = (int)(*format[1] - 48);
						int num3 = (int)(*format[2] - 48);
						if (num2 < 10 && num3 < 10)
						{
							digits = num2 * 10 + num3;
							return c;
						}
					}
					int num4 = 0;
					int num5 = 1;
					while (num5 < format.Length && *format[num5] - 48 < 10 && num4 < 10)
					{
						num4 = num4 * 10 + (int)(*format[num5++]) - 48;
					}
					if (num5 == format.Length || *format[num5] == 0)
					{
						digits = num4;
						return c;
					}
				}
			}
			digits = -1;
			if (format.Length != 0 && c != '\0')
			{
				return '\0';
			}
			return 'G';
		}

		internal unsafe static void NumberToString(ref ValueStringBuilder sb, ref global::System.Number.NumberBuffer number, char format, int nMaxDigits, NumberFormatInfo info, bool isDecimal)
		{
			if (format <= 'P')
			{
				switch (format)
				{
				case 'C':
					break;
				case 'D':
					goto IL_01FB;
				case 'E':
					goto IL_0119;
				case 'F':
					goto IL_00A1;
				case 'G':
					goto IL_0154;
				default:
					if (format == 'N')
					{
						goto IL_00EC;
					}
					if (format != 'P')
					{
						goto IL_01FB;
					}
					goto IL_01C3;
				}
			}
			else
			{
				switch (format)
				{
				case 'c':
					break;
				case 'd':
					goto IL_01FB;
				case 'e':
					goto IL_0119;
				case 'f':
					goto IL_00A1;
				case 'g':
					goto IL_0154;
				default:
					if (format == 'n')
					{
						goto IL_00EC;
					}
					if (format != 'p')
					{
						goto IL_01FB;
					}
					goto IL_01C3;
				}
			}
			int num = ((nMaxDigits >= 0) ? nMaxDigits : info.CurrencyDecimalDigits);
			if (nMaxDigits < 0)
			{
				nMaxDigits = info.CurrencyDecimalDigits;
			}
			global::System.Number.RoundNumber(ref number, number.scale + nMaxDigits);
			global::System.Number.FormatCurrency(ref sb, ref number, num, nMaxDigits, info);
			return;
			IL_00A1:
			if (nMaxDigits < 0)
			{
				num = (nMaxDigits = info.NumberDecimalDigits);
			}
			else
			{
				num = nMaxDigits;
			}
			global::System.Number.RoundNumber(ref number, number.scale + nMaxDigits);
			if (number.sign)
			{
				sb.Append(info.NegativeSign);
			}
			global::System.Number.FormatFixed(ref sb, ref number, num, nMaxDigits, info, null, info.NumberDecimalSeparator, null);
			return;
			IL_00EC:
			if (nMaxDigits < 0)
			{
				num = (nMaxDigits = info.NumberDecimalDigits);
			}
			else
			{
				num = nMaxDigits;
			}
			global::System.Number.RoundNumber(ref number, number.scale + nMaxDigits);
			global::System.Number.FormatNumber(ref sb, ref number, num, nMaxDigits, info);
			return;
			IL_0119:
			if (nMaxDigits < 0)
			{
				num = (nMaxDigits = 6);
			}
			else
			{
				num = nMaxDigits;
			}
			nMaxDigits++;
			global::System.Number.RoundNumber(ref number, nMaxDigits);
			if (number.sign)
			{
				sb.Append(info.NegativeSign);
			}
			global::System.Number.FormatScientific(ref sb, ref number, num, nMaxDigits, info, format);
			return;
			IL_0154:
			bool flag = true;
			if (nMaxDigits < 1)
			{
				if (isDecimal && nMaxDigits == -1)
				{
					num = (nMaxDigits = 29);
					flag = false;
				}
				else
				{
					num = (nMaxDigits = number.precision);
				}
			}
			else
			{
				num = nMaxDigits;
			}
			if (flag)
			{
				global::System.Number.RoundNumber(ref number, nMaxDigits);
			}
			else if (isDecimal && *number.digits == '\0')
			{
				number.sign = false;
			}
			if (number.sign)
			{
				sb.Append(info.NegativeSign);
			}
			global::System.Number.FormatGeneral(ref sb, ref number, num, nMaxDigits, info, format - '\u0002', !flag);
			return;
			IL_01C3:
			if (nMaxDigits < 0)
			{
				num = (nMaxDigits = info.PercentDecimalDigits);
			}
			else
			{
				num = nMaxDigits;
			}
			number.scale += 2;
			global::System.Number.RoundNumber(ref number, number.scale + nMaxDigits);
			global::System.Number.FormatPercent(ref sb, ref number, num, nMaxDigits, info);
			return;
			IL_01FB:
			throw new FormatException("Format specifier was invalid.");
		}

		internal unsafe static void NumberToStringFormat(ref ValueStringBuilder sb, ref global::System.Number.NumberBuffer number, ReadOnlySpan<char> format, NumberFormatInfo info)
		{
			int num = 0;
			char* digits = number.digits;
			int num2 = global::System.Number.FindSection(format, (*digits == '\0') ? 2 : (number.sign ? 1 : 0));
			int num3;
			int num4;
			int num5;
			int num6;
			bool flag;
			bool flag2;
			int i;
			for (;;)
			{
				num3 = 0;
				num4 = -1;
				num5 = int.MaxValue;
				num6 = 0;
				flag = false;
				int num7 = -1;
				flag2 = false;
				int num8 = 0;
				i = num2;
				fixed (char* ptr = MemoryMarshal.GetReference<char>(format))
				{
					char* ptr2 = ptr;
					char c;
					while (i < format.Length && (c = ptr2[(IntPtr)(i++) * 2]) != '\0' && c != ';')
					{
						if (c <= 'E')
						{
							switch (c)
							{
							case '"':
							case '\'':
								while (i < format.Length && ptr2[i] != '\0')
								{
									if (ptr2[(IntPtr)(i++) * 2] == c)
									{
										break;
									}
								}
								continue;
							case '#':
								num3++;
								continue;
							case '$':
							case '&':
								continue;
							case '%':
								num8 += 2;
								continue;
							default:
								switch (c)
								{
								case ',':
									if (num3 > 0 && num4 < 0)
									{
										if (num7 >= 0)
										{
											if (num7 == num3)
											{
												num++;
												continue;
											}
											flag2 = true;
										}
										num7 = num3;
										num = 1;
										continue;
									}
									continue;
								case '-':
								case '/':
									continue;
								case '.':
									if (num4 < 0)
									{
										num4 = num3;
										continue;
									}
									continue;
								case '0':
									if (num5 == 2147483647)
									{
										num5 = num3;
									}
									num3++;
									num6 = num3;
									continue;
								default:
									if (c != 'E')
									{
										continue;
									}
									break;
								}
								break;
							}
						}
						else if (c != '\\')
						{
							if (c != 'e')
							{
								if (c != '‰')
								{
									continue;
								}
								num8 += 3;
								continue;
							}
						}
						else
						{
							if (i < format.Length && ptr2[i] != '\0')
							{
								i++;
								continue;
							}
							continue;
						}
						if ((i < format.Length && ptr2[i] == '0') || (i + 1 < format.Length && (ptr2[i] == '+' || ptr2[i] == '-') && ptr2[i + 1] == '0'))
						{
							while (++i < format.Length && ptr2[i] == '0')
							{
							}
							flag = true;
						}
					}
				}
				if (num4 < 0)
				{
					num4 = num3;
				}
				if (num7 >= 0)
				{
					if (num7 == num4)
					{
						num8 -= num * 3;
					}
					else
					{
						flag2 = true;
					}
				}
				if (*digits == '\0')
				{
					break;
				}
				number.scale += num8;
				int num9 = (flag ? num3 : (number.scale + num3 - num4));
				global::System.Number.RoundNumber(ref number, num9);
				if (*digits != '\0')
				{
					goto IL_029E;
				}
				i = global::System.Number.FindSection(format, 2);
				if (i == num2)
				{
					goto IL_029E;
				}
				num2 = i;
			}
			number.sign = false;
			number.scale = 0;
			IL_029E:
			num5 = ((num5 < num4) ? (num4 - num5) : 0);
			num6 = ((num6 > num4) ? (num4 - num6) : 0);
			int num10;
			int j;
			if (flag)
			{
				num10 = num4;
				j = 0;
			}
			else
			{
				num10 = ((number.scale > num4) ? number.scale : num4);
				j = number.scale - num4;
			}
			i = num2;
			Span<int> span = new Span<int>(stackalloc byte[(UIntPtr)16], 4);
			int num11 = -1;
			if (flag2 && info.NumberGroupSeparator.Length > 0)
			{
				int[] numberGroupSizes = info.numberGroupSizes;
				int num12 = 0;
				int num13 = 0;
				int num14 = numberGroupSizes.Length;
				if (num14 != 0)
				{
					num13 = numberGroupSizes[num12];
				}
				int num15 = num13;
				int num16 = num10 + ((j < 0) ? j : 0);
				int num17 = ((num5 > num16) ? num5 : num16);
				while (num17 > num13 && num15 != 0)
				{
					num11++;
					if (num11 >= span.Length)
					{
						int[] array = new int[span.Length * 2];
						span.CopyTo(array);
						span = array;
					}
					*span[num11] = num13;
					if (num12 < num14 - 1)
					{
						num12++;
						num15 = numberGroupSizes[num12];
					}
					num13 += num15;
				}
			}
			if (number.sign && num2 == 0)
			{
				sb.Append(info.NegativeSign);
			}
			bool flag3 = false;
			fixed (char* ptr = MemoryMarshal.GetReference<char>(format))
			{
				char* ptr3 = ptr;
				char* ptr4 = digits;
				char c;
				while (i < format.Length && (c = ptr3[(IntPtr)(i++) * 2]) != '\0' && c != ';')
				{
					if (j > 0)
					{
						if (c == '#' || c == '.' || c == '0')
						{
							while (j > 0)
							{
								sb.Append((*ptr4 != '\0') ? (*(ptr4++)) : '0');
								if (flag2 && num10 > 1 && num11 >= 0 && num10 == *span[num11] + 1)
								{
									sb.Append(info.NumberGroupSeparator);
									num11--;
								}
								num10--;
								j--;
							}
						}
					}
					if (c <= 'E')
					{
						switch (c)
						{
						case '"':
						case '\'':
							while (i < format.Length && ptr3[i] != '\0' && ptr3[i] != c)
							{
								sb.Append(ptr3[(IntPtr)(i++) * 2]);
							}
							if (i < format.Length && ptr3[i] != '\0')
							{
								i++;
								continue;
							}
							continue;
						case '#':
							break;
						case '$':
						case '&':
							goto IL_0786;
						case '%':
							sb.Append(info.PercentSymbol);
							continue;
						default:
							switch (c)
							{
							case ',':
								continue;
							case '-':
							case '/':
								goto IL_0786;
							case '.':
								if (num10 == 0 && !flag3 && (num6 < 0 || (num4 < num3 && *ptr4 != '\0')))
								{
									sb.Append(info.NumberDecimalSeparator);
									flag3 = true;
									continue;
								}
								continue;
							case '0':
								break;
							default:
								if (c != 'E')
								{
									goto IL_0786;
								}
								goto IL_0631;
							}
							break;
						}
						if (j < 0)
						{
							j++;
							c = ((num10 <= num5) ? '0' : '\0');
						}
						else
						{
							c = ((*ptr4 != '\0') ? (*(ptr4++)) : ((num10 > num6) ? '0' : '\0'));
						}
						if (c != '\0')
						{
							sb.Append(c);
							if (flag2 && num10 > 1 && num11 >= 0 && num10 == *span[num11] + 1)
							{
								sb.Append(info.NumberGroupSeparator);
								num11--;
							}
						}
						num10--;
						continue;
					}
					if (c != '\\')
					{
						if (c != 'e')
						{
							if (c != '‰')
							{
								goto IL_0786;
							}
							sb.Append(info.PerMilleSymbol);
							continue;
						}
					}
					else
					{
						if (i < format.Length && ptr3[i] != '\0')
						{
							sb.Append(ptr3[(IntPtr)(i++) * 2]);
							continue;
						}
						continue;
					}
					IL_0631:
					bool flag4 = false;
					int num18 = 0;
					if (flag)
					{
						if (i < format.Length && ptr3[i] == '0')
						{
							num18++;
						}
						else if (i + 1 < format.Length && ptr3[i] == '+' && ptr3[i + 1] == '0')
						{
							flag4 = true;
						}
						else if (i + 1 >= format.Length || ptr3[i] != '-' || ptr3[i + 1] != '0')
						{
							sb.Append(c);
							continue;
						}
						while (++i < format.Length && ptr3[i] == '0')
						{
							num18++;
						}
						if (num18 > 10)
						{
							num18 = 10;
						}
						int num19 = ((*digits == '\0') ? 0 : (number.scale - num4));
						global::System.Number.FormatExponent(ref sb, info, num19, c, num18, flag4);
						flag = false;
						continue;
					}
					sb.Append(c);
					if (i < format.Length)
					{
						if (ptr3[i] == '+' || ptr3[i] == '-')
						{
							sb.Append(ptr3[(IntPtr)(i++) * 2]);
						}
						while (i < format.Length)
						{
							if (ptr3[i] != '0')
							{
								break;
							}
							sb.Append(ptr3[(IntPtr)(i++) * 2]);
						}
						continue;
					}
					continue;
					IL_0786:
					sb.Append(c);
				}
			}
		}

		private static void FormatCurrency(ref ValueStringBuilder sb, ref global::System.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info)
		{
			foreach (char c in number.sign ? global::System.Number.s_negCurrencyFormats[info.CurrencyNegativePattern] : global::System.Number.s_posCurrencyFormats[info.CurrencyPositivePattern])
			{
				if (c != '#')
				{
					if (c != '$')
					{
						if (c != '-')
						{
							sb.Append(c);
						}
						else
						{
							sb.Append(info.NegativeSign);
						}
					}
					else
					{
						sb.Append(info.CurrencySymbol);
					}
				}
				else
				{
					global::System.Number.FormatFixed(ref sb, ref number, nMinDigits, nMaxDigits, info, info.currencyGroupSizes, info.CurrencyDecimalSeparator, info.CurrencyGroupSeparator);
				}
			}
		}

		private unsafe static void FormatFixed(ref ValueStringBuilder sb, ref global::System.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info, int[] groupDigits, string sDecimal, string sGroup)
		{
			int i = number.scale;
			char* ptr = number.digits;
			if (i > 0)
			{
				if (groupDigits != null)
				{
					int num = 0;
					int num2 = i;
					int num3 = 0;
					if (groupDigits.Length != 0)
					{
						int num4 = groupDigits[num];
						while (i > num4)
						{
							num3 = groupDigits[num];
							if (num3 == 0)
							{
								break;
							}
							num2 += sGroup.Length;
							if (num < groupDigits.Length - 1)
							{
								num++;
							}
							num4 += groupDigits[num];
							if (num4 < 0 || num2 < 0)
							{
								throw new ArgumentOutOfRangeException();
							}
						}
						num3 = ((num4 == 0) ? 0 : groupDigits[0]);
					}
					num = 0;
					int num5 = 0;
					int num6 = string.wcslen(ptr);
					int num7 = ((i < num6) ? i : num6);
					fixed (char* reference = MemoryMarshal.GetReference<char>(sb.AppendSpan(num2)))
					{
						char* ptr2 = reference + num2 - 1;
						for (int j = i - 1; j >= 0; j--)
						{
							*(ptr2--) = ((j < num7) ? ptr[j] : '0');
							if (num3 > 0)
							{
								num5++;
								if (num5 == num3 && j != 0)
								{
									for (int k = sGroup.Length - 1; k >= 0; k--)
									{
										*(ptr2--) = sGroup[k];
									}
									if (num < groupDigits.Length - 1)
									{
										num++;
										num3 = groupDigits[num];
									}
									num5 = 0;
								}
							}
						}
						ptr += num7;
					}
				}
				else
				{
					do
					{
						sb.Append((*ptr != '\0') ? (*(ptr++)) : '0');
					}
					while (--i > 0);
				}
			}
			else
			{
				sb.Append('0');
			}
			if (nMaxDigits > 0)
			{
				sb.Append(sDecimal);
				if (i < 0 && nMaxDigits > 0)
				{
					int num8 = Math.Min(-i, nMaxDigits);
					sb.Append('0', num8);
					i += num8;
					nMaxDigits -= num8;
				}
				while (nMaxDigits > 0)
				{
					sb.Append((*ptr != '\0') ? (*(ptr++)) : '0');
					nMaxDigits--;
				}
			}
		}

		private static void FormatNumber(ref ValueStringBuilder sb, ref global::System.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info)
		{
			foreach (char c in number.sign ? global::System.Number.s_negNumberFormats[info.NumberNegativePattern] : "#")
			{
				if (c != '#')
				{
					if (c != '-')
					{
						sb.Append(c);
					}
					else
					{
						sb.Append(info.NegativeSign);
					}
				}
				else
				{
					global::System.Number.FormatFixed(ref sb, ref number, nMinDigits, nMaxDigits, info, info.numberGroupSizes, info.NumberDecimalSeparator, info.NumberGroupSeparator);
				}
			}
		}

		private unsafe static void FormatScientific(ref ValueStringBuilder sb, ref global::System.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info, char expChar)
		{
			char* digits = number.digits;
			sb.Append((*digits != '\0') ? (*(digits++)) : '0');
			if (nMaxDigits != 1)
			{
				sb.Append(info.NumberDecimalSeparator);
			}
			while (--nMaxDigits > 0)
			{
				sb.Append((*digits != '\0') ? (*(digits++)) : '0');
			}
			int num = ((*number.digits == '\0') ? 0 : (number.scale - 1));
			global::System.Number.FormatExponent(ref sb, info, num, expChar, 3, true);
		}

		private unsafe static void FormatExponent(ref ValueStringBuilder sb, NumberFormatInfo info, int value, char expChar, int minDigits, bool positiveSign)
		{
			sb.Append(expChar);
			if (value < 0)
			{
				sb.Append(info.NegativeSign);
				value = -value;
			}
			else if (positiveSign)
			{
				sb.Append(info.PositiveSign);
			}
			char* ptr = stackalloc char[(UIntPtr)20];
			char* ptr2 = global::System.Number.UInt32ToDecChars(ptr + 10, (uint)value, minDigits);
			long num = (long)(ptr + 10 - ptr2);
			sb.Append(ptr2, (int)((long)(ptr + 10 - ptr2)));
		}

		private unsafe static void FormatGeneral(ref ValueStringBuilder sb, ref global::System.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info, char expChar, bool bSuppressScientific)
		{
			int i = number.scale;
			bool flag = false;
			if (!bSuppressScientific && (i > nMaxDigits || i < -3))
			{
				i = 1;
				flag = true;
			}
			char* digits = number.digits;
			if (i > 0)
			{
				do
				{
					sb.Append((*digits != '\0') ? (*(digits++)) : '0');
				}
				while (--i > 0);
			}
			else
			{
				sb.Append('0');
			}
			if (*digits != '\0' || i < 0)
			{
				sb.Append(info.NumberDecimalSeparator);
				while (i < 0)
				{
					sb.Append('0');
					i++;
				}
				while (*digits != '\0')
				{
					sb.Append(*(digits++));
				}
			}
			if (flag)
			{
				global::System.Number.FormatExponent(ref sb, info, number.scale - 1, expChar, 2, true);
			}
		}

		private static void FormatPercent(ref ValueStringBuilder sb, ref global::System.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info)
		{
			foreach (char c in number.sign ? global::System.Number.s_negPercentFormats[info.PercentNegativePattern] : global::System.Number.s_posPercentFormats[info.PercentPositivePattern])
			{
				if (c != '#')
				{
					if (c != '%')
					{
						if (c != '-')
						{
							sb.Append(c);
						}
						else
						{
							sb.Append(info.NegativeSign);
						}
					}
					else
					{
						sb.Append(info.PercentSymbol);
					}
				}
				else
				{
					global::System.Number.FormatFixed(ref sb, ref number, nMinDigits, nMaxDigits, info, info.percentGroupSizes, info.PercentDecimalSeparator, info.PercentGroupSeparator);
				}
			}
		}

		private unsafe static void RoundNumber(ref global::System.Number.NumberBuffer number, int pos)
		{
			char* digits = number.digits;
			int num = 0;
			while (num < pos && digits[num] != '\0')
			{
				num++;
			}
			if (num == pos && digits[num] >= '5')
			{
				while (num > 0 && digits[num - 1] == '9')
				{
					num--;
				}
				if (num > 0)
				{
					char* ptr = digits + (num - 1);
					*ptr += '\u0001';
				}
				else
				{
					number.scale++;
					*digits = '1';
					num = 1;
				}
			}
			else
			{
				while (num > 0 && digits[num - 1] == '0')
				{
					num--;
				}
			}
			if (num == 0)
			{
				number.scale = 0;
				number.sign = false;
			}
			digits[num] = '\0';
		}

		private unsafe static int FindSection(ReadOnlySpan<char> format, int section)
		{
			if (section == 0)
			{
				return 0;
			}
			fixed (char* reference = MemoryMarshal.GetReference<char>(format))
			{
				char* ptr = reference;
				int i = 0;
				while (i < format.Length)
				{
					char c2;
					char c = (c2 = ptr[(IntPtr)(i++) * 2]);
					if (c2 <= '"')
					{
						if (c2 == '\0')
						{
							return 0;
						}
						if (c2 != '"')
						{
							continue;
						}
					}
					else if (c2 != '\'')
					{
						if (c2 != ';')
						{
							if (c2 != '\\')
							{
								continue;
							}
							if (i < format.Length && ptr[i] != '\0')
							{
								i++;
								continue;
							}
							continue;
						}
						else
						{
							if (--section != 0)
							{
								continue;
							}
							if (i < format.Length && ptr[i] != '\0' && ptr[i] != ';')
							{
								return i;
							}
							return 0;
						}
					}
					while (i < format.Length && ptr[i] != '\0')
					{
						if (ptr[(IntPtr)(i++) * 2] == c)
						{
							break;
						}
					}
				}
				return 0;
			}
		}

		private static uint Low32(ulong value)
		{
			return (uint)value;
		}

		private static uint High32(ulong value)
		{
			return (uint)((value & 18446744069414584320UL) >> 32);
		}

		private static uint Int64DivMod1E9(ref ulong value)
		{
			uint num = (uint)(value % 1000000000UL);
			value /= 1000000000UL;
			return num;
		}

		private unsafe static bool NumberToInt32(ref global::System.Number.NumberBuffer number, ref int value)
		{
			int num = number.scale;
			if (num > 10 || num < number.precision)
			{
				return false;
			}
			char* digits = number.digits;
			int num2 = 0;
			while (--num >= 0)
			{
				if (num2 > 214748364)
				{
					return false;
				}
				num2 *= 10;
				if (*digits != '\0')
				{
					num2 += (int)(*(digits++) - '0');
				}
			}
			if (number.sign)
			{
				num2 = -num2;
				if (num2 > 0)
				{
					return false;
				}
			}
			else if (num2 < 0)
			{
				return false;
			}
			value = num2;
			return true;
		}

		private unsafe static bool NumberToInt64(ref global::System.Number.NumberBuffer number, ref long value)
		{
			int num = number.scale;
			if (num > 19 || num < number.precision)
			{
				return false;
			}
			char* digits = number.digits;
			long num2 = 0L;
			while (--num >= 0)
			{
				if (num2 > 922337203685477580L)
				{
					return false;
				}
				num2 *= 10L;
				if (*digits != '\0')
				{
					num2 += (long)(*(digits++) - '0');
				}
			}
			if (number.sign)
			{
				num2 = -num2;
				if (num2 > 0L)
				{
					return false;
				}
			}
			else if (num2 < 0L)
			{
				return false;
			}
			value = num2;
			return true;
		}

		private unsafe static bool NumberToUInt32(ref global::System.Number.NumberBuffer number, ref uint value)
		{
			int num = number.scale;
			if (num > 10 || num < number.precision || number.sign)
			{
				return false;
			}
			char* digits = number.digits;
			uint num2 = 0U;
			while (--num >= 0)
			{
				if (num2 > 429496729U)
				{
					return false;
				}
				num2 *= 10U;
				if (*digits != '\0')
				{
					uint num3 = num2 + (uint)(*(digits++) - '0');
					if (num3 < num2)
					{
						return false;
					}
					num2 = num3;
				}
			}
			value = num2;
			return true;
		}

		private unsafe static bool NumberToUInt64(ref global::System.Number.NumberBuffer number, ref ulong value)
		{
			int num = number.scale;
			if (num > 20 || num < number.precision || number.sign)
			{
				return false;
			}
			char* digits = number.digits;
			ulong num2 = 0UL;
			while (--num >= 0)
			{
				if (num2 > 1844674407370955161UL)
				{
					return false;
				}
				num2 *= 10UL;
				if (*digits != '\0')
				{
					ulong num3 = num2 + (ulong)((long)(*(digits++) - '0'));
					if (num3 < num2)
					{
						return false;
					}
					num2 = num3;
				}
			}
			value = num2;
			return true;
		}

		internal static int ParseInt32(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				bool flag = false;
				int num;
				if (!global::System.Number.TryParseInt32IntegerStyle(value, styles, info, out num, ref flag))
				{
					global::System.Number.ThrowOverflowOrFormatException(flag, "Value was either too large or too small for an Int32.");
				}
				return num;
			}
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				bool flag2 = false;
				uint num2;
				if (!global::System.Number.TryParseUInt32HexNumberStyle(value, styles, info, out num2, ref flag2))
				{
					global::System.Number.ThrowOverflowOrFormatException(flag2, "Value was either too large or too small for an Int32.");
				}
				return (int)num2;
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			int num3 = 0;
			global::System.Number.StringToNumber(value, styles, ref numberBuffer, info, false);
			if (!global::System.Number.NumberToInt32(ref numberBuffer, ref num3))
			{
				global::System.Number.ThrowOverflowOrFormatException(true, "Value was either too large or too small for an Int32.");
			}
			return num3;
		}

		internal static long ParseInt64(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				bool flag = false;
				long num;
				if (!global::System.Number.TryParseInt64IntegerStyle(value, styles, info, out num, ref flag))
				{
					global::System.Number.ThrowOverflowOrFormatException(flag, "Value was either too large or too small for an Int64.");
				}
				return num;
			}
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				bool flag2 = false;
				ulong num2;
				if (!global::System.Number.TryParseUInt64HexNumberStyle(value, styles, info, out num2, ref flag2))
				{
					global::System.Number.ThrowOverflowOrFormatException(flag2, "Value was either too large or too small for an Int64.");
				}
				return (long)num2;
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			long num3 = 0L;
			global::System.Number.StringToNumber(value, styles, ref numberBuffer, info, false);
			if (!global::System.Number.NumberToInt64(ref numberBuffer, ref num3))
			{
				global::System.Number.ThrowOverflowOrFormatException(true, "Value was either too large or too small for an Int64.");
			}
			return num3;
		}

		internal static uint ParseUInt32(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			uint num = 0U;
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				bool flag = false;
				if (!global::System.Number.TryParseUInt32IntegerStyle(value, styles, info, out num, ref flag))
				{
					global::System.Number.ThrowOverflowOrFormatException(flag, "Value was either too large or too small for a UInt32.");
				}
				return num;
			}
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				bool flag2 = false;
				if (!global::System.Number.TryParseUInt32HexNumberStyle(value, styles, info, out num, ref flag2))
				{
					global::System.Number.ThrowOverflowOrFormatException(flag2, "Value was either too large or too small for a UInt32.");
				}
				return num;
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			global::System.Number.StringToNumber(value, styles, ref numberBuffer, info, false);
			if (!global::System.Number.NumberToUInt32(ref numberBuffer, ref num))
			{
				global::System.Number.ThrowOverflowOrFormatException(true, "Value was either too large or too small for a UInt32.");
			}
			return num;
		}

		internal static ulong ParseUInt64(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			ulong num = 0UL;
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				bool flag = false;
				if (!global::System.Number.TryParseUInt64IntegerStyle(value, styles, info, out num, ref flag))
				{
					global::System.Number.ThrowOverflowOrFormatException(flag, "Value was either too large or too small for a UInt64.");
				}
				return num;
			}
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				bool flag2 = false;
				if (!global::System.Number.TryParseUInt64HexNumberStyle(value, styles, info, out num, ref flag2))
				{
					global::System.Number.ThrowOverflowOrFormatException(flag2, "Value was either too large or too small for a UInt64.");
				}
				return num;
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			global::System.Number.StringToNumber(value, styles, ref numberBuffer, info, false);
			if (!global::System.Number.NumberToUInt64(ref numberBuffer, ref num))
			{
				global::System.Number.ThrowOverflowOrFormatException(true, "Value was either too large or too small for a UInt64.");
			}
			return num;
		}

		private unsafe static bool ParseNumber(ref char* str, char* strEnd, NumberStyles styles, ref global::System.Number.NumberBuffer number, NumberFormatInfo info, bool parseDecimal)
		{
			number.scale = 0;
			number.sign = false;
			string text = null;
			bool flag = false;
			string text2;
			string text3;
			if ((styles & NumberStyles.AllowCurrencySymbol) != NumberStyles.None)
			{
				text = info.CurrencySymbol;
				text2 = info.CurrencyDecimalSeparator;
				text3 = info.CurrencyGroupSeparator;
				flag = true;
			}
			else
			{
				text2 = info.NumberDecimalSeparator;
				text3 = info.NumberGroupSeparator;
			}
			int num = 0;
			char* ptr = str;
			char c = ((ptr < strEnd) ? (*ptr) : '\0');
			for (;;)
			{
				if (!global::System.Number.IsWhite((int)c) || (styles & NumberStyles.AllowLeadingWhite) == NumberStyles.None || ((num & 1) != 0 && (num & 32) == 0 && info.NumberNegativePattern != 2))
				{
					char* ptr2;
					if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None && (num & 1) == 0 && ((ptr2 = global::System.Number.MatchChars(ptr, strEnd, info.PositiveSign)) != null || ((ptr2 = global::System.Number.MatchChars(ptr, strEnd, info.NegativeSign)) != null && (number.sign = true))))
					{
						num |= 1;
						ptr = ptr2 - 1;
					}
					else if (c == '(' && (styles & NumberStyles.AllowParentheses) != NumberStyles.None && (num & 1) == 0)
					{
						num |= 3;
						number.sign = true;
					}
					else
					{
						if (text == null || (ptr2 = global::System.Number.MatchChars(ptr, strEnd, text)) == null)
						{
							break;
						}
						num |= 32;
						text = null;
						ptr = ptr2 - 1;
					}
				}
				c = ((++ptr < strEnd) ? (*ptr) : '\0');
			}
			int num2 = 0;
			int num3 = 0;
			for (;;)
			{
				char* ptr2;
				if (global::System.Number.IsDigit((int)c))
				{
					num |= 4;
					if (c != '0' || (num & 8) != 0)
					{
						if (num2 < 50)
						{
							number.digits[(IntPtr)(num2++) * 2] = c;
							if (c != '0' || parseDecimal)
							{
								num3 = num2;
							}
						}
						if ((num & 16) == 0)
						{
							number.scale++;
						}
						num |= 8;
					}
					else if ((num & 16) != 0)
					{
						number.scale--;
					}
				}
				else if ((styles & NumberStyles.AllowDecimalPoint) != NumberStyles.None && (num & 16) == 0 && ((ptr2 = global::System.Number.MatchChars(ptr, strEnd, text2)) != null || (flag && (num & 32) == 0 && (ptr2 = global::System.Number.MatchChars(ptr, strEnd, info.NumberDecimalSeparator)) != null)))
				{
					num |= 16;
					ptr = ptr2 - 1;
				}
				else
				{
					if ((styles & NumberStyles.AllowThousands) == NumberStyles.None || (num & 4) == 0 || (num & 16) != 0 || ((ptr2 = global::System.Number.MatchChars(ptr, strEnd, text3)) == null && (!flag || (num & 32) != 0 || (ptr2 = global::System.Number.MatchChars(ptr, strEnd, info.NumberGroupSeparator)) == null)))
					{
						break;
					}
					ptr = ptr2 - 1;
				}
				c = ((++ptr < strEnd) ? (*ptr) : '\0');
			}
			bool flag2 = false;
			number.precision = num3;
			number.digits[num3] = '\0';
			if ((num & 4) != 0)
			{
				if ((c == 'E' || c == 'e') && (styles & NumberStyles.AllowExponent) != NumberStyles.None)
				{
					char* ptr3 = ptr;
					c = ((++ptr < strEnd) ? (*ptr) : '\0');
					char* ptr2;
					if ((ptr2 = global::System.Number.MatchChars(ptr, strEnd, info.positiveSign)) != null)
					{
						c = (((ptr = ptr2) < strEnd) ? (*ptr) : '\0');
					}
					else if ((ptr2 = global::System.Number.MatchChars(ptr, strEnd, info.negativeSign)) != null)
					{
						c = (((ptr = ptr2) < strEnd) ? (*ptr) : '\0');
						flag2 = true;
					}
					if (global::System.Number.IsDigit((int)c))
					{
						int num4 = 0;
						do
						{
							num4 = num4 * 10 + (int)(c - '0');
							c = ((++ptr < strEnd) ? (*ptr) : '\0');
							if (num4 > 1000)
							{
								num4 = 9999;
								while (global::System.Number.IsDigit((int)c))
								{
									c = ((++ptr < strEnd) ? (*ptr) : '\0');
								}
							}
						}
						while (global::System.Number.IsDigit((int)c));
						if (flag2)
						{
							num4 = -num4;
						}
						number.scale += num4;
					}
					else
					{
						ptr = ptr3;
						c = ((ptr < strEnd) ? (*ptr) : '\0');
					}
				}
				for (;;)
				{
					if (!global::System.Number.IsWhite((int)c) || (styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
					{
						char* ptr2;
						if ((styles & NumberStyles.AllowTrailingSign) != NumberStyles.None && (num & 1) == 0 && ((ptr2 = global::System.Number.MatchChars(ptr, strEnd, info.PositiveSign)) != null || ((ptr2 = global::System.Number.MatchChars(ptr, strEnd, info.NegativeSign)) != null && (number.sign = true))))
						{
							num |= 1;
							ptr = ptr2 - 1;
						}
						else if (c == ')' && (num & 2) != 0)
						{
							num &= -3;
						}
						else
						{
							if (text == null || (ptr2 = global::System.Number.MatchChars(ptr, strEnd, text)) == null)
							{
								break;
							}
							text = null;
							ptr = ptr2 - 1;
						}
					}
					c = ((++ptr < strEnd) ? (*ptr) : '\0');
				}
				if ((num & 2) == 0)
				{
					if ((num & 8) == 0)
					{
						if (!parseDecimal)
						{
							number.scale = 0;
						}
						if ((num & 16) == 0)
						{
							number.sign = false;
						}
					}
					str = ptr;
					return true;
				}
			}
			str = ptr;
			return false;
		}

		internal static bool TryParseInt32(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out int result)
		{
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				bool flag = false;
				return global::System.Number.TryParseInt32IntegerStyle(value, styles, info, out result, ref flag);
			}
			result = 0;
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				bool flag2 = false;
				return global::System.Number.TryParseUInt32HexNumberStyle(value, styles, info, Unsafe.As<int, uint>(ref result), ref flag2);
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			return global::System.Number.TryStringToNumber(value, styles, ref numberBuffer, info, false) && global::System.Number.NumberToInt32(ref numberBuffer, ref result);
		}

		private unsafe static bool TryParseInt32IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out int result, ref bool failureIsOverflow)
		{
			if (value.Length >= 1)
			{
				bool flag = false;
				int num = 1;
				int num2 = 0;
				int num3 = (int)(*value[0]);
				if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && global::System.Number.IsWhite(num3))
				{
					do
					{
						num2++;
						if (num2 >= value.Length)
						{
							goto IL_0269;
						}
						num3 = (int)(*value[num2]);
					}
					while (global::System.Number.IsWhite(num3));
				}
				if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None)
				{
					string positiveSign = info.PositiveSign;
					string negativeSign = info.NegativeSign;
					if (positiveSign == "+" && negativeSign == "-")
					{
						if (num3 == 45)
						{
							num = -1;
							num2++;
							if (num2 >= value.Length)
							{
								goto IL_0269;
							}
							num3 = (int)(*value[num2]);
						}
						else if (num3 == 43)
						{
							num2++;
							if (num2 >= value.Length)
							{
								goto IL_0269;
							}
							num3 = (int)(*value[num2]);
						}
					}
					else
					{
						value = value.Slice(num2);
						num2 = 0;
						if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith<char>(positiveSign))
						{
							num2 += positiveSign.Length;
							if (num2 >= value.Length)
							{
								goto IL_0269;
							}
							num3 = (int)(*value[num2]);
						}
						else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith<char>(negativeSign))
						{
							num = -1;
							num2 += negativeSign.Length;
							if (num2 >= value.Length)
							{
								goto IL_0269;
							}
							num3 = (int)(*value[num2]);
						}
					}
				}
				int num4 = 0;
				if (global::System.Number.IsDigit(num3))
				{
					if (num3 == 48)
					{
						do
						{
							num2++;
							if (num2 >= value.Length)
							{
								goto IL_026E;
							}
							num3 = (int)(*value[num2]);
						}
						while (num3 == 48);
						if (!global::System.Number.IsDigit(num3))
						{
							goto IL_027F;
						}
					}
					num4 = num3 - 48;
					num2++;
					for (int i = 0; i < 8; i++)
					{
						if (num2 >= value.Length)
						{
							goto IL_026E;
						}
						num3 = (int)(*value[num2]);
						if (!global::System.Number.IsDigit(num3))
						{
							goto IL_027F;
						}
						num2++;
						num4 = 10 * num4 + num3 - 48;
					}
					if (num2 < value.Length)
					{
						num3 = (int)(*value[num2]);
						if (!global::System.Number.IsDigit(num3))
						{
							goto IL_027F;
						}
						num2++;
						if (num4 > 214748364)
						{
							flag = true;
						}
						num4 = num4 * 10 + num3 - 48;
						if ((ulong)num4 > (ulong)(2147483647L + (long)((-1 * num + 1) / 2)))
						{
							flag = true;
						}
						if (num2 < value.Length)
						{
							num3 = (int)(*value[num2]);
							while (global::System.Number.IsDigit(num3))
							{
								flag = true;
								num2++;
								if (num2 >= value.Length)
								{
									goto IL_026E;
								}
								num3 = (int)(*value[num2]);
							}
							goto IL_027F;
						}
					}
					IL_026E:
					if (flag)
					{
						failureIsOverflow = true;
						goto IL_0269;
					}
					result = num4 * num;
					return true;
					IL_027F:
					if (global::System.Number.IsWhite(num3))
					{
						if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
						{
							goto IL_0269;
						}
						num2++;
						while (num2 < value.Length && global::System.Number.IsWhite((int)(*value[num2])))
						{
							num2++;
						}
						if (num2 >= value.Length)
						{
							goto IL_026E;
						}
					}
					if (global::System.Number.TrailingZeros(value, num2))
					{
						goto IL_026E;
					}
				}
			}
			IL_0269:
			result = 0;
			return false;
		}

		private unsafe static bool TryParseInt64IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out long result, ref bool failureIsOverflow)
		{
			if (value.Length >= 1)
			{
				bool flag = false;
				int num = 1;
				int num2 = 0;
				int num3 = (int)(*value[0]);
				if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && global::System.Number.IsWhite(num3))
				{
					do
					{
						num2++;
						if (num2 >= value.Length)
						{
							goto IL_0278;
						}
						num3 = (int)(*value[num2]);
					}
					while (global::System.Number.IsWhite(num3));
				}
				if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None)
				{
					string positiveSign = info.PositiveSign;
					string negativeSign = info.NegativeSign;
					if (positiveSign == "+" && negativeSign == "-")
					{
						if (num3 == 45)
						{
							num = -1;
							num2++;
							if (num2 >= value.Length)
							{
								goto IL_0278;
							}
							num3 = (int)(*value[num2]);
						}
						else if (num3 == 43)
						{
							num2++;
							if (num2 >= value.Length)
							{
								goto IL_0278;
							}
							num3 = (int)(*value[num2]);
						}
					}
					else
					{
						value = value.Slice(num2);
						num2 = 0;
						if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith<char>(positiveSign))
						{
							num2 += positiveSign.Length;
							if (num2 >= value.Length)
							{
								goto IL_0278;
							}
							num3 = (int)(*value[num2]);
						}
						else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith<char>(negativeSign))
						{
							num = -1;
							num2 += negativeSign.Length;
							if (num2 >= value.Length)
							{
								goto IL_0278;
							}
							num3 = (int)(*value[num2]);
						}
					}
				}
				long num4 = 0L;
				if (global::System.Number.IsDigit(num3))
				{
					if (num3 == 48)
					{
						do
						{
							num2++;
							if (num2 >= value.Length)
							{
								goto IL_027E;
							}
							num3 = (int)(*value[num2]);
						}
						while (num3 == 48);
						if (!global::System.Number.IsDigit(num3))
						{
							goto IL_0290;
						}
					}
					num4 = (long)(num3 - 48);
					num2++;
					for (int i = 0; i < 17; i++)
					{
						if (num2 >= value.Length)
						{
							goto IL_027E;
						}
						num3 = (int)(*value[num2]);
						if (!global::System.Number.IsDigit(num3))
						{
							goto IL_0290;
						}
						num2++;
						num4 = 10L * num4 + (long)num3 - 48L;
					}
					if (num2 < value.Length)
					{
						num3 = (int)(*value[num2]);
						if (!global::System.Number.IsDigit(num3))
						{
							goto IL_0290;
						}
						num2++;
						if (num4 > 922337203685477580L)
						{
							flag = true;
						}
						num4 = num4 * 10L + (long)num3 - 48L;
						if (num4 > 9223372036854775807L + (long)((-1 * num + 1) / 2))
						{
							flag = true;
						}
						if (num2 < value.Length)
						{
							num3 = (int)(*value[num2]);
							while (global::System.Number.IsDigit(num3))
							{
								flag = true;
								num2++;
								if (num2 >= value.Length)
								{
									goto IL_027E;
								}
								num3 = (int)(*value[num2]);
							}
							goto IL_0290;
						}
					}
					IL_027E:
					if (flag)
					{
						failureIsOverflow = true;
						goto IL_0278;
					}
					result = num4 * (long)num;
					return true;
					IL_0290:
					if (global::System.Number.IsWhite(num3))
					{
						if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
						{
							goto IL_0278;
						}
						num2++;
						while (num2 < value.Length && global::System.Number.IsWhite((int)(*value[num2])))
						{
							num2++;
						}
						if (num2 >= value.Length)
						{
							goto IL_027E;
						}
					}
					if (global::System.Number.TrailingZeros(value, num2))
					{
						goto IL_027E;
					}
				}
			}
			IL_0278:
			result = 0L;
			return false;
		}

		internal static bool TryParseInt64(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out long result)
		{
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				bool flag = false;
				return global::System.Number.TryParseInt64IntegerStyle(value, styles, info, out result, ref flag);
			}
			result = 0L;
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				bool flag2 = false;
				return global::System.Number.TryParseUInt64HexNumberStyle(value, styles, info, Unsafe.As<long, ulong>(ref result), ref flag2);
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			return global::System.Number.TryStringToNumber(value, styles, ref numberBuffer, info, false) && global::System.Number.NumberToInt64(ref numberBuffer, ref result);
		}

		internal static bool TryParseUInt32(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out uint result)
		{
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				bool flag = false;
				return global::System.Number.TryParseUInt32IntegerStyle(value, styles, info, out result, ref flag);
			}
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				bool flag2 = false;
				return global::System.Number.TryParseUInt32HexNumberStyle(value, styles, info, out result, ref flag2);
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			result = 0U;
			return global::System.Number.TryStringToNumber(value, styles, ref numberBuffer, info, false) && global::System.Number.NumberToUInt32(ref numberBuffer, ref result);
		}

		private unsafe static bool TryParseUInt32IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out uint result, ref bool failureIsOverflow)
		{
			if (value.Length >= 1)
			{
				bool flag = false;
				bool flag2 = false;
				int num = 0;
				int num2 = (int)(*value[0]);
				if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && global::System.Number.IsWhite(num2))
				{
					do
					{
						num++;
						if (num >= value.Length)
						{
							goto IL_025B;
						}
						num2 = (int)(*value[num]);
					}
					while (global::System.Number.IsWhite(num2));
				}
				if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None)
				{
					string positiveSign = info.PositiveSign;
					string negativeSign = info.NegativeSign;
					if (positiveSign == "+" && negativeSign == "-")
					{
						if (num2 == 43)
						{
							num++;
							if (num >= value.Length)
							{
								goto IL_025B;
							}
							num2 = (int)(*value[num]);
						}
						else if (num2 == 45)
						{
							flag2 = true;
							num++;
							if (num >= value.Length)
							{
								goto IL_025B;
							}
							num2 = (int)(*value[num]);
						}
					}
					else
					{
						value = value.Slice(num);
						num = 0;
						if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith<char>(positiveSign))
						{
							num += positiveSign.Length;
							if (num >= value.Length)
							{
								goto IL_025B;
							}
							num2 = (int)(*value[num]);
						}
						else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith<char>(negativeSign))
						{
							flag2 = true;
							num += negativeSign.Length;
							if (num >= value.Length)
							{
								goto IL_025B;
							}
							num2 = (int)(*value[num]);
						}
					}
				}
				int num3 = 0;
				if (global::System.Number.IsDigit(num2))
				{
					if (num2 == 48)
					{
						do
						{
							num++;
							if (num >= value.Length)
							{
								goto IL_0260;
							}
							num2 = (int)(*value[num]);
						}
						while (num2 == 48);
						if (!global::System.Number.IsDigit(num2))
						{
							goto IL_0276;
						}
					}
					num3 = num2 - 48;
					num++;
					for (int i = 0; i < 8; i++)
					{
						if (num >= value.Length)
						{
							goto IL_0260;
						}
						num2 = (int)(*value[num]);
						if (!global::System.Number.IsDigit(num2))
						{
							goto IL_0276;
						}
						num++;
						num3 = 10 * num3 + num2 - 48;
					}
					if (num < value.Length)
					{
						num2 = (int)(*value[num]);
						if (!global::System.Number.IsDigit(num2))
						{
							goto IL_0276;
						}
						num++;
						if (num3 > 429496729 || (num3 == 429496729 && num2 > 53))
						{
							flag = true;
						}
						num3 = num3 * 10 + num2 - 48;
						if (num < value.Length)
						{
							num2 = (int)(*value[num]);
							while (global::System.Number.IsDigit(num2))
							{
								flag = true;
								num++;
								if (num >= value.Length)
								{
									goto IL_0260;
								}
								num2 = (int)(*value[num]);
							}
							goto IL_0276;
						}
					}
					IL_0260:
					if (flag || (flag2 && num3 != 0))
					{
						failureIsOverflow = true;
						goto IL_025B;
					}
					result = (uint)num3;
					return true;
					IL_0276:
					if (global::System.Number.IsWhite(num2))
					{
						if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
						{
							goto IL_025B;
						}
						num++;
						while (num < value.Length && global::System.Number.IsWhite((int)(*value[num])))
						{
							num++;
						}
						if (num >= value.Length)
						{
							goto IL_0260;
						}
					}
					if (global::System.Number.TrailingZeros(value, num))
					{
						goto IL_0260;
					}
				}
			}
			IL_025B:
			result = 0U;
			return false;
		}

		private unsafe static bool TryParseUInt32HexNumberStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out uint result, ref bool failureIsOverflow)
		{
			if (value.Length >= 1)
			{
				bool flag = false;
				int num = 0;
				int i = (int)(*value[0]);
				if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && global::System.Number.IsWhite(i))
				{
					do
					{
						num++;
						if (num >= value.Length)
						{
							goto IL_0174;
						}
						i = (int)(*value[num]);
					}
					while (global::System.Number.IsWhite(i));
				}
				int num2 = 0;
				int[] array = global::System.Number.s_charToHexLookup;
				if (i < array.Length && array[i] != 255)
				{
					if (i == 48)
					{
						do
						{
							num++;
							if (num >= value.Length)
							{
								goto IL_0182;
							}
							i = (int)(*value[num]);
						}
						while (i == 48);
						if (i >= array.Length || array[i] == 255)
						{
							goto IL_0188;
						}
					}
					num2 = array[i];
					num++;
					for (int j = 0; j < 7; j++)
					{
						if (num >= value.Length)
						{
							goto IL_0182;
						}
						i = (int)(*value[num]);
						int num3;
						if (i >= array.Length || (num3 = array[i]) == 255)
						{
							goto IL_0188;
						}
						num++;
						num2 = 16 * num2 + num3;
					}
					if (num >= value.Length)
					{
						goto IL_0182;
					}
					i = (int)(*value[num]);
					if (i >= array.Length || array[i] == 255)
					{
						goto IL_0188;
					}
					num++;
					flag = true;
					if (num < value.Length)
					{
						for (i = (int)(*value[num]); i < array.Length; i = (int)(*value[num]))
						{
							if (array[i] == 255)
							{
								break;
							}
							num++;
							if (num >= value.Length)
							{
								goto IL_0179;
							}
						}
						goto IL_0188;
					}
					IL_0179:
					if (flag)
					{
						failureIsOverflow = true;
						goto IL_0174;
					}
					IL_0182:
					result = (uint)num2;
					return true;
					IL_0188:
					if (global::System.Number.IsWhite(i))
					{
						if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
						{
							goto IL_0174;
						}
						num++;
						while (num < value.Length && global::System.Number.IsWhite((int)(*value[num])))
						{
							num++;
						}
						if (num >= value.Length)
						{
							goto IL_0179;
						}
					}
					if (global::System.Number.TrailingZeros(value, num))
					{
						goto IL_0179;
					}
				}
			}
			IL_0174:
			result = 0U;
			return false;
		}

		internal static bool TryParseUInt64(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out ulong result)
		{
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				bool flag = false;
				return global::System.Number.TryParseUInt64IntegerStyle(value, styles, info, out result, ref flag);
			}
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				bool flag2 = false;
				return global::System.Number.TryParseUInt64HexNumberStyle(value, styles, info, out result, ref flag2);
			}
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			result = 0UL;
			return global::System.Number.TryStringToNumber(value, styles, ref numberBuffer, info, false) && global::System.Number.NumberToUInt64(ref numberBuffer, ref result);
		}

		private unsafe static bool TryParseUInt64IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out ulong result, ref bool failureIsOverflow)
		{
			if (value.Length >= 1)
			{
				bool flag = false;
				bool flag2 = false;
				int num = 0;
				int num2 = (int)(*value[0]);
				if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && global::System.Number.IsWhite(num2))
				{
					do
					{
						num++;
						if (num >= value.Length)
						{
							goto IL_0272;
						}
						num2 = (int)(*value[num]);
					}
					while (global::System.Number.IsWhite(num2));
				}
				if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None)
				{
					string positiveSign = info.PositiveSign;
					string negativeSign = info.NegativeSign;
					if (positiveSign == "+" && negativeSign == "-")
					{
						if (num2 == 43)
						{
							num++;
							if (num >= value.Length)
							{
								goto IL_0272;
							}
							num2 = (int)(*value[num]);
						}
						else if (num2 == 45)
						{
							flag2 = true;
							num++;
							if (num >= value.Length)
							{
								goto IL_0272;
							}
							num2 = (int)(*value[num]);
						}
					}
					else
					{
						value = value.Slice(num);
						num = 0;
						if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith<char>(positiveSign))
						{
							num += positiveSign.Length;
							if (num >= value.Length)
							{
								goto IL_0272;
							}
							num2 = (int)(*value[num]);
						}
						else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith<char>(negativeSign))
						{
							flag2 = true;
							num += negativeSign.Length;
							if (num >= value.Length)
							{
								goto IL_0272;
							}
							num2 = (int)(*value[num]);
						}
					}
				}
				long num3 = 0L;
				if (global::System.Number.IsDigit(num2))
				{
					if (num2 == 48)
					{
						do
						{
							num++;
							if (num >= value.Length)
							{
								goto IL_0278;
							}
							num2 = (int)(*value[num]);
						}
						while (num2 == 48);
						if (!global::System.Number.IsDigit(num2))
						{
							goto IL_028E;
						}
					}
					num3 = (long)(num2 - 48);
					num++;
					for (int i = 0; i < 18; i++)
					{
						if (num >= value.Length)
						{
							goto IL_0278;
						}
						num2 = (int)(*value[num]);
						if (!global::System.Number.IsDigit(num2))
						{
							goto IL_028E;
						}
						num++;
						num3 = 10L * num3 + (long)num2 - 48L;
					}
					if (num < value.Length)
					{
						num2 = (int)(*value[num]);
						if (!global::System.Number.IsDigit(num2))
						{
							goto IL_028E;
						}
						num++;
						if (num3 > 1844674407370955161L || (num3 == 1844674407370955161L && num2 > 53))
						{
							flag = true;
						}
						num3 = num3 * 10L + (long)num2 - 48L;
						if (num < value.Length)
						{
							num2 = (int)(*value[num]);
							while (global::System.Number.IsDigit(num2))
							{
								flag = true;
								num++;
								if (num >= value.Length)
								{
									goto IL_0278;
								}
								num2 = (int)(*value[num]);
							}
							goto IL_028E;
						}
					}
					IL_0278:
					if (flag || (flag2 && num3 != 0L))
					{
						failureIsOverflow = true;
						goto IL_0272;
					}
					result = (ulong)num3;
					return true;
					IL_028E:
					if (global::System.Number.IsWhite(num2))
					{
						if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
						{
							goto IL_0272;
						}
						num++;
						while (num < value.Length && global::System.Number.IsWhite((int)(*value[num])))
						{
							num++;
						}
						if (num >= value.Length)
						{
							goto IL_0278;
						}
					}
					if (global::System.Number.TrailingZeros(value, num))
					{
						goto IL_0278;
					}
				}
			}
			IL_0272:
			result = 0UL;
			return false;
		}

		private unsafe static bool TryParseUInt64HexNumberStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out ulong result, ref bool failureIsOverflow)
		{
			if (value.Length >= 1)
			{
				bool flag = false;
				int num = 0;
				int i = (int)(*value[0]);
				if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && global::System.Number.IsWhite(i))
				{
					do
					{
						num++;
						if (num >= value.Length)
						{
							goto IL_0179;
						}
						i = (int)(*value[num]);
					}
					while (global::System.Number.IsWhite(i));
				}
				long num2 = 0L;
				int[] array = global::System.Number.s_charToHexLookup;
				if (i < array.Length && array[i] != 255)
				{
					if (i == 48)
					{
						do
						{
							num++;
							if (num >= value.Length)
							{
								goto IL_0188;
							}
							i = (int)(*value[num]);
						}
						while (i == 48);
						if (i >= array.Length || array[i] == 255)
						{
							goto IL_018E;
						}
					}
					num2 = (long)array[i];
					num++;
					for (int j = 0; j < 15; j++)
					{
						if (num >= value.Length)
						{
							goto IL_0188;
						}
						i = (int)(*value[num]);
						int num3;
						if (i >= array.Length || (num3 = array[i]) == 255)
						{
							goto IL_018E;
						}
						num++;
						num2 = 16L * num2 + (long)num3;
					}
					if (num >= value.Length)
					{
						goto IL_0188;
					}
					i = (int)(*value[num]);
					if (i >= array.Length || array[i] == 255)
					{
						goto IL_018E;
					}
					num++;
					flag = true;
					if (num < value.Length)
					{
						for (i = (int)(*value[num]); i < array.Length; i = (int)(*value[num]))
						{
							if (array[i] == 255)
							{
								break;
							}
							num++;
							if (num >= value.Length)
							{
								goto IL_017F;
							}
						}
						goto IL_018E;
					}
					IL_017F:
					if (flag)
					{
						failureIsOverflow = true;
						goto IL_0179;
					}
					IL_0188:
					result = (ulong)num2;
					return true;
					IL_018E:
					if (global::System.Number.IsWhite(i))
					{
						if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
						{
							goto IL_0179;
						}
						num++;
						while (num < value.Length && global::System.Number.IsWhite((int)(*value[num])))
						{
							num++;
						}
						if (num >= value.Length)
						{
							goto IL_017F;
						}
					}
					if (global::System.Number.TrailingZeros(value, num))
					{
						goto IL_017F;
					}
				}
			}
			IL_0179:
			result = 0UL;
			return false;
		}

		internal static decimal ParseDecimal(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			decimal num = 0m;
			global::System.Number.StringToNumber(value, styles, ref numberBuffer, info, true);
			if (!global::System.Number.NumberBufferToDecimal(ref numberBuffer, ref num))
			{
				global::System.Number.ThrowOverflowOrFormatException(true, "Value was either too large or too small for a Decimal.");
			}
			return num;
		}

		private unsafe static bool NumberBufferToDecimal(ref global::System.Number.NumberBuffer number, ref decimal value)
		{
			char* ptr = number.digits;
			int i = number.scale;
			bool sign = number.sign;
			uint num = (uint)(*ptr);
			if (num == 0U)
			{
				value = new decimal(0, 0, 0, sign, (byte)Math.Clamp(-i, 0, 28));
				return true;
			}
			if (i > 29)
			{
				return false;
			}
			ulong num2 = 0UL;
			while (i > -28)
			{
				i--;
				num2 *= 10UL;
				num2 += (ulong)(num - 48U);
				num = (uint)(*(++ptr));
				if (num2 >= 1844674407370955161UL)
				{
					break;
				}
				if (num == 0U)
				{
					while (i > 0)
					{
						i--;
						num2 *= 10UL;
						if (num2 >= 1844674407370955161UL)
						{
							break;
						}
					}
					break;
				}
			}
			uint num3 = 0U;
			while ((i > 0 || (num != 0U && i > -28)) && (num3 < 429496729U || (num3 == 429496729U && (num2 < 11068046444225730969UL || (num2 == 11068046444225730969UL && num <= 53U)))))
			{
				ulong num4 = (ulong)((uint)num2) * 10UL;
				ulong num5 = (ulong)((uint)(num2 >> 32)) * 10UL + (num4 >> 32);
				num2 = (ulong)((uint)num4) + (num5 << 32);
				num3 = (uint)(num5 >> 32) + num3 * 10U;
				if (num != 0U)
				{
					num -= 48U;
					num2 += (ulong)num;
					if (num2 < (ulong)num)
					{
						num3 += 1U;
					}
					num = (uint)(*(++ptr));
				}
				i--;
			}
			if (num >= 53U)
			{
				if (num == 53U && (num2 & 1UL) == 0UL)
				{
					num = (uint)(*(++ptr));
					int num6 = 20;
					while (num == 48U && num6 != 0)
					{
						num = (uint)(*(++ptr));
						num6--;
					}
					if (num == 0U || num6 == 0)
					{
						goto IL_01A0;
					}
				}
				if ((num2 += 1UL) == 0UL && (num3 += 1U) == 0U)
				{
					num2 = 11068046444225730970UL;
					num3 = 429496729U;
					i++;
				}
			}
			IL_01A0:
			if (i > 0)
			{
				return false;
			}
			if (i <= -29)
			{
				value = new decimal(0, 0, 0, sign, 28);
			}
			else
			{
				value = new decimal((int)num2, (int)(num2 >> 32), (int)num3, sign, (byte)(-(byte)i));
			}
			return true;
		}

		internal static double ParseDouble(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			double num = 0.0;
			if (!global::System.Number.TryStringToNumber(value, styles, ref numberBuffer, info, false))
			{
				ReadOnlySpan<char> readOnlySpan = value.Trim();
				if (readOnlySpan.EqualsOrdinal(info.PositiveInfinitySymbol))
				{
					return double.PositiveInfinity;
				}
				if (readOnlySpan.EqualsOrdinal(info.NegativeInfinitySymbol))
				{
					return double.NegativeInfinity;
				}
				if (readOnlySpan.EqualsOrdinal(info.NaNSymbol))
				{
					return double.NaN;
				}
				global::System.Number.ThrowOverflowOrFormatException(false, null);
			}
			if (!global::System.Number.NumberBufferToDouble(ref numberBuffer, ref num))
			{
				global::System.Number.ThrowOverflowOrFormatException(true, "Value was either too large or too small for a Double.");
			}
			return num;
		}

		internal static float ParseSingle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			double num = 0.0;
			if (!global::System.Number.TryStringToNumber(value, styles, ref numberBuffer, info, false))
			{
				ReadOnlySpan<char> readOnlySpan = value.Trim();
				if (readOnlySpan.EqualsOrdinal(info.PositiveInfinitySymbol))
				{
					return float.PositiveInfinity;
				}
				if (readOnlySpan.EqualsOrdinal(info.NegativeInfinitySymbol))
				{
					return float.NegativeInfinity;
				}
				if (readOnlySpan.EqualsOrdinal(info.NaNSymbol))
				{
					return float.NaN;
				}
				global::System.Number.ThrowOverflowOrFormatException(false, null);
			}
			if (!global::System.Number.NumberBufferToDouble(ref numberBuffer, ref num))
			{
				global::System.Number.ThrowOverflowOrFormatException(true, "Value was either too large or too small for a Single.");
			}
			float num2 = (float)num;
			if (float.IsInfinity(num2))
			{
				global::System.Number.ThrowOverflowOrFormatException(true, "Value was either too large or too small for a Single.");
			}
			return num2;
		}

		internal static bool TryParseDecimal(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out decimal result)
		{
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			result = 0m;
			return global::System.Number.TryStringToNumber(value, styles, ref numberBuffer, info, true) && global::System.Number.NumberBufferToDecimal(ref numberBuffer, ref result);
		}

		internal static bool TryParseDouble(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out double result)
		{
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			result = 0.0;
			return global::System.Number.TryStringToNumber(value, styles, ref numberBuffer, info, false) && global::System.Number.NumberBufferToDouble(ref numberBuffer, ref result);
		}

		internal static bool TryParseSingle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out float result)
		{
			global::System.Number.NumberBuffer numberBuffer = default(global::System.Number.NumberBuffer);
			result = 0f;
			double num = 0.0;
			if (!global::System.Number.TryStringToNumber(value, styles, ref numberBuffer, info, false))
			{
				return false;
			}
			if (!global::System.Number.NumberBufferToDouble(ref numberBuffer, ref num))
			{
				return false;
			}
			float num2 = (float)num;
			if (float.IsInfinity(num2))
			{
				return false;
			}
			result = num2;
			return true;
		}

		private unsafe static void StringToNumber(ReadOnlySpan<char> value, NumberStyles styles, ref global::System.Number.NumberBuffer number, NumberFormatInfo info, bool parseDecimal)
		{
			fixed (char* reference = MemoryMarshal.GetReference<char>(value))
			{
				char* ptr = reference;
				char* ptr2 = ptr;
				if (!global::System.Number.ParseNumber(ref ptr2, ptr2 + value.Length, styles, ref number, info, parseDecimal) || ((long)(ptr2 - ptr) < (long)value.Length && !global::System.Number.TrailingZeros(value, (int)((long)(ptr2 - ptr)))))
				{
					global::System.Number.ThrowOverflowOrFormatException(false, null);
				}
			}
		}

		internal unsafe static bool TryStringToNumber(ReadOnlySpan<char> value, NumberStyles styles, ref global::System.Number.NumberBuffer number, NumberFormatInfo info, bool parseDecimal)
		{
			fixed (char* reference = MemoryMarshal.GetReference<char>(value))
			{
				char* ptr = reference;
				char* ptr2 = ptr;
				if (!global::System.Number.ParseNumber(ref ptr2, ptr2 + value.Length, styles, ref number, info, parseDecimal) || ((long)(ptr2 - ptr) < (long)value.Length && !global::System.Number.TrailingZeros(value, (int)((long)(ptr2 - ptr)))))
				{
					return false;
				}
			}
			return true;
		}

		private unsafe static bool TrailingZeros(ReadOnlySpan<char> value, int index)
		{
			for (int i = index; i < value.Length; i++)
			{
				if (*value[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		private unsafe static char* MatchChars(char* p, char* pEnd, string value)
		{
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr;
				if (*ptr2 != '\0')
				{
					do
					{
						char c = ((p < pEnd) ? (*p) : '\0');
						if (c != *ptr2 && (*ptr2 != '\u00a0' || c != ' '))
						{
							goto IL_0042;
						}
						p++;
						ptr2++;
					}
					while (*ptr2 != '\0');
					return p;
				}
				IL_0042:;
			}
			return null;
		}

		private static bool IsWhite(int ch)
		{
			return ch == 32 || ch - 9 <= 4;
		}

		private static bool IsDigit(int ch)
		{
			return ch - 48 <= 9;
		}

		private static void ThrowOverflowOrFormatException(bool overflow, string overflowResourceKey)
		{
			throw overflow ? new OverflowException(SR.GetResourceString(overflowResourceKey)) : new FormatException("Input string was not in a correct format.");
		}

		private static bool NumberBufferToDouble(ref global::System.Number.NumberBuffer number, ref double value)
		{
			double num = global::System.Number.NumberToDouble(ref number);
			if (!double.IsFinite(num))
			{
				value = 0.0;
				return false;
			}
			if (num == 0.0)
			{
				num = 0.0;
			}
			value = num;
			return true;
		}

		private unsafe static uint DigitsToInt(char* p, int count)
		{
			char* ptr = p + count;
			uint num = (uint)(*p - '0');
			for (p++; p < ptr; p++)
			{
				num = 10U * num + (uint)(*p) - 48U;
			}
			return num;
		}

		private static ulong Mul32x32To64(uint a, uint b)
		{
			return (ulong)a * (ulong)b;
		}

		private static ulong Mul64Lossy(ulong a, ulong b, ref int pexp)
		{
			ulong num = global::System.Number.Mul32x32To64((uint)(a >> 32), (uint)(b >> 32)) + (global::System.Number.Mul32x32To64((uint)(a >> 32), (uint)b) >> 32) + (global::System.Number.Mul32x32To64((uint)a, (uint)(b >> 32)) >> 32);
			if ((num & 9223372036854775808UL) == 0UL)
			{
				num <<= 1;
				pexp--;
			}
			return num;
		}

		private static int abs(int value)
		{
			if (value < 0)
			{
				return -value;
			}
			return value;
		}

		private unsafe static double NumberToDouble(ref global::System.Number.NumberBuffer number)
		{
			char* ptr = number.digits;
			int num = string.wcslen(ptr);
			int num2 = num;
			while (*ptr == '0')
			{
				num2--;
				ptr++;
			}
			if (num2 == 0)
			{
				return 0.0;
			}
			int num3 = Math.Min(num2, 9);
			num2 -= num3;
			ulong num4 = (ulong)global::System.Number.DigitsToInt(ptr, num3);
			if (num2 > 0)
			{
				num3 = Math.Min(num2, 9);
				num2 -= num3;
				uint num5 = (uint)(global::System.Number.s_rgval64Power10[num3 - 1] >> (int)(64 - global::System.Number.s_rgexp64Power10[num3 - 1]));
				num4 = global::System.Number.Mul32x32To64((uint)num4, num5) + (ulong)global::System.Number.DigitsToInt(ptr + 9, num3);
			}
			int num6 = number.scale - (num - num2);
			int num7 = global::System.Number.abs(num6);
			if (num7 >= 352)
			{
				ulong num8 = ((num6 > 0) ? 9218868437227405312UL : 0UL);
				if (number.sign)
				{
					num8 |= 9223372036854775808UL;
				}
				return *(double*)(&num8);
			}
			int num9 = 64;
			if ((num4 & 18446744069414584320UL) == 0UL)
			{
				num4 <<= 32;
				num9 -= 32;
			}
			if ((num4 & 18446462598732840960UL) == 0UL)
			{
				num4 <<= 16;
				num9 -= 16;
			}
			if ((num4 & 18374686479671623680UL) == 0UL)
			{
				num4 <<= 8;
				num9 -= 8;
			}
			if ((num4 & 17293822569102704640UL) == 0UL)
			{
				num4 <<= 4;
				num9 -= 4;
			}
			if ((num4 & 13835058055282163712UL) == 0UL)
			{
				num4 <<= 2;
				num9 -= 2;
			}
			if ((num4 & 9223372036854775808UL) == 0UL)
			{
				num4 <<= 1;
				num9--;
			}
			int num10 = num7 & 15;
			if (num10 != 0)
			{
				int num11 = (int)global::System.Number.s_rgexp64Power10[num10 - 1];
				num9 += ((num6 < 0) ? (-num11 + 1) : num11);
				ulong num12 = global::System.Number.s_rgval64Power10[num10 + ((num6 < 0) ? 15 : 0) - 1];
				num4 = global::System.Number.Mul64Lossy(num4, num12, ref num9);
			}
			num10 = num7 >> 4;
			if (num10 != 0)
			{
				int num13 = (int)global::System.Number.s_rgexp64Power10By16[num10 - 1];
				num9 += ((num6 < 0) ? (-num13 + 1) : num13);
				ulong num14 = global::System.Number.s_rgval64Power10By16[num10 + ((num6 < 0) ? 21 : 0) - 1];
				num4 = global::System.Number.Mul64Lossy(num4, num14, ref num9);
			}
			if (((int)num4 & 1024) != 0)
			{
				ulong num15 = num4 + 1023UL + (ulong)((long)(((int)num4 >> 11) & 1));
				if (num15 < num4)
				{
					num15 = (num15 >> 1) | 9223372036854775808UL;
					num9++;
				}
				num4 = num15;
			}
			num9 += 1022;
			if (num9 <= 0)
			{
				if (num9 == -52 && num4 >= 9223372036854775896UL)
				{
					num4 = 1UL;
				}
				else if (num9 <= -52)
				{
					num4 = 0UL;
				}
				else
				{
					num4 >>= -num9 + 11 + 1;
				}
			}
			else if (num9 >= 2047)
			{
				num4 = 9218868437227405312UL;
			}
			else
			{
				num4 = (ulong)(((long)num9 << 52) + (long)((num4 >> 11) & 4503599627370495UL));
			}
			if (number.sign)
			{
				num4 |= 9223372036854775808UL;
			}
			return *(double*)(&num4);
		}

		private unsafe static void DoubleToNumber(double value, int precision, ref global::System.Number.NumberBuffer number)
		{
			number.precision = precision;
			if (!double.IsFinite(value))
			{
				number.scale = (double.IsNaN(value) ? int.MinValue : int.MaxValue);
				number.sign = double.IsNegative(value);
				*number.digits = '\0';
				return;
			}
			byte* ptr = stackalloc byte[(UIntPtr)349];
			int num;
			fixed (global::System.Number.NumberBuffer* ptr2 = &number)
			{
				global::System.Number.NumberBuffer* ptr3 = ptr2;
				RuntimeImports._ecvt_s(ptr, 349, value, precision, &ptr3->scale, &num);
			}
			number.sign = num != 0;
			char* digits = number.digits;
			if (*ptr != 48)
			{
				while (*ptr != 0)
				{
					*(digits++) = (char)(*(ptr++));
				}
			}
			*digits = '\0';
		}

		internal const int DecimalPrecision = 29;

		private const int FloatPrecision = 7;

		private const int DoublePrecision = 15;

		private const int ScaleNAN = -2147483648;

		private const int ScaleINF = 2147483647;

		private const int MaxUInt32DecDigits = 10;

		private const int CharStackBufferSize = 32;

		private const string PosNumberFormat = "#";

		private static readonly string[] s_posCurrencyFormats = new string[] { "$#", "#$", "$ #", "# $" };

		private static readonly string[] s_negCurrencyFormats = new string[]
		{
			"($#)", "-$#", "$-#", "$#-", "(#$)", "-#$", "#-$", "#$-", "-# $", "-$ #",
			"# $-", "$ #-", "$ -#", "#- $", "($ #)", "(# $)"
		};

		private static readonly string[] s_posPercentFormats = new string[] { "# %", "#%", "%#", "% #" };

		private static readonly string[] s_negPercentFormats = new string[]
		{
			"-# %", "-#%", "-%#", "%-#", "%#-", "#-%", "#%-", "-% #", "# %-", "% #-",
			"% -#", "#- %"
		};

		private static readonly string[] s_negNumberFormats = new string[] { "(#)", "-#", "- #", "#-", "# -" };

		private const int NumberMaxDigits = 50;

		private const int Int32Precision = 10;

		private const int UInt32Precision = 10;

		private const int Int64Precision = 19;

		private const int UInt64Precision = 20;

		private static readonly int[] s_charToHexLookup = new int[]
		{
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 0, 1,
			2, 3, 4, 5, 6, 7, 8, 9, 255, 255,
			255, 255, 255, 255, 255, 10, 11, 12, 13, 14,
			15, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 10, 11, 12,
			13, 14, 15, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255
		};

		private const int _CVTBUFSIZE = 349;

		private static readonly ulong[] s_rgval64Power10 = new ulong[]
		{
			11529215046068469760UL, 14411518807585587200UL, 18014398509481984000UL, 11258999068426240000UL, 14073748835532800000UL, 17592186044416000000UL, 10995116277760000000UL, 13743895347200000000UL, 17179869184000000000UL, 10737418240000000000UL,
			13421772800000000000UL, 16777216000000000000UL, 10485760000000000000UL, 13107200000000000000UL, 16384000000000000000UL, 14757395258967641293UL, 11805916207174113035UL, 9444732965739290428UL, 15111572745182864686UL, 12089258196146291749UL,
			9671406556917033399UL, 15474250491067253438UL, 12379400392853802751UL, 9903520314283042201UL, 15845632502852867522UL, 12676506002282294018UL, 10141204801825835215UL, 16225927682921336344UL, 12980742146337069075UL, 10384593717069655260UL
		};

		private static readonly sbyte[] s_rgexp64Power10 = new sbyte[]
		{
			4, 7, 10, 14, 17, 20, 24, 27, 30, 34,
			37, 40, 44, 47, 50
		};

		private static readonly ulong[] s_rgval64Power10By16 = new ulong[]
		{
			10240000000000000000UL, 11368683772161602974UL, 12621774483536188886UL, 14012984643248170708UL, 15557538194652854266UL, 17272337110188889248UL, 9588073174409622172UL, 10644899600020376798UL, 11818212630765741798UL, 13120851772591970216UL,
			14567071740625403792UL, 16172698447808779622UL, 17955302187076837696UL, 9967194951097567532UL, 11065809325636130658UL, 12285516299433008778UL, 13639663065038175358UL, 15143067982934716296UL, 16812182738118149112UL, 9332636185032188787UL,
			10361307573072618722UL, 16615349947311448416UL, 14965776766268445891UL, 13479973333575319909UL, 12141680576410806707UL, 10936253623915059637UL, 9850501549098619819UL, 17745086042373215136UL, 15983352577617880260UL, 14396524142538228461UL,
			12967236152753103031UL, 11679847981112819795UL, 10520271803096747049UL, 9475818434452569218UL, 17070116948172427008UL, 15375394465392026135UL, 13848924157002783096UL, 12474001934591998882UL, 11235582092889474480UL, 10120112665365530972UL,
			18230774251475056952UL, 16420821625123739930UL
		};

		private static readonly short[] s_rgexp64Power10By16 = new short[]
		{
			54, 107, 160, 213, 266, 319, 373, 426, 479, 532,
			585, 638, 691, 745, 798, 851, 904, 957, 1010, 1064,
			1117
		};

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal ref struct NumberBuffer
		{
			public bool sign
			{
				get
				{
					return this._sign != 0;
				}
				set
				{
					this._sign = (value ? 1 : 0);
				}
			}

			public unsafe char* digits
			{
				get
				{
					return (char*)Unsafe.AsPointer<global::System.Number.NumberBuffer.DigitsAndNullTerminator>(ref this._digits);
				}
			}

			public int precision;

			public int scale;

			private int _sign;

			private global::System.Number.NumberBuffer.DigitsAndNullTerminator _digits;

			private unsafe char* _allDigits;

			private struct DigitsAndNullTerminator
			{
			}
		}
	}
}
