using System;
using System.Text;

namespace I18N.Common
{
	public abstract class MonoEncoder : Encoder
	{
		public MonoEncoder(MonoEncoding encoding)
		{
			this.encoding = encoding;
		}

		public unsafe override int GetByteCount(char[] chars, int index, int count, bool refresh)
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
				return this.GetByteCountImpl(ptr + index, count, refresh);
			}
		}

		public unsafe override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush)
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
					return this.GetBytesImpl(ptr + charIndex, charCount, ptr2 + byteIndex, bytes.Length - byteIndex, flush);
				}
			}
		}

		public unsafe abstract int GetByteCountImpl(char* chars, int charCount, bool refresh);

		public unsafe abstract int GetBytesImpl(char* chars, int charCount, byte* bytes, int byteCount, bool refresh);

		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, bool flush)
		{
			return this.GetBytesImpl(chars, charCount, bytes, byteCount, flush);
		}

		public unsafe void HandleFallback(char* chars, ref int charIndex, ref int charCount, byte* bytes, ref int byteIndex, ref int byteCount)
		{
			EncoderFallbackBuffer fallbackBuffer = base.FallbackBuffer;
			this.encoding.HandleFallback(ref fallbackBuffer, chars, ref charIndex, ref charCount, bytes, ref byteIndex, ref byteCount);
		}

		private MonoEncoding encoding;
	}
}
