using System;

namespace ObjCRuntimeInternal
{
	internal static class NativeObjectHelper
	{
		public static IntPtr GetHandle(this INativeObject self)
		{
			if (self != null)
			{
				return self.Handle;
			}
			return IntPtr.Zero;
		}
	}
}
