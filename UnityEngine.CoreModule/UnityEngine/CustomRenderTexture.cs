using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Graphics/CustomRenderTexture.h")]
	public sealed class CustomRenderTexture : RenderTexture
	{
		public CustomRenderTexture(int width, int height, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			if (base.ValidateFormat(format))
			{
				CustomRenderTexture.Internal_CreateCustomRenderTexture(this, readWrite);
				this.width = width;
				this.height = height;
				base.format = format;
			}
		}

		public CustomRenderTexture(int width, int height, RenderTextureFormat format)
		{
			if (base.ValidateFormat(format))
			{
				CustomRenderTexture.Internal_CreateCustomRenderTexture(this, RenderTextureReadWrite.Default);
				this.width = width;
				this.height = height;
				base.format = format;
			}
		}

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

		[FreeFunction(Name = "CustomRenderTextureScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateCustomRenderTexture([Writable] CustomRenderTexture rt, RenderTextureReadWrite readWrite);

		[NativeName("TriggerUpdate")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update(int count);

		public void Update()
		{
			this.Update(1);
		}

		[NativeName("TriggerInitialization")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Initialize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearUpdateZones();

		public extern Material material
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Material initializationMaterial
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Texture initializationTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction(Name = "CustomRenderTextureScripting::GetUpdateZonesInternal", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetUpdateZonesInternal([NotNull] object updateZones);

		public void GetUpdateZones(List<CustomRenderTextureUpdateZone> updateZones)
		{
			this.GetUpdateZonesInternal(updateZones);
		}

		[FreeFunction(Name = "CustomRenderTextureScripting::SetUpdateZonesInternal", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetUpdateZonesInternal(CustomRenderTextureUpdateZone[] updateZones);

		public void SetUpdateZones(CustomRenderTextureUpdateZone[] updateZones)
		{
			if (updateZones == null)
			{
				throw new ArgumentNullException("updateZones");
			}
			this.SetUpdateZonesInternal(updateZones);
		}

		public extern CustomRenderTextureInitializationSource initializationSource
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color initializationColor
		{
			get
			{
				Color color;
				this.get_initializationColor_Injected(out color);
				return color;
			}
			set
			{
				this.set_initializationColor_Injected(ref value);
			}
		}

		public extern CustomRenderTextureUpdateMode updateMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CustomRenderTextureUpdateMode initializationMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CustomRenderTextureUpdateZoneSpace updateZoneSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int shaderPass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern uint cubemapFaceMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool doubleBuffered
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool wrapUpdateZones
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_initializationColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_initializationColor_Injected(ref Color value);
	}
}
