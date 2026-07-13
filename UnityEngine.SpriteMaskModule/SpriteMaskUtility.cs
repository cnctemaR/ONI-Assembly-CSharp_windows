using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	[StaticAccessor("SpriteUtilityBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/SpriteMask/Public/ScriptBindings/SpriteMask.bindings.h")]
	internal static class SpriteMaskUtility
	{
		internal static bool HasSpriteMaskInLayerRange(SortingLayerRange range)
		{
			return SpriteMaskUtility.HasSpriteMaskInLayerRange_Injected(ref range);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasSpriteMaskInLayerRange_Injected([In] ref SortingLayerRange range);
	}
}
