using System;
using System.Text;

namespace System.Net.Mail
{
	internal sealed class BufferBuilder
	{
		internal BufferBuilder()
			: this(256)
		{
		}

		internal BufferBuilder(int initialSize)
		{
			this._buffer = new byte[initialSize];
		}

		private void EnsureBuffer(int count)
		{
			if (count > this._buffer.Length - this._offset)
			{
				byte[] array = new byte[(this._buffer.Length * 2 > this._buffer.Length + count) ? (this._buffer.Length * 2) : (this._buffer.Length + count)];
				Buffer.BlockCopy(this._buffer, 0, array, 0, this._offset);
				this._buffer = array;
			}
		}

		internal void Append(byte value)
		{
			this.EnsureBuffer(1);
			byte[] buffer = this._buffer;
			int offset = this._offset;
			this._offset = offset + 1;
			buffer[offset] = value;
		}

		internal void Append(byte[] value)
		{
			this.Append(value, 0, value.Length);
		}

		internal void Append(byte[] value, int offset, int count)
		{
			this.EnsureBuffer(count);
			Buffer.BlockCopy(value, offset, this._buffer, this._offset, count);
			this._offset += count;
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
				int byteCount = Encoding.UTF8.GetByteCount(value, offset, count);
				this.EnsureBuffer(byteCount);
				Encoding.UTF8.GetBytes(value, offset, count, this._buffer, this._offset);
				this._offset += byteCount;
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
					throw new FormatException(SR.Format("An invalid character was found in the mail header: '{0}'.", c));
				}
				this._buffer[this._offset + i] = (byte)c;
			}
			this._offset += count;
		}

		internal int Length
		{
			get
			{
				return this._offset;
			}
		}

		internal byte[] GetBuffer()
		{
			return this._buffer;
		}

		internal void Reset()
		{
			this._offset = 0;
		}

		private byte[] _buffer;

		private int _offset;
	}
}
