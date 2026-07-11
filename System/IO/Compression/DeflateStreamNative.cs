using System;
using System.Runtime.InteropServices;
using Mono.Util;

namespace System.IO.Compression
{
	internal class DeflateStreamNative
	{
		private DeflateStreamNative()
		{
		}

		public static DeflateStreamNative Create(Stream compressedStream, CompressionMode mode, bool gzip)
		{
			DeflateStreamNative deflateStreamNative = new DeflateStreamNative();
			deflateStreamNative.data = GCHandle.Alloc(deflateStreamNative);
			deflateStreamNative.feeder = ((mode == CompressionMode.Compress) ? new DeflateStreamNative.UnmanagedReadOrWrite(DeflateStreamNative.UnmanagedWrite) : new DeflateStreamNative.UnmanagedReadOrWrite(DeflateStreamNative.UnmanagedRead));
			deflateStreamNative.z_stream = DeflateStreamNative.CreateZStream(mode, gzip, deflateStreamNative.feeder, GCHandle.ToIntPtr(deflateStreamNative.data));
			if (deflateStreamNative.z_stream.IsInvalid)
			{
				deflateStreamNative.Dispose(true);
				return null;
			}
			deflateStreamNative.base_stream = compressedStream;
			return deflateStreamNative;
		}

		~DeflateStreamNative()
		{
			this.Dispose(false);
		}

		public void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				GC.SuppressFinalize(this);
				this.io_buffer = null;
				this.z_stream.Dispose();
			}
			if (this.data.IsAllocated)
			{
				this.data.Free();
			}
		}

		public void Flush()
		{
			DeflateStreamNative.CheckResult(DeflateStreamNative.Flush(this.z_stream), "Flush");
		}

		public int ReadZStream(IntPtr buffer, int length)
		{
			int num = DeflateStreamNative.ReadZStream(this.z_stream, buffer, length);
			DeflateStreamNative.CheckResult(num, "ReadInternal");
			return num;
		}

		public void WriteZStream(IntPtr buffer, int length)
		{
			DeflateStreamNative.CheckResult(DeflateStreamNative.WriteZStream(this.z_stream, buffer, length), "WriteInternal");
		}

		[MonoPInvokeCallback(typeof(DeflateStreamNative.UnmanagedReadOrWrite))]
		private static int UnmanagedRead(IntPtr buffer, int length, IntPtr data)
		{
			DeflateStreamNative deflateStreamNative = GCHandle.FromIntPtr(data).Target as DeflateStreamNative;
			if (deflateStreamNative == null)
			{
				return -1;
			}
			return deflateStreamNative.UnmanagedRead(buffer, length);
		}

		private int UnmanagedRead(IntPtr buffer, int length)
		{
			if (this.io_buffer == null)
			{
				this.io_buffer = new byte[4096];
			}
			int num = Math.Min(length, this.io_buffer.Length);
			int num2 = this.base_stream.Read(this.io_buffer, 0, num);
			if (num2 > 0)
			{
				Marshal.Copy(this.io_buffer, 0, buffer, num2);
			}
			return num2;
		}

		[MonoPInvokeCallback(typeof(DeflateStreamNative.UnmanagedReadOrWrite))]
		private static int UnmanagedWrite(IntPtr buffer, int length, IntPtr data)
		{
			DeflateStreamNative deflateStreamNative = GCHandle.FromIntPtr(data).Target as DeflateStreamNative;
			if (deflateStreamNative == null)
			{
				return -1;
			}
			return deflateStreamNative.UnmanagedWrite(buffer, length);
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

		private static void CheckResult(int result, string where)
		{
			if (result >= 0)
			{
				return;
			}
			string text;
			switch (result)
			{
			case -11:
				text = "IO error";
				goto IL_0082;
			case -10:
				text = "Invalid argument(s)";
				goto IL_0082;
			case -6:
				text = "Invalid version";
				goto IL_0082;
			case -5:
				text = "Internal error (no progress possible)";
				goto IL_0082;
			case -4:
				text = "Not enough memory";
				goto IL_0082;
			case -3:
				text = "Corrupted data";
				goto IL_0082;
			case -2:
				text = "Internal error";
				goto IL_0082;
			case -1:
				text = "Unknown error";
				goto IL_0082;
			}
			text = "Unknown error";
			IL_0082:
			throw new IOException(text + " " + where);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern DeflateStreamNative.SafeDeflateStreamHandle CreateZStream(CompressionMode compress, bool gzip, DeflateStreamNative.UnmanagedReadOrWrite feeder, IntPtr data);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int CloseZStream(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int Flush(DeflateStreamNative.SafeDeflateStreamHandle stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int ReadZStream(DeflateStreamNative.SafeDeflateStreamHandle stream, IntPtr buffer, int length);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
		private static extern int WriteZStream(DeflateStreamNative.SafeDeflateStreamHandle stream, IntPtr buffer, int length);

		private const int BufferSize = 4096;

		private DeflateStreamNative.UnmanagedReadOrWrite feeder;

		private Stream base_stream;

		private DeflateStreamNative.SafeDeflateStreamHandle z_stream;

		private GCHandle data;

		private bool disposed;

		private byte[] io_buffer;

		private const string LIBNAME = "MonoPosixHelper";

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int UnmanagedReadOrWrite(IntPtr buffer, int length, IntPtr data);

		private sealed class SafeDeflateStreamHandle : SafeHandle
		{
			public override bool IsInvalid
			{
				get
				{
					return this.handle == IntPtr.Zero;
				}
			}

			private SafeDeflateStreamHandle()
				: base(IntPtr.Zero, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				DeflateStreamNative.CloseZStream(this.handle);
				return true;
			}
		}
	}
}
