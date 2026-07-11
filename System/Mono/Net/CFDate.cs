using System;
using System.Runtime.InteropServices;
using ObjCRuntimeInternal;

namespace Mono.Net
{
	internal class CFDate : INativeObject, IDisposable
	{
		internal CFDate(IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		~CFDate()
		{
			this.Dispose(false);
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDateCreate(IntPtr allocator, double at);

		public static CFDate Create(DateTime date)
		{
			DateTime dateTime = new DateTime(2001, 1, 1);
			double totalSeconds = (date - dateTime).TotalSeconds;
			IntPtr intPtr = CFDate.CFDateCreate(IntPtr.Zero, totalSeconds);
			if (intPtr == IntPtr.Zero)
			{
				throw new NotSupportedException();
			}
			return new CFDate(intPtr, true);
		}

		public IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.handle != IntPtr.Zero)
			{
				CFObject.CFRelease(this.handle);
				this.handle = IntPtr.Zero;
			}
		}

		private IntPtr handle;
	}
}
