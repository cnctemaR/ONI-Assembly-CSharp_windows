using System;
using System.Runtime.InteropServices;

namespace Mono.Net
{
	internal class CFNumber : CFObject
	{
		public CFNumber(IntPtr handle, bool own)
			: base(handle, own)
		{
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool CFNumberGetValue(IntPtr handle, IntPtr type, [MarshalAs(UnmanagedType.I1)] out bool value);

		public static bool AsBool(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return false;
			}
			bool flag;
			CFNumber.CFNumberGetValue(handle, (IntPtr)1, out flag);
			return flag;
		}

		public static implicit operator bool(CFNumber number)
		{
			return CFNumber.AsBool(number.Handle);
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool CFNumberGetValue(IntPtr handle, IntPtr type, out int value);

		public static int AsInt32(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return 0;
			}
			int num;
			CFNumber.CFNumberGetValue(handle, (IntPtr)9, out num);
			return num;
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFNumberCreate(IntPtr allocator, IntPtr theType, IntPtr valuePtr);

		public static CFNumber FromInt32(int number)
		{
			return new CFNumber(CFNumber.CFNumberCreate(IntPtr.Zero, (IntPtr)9, (IntPtr)number), true);
		}

		public static implicit operator int(CFNumber number)
		{
			return CFNumber.AsInt32(number.Handle);
		}
	}
}
