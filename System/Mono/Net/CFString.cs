using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Net
{
	internal class CFString : CFObject
	{
		public CFString(IntPtr handle, bool own)
			: base(handle, own)
		{
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFStringCreateWithCharacters(IntPtr alloc, IntPtr chars, IntPtr length);

		public unsafe static CFString Create(string value)
		{
			IntPtr intPtr;
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				intPtr = CFString.CFStringCreateWithCharacters(IntPtr.Zero, (IntPtr)((void*)ptr), (IntPtr)value.Length);
			}
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return new CFString(intPtr, true);
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFStringGetLength(IntPtr handle);

		public int Length
		{
			get
			{
				if (this.str != null)
				{
					return this.str.Length;
				}
				return (int)CFString.CFStringGetLength(base.Handle);
			}
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFStringGetCharactersPtr(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFStringGetCharacters(IntPtr handle, CFRange range, IntPtr buffer);

		public unsafe static string AsString(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			int num = (int)CFString.CFStringGetLength(handle);
			if (num == 0)
			{
				return string.Empty;
			}
			IntPtr intPtr = CFString.CFStringGetCharactersPtr(handle);
			IntPtr intPtr2 = IntPtr.Zero;
			if (intPtr == IntPtr.Zero)
			{
				CFRange cfrange = new CFRange(0, num);
				intPtr2 = Marshal.AllocHGlobal(num * 2);
				CFString.CFStringGetCharacters(handle, cfrange, intPtr2);
				intPtr = intPtr2;
			}
			string text = new string((char*)(void*)intPtr, 0, num);
			if (intPtr2 != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr2);
			}
			return text;
		}

		public override string ToString()
		{
			if (this.str == null)
			{
				this.str = CFString.AsString(base.Handle);
			}
			return this.str;
		}

		public static implicit operator string(CFString str)
		{
			return str.ToString();
		}

		public static implicit operator CFString(string str)
		{
			return CFString.Create(str);
		}

		private string str;
	}
}
