using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Custom Render Textures are an extension to Render Textures, enabling you to render directly to the Texture using a Shader.</para>
	/// </summary>
	[UsedByNativeCode]
	public sealed class CustomRenderTexture : RenderTexture
	{
		/// <summary>
		///   <para>Create a new Custom Render Texture.</para>
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="format"></param>
		/// <param name="readWrite"></param>
		public CustomRenderTexture(int width, int height, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			CustomRenderTexture.Internal_CreateCustomRenderTexture(this, readWrite);
			this.width = width;
			this.height = height;
			base.format = format;
		}

		/// <summary>
		///   <para>Create a new Custom Render Texture.</para>
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="format"></param>
		/// <param name="readWrite"></param>
		public CustomRenderTexture(int width, int height, RenderTextureFormat format)
		{
			CustomRenderTexture.Internal_CreateCustomRenderTexture(this, RenderTextureReadWrite.Default);
			this.width = width;
			this.height = height;
			base.format = format;
		}

		/// <summary>
		///   <para>Create a new Custom Render Texture.</para>
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="format"></param>
		/// <param name="readWrite"></param>
		public CustomRenderTexture(int width, int height)
		{
			CustomRenderTexture.Internal_CreateCustomRenderTexture(this, RenderTextureReadWrite.Default);
			this.width = width;
			this.height = height;
			base.format = RenderTextureFormat.Default;
		}

		public CustomRenderTexture(int width, int height, GraphicsFormat format)
		{
			CustomRenderTexture.Internal_CreateCustomRenderTexture(this, (!GraphicsFormatUtility.IsSRGBFormat(format)) ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
			this.width = width;
			this.height = height;
			base.format = GraphicsFormatUtility.GetRenderTextureFormat(format);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateCustomRenderTexture([Writable] CustomRenderTexture rt, RenderTextureReadWrite readWrite);

		/// <summary>
		///   <para>Triggers the update of the Custom Render Texture.</para>
		/// </summary>
		/// <param name="count">Number of upate pass to perform.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update([DefaultValue("1")] int count);

		[ExcludeFromDocs]
		public void Update()
		{
			int num = 1;
			this.Update(num);
		}

		/// <summary>
		///   <para>Triggers an initialization of the Custom Render Texture.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Initialize();

		/// <summary>
		///   <para>Clear all Update Zones.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearUpdateZones();

		/// <summary>
		///   <para>Material with which the content of the Custom Render Texture is updated.</para>
		/// </summary>
		public extern Material material
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Material with which the Custom Render Texture is initialized. Initialization texture and color are ignored if this parameter is set.</para>
		/// </summary>
		public extern Material initializationMaterial
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Texture with which the Custom Render Texture is initialized (multiplied by the initialization color). This parameter will be ignored if an initializationMaterial is set.</para>
		/// </summary>
		public extern Texture initializationTexture
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		private bool IsCubemapFaceEnabled(CubemapFace face)
		{
			return ((ulong)this.cubemapFaceMask & (ulong)(1L << (int)(face & (CubemapFace)31))) != 0UL;
		}

		private void EnableCubemapFace(CubemapFace face, bool value)
		{
			uint num = this.cubemapFaceMask;
			uint num2 = 1U << (int)face;
			if (value)
			{
				num |= num2;
			}
			else
			{
				num &= ~num2;
			}
			this.cubemapFaceMask = num;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetUpdateZonesInternal(object updateZones);

		public void GetUpdateZones(List<CustomRenderTextureUpdateZone> updateZones)
		{
			this.GetUpdateZonesInternal(updateZones);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetUpdateZonesInternal(CustomRenderTextureUpdateZone[] updateZones);

		/// <summary>
		///   <para>Setup the list of Update Zones for the Custom Render Texture.</para>
		/// </summary>
		/// <param name="updateZones"></param>
		public void SetUpdateZones(CustomRenderTextureUpdateZone[] updateZones)
		{
			if (updateZones == null)
			{
				throw new ArgumentNullException("updateZones");
			}
			this.SetUpdateZonesInternal(updateZones);
		}

		/// <summary>
		///   <para>Specify if the texture should be initialized with a Texture and a Color or a Material.</para>
		/// </summary>
		public extern CustomRenderTextureInitializationSource initializationSource
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Color with which the Custom Render Texture is initialized. This parameter will be ignored if an initializationMaterial is set.</para>
		/// </summary>
		public Color initializationColor
		{
			get
			{
				Color color;
				this.INTERNAL_get_initializationColor(out color);
				return color;
			}
			set
			{
				this.INTERNAL_set_initializationColor(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_initializationColor(out Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_initializationColor(ref Color value);

		/// <summary>
		///   <para>Specify how the texture should be updated.</para>
		/// </summary>
		public extern CustomRenderTextureUpdateMode updateMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Specify how the texture should be initialized.</para>
		/// </summary>
		public extern CustomRenderTextureUpdateMode initializationMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Space in which the update zones are expressed (Normalized or Pixel space).</para>
		/// </summary>
		public extern CustomRenderTextureUpdateZoneSpace updateZoneSpace
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Shader Pass used to update the Custom Render Texture.</para>
		/// </summary>
		public extern int shaderPass
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Bitfield that allows to enable or disable update on each of the cubemap faces. Order from least significant bit is +X, -X, +Y, -Y, +Z, -Z.</para>
		/// </summary>
		public extern uint cubemapFaceMask
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>If true, the Custom Render Texture is double buffered so that you can access it during its own update. otherwise the Custom Render Texture will be not be double buffered.</para>
		/// </summary>
		public extern bool doubleBuffered
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>If true, Update zones will wrap around the border of the Custom Render Texture. Otherwise, Update zones will be clamped at the border of the Custom Render Texture.</para>
		/// </summary>
		public extern bool wrapUpdateZones
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
