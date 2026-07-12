using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct StringWrapper
	{
		public StringWrapper(IntPtr ptr)
		{
			this.nativeUtf8Ptr = ptr;
		}

		public static implicit operator string(StringWrapper fstring)
		{
			string text;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				text = freeHelper.stringFromNative(fstring.nativeUtf8Ptr);
			}
			return text;
		}

		public bool StartsWith(byte[] prefix)
		{
			if (this.nativeUtf8Ptr == IntPtr.Zero)
			{
				return false;
			}
			for (int i = 0; i < prefix.Length; i++)
			{
				if (Marshal.ReadByte(this.nativeUtf8Ptr, i) != prefix[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool Equals(byte[] comparison)
		{
			if (this.nativeUtf8Ptr == IntPtr.Zero)
			{
				return false;
			}
			for (int i = 0; i < comparison.Length; i++)
			{
				if (Marshal.ReadByte(this.nativeUtf8Ptr, i) != comparison[i])
				{
					return false;
				}
			}
			return Marshal.ReadByte(this.nativeUtf8Ptr, comparison.Length) == 0;
		}

		private IntPtr nativeUtf8Ptr;
	}
}
