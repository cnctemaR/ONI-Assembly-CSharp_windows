using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Xml
{
	internal class BufferedWrite
	{
		internal BufferedWrite()
			: this(256)
		{
		}

		internal BufferedWrite(int initialSize)
		{
			this.buffer = new byte[initialSize];
		}

		private void EnsureBuffer(int count)
		{
			int num = this.buffer.Length;
			if (count > num - this.offset)
			{
				int num2 = num;
				while (num2 != 2147483647)
				{
					num2 = ((num2 < 1073741823) ? (num2 * 2) : int.MaxValue);
					if (count <= num2 - this.offset)
					{
						byte[] array = new byte[num2];
						Buffer.BlockCopy(this.buffer, 0, array, 0, this.offset);
						this.buffer = array;
						return;
					}
				}
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Write buffer overflow.")));
			}
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

		internal void Write(byte[] value)
		{
			this.Write(value, 0, value.Length);
		}

		internal void Write(byte[] value, int index, int count)
		{
			this.EnsureBuffer(count);
			Buffer.BlockCopy(value, index, this.buffer, this.offset, count);
			this.offset += count;
		}

		internal void Write(string value)
		{
			this.Write(value, 0, value.Length);
		}

		internal void Write(string value, int index, int count)
		{
			this.EnsureBuffer(count);
			for (int i = 0; i < count; i++)
			{
				char c = value[index + i];
				if (c > 'ÿ')
				{
					string text = "MIME header has an invalid character ('{0}', {1} in hexadecimal value).";
					object[] array = new object[2];
					array[0] = c;
					int num = 1;
					int num2 = (int)c;
					array[num] = num2.ToString("X", CultureInfo.InvariantCulture);
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString(text, array)));
				}
				this.buffer[this.offset + i] = (byte)c;
			}
			this.offset += count;
		}

		private byte[] buffer;

		private int offset;
	}
}
