using System;

namespace System.Security.Cryptography.Asn1
{
	internal class BMPEncoding : SpanBasedEncoding
	{
		protected unsafe override int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes, bool write)
		{
			if (chars.IsEmpty)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < chars.Length; i++)
			{
				char c = (char)(*chars[i]);
				if (char.IsSurrogate(c))
				{
					base.EncoderFallback.CreateFallbackBuffer().Fallback(c, i);
					throw new CryptographicException();
				}
				ushort num2 = (ushort)c;
				if (write)
				{
					*bytes[num + 1] = (byte)num2;
					*bytes[num] = (byte)(num2 >> 8);
				}
				num += 2;
			}
			return num;
		}

		protected unsafe override int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars, bool write)
		{
			if (bytes.IsEmpty)
			{
				return 0;
			}
			if (bytes.Length % 2 != 0)
			{
				base.DecoderFallback.CreateFallbackBuffer().Fallback(bytes.Slice(bytes.Length - 1).ToArray(), bytes.Length - 1);
				throw new CryptographicException();
			}
			int num = 0;
			for (int i = 0; i < bytes.Length; i += 2)
			{
				char c = (char)(((int)(*bytes[i]) << 8) | (int)(*bytes[i + 1]));
				if (char.IsSurrogate(c))
				{
					base.DecoderFallback.CreateFallbackBuffer().Fallback(bytes.Slice(i, 2).ToArray(), i);
					throw new CryptographicException();
				}
				if (write)
				{
					*chars[num] = c;
				}
				num++;
			}
			return num;
		}

		public override int GetMaxByteCount(int charCount)
		{
			return checked(charCount * 2);
		}

		public override int GetMaxCharCount(int byteCount)
		{
			return byteCount / 2;
		}
	}
}
