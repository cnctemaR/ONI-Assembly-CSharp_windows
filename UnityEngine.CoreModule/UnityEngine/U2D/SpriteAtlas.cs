using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.U2D
{
	[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
	[NativeType(Header = "Runtime/2D/SpriteAtlas/SpriteAtlas.h")]
	public class SpriteAtlas : Object
	{
		public extern bool isVariant
		{
			[NativeMethod("IsVariant")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string tag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int spriteCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool CanBindTo([NotNull] Sprite sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Sprite GetSprite(string name);

		public int GetSprites(Sprite[] sprites)
		{
			return this.GetSpritesScripting(sprites);
		}

		public int GetSprites(Sprite[] sprites, string name)
		{
			return this.GetSpritesWithNameScripting(sprites, name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetSpritesScripting(Sprite[] sprites);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetSpritesWithNameScripting(Sprite[] sprites, string name);
	}
}
