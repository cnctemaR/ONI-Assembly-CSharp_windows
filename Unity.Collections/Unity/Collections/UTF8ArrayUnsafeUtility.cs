using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	public static class UTF8ArrayUnsafeUtility
	{
		public unsafe static CopyError Copy(byte* dest, out int destLength, int destUTF8MaxLengthInBytes, char* src, int srcLength)
		{
			if (Unicode.Utf16ToUtf8(src, srcLength, dest, out destLength, destUTF8MaxLengthInBytes) == ConversionError.None)
			{
				return CopyError.None;
			}
			return CopyError.Truncation;
		}

		public unsafe static CopyError Copy(byte* dest, out ushort destLength, ushort destUTF8MaxLengthInBytes, char* src, int srcLength)
		{
			int num;
			bool flag = Unicode.Utf16ToUtf8(src, srcLength, dest, out num, (int)destUTF8MaxLengthInBytes) != ConversionError.None;
			destLength = (ushort)num;
			if (!flag)
			{
				return CopyError.None;
			}
			return CopyError.Truncation;
		}

		public unsafe static CopyError Copy(byte* dest, out int destLength, int destUTF8MaxLengthInBytes, byte* src, int srcLength)
		{
			int num;
			bool flag = Unicode.Utf8ToUtf8(src, srcLength, dest, out num, destUTF8MaxLengthInBytes) != ConversionError.None;
			destLength = num;
			if (!flag)
			{
				return CopyError.None;
			}
			return CopyError.Truncation;
		}

		public unsafe static CopyError Copy(byte* dest, out ushort destLength, ushort destUTF8MaxLengthInBytes, byte* src, ushort srcLength)
		{
			int num;
			bool flag = Unicode.Utf8ToUtf8(src, (int)srcLength, dest, out num, (int)destUTF8MaxLengthInBytes) != ConversionError.None;
			destLength = (ushort)num;
			if (!flag)
			{
				return CopyError.None;
			}
			return CopyError.Truncation;
		}

		public unsafe static CopyError Copy(char* dest, out int destLength, int destUCS2MaxLengthInChars, byte* src, int srcLength)
		{
			if (Unicode.Utf8ToUtf16(src, srcLength, dest, out destLength, destUCS2MaxLengthInChars) == ConversionError.None)
			{
				return CopyError.None;
			}
			return CopyError.Truncation;
		}

		public unsafe static CopyError Copy(char* dest, out ushort destLength, ushort destUCS2MaxLengthInChars, byte* src, ushort srcLength)
		{
			int num;
			bool flag = Unicode.Utf8ToUtf16(src, (int)srcLength, dest, out num, (int)destUCS2MaxLengthInChars) != ConversionError.None;
			destLength = (ushort)num;
			if (!flag)
			{
				return CopyError.None;
			}
			return CopyError.Truncation;
		}

		public unsafe static FormatError AppendUTF8Bytes(byte* dest, ref int destLength, int destCapacity, byte* src, int srcLength)
		{
			if (destLength + srcLength > destCapacity)
			{
				return FormatError.Overflow;
			}
			UnsafeUtility.MemCpy((void*)(dest + destLength), (void*)src, (long)srcLength);
			destLength += srcLength;
			return FormatError.None;
		}

		public unsafe static CopyError Append(byte* dest, ref ushort destLength, ushort destUTF8MaxLengthInBytes, byte* src, ushort srcLength)
		{
			int num;
			bool flag = Unicode.Utf8ToUtf8(src, (int)srcLength, dest + destLength, out num, (int)(destUTF8MaxLengthInBytes - destLength)) != ConversionError.None;
			destLength += (ushort)num;
			if (!flag)
			{
				return CopyError.None;
			}
			return CopyError.Truncation;
		}

		public unsafe static CopyError Append(byte* dest, ref ushort destLength, ushort destUTF8MaxLengthInBytes, char* src, int srcLength)
		{
			int num;
			bool flag = Unicode.Utf16ToUtf8(src, srcLength, dest + destLength, out num, (int)(destUTF8MaxLengthInBytes - destLength)) != ConversionError.None;
			destLength += (ushort)num;
			if (!flag)
			{
				return CopyError.None;
			}
			return CopyError.Truncation;
		}

		public unsafe static CopyError Append(char* dest, ref ushort destLength, ushort destUCS2MaxLengthInChars, byte* src, ushort srcLength)
		{
			int num;
			bool flag = Unicode.Utf8ToUtf16(src, (int)srcLength, dest + destLength, out num, (int)(destUCS2MaxLengthInChars - destLength)) != ConversionError.None;
			destLength += (ushort)num;
			if (!flag)
			{
				return CopyError.None;
			}
			return CopyError.Truncation;
		}

		public unsafe static int StrCmp(byte* utf8BufferA, int utf8LengthInBytesA, byte* utf8BufferB, int utf8LengthInBytesB)
		{
			int num = 0;
			int num2 = 0;
			UTF8ArrayUnsafeUtility.Comparison comparison;
			do
			{
				Unicode.Rune rune;
				ConversionError conversionError = Unicode.Utf8ToUcs(out rune, utf8BufferA, ref num, utf8LengthInBytesA);
				Unicode.Rune rune2;
				ConversionError conversionError2 = Unicode.Utf8ToUcs(out rune2, utf8BufferB, ref num2, utf8LengthInBytesB);
				comparison = new UTF8ArrayUnsafeUtility.Comparison(rune, conversionError, rune2, conversionError2);
			}
			while (!comparison.terminates);
			return comparison.result;
		}

		internal unsafe static int StrCmp(byte* utf8BufferA, int utf8LengthInBytesA, Unicode.Rune* runeBufferB, int lengthInRunesB)
		{
			int num = 0;
			int num2 = 0;
			UTF8ArrayUnsafeUtility.Comparison comparison;
			do
			{
				Unicode.Rune rune;
				ConversionError conversionError = Unicode.Utf8ToUcs(out rune, utf8BufferA, ref num, utf8LengthInBytesA);
				Unicode.Rune rune2;
				ConversionError conversionError2 = Unicode.UcsToUcs(out rune2, runeBufferB, ref num2, lengthInRunesB);
				comparison = new UTF8ArrayUnsafeUtility.Comparison(rune, conversionError, rune2, conversionError2);
			}
			while (!comparison.terminates);
			return comparison.result;
		}

		public unsafe static int StrCmp(char* utf16BufferA, int utf16LengthInCharsA, char* utf16BufferB, int utf16LengthInCharsB)
		{
			int num = 0;
			int num2 = 0;
			UTF8ArrayUnsafeUtility.Comparison comparison;
			do
			{
				Unicode.Rune rune;
				ConversionError conversionError = Unicode.Utf16ToUcs(out rune, utf16BufferA, ref num, utf16LengthInCharsA);
				Unicode.Rune rune2;
				ConversionError conversionError2 = Unicode.Utf16ToUcs(out rune2, utf16BufferB, ref num2, utf16LengthInCharsB);
				comparison = new UTF8ArrayUnsafeUtility.Comparison(rune, conversionError, rune2, conversionError2);
			}
			while (!comparison.terminates);
			return comparison.result;
		}

		public unsafe static bool EqualsUTF8Bytes(byte* aBytes, int aLength, byte* bBytes, int bLength)
		{
			return aLength == bLength && UTF8ArrayUnsafeUtility.StrCmp(aBytes, aLength, bBytes, bLength) == 0;
		}

		public unsafe static int StrCmp(byte* utf8Buffer, int utf8LengthInBytes, char* utf16Buffer, int utf16LengthInChars)
		{
			int num = 0;
			int num2 = 0;
			UTF8ArrayUnsafeUtility.Comparison comparison;
			do
			{
				Unicode.Rune rune;
				ConversionError conversionError = Unicode.Utf8ToUcs(out rune, utf8Buffer, ref num, utf8LengthInBytes);
				Unicode.Rune rune2;
				ConversionError conversionError2 = Unicode.Utf16ToUcs(out rune2, utf16Buffer, ref num2, utf16LengthInChars);
				comparison = new UTF8ArrayUnsafeUtility.Comparison(rune, conversionError, rune2, conversionError2);
			}
			while (!comparison.terminates);
			return comparison.result;
		}

		public unsafe static int StrCmp(char* utf16Buffer, int utf16LengthInChars, byte* utf8Buffer, int utf8LengthInBytes)
		{
			return -UTF8ArrayUnsafeUtility.StrCmp(utf8Buffer, utf8LengthInBytes, utf16Buffer, utf16LengthInChars);
		}

		internal struct Comparison
		{
			public Comparison(Unicode.Rune runeA, ConversionError errorA, Unicode.Rune runeB, ConversionError errorB)
			{
				if (errorA != ConversionError.None)
				{
					runeA.value = 0;
				}
				if (errorB != ConversionError.None)
				{
					runeB.value = 0;
				}
				if (runeA.value != runeB.value)
				{
					this.result = runeA.value - runeB.value;
					this.terminates = true;
					return;
				}
				this.result = 0;
				this.terminates = runeA.value == 0 && runeB.value == 0;
			}

			public bool terminates;

			public int result;
		}
	}
}
