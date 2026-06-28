using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace I18N.Common
{
	[Serializable]
	public abstract class MonoEncoding : Encoding
	{
		public MonoEncoding(int codePage)
			: this(codePage, 0)
		{
		}

		public MonoEncoding(int codePage, int windowsCodePage)
			: base(codePage)
		{
			this.win_code_page = windowsCodePage;
		}

		public override int WindowsCodePage
		{
			get
			{
				return (this.win_code_page == 0) ? base.WindowsCodePage : this.win_code_page;
			}
		}

		public unsafe void HandleFallback(ref EncoderFallbackBuffer buffer, char* chars, ref int charIndex, ref int charCount, byte* bytes, ref int byteIndex, ref int byteCount)
		{
			if (buffer == null)
			{
				buffer = base.EncoderFallback.CreateFallbackBuffer();
			}
			if (char.IsSurrogate(chars[charIndex]) && charCount > 0 && char.IsSurrogate(chars[charIndex + 1]))
			{
				buffer.Fallback(chars[charIndex], chars[charIndex + 1], charIndex);
				charIndex++;
				charCount--;
			}
			else
			{
				buffer.Fallback(chars[charIndex], charIndex);
			}
			char[] array = new char[buffer.Remaining];
			int num = 0;
			while (buffer.Remaining > 0)
			{
				array[num++] = buffer.GetNextChar();
			}
			fixed (char* ptr = (ref array != null && array.Length != 0 ? ref array[0] : ref *null))
			{
				byteIndex += this.GetBytes(ptr, array.Length, bytes + byteIndex, byteCount);
			}
		}

		public unsafe override int GetByteCount(char[] chars, int index, int count)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (index < 0 || index > chars.Length)
			{
				throw new ArgumentOutOfRangeException("index", Strings.GetString("ArgRange_Array"));
			}
			if (count < 0 || count > chars.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", Strings.GetString("ArgRange_Array"));
			}
			if (count == 0)
			{
				return 0;
			}
			fixed (char* ptr = (ref chars != null && chars.Length != 0 ? ref chars[0] : ref *null))
			{
				return this.GetByteCountImpl(ptr + index, count);
			}
		}

		public unsafe override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", Strings.GetString("ArgRange_Array"));
			}
			if (charCount < 0 || charCount > chars.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", Strings.GetString("ArgRange_Array"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Strings.GetString("ArgRange_Array"));
			}
			if (bytes.Length - byteIndex < charCount)
			{
				throw new ArgumentException(Strings.GetString("Arg_InsufficientSpace"), "bytes");
			}
			if (charCount == 0)
			{
				return 0;
			}
			fixed (char* ptr = (ref chars != null && chars.Length != 0 ? ref chars[0] : ref *null))
			{
				fixed (byte* ptr2 = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
				{
					return this.GetBytesImpl(ptr + charIndex, charCount, ptr2 + byteIndex, bytes.Length - byteIndex);
				}
			}
		}

		public unsafe override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || charIndex > s.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", Strings.GetString("ArgRange_StringIndex"));
			}
			if (charCount < 0 || charCount > s.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", Strings.GetString("ArgRange_StringRange"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Strings.GetString("ArgRange_Array"));
			}
			if (bytes.Length - byteIndex < charCount)
			{
				throw new ArgumentException(Strings.GetString("Arg_InsufficientSpace"), "bytes");
			}
			if (charCount == 0 || bytes.Length == byteIndex)
			{
				return 0;
			}
			fixed (char* ptr = s + RuntimeHelpers.OffsetToStringData / 2)
			{
				fixed (byte* ptr2 = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
				{
					return this.GetBytesImpl(ptr + charIndex, charCount, ptr2 + byteIndex, bytes.Length - byteIndex);
				}
			}
		}

		public unsafe override int GetByteCount(char* chars, int count)
		{
			return this.GetByteCountImpl(chars, count);
		}

		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			return this.GetBytesImpl(chars, charCount, bytes, byteCount);
		}

		public unsafe abstract int GetByteCountImpl(char* chars, int charCount);

		public unsafe abstract int GetBytesImpl(char* chars, int charCount, byte* bytes, int byteCount);

		private readonly int win_code_page;
	}
}
