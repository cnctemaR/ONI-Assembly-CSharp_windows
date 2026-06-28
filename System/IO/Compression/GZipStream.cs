using System;

namespace System.IO.Compression
{
	public class GZipStream : Stream
	{
		public GZipStream(Stream compressedStream, CompressionMode mode)
			: this(compressedStream, mode, false)
		{
		}

		public GZipStream(Stream compressedStream, CompressionMode mode, bool leaveOpen)
		{
			this.deflateStream = new DeflateStream(compressedStream, mode, leaveOpen, true);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.deflateStream.Dispose();
			}
			base.Dispose(disposing);
		}

		public override int Read(byte[] dest, int dest_offset, int count)
		{
			return this.deflateStream.Read(dest, dest_offset, count);
		}

		public override void Write(byte[] src, int src_offset, int count)
		{
			this.deflateStream.Write(src, src_offset, count);
		}

		public override void Flush()
		{
			this.deflateStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.deflateStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			this.deflateStream.SetLength(value);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			return this.deflateStream.BeginRead(buffer, offset, count, cback, state);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			return this.deflateStream.BeginWrite(buffer, offset, count, cback, state);
		}

		public override int EndRead(IAsyncResult async_result)
		{
			return this.deflateStream.EndRead(async_result);
		}

		public override void EndWrite(IAsyncResult async_result)
		{
			this.deflateStream.EndWrite(async_result);
		}

		public Stream BaseStream
		{
			get
			{
				return this.deflateStream.BaseStream;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.deflateStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.deflateStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.deflateStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return this.deflateStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.deflateStream.Position;
			}
			set
			{
				this.deflateStream.Position = value;
			}
		}

		private DeflateStream deflateStream;
	}
}
