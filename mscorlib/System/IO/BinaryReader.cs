using System;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Security;

namespace System.IO
{
	[ComVisible(true)]
	public class BinaryReader : IDisposable
	{
		public BinaryReader(Stream input)
			: this(input, Encoding.UTF8UnmarkedUnsafe)
		{
		}

		public BinaryReader(Stream input, Encoding encoding)
		{
			if (input == null || encoding == null)
			{
				throw new ArgumentNullException(Locale.GetText("Input or Encoding is a null reference."));
			}
			if (!input.CanRead)
			{
				throw new ArgumentException(Locale.GetText("The stream doesn't support reading."));
			}
			this.m_stream = input;
			this.m_encoding = encoding;
			this.decoder = encoding.GetDecoder();
			this.m_buffer = new byte[32];
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		public virtual Stream BaseStream
		{
			get
			{
				return this.m_stream;
			}
		}

		public virtual void Close()
		{
			this.Dispose(true);
			this.m_disposed = true;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.m_stream != null)
			{
				this.m_stream.Close();
			}
			this.m_disposed = true;
			this.m_buffer = null;
			this.m_encoding = null;
			this.m_stream = null;
			this.charBuffer = null;
		}

		protected virtual void FillBuffer(int numBytes)
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
			}
			if (this.m_stream == null)
			{
				throw new IOException("Stream is invalid");
			}
			this.CheckBuffer(numBytes);
			int num;
			for (int i = 0; i < numBytes; i += num)
			{
				num = this.m_stream.Read(this.m_buffer, i, numBytes - i);
				if (num == 0)
				{
					throw new EndOfStreamException();
				}
			}
		}

		public virtual int PeekChar()
		{
			if (this.m_stream == null)
			{
				if (this.m_disposed)
				{
					throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
				}
				throw new IOException("Stream is invalid");
			}
			else
			{
				if (!this.m_stream.CanSeek)
				{
					return -1;
				}
				char[] array = new char[1];
				int num2;
				int num = this.ReadCharBytes(array, 0, 1, out num2);
				this.m_stream.Position -= (long)num2;
				if (num == 0)
				{
					return -1;
				}
				return (int)array[0];
			}
		}

		public virtual int Read()
		{
			if (this.charBuffer == null)
			{
				this.charBuffer = new char[128];
			}
			if (this.Read(this.charBuffer, 0, 1) == 0)
			{
				return -1;
			}
			return (int)this.charBuffer[0];
		}

		public virtual int Read(byte[] buffer, int index, int count)
		{
			if (this.m_stream == null)
			{
				if (this.m_disposed)
				{
					throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
				}
				throw new IOException("Stream is invalid");
			}
			else
			{
				if (buffer == null)
				{
					throw new ArgumentNullException("buffer is null");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index is less than 0");
				}
				if (count < 0)
				{
					throw new ArgumentOutOfRangeException("count is less than 0");
				}
				if (buffer.Length - index < count)
				{
					throw new ArgumentException("buffer is too small");
				}
				return this.m_stream.Read(buffer, index, count);
			}
		}

		public virtual int Read(char[] buffer, int index, int count)
		{
			if (this.m_stream == null)
			{
				if (this.m_disposed)
				{
					throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
				}
				throw new IOException("Stream is invalid");
			}
			else
			{
				if (buffer == null)
				{
					throw new ArgumentNullException("buffer is null");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index is less than 0");
				}
				if (count < 0)
				{
					throw new ArgumentOutOfRangeException("count is less than 0");
				}
				if (buffer.Length - index < count)
				{
					throw new ArgumentException("buffer is too small");
				}
				int num;
				return this.ReadCharBytes(buffer, index, count, out num);
			}
		}

		private int ReadCharBytes(char[] buffer, int index, int count, out int bytes_read)
		{
			int i = 0;
			bytes_read = 0;
			while (i < count)
			{
				int num = 0;
				int chars;
				do
				{
					this.CheckBuffer(num + 1);
					int num2 = this.m_stream.ReadByte();
					if (num2 == -1)
					{
						return i;
					}
					this.m_buffer[num++] = (byte)num2;
					bytes_read++;
					chars = this.m_encoding.GetChars(this.m_buffer, 0, num, buffer, index + i);
				}
				while (chars <= 0);
				i++;
			}
			return i;
		}

		protected int Read7BitEncodedInt()
		{
			int num = 0;
			int num2 = 0;
			int i;
			for (i = 0; i < 5; i++)
			{
				byte b = this.ReadByte();
				num |= (int)(b & 127) << num2;
				num2 += 7;
				if ((b & 128) == 0)
				{
					break;
				}
			}
			if (i < 5)
			{
				return num;
			}
			throw new FormatException("Too many bytes in what should have been a 7 bit encoded Int32.");
		}

		public virtual bool ReadBoolean()
		{
			return this.ReadByte() != 0;
		}

		public virtual byte ReadByte()
		{
			if (this.m_stream == null)
			{
				if (this.m_disposed)
				{
					throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
				}
				throw new IOException("Stream is invalid");
			}
			else
			{
				int num = this.m_stream.ReadByte();
				if (num != -1)
				{
					return (byte)num;
				}
				throw new EndOfStreamException();
			}
		}

		public virtual byte[] ReadBytes(int count)
		{
			if (this.m_stream == null)
			{
				if (this.m_disposed)
				{
					throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
				}
				throw new IOException("Stream is invalid");
			}
			else
			{
				if (count < 0)
				{
					throw new ArgumentOutOfRangeException("count is less than 0");
				}
				byte[] array = new byte[count];
				int i;
				int num;
				for (i = 0; i < count; i += num)
				{
					num = this.m_stream.Read(array, i, count - i);
					if (num == 0)
					{
						break;
					}
				}
				if (i != count)
				{
					byte[] array2 = new byte[i];
					Buffer.BlockCopyInternal(array, 0, array2, 0, i);
					return array2;
				}
				return array;
			}
		}

		public virtual char ReadChar()
		{
			int num = this.Read();
			if (num == -1)
			{
				throw new EndOfStreamException();
			}
			return (char)num;
		}

		public virtual char[] ReadChars(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count is less than 0");
			}
			if (count == 0)
			{
				return new char[0];
			}
			char[] array = new char[count];
			int num = this.Read(array, 0, count);
			if (num == 0)
			{
				throw new EndOfStreamException();
			}
			if (num != array.Length)
			{
				char[] array2 = new char[num];
				Array.Copy(array, 0, array2, 0, num);
				return array2;
			}
			return array;
		}

		public unsafe virtual decimal ReadDecimal()
		{
			this.FillBuffer(16);
			decimal num;
			if (BitConverter.IsLittleEndian)
			{
				for (int i = 0; i < 16; i++)
				{
					if (i < 4)
					{
						*((ref num) + (i + 8)) = this.m_buffer[i];
					}
					else if (i < 8)
					{
						*((ref num) + (i + 8)) = this.m_buffer[i];
					}
					else if (i < 12)
					{
						*((ref num) + (i - 4)) = this.m_buffer[i];
					}
					else if (i < 16)
					{
						*((ref num) + (i - 12)) = this.m_buffer[i];
					}
				}
			}
			else
			{
				for (int j = 0; j < 16; j++)
				{
					if (j < 4)
					{
						*((ref num) + (11 - j)) = this.m_buffer[j];
					}
					else if (j < 8)
					{
						*((ref num) + (19 - j)) = this.m_buffer[j];
					}
					else if (j < 12)
					{
						*((ref num) + (15 - j)) = this.m_buffer[j];
					}
					else if (j < 16)
					{
						*((ref num) + (15 - j)) = this.m_buffer[j];
					}
				}
			}
			return num;
		}

		public virtual double ReadDouble()
		{
			this.FillBuffer(8);
			return BitConverterLE.ToDouble(this.m_buffer, 0);
		}

		public virtual short ReadInt16()
		{
			this.FillBuffer(2);
			return (short)((int)this.m_buffer[0] | ((int)this.m_buffer[1] << 8));
		}

		public virtual int ReadInt32()
		{
			this.FillBuffer(4);
			return (int)this.m_buffer[0] | ((int)this.m_buffer[1] << 8) | ((int)this.m_buffer[2] << 16) | ((int)this.m_buffer[3] << 24);
		}

		public virtual long ReadInt64()
		{
			this.FillBuffer(8);
			uint num = (uint)((int)this.m_buffer[0] | ((int)this.m_buffer[1] << 8) | ((int)this.m_buffer[2] << 16) | ((int)this.m_buffer[3] << 24));
			uint num2 = (uint)((int)this.m_buffer[4] | ((int)this.m_buffer[5] << 8) | ((int)this.m_buffer[6] << 16) | ((int)this.m_buffer[7] << 24));
			return (long)(((ulong)num2 << 32) | (ulong)num);
		}

		[CLSCompliant(false)]
		public virtual sbyte ReadSByte()
		{
			return (sbyte)this.ReadByte();
		}

		public virtual string ReadString()
		{
			int num = this.Read7BitEncodedInt();
			if (num < 0)
			{
				throw new IOException("Invalid binary file (string len < 0)");
			}
			if (num == 0)
			{
				return string.Empty;
			}
			if (this.charBuffer == null)
			{
				this.charBuffer = new char[128];
			}
			StringBuilder stringBuilder = null;
			int chars;
			for (;;)
			{
				int num2 = ((num <= 128) ? num : 128);
				this.FillBuffer(num2);
				chars = this.decoder.GetChars(this.m_buffer, 0, num2, this.charBuffer, 0);
				if (stringBuilder == null && num2 == num)
				{
					break;
				}
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(num);
				}
				stringBuilder.Append(this.charBuffer, 0, chars);
				num -= num2;
				if (num <= 0)
				{
					goto Block_8;
				}
			}
			return new string(this.charBuffer, 0, chars);
			Block_8:
			return stringBuilder.ToString();
		}

		public virtual float ReadSingle()
		{
			this.FillBuffer(4);
			return BitConverterLE.ToSingle(this.m_buffer, 0);
		}

		[CLSCompliant(false)]
		public virtual ushort ReadUInt16()
		{
			this.FillBuffer(2);
			return (ushort)((int)this.m_buffer[0] | ((int)this.m_buffer[1] << 8));
		}

		[CLSCompliant(false)]
		public virtual uint ReadUInt32()
		{
			this.FillBuffer(4);
			return (uint)((int)this.m_buffer[0] | ((int)this.m_buffer[1] << 8) | ((int)this.m_buffer[2] << 16) | ((int)this.m_buffer[3] << 24));
		}

		[CLSCompliant(false)]
		public virtual ulong ReadUInt64()
		{
			this.FillBuffer(8);
			uint num = (uint)((int)this.m_buffer[0] | ((int)this.m_buffer[1] << 8) | ((int)this.m_buffer[2] << 16) | ((int)this.m_buffer[3] << 24));
			uint num2 = (uint)((int)this.m_buffer[4] | ((int)this.m_buffer[5] << 8) | ((int)this.m_buffer[6] << 16) | ((int)this.m_buffer[7] << 24));
			return ((ulong)num2 << 32) | (ulong)num;
		}

		private void CheckBuffer(int length)
		{
			if (this.m_buffer.Length <= length)
			{
				byte[] array = new byte[length];
				Buffer.BlockCopyInternal(this.m_buffer, 0, array, 0, this.m_buffer.Length);
				this.m_buffer = array;
			}
		}

		private const int MaxBufferSize = 128;

		private Stream m_stream;

		private Encoding m_encoding;

		private byte[] m_buffer;

		private Decoder decoder;

		private char[] charBuffer;

		private bool m_disposed;
	}
}
