using System;

namespace System.Numerics
{
	internal static class Vector
	{
		[JitIntrinsic]
		public static bool IsHardwareAccelerated
		{
			get
			{
				return false;
			}
		}
	}
}
