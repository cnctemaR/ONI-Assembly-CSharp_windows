using System;
using System.Collections.Generic;

namespace System.Security.Cryptography.Asn1
{
	internal abstract class RestrictedAsciiStringEncoding : SpanBasedEncoding
	{
		protected RestrictedAsciiStringEncoding(byte minCharAllowed, byte maxCharAllowed)
		{
			bool[] array = new bool[128];
			for (byte b = minCharAllowed; b <= maxCharAllowed; b += 1)
			{
				array[(int)b] = true;
			}
			this._isAllowed = array;
		}

		protected RestrictedAsciiStringEncoding(IEnumerable<char> allowedChars)
		{
			bool[] array = new bool[127];
			foreach (char c in allowedChars)
			{
				if ((int)c >= array.Length)
				{
					throw new ArgumentOutOfRangeException("allowedChars");
				}
				array[(int)c] = true;
			}
			this._isAllowed = array;
		}

		public override int GetMaxByteCount(int charCount)
		{
			return charCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			return byteCount;
		}

		protected unsafe override int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes, bool write)
		{
			if (chars.IsEmpty)
			{
				return 0;
			}
			for (int i = 0; i < chars.Length; i++)
			{
				char c = (char)(*chars[i]);
				if ((int)c >= this._isAllowed.Length || !this._isAllowed[(int)c])
				{
					base.EncoderFallback.CreateFallbackBuffer().Fallback(c, i);
					throw new CryptographicException();
				}
				if (write)
				{
					*bytes[i] = (byte)c;
				}
			}
			return chars.Length;
		}

		protected unsafe override int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars, bool write)
		{
			if (bytes.IsEmpty)
			{
				return 0;
			}
			for (int i = 0; i < bytes.Length; i++)
			{
				byte b = *bytes[i];
				if ((int)b >= this._isAllowed.Length || !this._isAllowed[(int)b])
				{
					base.DecoderFallback.CreateFallbackBuffer().Fallback(new byte[] { b }, i);
					throw new CryptographicException();
				}
				if (write)
				{
					*chars[i] = (char)b;
				}
			}
			return bytes.Length;
		}

		private readonly bool[] _isAllowed;
	}
}
