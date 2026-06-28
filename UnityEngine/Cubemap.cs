using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Cubemap : Texture
	{
		public Cubemap(int size, TextureFormat format, bool mipmap)
		{
			Cubemap.Internal_Create(this, size, format, mipmap);
		}

		public void SetPixel(CubemapFace face, int x, int y, Color color)
		{
			Cubemap.INTERNAL_CALL_SetPixel(this, face, x, y, ref color);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPixel(Cubemap self, CubemapFace face, int x, int y, ref Color color);

		public Color GetPixel(CubemapFace face, int x, int y)
		{
			Color color;
			Cubemap.INTERNAL_CALL_GetPixel(this, face, x, y, out color);
			return color;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPixel(Cubemap self, CubemapFace face, int x, int y, out Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(CubemapFace face, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels(CubemapFace face)
		{
			int num = 0;
			return this.GetPixels(face, num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, CubemapFace face, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors, CubemapFace face)
		{
			int num = 0;
			this.SetPixels(colors, face, num);
		}

		public extern int mipmapCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable);

		[ExcludeFromDocs]
		public void Apply(bool updateMipmaps)
		{
			bool flag = false;
			this.Apply(updateMipmaps, flag);
		}

		[ExcludeFromDocs]
		public void Apply()
		{
			bool flag = false;
			bool flag2 = true;
			this.Apply(flag2, flag);
		}

		public extern TextureFormat format
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] Cubemap mono, int size, TextureFormat format, bool mipmap);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SmoothEdges([DefaultValue("1")] int smoothRegionWidthInPixels);

		[ExcludeFromDocs]
		public void SmoothEdges()
		{
			int num = 1;
			this.SmoothEdges(num);
		}
	}
}
