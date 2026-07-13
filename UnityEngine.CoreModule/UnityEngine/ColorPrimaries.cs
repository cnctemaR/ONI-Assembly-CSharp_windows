using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Graphics/ColorGamut.h")]
	public enum ColorPrimaries
	{
		Unknown = -1,
		Rec709,
		Rec2020,
		P3
	}
}
