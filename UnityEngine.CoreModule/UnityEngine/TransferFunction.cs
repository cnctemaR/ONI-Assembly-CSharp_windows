using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/ColorGamut.h")]
	[UsedByNativeCode]
	public enum TransferFunction
	{
		Unknown = -1,
		sRGB,
		BT1886,
		PQ,
		Linear,
		Gamma22
	}
}
