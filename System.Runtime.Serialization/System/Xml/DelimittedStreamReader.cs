using System;
using System.IO;
using System.Runtime.Serialization;

namespace System.Xml
{
	internal class DelimittedStreamReader
	{
		public DelimittedStreamReader(Stream stream)
		{
			this.stream = new BufferedReadStream(stream);
		}

		public void Close()
		{
			this.stream.Close();
		}

		private void Close(DelimittedStreamReader.DelimittedReadStream caller)
		{
			if (this.currentStream == caller)
			{
				if (this.delimitter == null)
				{
					this.stream.Close();
				}
				else
				{
					if (this.scratch == null)
					{
						this.scratch = new byte[1024];
					}
					while (this.Read(caller, this.scratch, 0, this.scratch.Length) != 0)
					{
					}
				}
				this.currentStream = null;
			}
		}

		public Stream GetNextStream(byte[] delimitter)
		{
			if (this.currentStream != null)
			{
				this.currentStream.Close();
				this.currentStream = null;
			}
			if (!this.canGetNextStream)
			{
				return null;
			}
			this.delimitter = delimitter;
			this.canGetNextStream = delimitter != null;
			this.currentStream = new DelimittedStreamReader.DelimittedReadStream(this);
			return this.currentStream;
		}

		private DelimittedStreamReader.MatchState MatchDelimitter(byte[] buffer, int start, int end)
		{
			if (this.delimitter.Length > end - start)
			{
				for (int i = end - start - 1; i >= 1; i--)
				{
					if (buffer[start + i] != this.delimitter[i])
					{
						return DelimittedStreamReader.MatchState.False;
					}
				}
				return DelimittedStreamReader.MatchState.InsufficientData;
			}
			for (int j = this.delimitter.Length - 1; j >= 1; j--)
			{
				if (buffer[start + j] != this.delimitter[j])
				{
					return DelimittedStreamReader.MatchState.False;
				}
			}
			return DelimittedStreamReader.MatchState.True;
		}

		private int ProcessRead(byte[] buffer, int offset, int read)
		{
			if (read == 0)
			{
				return read;
			}
			int i = offset;
			int num = offset + read;
			while (i < num)
			{
				if (buffer[i] == this.delimitter[0])
				{
					switch (this.MatchDelimitter(buffer, i, num))
					{
					case DelimittedStreamReader.MatchState.True:
					{
						int num2 = i - offset;
						i += this.delimitter.Length;
						this.stream.Push(buffer, i, num - i);
						this.currentStream = null;
						return num2;
					}
					case DelimittedStreamReader.MatchState.InsufficientData:
					{
						int num3 = i - offset;
						if (num3 > 0)
						{
							this.stream.Push(buffer, i, num - i);
							return num3;
						}
						return -1;
					}
					}
				}
				i++;
			}
			return read;
		}

		private int Read(DelimittedStreamReader.DelimittedReadStream caller, byte[] buffer, int offset, int count)
		{
			if (this.currentStream != caller)
			{
				return 0;
			}
			int num = this.stream.Read(buffer, offset, count);
			if (num == 0)
			{
				this.canGetNextStream = false;
				this.currentStream = null;
				return num;
			}
			if (this.delimitter == null)
			{
				return num;
			}
			int num2 = this.ProcessRead(buffer, offset, num);
			if (num2 < 0)
			{
				if (this.matchBuffer == null || this.matchBuffer.Length < this.delimitter.Length - num)
				{
					this.matchBuffer = new byte[this.delimitter.Length - num];
				}
				int num3 = this.stream.ReadBlock(this.matchBuffer, 0, this.delimitter.Length - num);
				if (this.MatchRemainder(num, num3))
				{
					this.currentStream = null;
					num2 = 0;
				}
				else
				{
					this.stream.Push(this.matchBuffer, 0, num3);
					int num4 = 1;
					while (num4 < num && buffer[num4] != this.delimitter[0])
					{
						num4++;
					}
					if (num4 < num)
					{
						this.stream.Push(buffer, offset + num4, num - num4);
					}
					num2 = num4;
				}
			}
			return num2;
		}

		private bool MatchRemainder(int start, int count)
		{
			if (start + count != this.delimitter.Length)
			{
				return false;
			}
			for (count--; count >= 0; count--)
			{
				if (this.delimitter[start + count] != this.matchBuffer[count])
				{
					return false;
				}
			}
			return true;
		}

		internal void Push(byte[] buffer, int offset, int count)
		{
			this.stream.Push(buffer, offset, count);
		}

		private bool canGetNextStream = true;

		private DelimittedStreamReader.DelimittedReadStream currentStream;

		private byte[] delimitter;

		private byte[] matchBuffer;

		private byte[] scratch;

		private BufferedReadStream stream;

		private enum MatchState
		{
			True,
			False,
			InsufficientData
		}

		private class DelimittedReadStream : Stream
		{
			public DelimittedReadStream(DelimittedStreamReader reader)
			{
				if (reader == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
				}
				this.reader = reader;
			}

			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return false;
				}
			}

			public override long Length
			{
				get
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Seek operation is not supported on this Stream.", new object[] { base.GetType().FullName })));
				}
			}

			public override long Position
			{
				get
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Seek operation is not supported on this Stream.", new object[] { base.GetType().FullName })));
				}
				set
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Seek operation is not supported on this Stream.", new object[] { base.GetType().FullName })));
				}
			}

			public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Write operation is not supported on this '{0}' Stream.", new object[] { base.GetType().FullName })));
			}

			public override void Close()
			{
				this.reader.Close(this);
			}

			public override void EndWrite(IAsyncResult asyncResult)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Write operation is not supported on this '{0}' Stream.", new object[] { base.GetType().FullName })));
			}

			public override void Flush()
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Write operation is not supported on this '{0}' Stream.", new object[] { base.GetType().FullName })));
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
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
				return this.reader.Read(this, buffer, offset, count);
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Seek operation is not supported on this Stream.", new object[] { base.GetType().FullName })));
			}

			public override void SetLength(long value)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Write operation is not supported on this '{0}' Stream.", new object[] { base.GetType().FullName })));
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Write operation is not supported on this '{0}' Stream.", new object[] { base.GetType().FullName })));
			}

			private DelimittedStreamReader reader;
		}
	}
}
