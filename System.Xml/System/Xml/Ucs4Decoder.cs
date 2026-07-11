using System;
using System.Text;

namespace System.Xml
{
	internal abstract class Ucs4Decoder : Decoder
	{
		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return (count + this.lastBytesCount) / 4;
		}

		internal abstract int GetFullChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			int num = this.lastBytesCount;
			if (this.lastBytesCount > 0)
			{
				while (this.lastBytesCount < 4 && byteCount > 0)
				{
					this.lastBytes[this.lastBytesCount] = bytes[byteIndex];
					byteIndex++;
					byteCount--;
					this.lastBytesCount++;
				}
				if (this.lastBytesCount < 4)
				{
					return 0;
				}
				num = this.GetFullChars(this.lastBytes, 0, 4, chars, charIndex);
				charIndex += num;
				this.lastBytesCount = 0;
			}
			else
			{
				num = 0;
			}
			num = this.GetFullChars(bytes, byteIndex, byteCount, chars, charIndex) + num;
			int num2 = byteCount & 3;
			if (num2 >= 0)
			{
				for (int i = 0; i < num2; i++)
				{
					this.lastBytes[i] = bytes[byteIndex + byteCount - num2 + i];
				}
				this.lastBytesCount = num2;
			}
			return num;
		}

		public override void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
		{
			bytesUsed = 0;
			charsUsed = 0;
			int num = this.lastBytesCount;
			int num2;
			if (num > 0)
			{
				while (num < 4 && byteCount > 0)
				{
					this.lastBytes[num] = bytes[byteIndex];
					byteIndex++;
					byteCount--;
					bytesUsed++;
					num++;
				}
				if (num < 4)
				{
					this.lastBytesCount = num;
					completed = true;
					return;
				}
				num2 = this.GetFullChars(this.lastBytes, 0, 4, chars, charIndex);
				charIndex += num2;
				charCount -= num2;
				charsUsed = num2;
				this.lastBytesCount = 0;
				if (charCount == 0)
				{
					completed = byteCount == 0;
					return;
				}
			}
			else
			{
				num2 = 0;
			}
			if (charCount * 4 < byteCount)
			{
				byteCount = charCount * 4;
				completed = false;
			}
			else
			{
				completed = true;
			}
			bytesUsed += byteCount;
			charsUsed = this.GetFullChars(bytes, byteIndex, byteCount, chars, charIndex) + num2;
			int num3 = byteCount & 3;
			if (num3 >= 0)
			{
				for (int i = 0; i < num3; i++)
				{
					this.lastBytes[i] = bytes[byteIndex + byteCount - num3 + i];
				}
				this.lastBytesCount = num3;
			}
		}

		internal void Ucs4ToUTF16(uint code, char[] chars, int charIndex)
		{
			chars[charIndex] = (char)(55296 + (ushort)((code >> 16) - 1U) + (ushort)((code >> 10) & 63U));
			chars[charIndex + 1] = (char)(56320 + (ushort)(code & 1023U));
		}

		internal byte[] lastBytes = new byte[4];

		internal int lastBytesCount;
	}
}
