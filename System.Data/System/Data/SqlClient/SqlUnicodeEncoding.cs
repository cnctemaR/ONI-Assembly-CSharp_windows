using System;
using System.Text;

namespace System.Data.SqlClient
{
	internal sealed class SqlUnicodeEncoding : UnicodeEncoding
	{
		private SqlUnicodeEncoding()
			: base(false, false, false)
		{
		}

		public override Decoder GetDecoder()
		{
			return new SqlUnicodeEncoding.SqlUnicodeDecoder();
		}

		public override int GetMaxByteCount(int charCount)
		{
			return charCount * 2;
		}

		public static Encoding SqlUnicodeEncodingInstance
		{
			get
			{
				return SqlUnicodeEncoding.s_singletonEncoding;
			}
		}

		private static SqlUnicodeEncoding s_singletonEncoding = new SqlUnicodeEncoding();

		private sealed class SqlUnicodeDecoder : Decoder
		{
			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				return count / 2;
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				int num;
				int num2;
				bool flag;
				this.Convert(bytes, byteIndex, byteCount, chars, charIndex, chars.Length - charIndex, true, out num, out num2, out flag);
				return num2;
			}

			public override void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
			{
				charsUsed = Math.Min(charCount, byteCount / 2);
				bytesUsed = charsUsed * 2;
				completed = bytesUsed == byteCount;
				Buffer.BlockCopy(bytes, byteIndex, chars, charIndex * 2, bytesUsed);
			}
		}
	}
}
