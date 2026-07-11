using System;
using System.Threading.Tasks;

namespace System.Xml
{
	internal abstract class Base64Encoder
	{
		internal Base64Encoder()
		{
			this.charsLine = new char[76];
		}

		internal abstract void WriteChars(char[] chars, int index, int count);

		internal void Encode(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count > buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.leftOverBytesCount > 0)
			{
				int num = this.leftOverBytesCount;
				while (num < 3 && count > 0)
				{
					this.leftOverBytes[num++] = buffer[index++];
					count--;
				}
				if (count == 0 && num < 3)
				{
					this.leftOverBytesCount = num;
					return;
				}
				int num2 = Convert.ToBase64CharArray(this.leftOverBytes, 0, 3, this.charsLine, 0);
				this.WriteChars(this.charsLine, 0, num2);
			}
			this.leftOverBytesCount = count % 3;
			if (this.leftOverBytesCount > 0)
			{
				count -= this.leftOverBytesCount;
				if (this.leftOverBytes == null)
				{
					this.leftOverBytes = new byte[3];
				}
				for (int i = 0; i < this.leftOverBytesCount; i++)
				{
					this.leftOverBytes[i] = buffer[index + count + i];
				}
			}
			int num3 = index + count;
			int num4 = 57;
			while (index < num3)
			{
				if (index + num4 > num3)
				{
					num4 = num3 - index;
				}
				int num5 = Convert.ToBase64CharArray(buffer, index, num4, this.charsLine, 0);
				this.WriteChars(this.charsLine, 0, num5);
				index += num4;
			}
		}

		internal void Flush()
		{
			if (this.leftOverBytesCount > 0)
			{
				int num = Convert.ToBase64CharArray(this.leftOverBytes, 0, this.leftOverBytesCount, this.charsLine, 0);
				this.WriteChars(this.charsLine, 0, num);
				this.leftOverBytesCount = 0;
			}
		}

		internal abstract Task WriteCharsAsync(char[] chars, int index, int count);

		internal async Task EncodeAsync(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count > buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.leftOverBytesCount > 0)
			{
				int num = this.leftOverBytesCount;
				while (num < 3 && count > 0)
				{
					byte[] array = this.leftOverBytes;
					int num2 = num++;
					int num3 = index;
					index = num3 + 1;
					array[num2] = buffer[num3];
					num3 = count;
					count = num3 - 1;
				}
				if (count == 0 && num < 3)
				{
					this.leftOverBytesCount = num;
					return;
				}
				int num4 = Convert.ToBase64CharArray(this.leftOverBytes, 0, 3, this.charsLine, 0);
				await this.WriteCharsAsync(this.charsLine, 0, num4).ConfigureAwait(false);
			}
			this.leftOverBytesCount = count % 3;
			if (this.leftOverBytesCount > 0)
			{
				count -= this.leftOverBytesCount;
				if (this.leftOverBytes == null)
				{
					this.leftOverBytes = new byte[3];
				}
				for (int i = 0; i < this.leftOverBytesCount; i++)
				{
					this.leftOverBytes[i] = buffer[index + count + i];
				}
			}
			int endIndex = index + count;
			int chunkSize = 57;
			while (index < endIndex)
			{
				if (index + chunkSize > endIndex)
				{
					chunkSize = endIndex - index;
				}
				int num5 = Convert.ToBase64CharArray(buffer, index, chunkSize, this.charsLine, 0);
				await this.WriteCharsAsync(this.charsLine, 0, num5).ConfigureAwait(false);
				index += chunkSize;
			}
		}

		internal async Task FlushAsync()
		{
			if (this.leftOverBytesCount > 0)
			{
				int num = Convert.ToBase64CharArray(this.leftOverBytes, 0, this.leftOverBytesCount, this.charsLine, 0);
				await this.WriteCharsAsync(this.charsLine, 0, num).ConfigureAwait(false);
				this.leftOverBytesCount = 0;
			}
		}

		private byte[] leftOverBytes;

		private int leftOverBytesCount;

		private char[] charsLine;

		internal const int Base64LineSize = 76;

		internal const int LineSizeInBytes = 57;
	}
}
