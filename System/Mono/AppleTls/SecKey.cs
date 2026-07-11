using System;
using System.Runtime.InteropServices;
using Mono.Net;
using ObjCRuntimeInternal;

namespace Mono.AppleTls
{
	internal class SecKey : INativeObject, IDisposable
	{
		public SecKey(IntPtr handle, bool owns = false)
		{
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		internal SecKey(IntPtr handle, IntPtr owner)
		{
			this.handle = handle;
			this.owner = owner;
			CFObject.CFRetain(owner);
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security", EntryPoint = "SecKeyGetTypeID")]
		public static extern IntPtr GetTypeID();

		~SecKey()
		{
			this.Dispose(false);
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
			if (this.owner != IntPtr.Zero)
			{
				CFObject.CFRelease(this.owner);
				this.owner = (this.handle = IntPtr.Zero);
				return;
			}
			if (this.handle != IntPtr.Zero)
			{
				CFObject.CFRelease(this.handle);
				this.handle = IntPtr.Zero;
			}
		}

		internal IntPtr handle;

		internal IntPtr owner;
	}
}
