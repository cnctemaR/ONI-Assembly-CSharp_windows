using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	[StaticAccessor("SpriteUtilityBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/SpriteMask/Public/ScriptBindings/SpriteMask.bindings.h")]
	public static class SpriteMaskUtility
	{
		public static bool HasSpriteMaskInLayerRange(SortingLayerRange range)
		{
			return SpriteMaskUtility.HasSpriteMaskInLayerRange_Injected(ref range);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasSpriteMaskInLayerRange_Injected(ref SortingLayerRange range);
	}
}
