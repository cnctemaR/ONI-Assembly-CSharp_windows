using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Util;

namespace Mono.Btls
{
	internal class MonoBtlsBioMono : MonoBtlsBio
	{
		public MonoBtlsBioMono(IMonoBtlsBioMono backend)
			: base(new MonoBtlsBio.BoringBioHandle(MonoBtlsBioMono.mono_btls_bio_mono_new()))
		{
			this.backend = backend;
			this.handle = GCHandle.Alloc(this);
			this.instance = GCHandle.ToIntPtr(this.handle);
			this.readFunc = new MonoBtlsBioMono.BioReadFunc(MonoBtlsBioMono.OnRead);
			this.writeFunc = new MonoBtlsBioMono.BioWriteFunc(MonoBtlsBioMono.OnWrite);
			this.controlFunc = new MonoBtlsBioMono.BioControlFunc(MonoBtlsBioMono.Control);
			this.readFuncPtr = Marshal.GetFunctionPointerForDelegate<MonoBtlsBioMono.BioReadFunc>(this.readFunc);
			this.writeFuncPtr = Marshal.GetFunctionPointerForDelegate<MonoBtlsBioMono.BioWriteFunc>(this.writeFunc);
			this.controlFuncPtr = Marshal.GetFunctionPointerForDelegate<MonoBtlsBioMono.BioControlFunc>(this.controlFunc);
			MonoBtlsBioMono.mono_btls_bio_mono_initialize(base.Handle.DangerousGetHandle(), this.instance, this.readFuncPtr, this.writeFuncPtr, this.controlFuncPtr);
		}

		public static MonoBtlsBioMono CreateStream(Stream stream, bool ownsStream)
		{
			return new MonoBtlsBioMono(new MonoBtlsBioMono.StreamBackend(stream, ownsStream));
		}

		public static MonoBtlsBioMono CreateString(StringWriter writer)
		{
			return new MonoBtlsBioMono(new MonoBtlsBioMono.StringBackend(writer));
		}

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_bio_mono_new();

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_bio_mono_initialize(IntPtr handle, IntPtr instance, IntPtr readFunc, IntPtr writeFunc, IntPtr controlFunc);

		private long Control(MonoBtlsBioMono.ControlCommand command, long arg)
		{
			if (command == MonoBtlsBioMono.ControlCommand.Flush)
			{
				this.backend.Flush();
				return 1L;
			}
			throw new NotImplementedException();
		}

		private int OnRead(IntPtr data, int dataLength, out int wantMore)
		{
			byte[] array = new byte[dataLength];
			bool flag;
			int num = this.backend.Read(array, 0, dataLength, out flag);
			wantMore = (flag ? 1 : 0);
			if (num <= 0)
			{
				return num;
			}
			Marshal.Copy(array, 0, data, num);
			return num;
		}

		[MonoPInvokeCallback(typeof(MonoBtlsBioMono.BioReadFunc))]
		private static int OnRead(IntPtr instance, IntPtr data, int dataLength, out int wantMore)
		{
			MonoBtlsBioMono monoBtlsBioMono = (MonoBtlsBioMono)GCHandle.FromIntPtr(instance).Target;
			int num;
			try
			{
				num = monoBtlsBioMono.OnRead(data, dataLength, out wantMore);
			}
			catch (Exception ex)
			{
				monoBtlsBioMono.SetException(ex);
				wantMore = 0;
				num = -1;
			}
			return num;
		}

		private int OnWrite(IntPtr data, int dataLength)
		{
			byte[] array = new byte[dataLength];
			Marshal.Copy(data, array, 0, dataLength);
			if (!this.backend.Write(array, 0, dataLength))
			{
				return -1;
			}
			return dataLength;
		}

		[MonoPInvokeCallback(typeof(MonoBtlsBioMono.BioWriteFunc))]
		private static int OnWrite(IntPtr instance, IntPtr data, int dataLength)
		{
			MonoBtlsBioMono monoBtlsBioMono = (MonoBtlsBioMono)GCHandle.FromIntPtr(instance).Target;
			int num;
			try
			{
				num = monoBtlsBioMono.OnWrite(data, dataLength);
			}
			catch (Exception ex)
			{
				monoBtlsBioMono.SetException(ex);
				num = -1;
			}
			return num;
		}

		[MonoPInvokeCallback(typeof(MonoBtlsBioMono.BioControlFunc))]
		private static long Control(IntPtr instance, MonoBtlsBioMono.ControlCommand command, long arg)
		{
			MonoBtlsBioMono monoBtlsBioMono = (MonoBtlsBioMono)GCHandle.FromIntPtr(instance).Target;
			long num;
			try
			{
				num = monoBtlsBioMono.Control(command, arg);
			}
			catch (Exception ex)
			{
				monoBtlsBioMono.SetException(ex);
				num = -1L;
			}
			return num;
		}

		protected override void Close()
		{
			try
			{
				if (this.backend != null)
				{
					this.backend.Close();
					this.backend = null;
				}
				if (this.handle.IsAllocated)
				{
					this.handle.Free();
				}
			}
			finally
			{
				base.Close();
			}
		}

		private GCHandle handle;

		private IntPtr instance;

		private MonoBtlsBioMono.BioReadFunc readFunc;

		private MonoBtlsBioMono.BioWriteFunc writeFunc;

		private MonoBtlsBioMono.BioControlFunc controlFunc;

		private IntPtr readFuncPtr;

		private IntPtr writeFuncPtr;

		private IntPtr controlFuncPtr;

		private IMonoBtlsBioMono backend;

		private enum ControlCommand
		{
			Flush = 1
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int BioReadFunc(IntPtr bio, IntPtr data, int dataLength, out int wantMore);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int BioWriteFunc(IntPtr bio, IntPtr data, int dataLength);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate long BioControlFunc(IntPtr bio, MonoBtlsBioMono.ControlCommand command, long arg);

		private class StreamBackend : IMonoBtlsBioMono
		{
			public Stream InnerStream
			{
				get
				{
					return this.stream;
				}
			}

			public StreamBackend(Stream stream, bool ownsStream)
			{
				this.stream = stream;
				this.ownsStream = ownsStream;
			}

			public int Read(byte[] buffer, int offset, int size, out bool wantMore)
			{
				wantMore = false;
				return this.stream.Read(buffer, offset, size);
			}

			public bool Write(byte[] buffer, int offset, int size)
			{
				this.stream.Write(buffer, offset, size);
				return true;
			}

			public void Flush()
			{
				this.stream.Flush();
			}

			public void Close()
			{
				if (this.ownsStream && this.stream != null)
				{
					this.stream.Dispose();
				}
				this.stream = null;
			}

			private Stream stream;

			private bool ownsStream;
		}

		private class StringBackend : IMonoBtlsBioMono
		{
			public StringBackend(StringWriter writer)
			{
				this.writer = writer;
			}

			public int Read(byte[] buffer, int offset, int size, out bool wantMore)
			{
				wantMore = false;
				return -1;
			}

			public bool Write(byte[] buffer, int offset, int size)
			{
				string @string = this.encoding.GetString(buffer, offset, size);
				this.writer.Write(@string);
				return true;
			}

			public void Flush()
			{
			}

			public void Close()
			{
			}

			private StringWriter writer;

			private Encoding encoding = new UTF8Encoding();
		}
	}
}
