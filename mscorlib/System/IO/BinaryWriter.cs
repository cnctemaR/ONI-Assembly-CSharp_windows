using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using Mono.Security;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public class BinaryWriter : IDisposable
	{
		protected BinaryWriter()
		{
			this.OutStream = Stream.Null;
			this._buffer = new byte[16];
			this._encoding = new UTF8Encoding(false, true);
			this._encoder = this._encoding.GetEncoder();
		}

		public BinaryWriter(Stream output)
			: this(output, new UTF8Encoding(false, true), false)
		{
		}

		public BinaryWriter(Stream output, Encoding encoding)
			: this(output, encoding, false)
		{
		}

		public BinaryWriter(Stream output, Encoding encoding, bool leaveOpen)
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
				throw new ArgumentException(Environment.GetResourceString("Stream was not writable."));
			}
			this.OutStream = output;
			this._buffer = new byte[16];
			this._encoding = encoding;
			this._encoder = this._encoding.GetEncoder();
			this._leaveOpen = leaveOpen;
		}

		public virtual void Close()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this._leaveOpen)
				{
					this.OutStream.Flush();
					return;
				}
				this.OutStream.Close();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public virtual Stream BaseStream
		{
			get
			{
				this.Flush();
				return this.OutStream;
			}
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
			this._buffer[0] = (value ? 1 : 0);
			this.OutStream.Write(this._buffer, 0, 1);
		}

		public virtual void Write(byte value)
		{
			this.OutStream.WriteByte(value);
		}

		[CLSCompliant(false)]
		public virtual void Write(sbyte value)
		{
			this.OutStream.WriteByte((byte)value);
		}

		public virtual void Write(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.OutStream.Write(buffer, 0, buffer.Length);
		}

		public virtual void Write(byte[] buffer, int index, int count)
		{
			this.OutStream.Write(buffer, index, count);
		}

		[SecuritySafeCritical]
		public unsafe virtual void Write(char ch)
		{
			if (char.IsSurrogate(ch))
			{
				throw new ArgumentException(Environment.GetResourceString("Unicode surrogate characters must be written out as pairs together in the same call, not individually. Consider passing in a character array instead."));
			}
			byte[] array;
			byte* ptr;
			if ((array = this._buffer) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			int bytes = this._encoder.GetBytes(&ch, 1, ptr, this._buffer.Length, true);
			array = null;
			this.OutStream.Write(this._buffer, 0, bytes);
		}

		public virtual void Write(char[] chars)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			byte[] bytes = this._encoding.GetBytes(chars, 0, chars.Length);
			this.OutStream.Write(bytes, 0, bytes.Length);
		}

		public virtual void Write(char[] chars, int index, int count)
		{
			byte[] bytes = this._encoding.GetBytes(chars, index, count);
			this.OutStream.Write(bytes, 0, bytes.Length);
		}

		[SecuritySafeCritical]
		public virtual void Write(double value)
		{
			this.OutStream.Write(BitConverterLE.GetBytes(value), 0, 8);
		}

		public virtual void Write(decimal value)
		{
			decimal.GetBytes(value, this._buffer);
			this.OutStream.Write(this._buffer, 0, 16);
		}

		public virtual void Write(short value)
		{
			this._buffer[0] = (byte)value;
			this._buffer[1] = (byte)(value >> 8);
			this.OutStream.Write(this._buffer, 0, 2);
		}

		[CLSCompliant(false)]
		public virtual void Write(ushort value)
		{
			this._buffer[0] = (byte)value;
			this._buffer[1] = (byte)(value >> 8);
			this.OutStream.Write(this._buffer, 0, 2);
		}

		public virtual void Write(int value)
		{
			this._buffer[0] = (byte)value;
			this._buffer[1] = (byte)(value >> 8);
			this._buffer[2] = (byte)(value >> 16);
			this._buffer[3] = (byte)(value >> 24);
			this.OutStream.Write(this._buffer, 0, 4);
		}

		[CLSCompliant(false)]
		public virtual void Write(uint value)
		{
			this._buffer[0] = (byte)value;
			this._buffer[1] = (byte)(value >> 8);
			this._buffer[2] = (byte)(value >> 16);
			this._buffer[3] = (byte)(value >> 24);
			this.OutStream.Write(this._buffer, 0, 4);
		}

		public virtual void Write(long value)
		{
			this._buffer[0] = (byte)value;
			this._buffer[1] = (byte)(value >> 8);
			this._buffer[2] = (byte)(value >> 16);
			this._buffer[3] = (byte)(value >> 24);
			this._buffer[4] = (byte)(value >> 32);
			this._buffer[5] = (byte)(value >> 40);
			this._buffer[6] = (byte)(value >> 48);
			this._buffer[7] = (byte)(value >> 56);
			this.OutStream.Write(this._buffer, 0, 8);
		}

		[CLSCompliant(false)]
		public virtual void Write(ulong value)
		{
			this._buffer[0] = (byte)value;
			this._buffer[1] = (byte)(value >> 8);
			this._buffer[2] = (byte)(value >> 16);
			this._buffer[3] = (byte)(value >> 24);
			this._buffer[4] = (byte)(value >> 32);
			this._buffer[5] = (byte)(value >> 40);
			this._buffer[6] = (byte)(value >> 48);
			this._buffer[7] = (byte)(value >> 56);
			this.OutStream.Write(this._buffer, 0, 8);
		}

		[SecuritySafeCritical]
		public virtual void Write(float value)
		{
			this.OutStream.Write(BitConverterLE.GetBytes(value), 0, 4);
		}

		[SecuritySafeCritical]
		public unsafe virtual void Write(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int byteCount = this._encoding.GetByteCount(value);
			this.Write7BitEncodedInt(byteCount);
			if (this._largeByteBuffer == null)
			{
				this._largeByteBuffer = new byte[256];
				this._maxChars = this._largeByteBuffer.Length / this._encoding.GetMaxByteCount(1);
			}
			if (byteCount <= this._largeByteBuffer.Length)
			{
				this._encoding.GetBytes(value, 0, value.Length, this._largeByteBuffer, 0);
				this.OutStream.Write(this._largeByteBuffer, 0, byteCount);
				return;
			}
			int num = 0;
			int num2;
			for (int i = value.Length; i > 0; i -= num2)
			{
				num2 = ((i > this._maxChars) ? this._maxChars : i);
				if (num < 0 || num2 < 0 || checked(num + num2) > value.Length)
				{
					throw new ArgumentOutOfRangeException("charCount");
				}
				int bytes;
				fixed (string text = value)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					byte[] array;
					byte* ptr2;
					if ((array = this._largeByteBuffer) == null || array.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array[0];
					}
					bytes = this._encoder.GetBytes(checked(ptr + num), num2, ptr2, this._largeByteBuffer.Length, num2 == i);
					array = null;
				}
				this.OutStream.Write(this._largeByteBuffer, 0, bytes);
				num += num2;
			}
		}

		protected void Write7BitEncodedInt(int value)
		{
			uint num;
			for (num = (uint)value; num >= 128U; num >>= 7)
			{
				this.Write((byte)(num | 128U));
			}
			this.Write((byte)num);
		}

		public static readonly BinaryWriter Null = new BinaryWriter();

		protected Stream OutStream;

		private byte[] _buffer;

		private Encoding _encoding;

		private Encoder _encoder;

		[OptionalField]
		private bool _leaveOpen;

		[OptionalField]
		private char[] _tmpOneCharBuffer;

		private byte[] _largeByteBuffer;

		private int _maxChars;

		private const int LargeByteBufferSize = 256;
	}
}
