using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class DelegatedStream : Stream
	{
		protected DelegatedStream()
		{
		}

		protected DelegatedStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.stream = stream;
			this.netStream = stream as NetworkStream;
		}

		protected Stream BaseStream
		{
			get
			{
				return this.stream;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.stream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.stream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				if (!this.CanSeek)
				{
					throw new NotSupportedException(global::SR.GetString("Seeking is not supported on this stream."));
				}
				return this.stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				if (!this.CanSeek)
				{
					throw new NotSupportedException(global::SR.GetString("Seeking is not supported on this stream."));
				}
				return this.stream.Position;
			}
			set
			{
				if (!this.CanSeek)
				{
					throw new NotSupportedException(global::SR.GetString("Seeking is not supported on this stream."));
				}
				this.stream.Position = value;
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException(global::SR.GetString("Reading is not supported on this stream."));
			}
			IAsyncResult asyncResult;
			if (this.netStream != null)
			{
				asyncResult = this.netStream.UnsafeBeginRead(buffer, offset, count, callback, state);
			}
			else
			{
				asyncResult = this.stream.BeginRead(buffer, offset, count, callback, state);
			}
			return asyncResult;
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException(global::SR.GetString("Writing is not supported on this stream."));
			}
			IAsyncResult asyncResult;
			if (this.netStream != null)
			{
				asyncResult = this.netStream.UnsafeBeginWrite(buffer, offset, count, callback, state);
			}
			else
			{
				asyncResult = this.stream.BeginWrite(buffer, offset, count, callback, state);
			}
			return asyncResult;
		}

		public override void Close()
		{
			this.stream.Close();
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException(global::SR.GetString("Reading is not supported on this stream."));
			}
			return this.stream.EndRead(asyncResult);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException(global::SR.GetString("Writing is not supported on this stream."));
			}
			this.stream.EndWrite(asyncResult);
		}

		public override void Flush()
		{
			this.stream.Flush();
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			return this.stream.FlushAsync(cancellationToken);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException(global::SR.GetString("Reading is not supported on this stream."));
			}
			return this.stream.Read(buffer, offset, count);
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException(global::SR.GetString("Reading is not supported on this stream."));
			}
			return this.stream.ReadAsync(buffer, offset, count, cancellationToken);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (!this.CanSeek)
			{
				throw new NotSupportedException(global::SR.GetString("Seeking is not supported on this stream."));
			}
			return this.stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			if (!this.CanSeek)
			{
				throw new NotSupportedException(global::SR.GetString("Seeking is not supported on this stream."));
			}
			this.stream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException(global::SR.GetString("Writing is not supported on this stream."));
			}
			this.stream.Write(buffer, offset, count);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException(global::SR.GetString("Writing is not supported on this stream."));
			}
			return this.stream.WriteAsync(buffer, offset, count, cancellationToken);
		}

		private Stream stream;

		private NetworkStream netStream;
	}
}
