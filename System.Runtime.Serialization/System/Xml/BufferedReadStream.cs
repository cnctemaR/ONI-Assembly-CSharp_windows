using System;
using System.IO;
using System.Runtime.Serialization;

namespace System.Xml
{
	internal class BufferedReadStream : Stream
	{
		public BufferedReadStream(Stream stream)
			: this(stream, false)
		{
		}

		public BufferedReadStream(Stream stream, bool readMore)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			this.stream = stream;
			this.readMore = readMore;
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.stream.CanRead;
			}
		}

		public override long Length
		{
			get
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Seek operation is not supported on this Stream.", new object[] { this.stream.GetType().FullName })));
			}
		}

		public override long Position
		{
			get
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Seek operation is not supported on this Stream.", new object[] { this.stream.GetType().FullName })));
			}
			set
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Seek operation is not supported on this Stream.", new object[] { this.stream.GetType().FullName })));
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (!this.CanRead)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Read operation is not supported on the Stream.", new object[] { this.stream.GetType().FullName })));
			}
			return this.stream.BeginRead(buffer, offset, count, callback, state);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Write operation is not supported on this '{0}' Stream.", new object[] { this.stream.GetType().FullName })));
		}

		public override void Close()
		{
			this.stream.Close();
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (!this.CanRead)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Read operation is not supported on the Stream.", new object[] { this.stream.GetType().FullName })));
			}
			return this.stream.EndRead(asyncResult);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Write operation is not supported on this '{0}' Stream.", new object[] { this.stream.GetType().FullName })));
		}

		public override void Flush()
		{
			this.stream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!this.CanRead)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Read operation is not supported on the Stream.", new object[] { this.stream.GetType().FullName })));
			}
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > buffer.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { buffer.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
			}
			int num = 0;
			if (this.storedOffset < this.storedLength)
			{
				num = Math.Min(count, this.storedLength - this.storedOffset);
				Buffer.BlockCopy(this.storedBuffer, this.storedOffset, buffer, offset, num);
				this.storedOffset += num;
				if (num == count || !this.readMore)
				{
					return num;
				}
				offset += num;
				count -= num;
			}
			return num + this.stream.Read(buffer, offset, count);
		}

		public override int ReadByte()
		{
			if (this.storedOffset < this.storedLength)
			{
				byte[] array = this.storedBuffer;
				int num = this.storedOffset;
				this.storedOffset = num + 1;
				return array[num];
			}
			return base.ReadByte();
		}

		public int ReadBlock(byte[] buffer, int offset, int count)
		{
			int num = 0;
			int num2;
			while (num < count && (num2 = this.Read(buffer, offset + num, count - num)) != 0)
			{
				num += num2;
			}
			return num;
		}

		public void Push(byte[] buffer, int offset, int count)
		{
			if (count == 0)
			{
				return;
			}
			if (this.storedOffset == this.storedLength)
			{
				if (this.storedBuffer == null || this.storedBuffer.Length < count)
				{
					this.storedBuffer = new byte[count];
				}
				this.storedOffset = 0;
				this.storedLength = count;
			}
			else if (count <= this.storedOffset)
			{
				this.storedOffset -= count;
			}
			else if (count <= this.storedBuffer.Length - this.storedLength + this.storedOffset)
			{
				Buffer.BlockCopy(this.storedBuffer, this.storedOffset, this.storedBuffer, count, this.storedLength - this.storedOffset);
				this.storedLength += count - this.storedOffset;
				this.storedOffset = 0;
			}
			else
			{
				byte[] array = new byte[count + this.storedLength - this.storedOffset];
				Buffer.BlockCopy(this.storedBuffer, this.storedOffset, array, count, this.storedLength - this.storedOffset);
				this.storedLength += count - this.storedOffset;
				this.storedOffset = 0;
				this.storedBuffer = array;
			}
			Buffer.BlockCopy(buffer, offset, this.storedBuffer, this.storedOffset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Seek operation is not supported on this Stream.", new object[] { this.stream.GetType().FullName })));
		}

		public override void SetLength(long value)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Seek operation is not supported on this Stream.", new object[] { this.stream.GetType().FullName })));
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Write operation is not supported on this '{0}' Stream.", new object[] { this.stream.GetType().FullName })));
		}

		private Stream stream;

		private byte[] storedBuffer;

		private int storedLength;

		private int storedOffset;

		private bool readMore;
	}
}
