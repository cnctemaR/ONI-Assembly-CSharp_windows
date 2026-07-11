using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression
{
	public class DeflateStream : Stream
	{
		public DeflateStream(Stream stream, CompressionMode mode)
			: this(stream, mode, false, false)
		{
		}

		public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen)
			: this(stream, mode, leaveOpen, false)
		{
		}

		internal DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen, int windowsBits)
			: this(stream, mode, leaveOpen, true)
		{
		}

		internal DeflateStream(Stream compressedStream, CompressionMode mode, bool leaveOpen, bool gzip)
		{
			if (compressedStream == null)
			{
				throw new ArgumentNullException("compressedStream");
			}
			if (mode != CompressionMode.Compress && mode != CompressionMode.Decompress)
			{
				throw new ArgumentException("mode");
			}
			this.base_stream = compressedStream;
			this.native = DeflateStreamNative.Create(compressedStream, mode, gzip);
			if (this.native == null)
			{
				throw new NotImplementedException("Failed to initialize zlib. You probably have an old zlib installed. Version 1.2.0.4 or later is required.");
			}
			this.mode = mode;
			this.leaveOpen = leaveOpen;
		}

		public DeflateStream(Stream stream, CompressionLevel compressionLevel)
			: this(stream, compressionLevel, false, false)
		{
		}

		public DeflateStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen)
			: this(stream, compressionLevel, leaveOpen, false)
		{
		}

		internal DeflateStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen, int windowsBits)
			: this(stream, compressionLevel, leaveOpen, true)
		{
		}

		internal DeflateStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen, bool gzip)
			: this(stream, CompressionMode.Compress, leaveOpen, gzip)
		{
		}

		protected override void Dispose(bool disposing)
		{
			this.native.Dispose(disposing);
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				if (!this.leaveOpen)
				{
					Stream stream = this.base_stream;
					if (stream != null)
					{
						stream.Close();
					}
					this.base_stream = null;
				}
			}
			base.Dispose(disposing);
		}

		private unsafe int ReadInternal(byte[] array, int offset, int count)
		{
			if (count == 0)
			{
				return 0;
			}
			byte* ptr;
			if (array == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			IntPtr intPtr = new IntPtr((void*)(ptr + offset));
			return this.native.ReadZStream(intPtr, count);
		}

		internal ValueTask<int> ReadAsyncMemory(Memory<byte> destination, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		internal int ReadCore(Span<byte> destination)
		{
			throw new NotImplementedException();
		}

		public override int Read(byte[] array, int offset, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (array == null)
			{
				throw new ArgumentNullException("Destination array is null.");
			}
			if (!this.CanRead)
			{
				throw new InvalidOperationException("Stream does not support reading.");
			}
			int num = array.Length;
			if (offset < 0 || count < 0)
			{
				throw new ArgumentException("Dest or count is negative.");
			}
			if (offset > num)
			{
				throw new ArgumentException("destination offset is beyond array size");
			}
			if (offset + count > num)
			{
				throw new ArgumentException("Reading would overrun buffer");
			}
			return this.ReadInternal(array, offset, count);
		}

		private unsafe void WriteInternal(byte[] array, int offset, int count)
		{
			if (count == 0)
			{
				return;
			}
			fixed (byte[] array2 = array)
			{
				byte* ptr;
				if (array == null || array2.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array2[0];
				}
				IntPtr intPtr = new IntPtr((void*)(ptr + offset));
				this.native.WriteZStream(intPtr, count);
			}
		}

		internal Task WriteAsyncMemory(ReadOnlyMemory<byte> source, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		internal void WriteCore(ReadOnlySpan<byte> source)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] array, int offset, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Stream does not support writing");
			}
			if (offset > array.Length - count)
			{
				throw new ArgumentException("Buffer too small. count/offset wrong.");
			}
			this.WriteInternal(array, offset, count);
		}

		public override void Flush()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (this.CanWrite)
			{
				this.native.Flush();
			}
		}

		public override IAsyncResult BeginRead(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (!this.CanRead)
			{
				throw new NotSupportedException("This stream does not support reading");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Must be >= 0");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
			}
			if (count + offset > array.Length)
			{
				throw new ArgumentException("Buffer too small. count/offset wrong.");
			}
			return new DeflateStream.ReadMethod(this.ReadInternal).BeginInvoke(array, offset, count, asyncCallback, asyncState);
		}

		public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (!this.CanWrite)
			{
				throw new InvalidOperationException("This stream does not support writing");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Must be >= 0");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
			}
			if (count + offset > array.Length)
			{
				throw new ArgumentException("Buffer too small. count/offset wrong.");
			}
			return new DeflateStream.WriteMethod(this.WriteInternal).BeginInvoke(array, offset, count, asyncCallback, asyncState);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			DeflateStream.ReadMethod readMethod = asyncResult2.AsyncDelegate as DeflateStream.ReadMethod;
			if (readMethod == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			return readMethod.EndInvoke(asyncResult);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			DeflateStream.WriteMethod writeMethod = asyncResult2.AsyncDelegate as DeflateStream.WriteMethod;
			if (writeMethod == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			writeMethod.EndInvoke(asyncResult);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public Stream BaseStream
		{
			get
			{
				return this.base_stream;
			}
		}

		public override bool CanRead
		{
			get
			{
				return !this.disposed && this.mode == CompressionMode.Decompress && this.base_stream.CanRead;
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
				return !this.disposed && this.mode == CompressionMode.Compress && this.base_stream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		private Stream base_stream;

		private CompressionMode mode;

		private bool leaveOpen;

		private bool disposed;

		private DeflateStreamNative native;

		private delegate int ReadMethod(byte[] array, int offset, int count);

		private delegate void WriteMethod(byte[] array, int offset, int count);
	}
}
