using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.U2D
{
	/// <summary>
	///   <para>Sprite Atlas is an asset created within Unity. It is part of the built-in sprite packing solution.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
	[NativeType(Header = "Runtime/2D/SpriteAtlas/SpriteAtlas.h")]
	public class SpriteAtlas : Object
	{
		/// <summary>
		///   <para>Return true if this SpriteAtlas is a variant.</para>
		/// </summary>
		public extern bool isVariant
		{
			[NativeMethod("IsVariant")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get the tag of this SpriteAtlas.</para>
		/// </summary>
		public extern string tag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get the total number of Sprite packed into this atlas.</para>
		/// </summary>
		public extern int spriteCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Clone the first Sprite in this atlas that matches the name packed in this atlas and return it.</para>
		/// </summary>
		/// <param name="name">The name of the Sprite.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Sprite GetSprite(string name);

		/// <summary>
		///   <para>Clone all the Sprite in this atlas and fill them into the supplied array.</para>
		/// </summary>
		/// <param name="sprites">Array of Sprite that will be filled.</param>
		/// <returns>
		///   <para>The size of the returned array.</para>
		/// </returns>
		public int GetSprites(Sprite[] sprites)
		{
			return this.GetSpritesScripting(sprites);
		}

		/// <summary>
		///   <para>Clone all the Sprite matching the name in this atlas and fill them into the supplied array.</para>
		/// </summary>
		/// <param name="sprites">Array of Sprite that will be filled.</param>
		/// <param name="name">The name of the Sprite.</param>
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
