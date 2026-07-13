using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.U2D
{
	[NativeHeader("Runtime/2D/SpriteAtlas/SpriteAtlas.h")]
	[StaticAccessor("GetSpriteAtlasManager()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/2D/SpriteAtlas/SpriteAtlasManager.h")]
	public class SpriteAtlasManager
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<string, Action<SpriteAtlas>> atlasRequested;

		[RequiredByNativeCode]
		private static bool RequestAtlas(string tag)
		{
			bool flag = SpriteAtlasManager.atlasRequested != null;
			bool flag2;
			if (flag)
			{
				SpriteAtlasManager.atlasRequested(tag, new Action<SpriteAtlas>(SpriteAtlasManager.Register));
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<SpriteAtlas> atlasRegistered;

		[RequiredByNativeCode]
		private static void PostRegisteredAtlas(SpriteAtlas spriteAtlas)
		{
			Action<SpriteAtlas> action = SpriteAtlasManager.atlasRegistered;
			if (action != null)
			{
				action(spriteAtlas);
			}
		}

		internal static void Register(SpriteAtlas spriteAtlas)
		{
			SpriteAtlasManager.Register_Injected(Object.MarshalledUnityObject.Marshal<SpriteAtlas>(spriteAtlas));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Register_Injected(IntPtr spriteAtlas);
	}
}
