using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.U2D
{
	/// <summary>
	///   <para>Manages SpriteAtlas during runtime.</para>
	/// </summary>
	[NativeHeader("Runtime/2D/SpriteAtlas/SpriteAtlasManager.h")]
	[NativeHeader("Runtime/2D/SpriteAtlas/SpriteAtlas.h")]
	[StaticAccessor("GetSpriteAtlasManager()", StaticAccessorType.Dot)]
	public class SpriteAtlasManager
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event SpriteAtlasManager.RequestAtlasCallback atlasRequested;

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Register(SpriteAtlas spriteAtlas);

		// Note: this type is marked as 'beforefieldinit'.
		static SpriteAtlasManager()
		{
			SpriteAtlasManager.atlasRequested = null;
		}

		/// <summary>
		///   <para>Delegate type for atlas request callback.</para>
		/// </summary>
		/// <param name="tag">Tag of SpriteAtlas that needs to be provided by user.</param>
		/// <param name="action">An Action that takes user loaded SpriteAtlas.</param>
		public delegate void RequestAtlasCallback(string tag, Action<SpriteAtlas> action);
	}
}
