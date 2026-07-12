using System;
using System.Text;

namespace System.Security.Cryptography.Asn1
{
	internal abstract class SpanBasedEncoding : Encoding
	{
		protected SpanBasedEncoding()
			: base(0, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
		{
		}

		protected abstract int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes, bool write);

		protected abstract int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars, bool write);

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return this.GetByteCount(new ReadOnlySpan<char>(chars, index, count));
		}

		public unsafe override int GetByteCount(char* chars, int count)
		{
			return this.GetByteCount(new ReadOnlySpan<char>((void*)chars, count));
		}

		public override int GetByteCount(string s)
		{
			return this.GetByteCount(s.AsSpan());
		}

		public new int GetByteCount(ReadOnlySpan<char> chars)
		{
			return this.GetBytes(chars, Span<byte>.Empty, false);
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return this.GetBytes(new ReadOnlySpan<char>(chars, charIndex, charCount), new Span<byte>(bytes, byteIndex, bytes.Length - byteIndex), true);
		}

		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			return this.GetBytes(new ReadOnlySpan<char>((void*)chars, charCount), new Span<byte>((void*)bytes, byteCount), true);
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return this.GetCharCount(new ReadOnlySpan<byte>(bytes, index, count));
		}

		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			return this.GetCharCount(new ReadOnlySpan<byte>((void*)bytes, count));
		}

		public new int GetCharCount(ReadOnlySpan<byte> bytes)
		{
			return this.GetChars(bytes, Span<char>.Empty, false);
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			return this.GetChars(new ReadOnlySpan<byte>(bytes, byteIndex, byteCount), new Span<char>(chars, charIndex, chars.Length - charIndex), true);
		}

		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			return this.GetChars(new ReadOnlySpan<byte>((void*)bytes, byteCount), new Span<char>((void*)chars, charCount), true);
		}
	}
}
