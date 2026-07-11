using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.U2D
{
	[StaticAccessor("GetSpriteAtlasManager()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/2D/SpriteAtlas/SpriteAtlas.h")]
	[NativeHeader("Runtime/2D/SpriteAtlas/SpriteAtlasManager.h")]
	public class SpriteAtlasManager
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<string, Action<SpriteAtlas>> atlasRequested;

		[RequiredByNativeCode]
		private static bool RequestAtlas(string tag)
		{
			bool flag;
			if (SpriteAtlasManager.atlasRequested != null)
			{
				SpriteAtlasManager.atlasRequested(tag, new Action<SpriteAtlas>(SpriteAtlasManager.Register));
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<SpriteAtlas> atlasRegistered;

		[RequiredByNativeCode]
		private static void PostRegisteredAtlas(SpriteAtlas spriteAtlas)
		{
			if (SpriteAtlasManager.atlasRegistered != null)
			{
				SpriteAtlasManager.atlasRegistered(spriteAtlas);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Register(SpriteAtlas spriteAtlas);

		// Note: this type is marked as 'beforefieldinit'.
		static SpriteAtlasManager()
		{
			SpriteAtlasManager.atlasRequested = null;
			SpriteAtlasManager.atlasRegistered = null;
		}
	}
}
