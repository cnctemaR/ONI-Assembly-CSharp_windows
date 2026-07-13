using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.U2D
{
	[NativeType(Header = "Runtime/2D/SpriteAtlas/SpriteAtlas.h")]
	[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
	public class SpriteAtlas : Object
	{
		public bool isVariant
		{
			[NativeMethod("IsVariant")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteAtlas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteAtlas.get_isVariant_Injected(intPtr);
			}
		}

		public string tag
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteAtlas>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					SpriteAtlas.get_tag_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public int spriteCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteAtlas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteAtlas.get_spriteCount_Injected(intPtr);
			}
		}

		public bool CanBindTo([NotNull] Sprite sprite)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteAtlas>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			return SpriteAtlas.CanBindTo_Injected(intPtr, intPtr2);
		}

		public unsafe Sprite GetSprite(string name)
		{
			Sprite sprite;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteAtlas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr sprite_Injected = SpriteAtlas.GetSprite_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				IntPtr sprite_Injected;
				sprite = Unmarshal.UnmarshalUnityObject<Sprite>(sprite_Injected);
				char* ptr = null;
			}
			return sprite;
		}

		public int GetSprites(Sprite[] sprites)
		{
			return this.GetSpritesScripting(sprites);
		}

		public int GetSprites(Sprite[] sprites, string name)
		{
			return this.GetSpritesWithNameScripting(sprites, name);
		}

		private int GetSpritesScripting([UnityMarshalAs(NativeType.ScriptingObjectPtr)] Sprite[] sprites)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteAtlas>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return SpriteAtlas.GetSpritesScripting_Injected(intPtr, sprites);
		}

		private unsafe int GetSpritesWithNameScripting([UnityMarshalAs(NativeType.ScriptingObjectPtr)] Sprite[] sprites, string name)
		{
			int spritesWithNameScripting_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteAtlas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				spritesWithNameScripting_Injected = SpriteAtlas.GetSpritesWithNameScripting_Injected(intPtr, sprites, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return spritesWithNameScripting_Injected;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isVariant_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_tag_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_spriteCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CanBindTo_Injected(IntPtr _unity_self, IntPtr sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSprite_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSpritesScripting_Injected(IntPtr _unity_self, Sprite[] sprites);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSpritesWithNameScripting_Injected(IntPtr _unity_self, Sprite[] sprites, ref ManagedSpanWrapper name);
	}
}
