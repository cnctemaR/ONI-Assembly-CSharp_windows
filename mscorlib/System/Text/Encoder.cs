using System;
using System.Runtime.InteropServices;

namespace System.Text
{
	[ComVisible(true)]
	[Serializable]
	public abstract class Encoder
	{
		[ComVisible(false)]
		public EncoderFallback Fallback
		{
			get
			{
				return this.fallback;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.fallback = value;
				this.fallback_buffer = null;
			}
		}

		[ComVisible(false)]
		public EncoderFallbackBuffer FallbackBuffer
		{
			get
			{
				if (this.fallback_buffer == null)
				{
					this.fallback_buffer = this.Fallback.CreateFallbackBuffer();
				}
				return this.fallback_buffer;
			}
		}

		public abstract int GetByteCount(char[] chars, int index, int count, bool flush);

		public abstract int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush);

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe virtual int GetByteCount(char* chars, int count, bool flush)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			char[] array = new char[count];
			Marshal.Copy((IntPtr)((void*)chars), array, 0, count);
			return this.GetByteCount(array, 0, count, flush);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe virtual int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, bool flush)
		{
			this.CheckArguments(chars, charCount, bytes, byteCount);
			char[] array = new char[charCount];
			Marshal.Copy((IntPtr)((void*)chars), array, 0, charCount);
			byte[] array2 = new byte[byteCount];
			Marshal.Copy((IntPtr)((void*)bytes), array2, 0, byteCount);
			return this.GetBytes(array, 0, charCount, array2, 0, flush);
		}

		[ComVisible(false)]
		public virtual void Reset()
		{
			if (this.fallback_buffer != null)
			{
				this.fallback_buffer.Reset();
			}
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe virtual void Convert(char* chars, int charCount, byte* bytes, int byteCount, bool flush, out int charsUsed, out int bytesUsed, out bool completed)
		{
			this.CheckArguments(chars, charCount, bytes, byteCount);
			charsUsed = charCount;
			for (;;)
			{
				bytesUsed = this.GetByteCount(chars, charsUsed, flush);
				if (bytesUsed <= byteCount)
				{
					break;
				}
				flush = false;
				charsUsed >>= 1;
			}
			completed = charsUsed == charCount;
			bytesUsed = this.GetBytes(chars, charsUsed, bytes, byteCount, flush);
		}

		[ComVisible(false)]
		public virtual void Convert(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, int byteCount, bool flush, out int charsUsed, out int bytesUsed, out bool completed)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || chars.Length <= charIndex)
			{
				throw new ArgumentOutOfRangeException("charIndex");
			}
			if (charCount < 0 || chars.Length < charIndex + charCount)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			if (byteIndex < 0 || bytes.Length <= byteIndex)
			{
				throw new ArgumentOutOfRangeException("byteIndex");
			}
			if (byteCount < 0 || bytes.Length < byteIndex + byteCount)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
			charsUsed = charCount;
			for (;;)
			{
				bytesUsed = this.GetByteCount(chars, charIndex, charsUsed, flush);
				if (bytesUsed <= byteCount)
				{
					break;
				}
				flush = false;
				charsUsed >>= 1;
			}
			completed = charsUsed == charCount;
			bytesUsed = this.GetBytes(chars, charIndex, charsUsed, bytes, byteIndex, flush);
		}

		private unsafe void CheckArguments(char* chars, int charCount, byte* bytes, int byteCount)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
		}

		private EncoderFallback fallback = new EncoderReplacementFallback();

		private EncoderFallbackBuffer fallback_buffer;
	}
}
