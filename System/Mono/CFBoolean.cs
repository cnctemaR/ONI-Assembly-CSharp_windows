using System;
using System.Runtime.InteropServices;
using ObjCRuntimeInternal;

namespace Mono
{
	internal class CFBoolean : INativeObject, IDisposable
	{
		static CFBoolean()
		{
			IntPtr intPtr = CFObject.dlopen("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", 0);
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			try
			{
				CFBoolean.True = new CFBoolean(CFObject.GetCFObjectHandle(intPtr, "kCFBooleanTrue"), false);
				CFBoolean.False = new CFBoolean(CFObject.GetCFObjectHandle(intPtr, "kCFBooleanFalse"), false);
			}
			finally
			{
				CFObject.dlclose(intPtr);
			}
		}

		internal CFBoolean(IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		~CFBoolean()
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
			if (this.handle != IntPtr.Zero)
			{
				CFObject.CFRelease(this.handle);
				this.handle = IntPtr.Zero;
			}
		}

		public static implicit operator bool(CFBoolean value)
		{
			return value.Value;
		}

		public static explicit operator CFBoolean(bool value)
		{
			return CFBoolean.FromBoolean(value);
		}

		public static CFBoolean FromBoolean(bool value)
		{
			if (!value)
			{
				return CFBoolean.False;
			}
			return CFBoolean.True;
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool CFBooleanGetValue(IntPtr boolean);

		public bool Value
		{
			get
			{
				return CFBoolean.CFBooleanGetValue(this.handle);
			}
		}

		public static bool GetValue(IntPtr boolean)
		{
			return CFBoolean.CFBooleanGetValue(boolean);
		}

		private IntPtr handle;

		public static readonly CFBoolean True;

		public static readonly CFBoolean False;
	}
}
