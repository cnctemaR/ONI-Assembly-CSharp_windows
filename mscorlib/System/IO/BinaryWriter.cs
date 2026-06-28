using System;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Security;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public class BinaryWriter : IDisposable
	{
		protected BinaryWriter()
			: this(Stream.Null, Encoding.UTF8UnmarkedUnsafe)
		{
		}

		public BinaryWriter(Stream output)
			: this(output, Encoding.UTF8UnmarkedUnsafe)
		{
		}

		public BinaryWriter(Stream output, Encoding encoding)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (!output.CanWrite)
			{
				throw new ArgumentException(Locale.GetText("Stream does not support writing or already closed."));
			}
			this.OutStream = output;
			this.m_encoding = encoding;
			this.buffer = new byte[16];
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		public virtual Stream BaseStream
		{
			get
			{
				return this.OutStream;
			}
		}

		public virtual void Close()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.OutStream != null)
			{
				this.OutStream.Close();
			}
			this.buffer = null;
			this.m_encoding = null;
			this.disposed = true;
		}

		public virtual void Flush()
		{
			this.OutStream.Flush();
		}

		public virtual long Seek(int offset, SeekOrigin origin)
		{
			return this.OutStream.Seek((long)offset, origin);
		}

		public virtual void Write(bool value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			this.buffer[0] = ((!value) ? 0 : 1);
			this.OutStream.Write(this.buffer, 0, 1);
		}

		public virtual void Write(byte value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			this.OutStream.WriteByte(value);
		}

		public virtual void Write(byte[] buffer)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.OutStream.Write(buffer, 0, buffer.Length);
		}

		public virtual void Write(byte[] buffer, int index, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.OutStream.Write(buffer, index, count);
		}

		public virtual void Write(char ch)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			char[] array = new char[] { ch };
			byte[] bytes = this.m_encoding.GetBytes(array, 0, 1);
			this.OutStream.Write(bytes, 0, bytes.Length);
		}

		public virtual void Write(char[] chars)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			byte[] bytes = this.m_encoding.GetBytes(chars, 0, chars.Length);
			this.OutStream.Write(bytes, 0, bytes.Length);
		}

		public virtual void Write(char[] chars, int index, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			byte[] bytes = this.m_encoding.GetBytes(chars, index, count);
			this.OutStream.Write(bytes, 0, bytes.Length);
		}

		public unsafe virtual void Write(decimal value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			if (BitConverter.IsLittleEndian)
			{
				for (int i = 0; i < 16; i++)
				{
					if (i < 4)
					{
						this.buffer[i + 12] = *((ref value) + i);
					}
					else if (i < 8)
					{
						this.buffer[i + 4] = *((ref value) + i);
					}
					else if (i < 12)
					{
						this.buffer[i - 8] = *((ref value) + i);
					}
					else
					{
						this.buffer[i - 8] = *((ref value) + i);
					}
				}
			}
			else
			{
				for (int j = 0; j < 16; j++)
				{
					if (j < 4)
					{
						this.buffer[15 - j] = *((ref value) + j);
					}
					else if (j < 8)
					{
						this.buffer[15 - j] = *((ref value) + j);
					}
					else if (j < 12)
					{
						this.buffer[11 - j] = *((ref value) + j);
					}
					else
					{
						this.buffer[19 - j] = *((ref value) + j);
					}
				}
			}
			this.OutStream.Write(this.buffer, 0, 16);
		}

		public virtual void Write(double value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			this.OutStream.Write(BitConverterLE.GetBytes(value), 0, 8);
		}

		public virtual void Write(short value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			this.buffer[0] = (byte)value;
			this.buffer[1] = (byte)(value >> 8);
			this.OutStream.Write(this.buffer, 0, 2);
		}

		public virtual void Write(int value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			this.buffer[0] = (byte)value;
			this.buffer[1] = (byte)(value >> 8);
			this.buffer[2] = (byte)(value >> 16);
			this.buffer[3] = (byte)(value >> 24);
			this.OutStream.Write(this.buffer, 0, 4);
		}

		public virtual void Write(long value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			int i = 0;
			int num = 0;
			while (i < 8)
			{
				this.buffer[i] = (byte)(value >> num);
				i++;
				num += 8;
			}
			this.OutStream.Write(this.buffer, 0, 8);
		}

		[CLSCompliant(false)]
		public virtual void Write(sbyte value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			this.buffer[0] = (byte)value;
			this.OutStream.Write(this.buffer, 0, 1);
		}

		public virtual void Write(float value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			this.OutStream.Write(BitConverterLE.GetBytes(value), 0, 4);
		}

		public virtual void Write(string value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			int byteCount = this.m_encoding.GetByteCount(value);
			this.Write7BitEncodedInt(byteCount);
			if (this.stringBuffer == null)
			{
				this.stringBuffer = new byte[512];
				this.maxCharsPerRound = 512 / this.m_encoding.GetMaxByteCount(1);
			}
			int num = 0;
			int num2;
			for (int i = value.Length; i > 0; i -= num2)
			{
				num2 = ((i <= this.maxCharsPerRound) ? i : this.maxCharsPerRound);
				int bytes = this.m_encoding.GetBytes(value, num, num2, this.stringBuffer, 0);
				this.OutStream.Write(this.stringBuffer, 0, bytes);
				num += num2;
			}
		}

		[CLSCompliant(false)]
		public virtual void Write(ushort value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			this.buffer[0] = (byte)value;
			this.buffer[1] = (byte)(value >> 8);
			this.OutStream.Write(this.buffer, 0, 2);
		}

		[CLSCompliant(false)]
		public virtual void Write(uint value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			this.buffer[0] = (byte)value;
			this.buffer[1] = (byte)(value >> 8);
			this.buffer[2] = (byte)(value >> 16);
			this.buffer[3] = (byte)(value >> 24);
			this.OutStream.Write(this.buffer, 0, 4);
		}

		[CLSCompliant(false)]
		public virtual void Write(ulong value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			int i = 0;
			int num = 0;
			while (i < 8)
			{
				this.buffer[i] = (byte)(value >> num);
				i++;
				num += 8;
			}
			this.OutStream.Write(this.buffer, 0, 8);
		}

		protected void Write7BitEncodedInt(int value)
		{
			do
			{
				int num = (value >> 7) & 33554431;
				byte b = (byte)(value & 127);
				if (num != 0)
				{
					b |= 128;
				}
				this.Write(b);
				value = num;
			}
			while (value != 0);
		}

		public static readonly BinaryWriter Null = new BinaryWriter();

		protected Stream OutStream;

		private Encoding m_encoding;

		private byte[] buffer;

		private bool disposed;

		private byte[] stringBuffer;

		private int maxCharsPerRound;
	}
}
