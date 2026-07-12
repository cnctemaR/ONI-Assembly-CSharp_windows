using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Mono.Btls
{
	internal class MonoBtlsBio : MonoBtlsObject
	{
		internal MonoBtlsBio(MonoBtlsBio.BoringBioHandle handle)
			: base(handle)
		{
		}

		protected internal new MonoBtlsBio.BoringBioHandle Handle
		{
			get
			{
				return (MonoBtlsBio.BoringBioHandle)base.Handle;
			}
		}

		public static MonoBtlsBio CreateMonoStream(Stream stream)
		{
			return MonoBtlsBioMono.CreateStream(stream, false);
		}

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_read(IntPtr bio, IntPtr data, int len);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_write(IntPtr bio, IntPtr data, int len);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_flush(IntPtr bio);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_indent(IntPtr bio, uint indent, uint max_indent);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_hexdump(IntPtr bio, IntPtr data, int len, uint indent);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_bio_print_errors(IntPtr bio);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_bio_free(IntPtr handle);

		public int Read(byte[] buffer, int offset, int size)
		{
			base.CheckThrow();
			IntPtr intPtr = Marshal.AllocHGlobal(size);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			bool flag = false;
			int num2;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				int num = MonoBtlsBio.mono_btls_bio_read(this.Handle.DangerousGetHandle(), intPtr, size);
				if (num > 0)
				{
					Marshal.Copy(intPtr, buffer, offset, num);
				}
				num2 = num;
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
				Marshal.FreeHGlobal(intPtr);
			}
			return num2;
		}

		public int Write(byte[] buffer, int offset, int size)
		{
			base.CheckThrow();
			IntPtr intPtr = Marshal.AllocHGlobal(size);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			bool flag = false;
			int num;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				Marshal.Copy(buffer, offset, intPtr, size);
				num = MonoBtlsBio.mono_btls_bio_write(this.Handle.DangerousGetHandle(), intPtr, size);
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
				Marshal.FreeHGlobal(intPtr);
			}
			return num;
		}

		public int Flush()
		{
			base.CheckThrow();
			bool flag = false;
			int num;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				num = MonoBtlsBio.mono_btls_bio_flush(this.Handle.DangerousGetHandle());
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
			}
			return num;
		}

		public int Indent(uint indent, uint max_indent)
		{
			base.CheckThrow();
			bool flag = false;
			int num;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				num = MonoBtlsBio.mono_btls_bio_indent(this.Handle.DangerousGetHandle(), indent, max_indent);
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
			}
			return num;
		}

		public int HexDump(byte[] buffer, uint indent)
		{
			base.CheckThrow();
			IntPtr intPtr = Marshal.AllocHGlobal(buffer.Length);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			bool flag = false;
			int num;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				Marshal.Copy(buffer, 0, intPtr, buffer.Length);
				num = MonoBtlsBio.mono_btls_bio_hexdump(this.Handle.DangerousGetHandle(), intPtr, buffer.Length, indent);
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
				Marshal.FreeHGlobal(intPtr);
			}
			return num;
		}

		public void PrintErrors()
		{
			base.CheckThrow();
			bool flag = false;
			try
			{
				this.Handle.DangerousAddRef(ref flag);
				MonoBtlsBio.mono_btls_bio_print_errors(this.Handle.DangerousGetHandle());
			}
			finally
			{
				if (flag)
				{
					this.Handle.DangerousRelease();
				}
			}
		}

		protected internal class BoringBioHandle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringBioHandle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				if (this.handle != IntPtr.Zero)
				{
					MonoBtlsBio.mono_btls_bio_free(this.handle);
					this.handle = IntPtr.Zero;
				}
				return true;
			}
		}
	}
}
