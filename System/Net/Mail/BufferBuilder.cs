using System;
using System.Text;

namespace System.Net.Mail
{
	internal class BufferBuilder
	{
		internal BufferBuilder()
			: this(256)
		{
		}

		internal BufferBuilder(int initialSize)
		{
			this.buffer = new byte[initialSize];
		}

		private void EnsureBuffer(int count)
		{
			if (count > this.buffer.Length - this.offset)
			{
				byte[] array = new byte[(this.buffer.Length * 2 > this.buffer.Length + count) ? (this.buffer.Length * 2) : (this.buffer.Length + count)];
				Buffer.BlockCopy(this.buffer, 0, array, 0, this.offset);
				this.buffer = array;
			}
		}

		internal void Append(byte value)
		{
			this.EnsureBuffer(1);
			byte[] array = this.buffer;
			int num = this.offset;
			this.offset = num + 1;
			array[num] = value;
		}

		internal void Append(byte[] value)
		{
			this.Append(value, 0, value.Length);
		}

		internal void Append(byte[] value, int offset, int count)
		{
			this.EnsureBuffer(count);
			Buffer.BlockCopy(value, offset, this.buffer, this.offset, count);
			this.offset += count;
		}

		internal void Append(string value)
		{
			this.Append(value, false);
		}

		internal void Append(string value, bool allowUnicode)
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			this.Append(value, 0, value.Length, allowUnicode);
		}

		internal void Append(string value, int offset, int count, bool allowUnicode)
		{
			if (allowUnicode)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(value.ToCharArray(), offset, count);
				this.Append(bytes);
				return;
			}
			this.Append(value, offset, count);
		}

		internal void Append(string value, int offset, int count)
		{
			this.EnsureBuffer(count);
			for (int i = 0; i < count; i++)
			{
				char c = value[offset + i];
				if (c > 'ÿ')
				{
					throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'.", new object[] { c }));
				}
				this.buffer[this.offset + i] = (byte)c;
			}
			this.offset += count;
		}

		internal int Length
		{
			get
			{
				return this.offset;
			}
		}

		internal byte[] GetBuffer()
		{
			return this.buffer;
		}

		internal void Reset()
		{
			this.offset = 0;
		}

		private byte[] buffer;

		private int offset;
	}
}
