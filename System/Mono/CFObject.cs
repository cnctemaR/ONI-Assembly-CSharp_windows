using System;
using System.Runtime.InteropServices;
using ObjCRuntimeInternal;

namespace Mono
{
	internal class CFObject : IDisposable, INativeObject
	{
		[DllImport("/usr/lib/libSystem.dylib")]
		public static extern IntPtr dlopen(string path, int mode);

		[DllImport("/usr/lib/libSystem.dylib")]
		private static extern IntPtr dlsym(IntPtr handle, string symbol);

		[DllImport("/usr/lib/libSystem.dylib")]
		public static extern void dlclose(IntPtr handle);

		public static IntPtr GetIndirect(IntPtr handle, string symbol)
		{
			return CFObject.dlsym(handle, symbol);
		}

		public static CFString GetStringConstant(IntPtr handle, string symbol)
		{
			IntPtr intPtr = CFObject.dlsym(handle, symbol);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			IntPtr intPtr2 = Marshal.ReadIntPtr(intPtr);
			if (intPtr2 == IntPtr.Zero)
			{
				return null;
			}
			return new CFString(intPtr2, false);
		}

		public static IntPtr GetIntPtr(IntPtr handle, string symbol)
		{
			IntPtr intPtr = CFObject.dlsym(handle, symbol);
			if (intPtr == IntPtr.Zero)
			{
				return IntPtr.Zero;
			}
			return Marshal.ReadIntPtr(intPtr);
		}

		public static IntPtr GetCFObjectHandle(IntPtr handle, string symbol)
		{
			IntPtr intPtr = CFObject.dlsym(handle, symbol);
			if (intPtr == IntPtr.Zero)
			{
				return IntPtr.Zero;
			}
			return Marshal.ReadIntPtr(intPtr);
		}

		public CFObject(IntPtr handle, bool own)
		{
			this.Handle = handle;
			if (!own)
			{
				this.Retain();
			}
		}

		~CFObject()
		{
			this.Dispose(false);
		}

		public IntPtr Handle { get; private set; }

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		internal static extern IntPtr CFRetain(IntPtr handle);

		private void Retain()
		{
			CFObject.CFRetain(this.Handle);
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		internal static extern void CFRelease(IntPtr handle);

		private void Release()
		{
			CFObject.CFRelease(this.Handle);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.Handle != IntPtr.Zero)
			{
				this.Release();
				this.Handle = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

		private const string SystemLibrary = "/usr/lib/libSystem.dylib";
	}
}
