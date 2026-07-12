using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	public class ColorGamutUtility
	{
		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ColorPrimaries GetColorPrimaries(ColorGamut gamut);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern WhitePoint GetWhitePoint(ColorGamut gamut);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern TransferFunction GetTransferFunction(ColorGamut gamut);
	}
}
