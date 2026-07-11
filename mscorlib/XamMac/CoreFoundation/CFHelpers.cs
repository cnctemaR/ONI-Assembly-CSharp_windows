using System;
using System.Runtime.InteropServices;

namespace XamMac.CoreFoundation
{
	internal static class CFHelpers
	{
		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		internal static extern void CFRelease(IntPtr obj);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		internal static extern IntPtr CFRetain(IntPtr obj);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern IntPtr CFStringCreateWithCharacters(IntPtr allocator, string str, IntPtr count);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern IntPtr CFStringGetLength(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern IntPtr CFStringGetCharactersPtr(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern IntPtr CFStringGetCharacters(IntPtr handle, CFHelpers.CFRange range, IntPtr buffer);

		internal unsafe static string FetchString(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			int num = (int)CFHelpers.CFStringGetLength(handle);
			IntPtr intPtr = CFHelpers.CFStringGetCharactersPtr(handle);
			IntPtr intPtr2 = IntPtr.Zero;
			if (intPtr == IntPtr.Zero)
			{
				CFHelpers.CFRange cfrange = new CFHelpers.CFRange(0, num);
				intPtr2 = Marshal.AllocCoTaskMem(num * 2);
				CFHelpers.CFStringGetCharacters(handle, cfrange, intPtr2);
				intPtr = intPtr2;
			}
			string text = new string((char*)(void*)intPtr, 0, num);
			if (intPtr2 != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(intPtr2);
			}
			return text;
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDataGetLength(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDataGetBytePtr(IntPtr handle);

		internal static byte[] FetchDataBuffer(IntPtr handle)
		{
			byte[] array = new byte[(int)CFHelpers.CFDataGetLength(handle)];
			Marshal.Copy(CFHelpers.CFDataGetBytePtr(handle), array, 0, array.Length);
			return array;
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDataCreateWithBytesNoCopy(IntPtr allocator, IntPtr bytes, IntPtr length, IntPtr bytesDeallocator);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDataCreate(IntPtr allocator, IntPtr bytes, IntPtr length);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecCertificateCreateWithData(IntPtr allocator, IntPtr cfData);

		internal unsafe static IntPtr CreateCertificateFromData(byte[] data)
		{
			void* ptr;
			if (data == null || data.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = (void*)(&data[0]);
			}
			IntPtr intPtr = CFHelpers.CFDataCreate(IntPtr.Zero, (IntPtr)ptr, new IntPtr(data.Length));
			if (intPtr == IntPtr.Zero)
			{
				return IntPtr.Zero;
			}
			IntPtr intPtr2 = CFHelpers.SecCertificateCreateWithData(IntPtr.Zero, intPtr);
			if (intPtr != IntPtr.Zero)
			{
				CFHelpers.CFRelease(intPtr);
			}
			return intPtr2;
		}

		internal const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

		internal const string SecurityLibrary = "/System/Library/Frameworks/Security.framework/Security";

		private struct CFRange
		{
			public CFRange(int loc, int len)
			{
				this = new CFHelpers.CFRange((long)loc, (long)len);
			}

			public CFRange(long l, long len)
			{
				this.loc = (IntPtr)l;
				this.len = (IntPtr)len;
			}

			public IntPtr loc;

			public IntPtr len;
		}
	}
}
