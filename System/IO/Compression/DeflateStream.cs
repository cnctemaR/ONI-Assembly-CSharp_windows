using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.IO.Compression
{
	public class DeflateStream : Stream
	{
		public DeflateStream(Stream compressedStream, CompressionMode mode)
			: this(compressedStream, mode, false, false)
		{
		}

		public DeflateStream(Stream compressedStream, CompressionMode mode, bool leaveOpen)
			: this(compressedStream, mode, leaveOpen, false)
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
			this.data = GCHandle.Alloc(this);
			this.base_stream = compressedStream;
			this.feeder = ((mode != CompressionMode.Compress) ? new DeflateStream.UnmanagedReadOrWrite(DeflateStream.UnmanagedRead) : new DeflateStream.UnmanagedReadOrWrite(DeflateStream.UnmanagedWrite));
			this.z_stream = DeflateStream.CreateZStream(mode, gzip, this.feeder, GCHandle.ToIntPtr(this.data));
			if (this.z_stream == IntPtr.Zero)
			{
				this.base_stream = null;
				this.feeder = null;
				throw new NotImplementedException("Failed to initialize zlib. You probably have an old zlib installed. Version 1.2.0.4 or later is required.");
			}
			this.mode = mode;
			this.leaveOpen = leaveOpen;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				IntPtr intPtr = this.z_stream;
				this.z_stream = IntPtr.Zero;
				int num = 0;
				if (intPtr != IntPtr.Zero)
				{
					num = DeflateStream.CloseZStream(intPtr);
				}
				this.io_buffer = null;
				if (!this.leaveOpen)
				{
					Stream stream = this.base_stream;
					if (stream != null)
					{
						stream.Close();
					}
					this.base_stream = null;
				}
				DeflateStream.CheckResult(num, "Dispose");
			}
			if (this.data.IsAllocated)
			{
				this.data.Free();
				this.data = default(GCHandle);
			}
			base.Dispose(disposing);
		}

		private static int UnmanagedRead(IntPtr buffer, int length, IntPtr data)
		{
			DeflateStream deflateStream = GCHandle.FromIntPtr(data).Target as DeflateStream;
			if (deflateStream == null)
			{
				return -1;
			}
			return deflateStream.UnmanagedRead(buffer, length);
		}

		private unsafe int UnmanagedRead(IntPtr buffer, int length)
		{
			int num = 0;
			int num2 = 1;
			while (length > 0 && num2 > 0)
			{
				if (this.io_buffer == null)
				{
					this.io_buffer = new byte[4096];
				}
				int num3 = Math.Min(length, this.io_buffer.Length);
				num2 = this.base_stream.Read(this.io_buffer, 0, num3);
				if (num2 > 0)
				{
					Marshal.Copy(this.io_buffer, 0, buffer, num2);
					buffer = new IntPtr((void*)((byte*)buffer.ToPointer() + num2));
					length -= num2;
					num += num2;
				}
			}
			return num;
		}

		private static int UnmanagedWrite(IntPtr buffer, int length, IntPtr data)
		{
			DeflateStream deflateStream = GCHandle.FromIntPtr(data).Target as DeflateStream;
			if (deflateStream == null)
			{
				return -1;
			}
			return deflateStream.UnmanagedWrite(buffer, length);
		}

		private unsafe int UnmanagedWrite(IntPtr buffer, int length)
		{
			int num = 0;
			while (length > 0)
			{
				if (this.io_buffer == null)
				{
					this.io_buffer = new byte[4096];
				}
				int num2 = Math.Min(length, this.io_buffer.Length);
				Marshal.Copy(buffer, this.io_buffer, 0, num2);
				this.base_stream.Write(this.io_buffer, 0, num2);
				buffer = new IntPtr((void*)((byte*)buffer.ToPointer() + num2));
				length -= num2;
				num += num2;
			}
			return num;
		}

		private unsafe int ReadInternal(byte[] array, int offset, int count)
		{
			if (count == 0)
			{
				return 0;
			}
			int num;
			fixed (byte* ptr = (ref array != null && array.Length != 0 ? ref array[0] : ref *null))
			{
				IntPtr intPtr = new IntPtr((void*)(ptr + offset));
				num = DeflateStream.ReadZStream(this.z_stream, intPtr, count);
			}
			DeflateStream.CheckResult(num, "ReadInternal");
			return num;
		}

		public override int Read(byte[] dest, int dest_offset, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (dest == null)
			{
				throw new ArgumentNullException("Destination array is null.");
			}
			if (!this.CanRead)
			{
				throw new InvalidOperationException("Stream does not support reading.");
			}
			int num = dest.Length;
			if (dest_offset < 0 || count < 0)
			{
				throw new ArgumentException("Dest or count is negative.");
			}
			if (dest_offset > num)
			{
				throw new ArgumentException("destination offset is beyond array size");
			}
			if (dest_offset + count > num)
			{
				throw new ArgumentException("Reading would overrun buffer");
			}
			return this.ReadInternal(dest, dest_offset, count);
		}

		private unsafe void WriteInternal(byte[] array, int offset, int count)
		{
			if (count == 0)
			{
				return;
			}
			int num;
			fixed (byte* ptr = (ref array != null && array.Length != 0 ? ref array[0] : ref *null))
			{
				IntPtr intPtr = new IntPtr((void*)(ptr + offset));
				num = DeflateStream.WriteZStream(this.z_stream, intPtr, count);
			}
			DeflateStream.CheckResult(num, "WriteInternal");
		}

		public override void Write(byte[] src, int src_offset, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (src == null)
			{
				throw new ArgumentNullException("src");
			}
			if (src_offset < 0)
			{
				throw new ArgumentOutOfRangeException("src_offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Stream does not support writing");
			}
			this.WriteInternal(src, src_offset, count);
		}

		private static void CheckResult(int result, string where)
		{
			if (result >= 0)
			{
				return;
			}
			string text;
			switch (result + 11)
			{
			case 0:
				text = "IO error";
				goto IL_00A7;
			case 1:
				text = "Invalid argument(s)";
				goto IL_00A7;
			case 5:
				text = "Invalid version";
				goto IL_00A7;
			case 6:
				text = "Internal error (no progress possible)";
				goto IL_00A7;
			case 7:
				text = "Not enough memory";
				goto IL_00A7;
			case 8:
				text = "Corrupted data";
				goto IL_00A7;
			case 9:
				text = "Internal error";
				goto IL_00A7;
			case 10:
				text = "Unknown error";
				goto IL_00A7;
			}
			text = "Unknown error";
			IL_00A7:
			throw new IOException(text + " " + where);
		}

		public override void Flush()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (this.CanWrite)
			{
				int num = DeflateStream.Flush(this.z_stream);
				DeflateStream.CheckResult(num, "Flush");
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (!this.CanRead)
			{
				throw new NotSupportedException("This stream does not support reading");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Must be >= 0");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentException("Buffer too small. count/offset wrong.");
			}
			DeflateStream.ReadMethod readMethod = new DeflateStream.ReadMethod(this.ReadInternal);
			return readMethod.BeginInvoke(buffer, offset, count, cback, state);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (!this.CanWrite)
			{
				throw new InvalidOperationException("This stream does not support writing");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Must be >= 0");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentException("Buffer too small. count/offset wrong.");
			}
			DeflateStream.WriteMethod writeMethod = new DeflateStream.WriteMethod(this.WriteInternal);
			return writeMethod.BeginInvoke(buffer, offset, count, cback, state);
		}

		public override int EndRead(IAsyncResult async_result)
		{
			if (async_result == null)
			{
				throw new ArgumentNullException("async_result");
			}
			AsyncResult asyncResult = async_result as AsyncResult;
			if (asyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "async_result");
			}
			DeflateStream.ReadMethod readMethod = asyncResult.AsyncDelegate as DeflateStream.ReadMethod;
			if (readMethod == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "async_result");
			}
			return readMethod.EndInvoke(async_result);
		}

		public override void EndWrite(IAsyncResult async_result)
		{
			if (async_result == null)
			{
				throw new ArgumentNullException("async_result");
			}
			AsyncResult asyncResult = async_result as AsyncResult;
			if (asyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "async_result");
			}
			DeflateStream.WriteMethod writeMethod = asyncResult.AsyncDelegate as DeflateStream.WriteMethod;
			if (writeMethod == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "async_result");
			}
			writeMethod.EndInvoke(async_result);
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

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr CreateZStream(CompressionMode compress, bool gzip, DeflateStream.UnmanagedReadOrWrite feeder, IntPtr data);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int CloseZStream(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int Flush(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int ReadZStream(IntPtr stream, IntPtr buffer, int length);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int WriteZStream(IntPtr stream, IntPtr buffer, int length);

		private const int BufferSize = 4096;

		private const string LIBNAME = "MonoPosixHelper";

		private Stream base_stream;

		private CompressionMode mode;

		private bool leaveOpen;

		private bool disposed;

		private DeflateStream.UnmanagedReadOrWrite feeder;

		private IntPtr z_stream;

		private byte[] io_buffer;

		private GCHandle data;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int UnmanagedReadOrWrite(IntPtr buffer, int length, IntPtr data);

		private delegate int ReadMethod(byte[] array, int offset, int count);

		private delegate void WriteMethod(byte[] array, int offset, int count);
	}
}
