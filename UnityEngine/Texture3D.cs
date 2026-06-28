using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Texture3D : Texture
	{
		public Texture3D(int width, int height, int depth, TextureFormat format, bool mipmap)
		{
			Texture3D.Internal_Create(this, width, height, depth, format, mipmap);
		}

		public extern int depth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels([DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels()
		{
			int num = 0;
			return this.GetPixels(num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32([DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color32[] GetPixels32()
		{
			int num = 0;
			return this.GetPixels32(num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors)
		{
			int num = 0;
			this.SetPixels(colors, num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels32(Color32[] colors)
		{
			int num = 0;
			this.SetPixels32(colors, num);
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
		private static extern void Internal_Create([Writable] Texture3D mono, int width, int height, int depth, TextureFormat format, bool mipmap);
	}
}
