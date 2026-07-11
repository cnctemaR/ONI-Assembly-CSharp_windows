using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/SparseTexture.h")]
	public sealed class SparseTexture : Texture
	{
		public SparseTexture(int width, int height, GraphicsFormat format, int mipCount)
		{
			if (base.ValidateFormat(format, FormatUsage.Sample))
			{
				SparseTexture.Internal_Create(this, width, height, GraphicsFormatUtility.GetTextureFormat(format), GraphicsFormatUtility.IsSRGBFormat(format), mipCount);
			}
		}

		public SparseTexture(int width, int height, TextureFormat format, int mipCount)
		{
			if (base.ValidateFormat(format))
			{
				SparseTexture.Internal_Create(this, width, height, format, false, mipCount);
			}
		}

		public SparseTexture(int width, int height, TextureFormat format, int mipCount, bool linear)
		{
			if (base.ValidateFormat(format))
			{
				SparseTexture.Internal_Create(this, width, height, format, linear, mipCount);
			}
		}

		public extern int tileWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int tileHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isCreated
		{
			[NativeName("IsInitialized")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction(Name = "SparseTextureScripting::Create", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] SparseTexture mono, int width, int height, TextureFormat format, bool linear, int mipCount);

		[FreeFunction(Name = "SparseTextureScripting::UpdateTile", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateTile(int tileX, int tileY, int miplevel, Color32[] data);

		[FreeFunction(Name = "SparseTextureScripting::UpdateTileRaw", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateTileRaw(int tileX, int tileY, int miplevel, byte[] data);

		public void UnloadTile(int tileX, int tileY, int miplevel)
		{
			this.UpdateTileRaw(tileX, tileY, miplevel, null);
		}
	}
}
