using System;
using System.Runtime.InteropServices;

namespace Mono.Net
{
	internal class CFData : CFObject
	{
		public CFData(IntPtr handle, bool own)
			: base(handle, own)
		{
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDataCreate(IntPtr allocator, IntPtr bytes, IntPtr length);

		public unsafe static CFData FromData(byte[] buffer)
		{
			byte* ptr;
			if (buffer == null || buffer.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &buffer[0];
			}
			return CFData.FromData((IntPtr)((void*)ptr), (IntPtr)buffer.Length);
		}

		public static CFData FromData(IntPtr buffer, IntPtr length)
		{
			return new CFData(CFData.CFDataCreate(IntPtr.Zero, buffer, length), true);
		}

		public IntPtr Length
		{
			get
			{
				return CFData.CFDataGetLength(base.Handle);
			}
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDataGetLength(IntPtr theData);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDataGetBytePtr(IntPtr theData);

		public IntPtr Bytes
		{
			get
			{
				return CFData.CFDataGetBytePtr(base.Handle);
			}
		}

		public byte this[long idx]
		{
			get
			{
				if (idx < 0L || idx > (long)this.Length)
				{
					throw new ArgumentException("idx");
				}
				return Marshal.ReadByte(new IntPtr(this.Bytes.ToInt64() + idx));
			}
			set
			{
				throw new NotImplementedException("NSData arrays can not be modified, use an NSMutableData instead");
			}
		}
	}
}
