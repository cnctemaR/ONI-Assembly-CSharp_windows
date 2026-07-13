using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/CustomRenderTexture.h")]
	[UsedByNativeCode]
	public sealed class CustomRenderTexture : RenderTexture
	{
		[FreeFunction(Name = "CustomRenderTextureScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateCustomRenderTexture([Writable] CustomRenderTexture rt);

		[NativeName("TriggerUpdate")]
		private void TriggerUpdate(int count)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CustomRenderTexture.TriggerUpdate_Injected(intPtr, count);
		}

		public void Update(int count)
		{
			CustomRenderTextureManager.InvokeTriggerUpdate(this, count);
			this.TriggerUpdate(count);
		}

		public void Update()
		{
			this.Update(1);
		}

		[NativeName("TriggerInitialization")]
		private void TriggerInitialization()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CustomRenderTexture.TriggerInitialization_Injected(intPtr);
		}

		public void Initialize()
		{
			this.TriggerInitialization();
			CustomRenderTextureManager.InvokeTriggerInitialize(this);
		}

		public void ClearUpdateZones()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CustomRenderTexture.ClearUpdateZones_Injected(intPtr);
		}

		public Material material
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Material>(CustomRenderTexture.get_material_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_material_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(value));
			}
		}

		public Material initializationMaterial
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Material>(CustomRenderTexture.get_initializationMaterial_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_initializationMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(value));
			}
		}

		public Texture initializationTexture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture>(CustomRenderTexture.get_initializationTexture_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_initializationTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(value));
			}
		}

		[FreeFunction(Name = "CustomRenderTextureScripting::GetUpdateZonesInternal", HasExplicitThis = true)]
		internal void GetUpdateZonesInternal([NotNull] object updateZones)
		{
			if (updateZones == null)
			{
				ThrowHelper.ThrowArgumentNullException(updateZones, "updateZones");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CustomRenderTexture.GetUpdateZonesInternal_Injected(intPtr, updateZones);
		}

		public void GetUpdateZones(List<CustomRenderTextureUpdateZone> updateZones)
		{
			this.GetUpdateZonesInternal(updateZones);
		}

		[FreeFunction(Name = "CustomRenderTextureScripting::SetUpdateZonesInternal", HasExplicitThis = true)]
		private unsafe void SetUpdateZonesInternal(CustomRenderTextureUpdateZone[] updateZones)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<CustomRenderTextureUpdateZone> span = new Span<CustomRenderTextureUpdateZone>(updateZones);
			fixed (CustomRenderTextureUpdateZone* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CustomRenderTexture.SetUpdateZonesInternal_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "CustomRenderTextureScripting::GetDoubleBufferRenderTexture", HasExplicitThis = true)]
		public RenderTexture GetDoubleBufferRenderTexture()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(CustomRenderTexture.GetDoubleBufferRenderTexture_Injected(intPtr));
		}

		public void EnsureDoubleBufferConsistency()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CustomRenderTexture.EnsureDoubleBufferConsistency_Injected(intPtr);
		}

		public void SetUpdateZones(CustomRenderTextureUpdateZone[] updateZones)
		{
			bool flag = updateZones == null;
			if (flag)
			{
				throw new ArgumentNullException("updateZones");
			}
			this.SetUpdateZonesInternal(updateZones);
		}

		public CustomRenderTextureInitializationSource initializationSource
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CustomRenderTexture.get_initializationSource_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_initializationSource_Injected(intPtr, value);
			}
		}

		public Color initializationColor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				CustomRenderTexture.get_initializationColor_Injected(intPtr, out color);
				return color;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_initializationColor_Injected(intPtr, ref value);
			}
		}

		public CustomRenderTextureUpdateMode updateMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CustomRenderTexture.get_updateMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_updateMode_Injected(intPtr, value);
			}
		}

		public CustomRenderTextureUpdateMode initializationMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CustomRenderTexture.get_initializationMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_initializationMode_Injected(intPtr, value);
			}
		}

		public CustomRenderTextureUpdateZoneSpace updateZoneSpace
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CustomRenderTexture.get_updateZoneSpace_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_updateZoneSpace_Injected(intPtr, value);
			}
		}

		public int shaderPass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CustomRenderTexture.get_shaderPass_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_shaderPass_Injected(intPtr, value);
			}
		}

		public uint cubemapFaceMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CustomRenderTexture.get_cubemapFaceMask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_cubemapFaceMask_Injected(intPtr, value);
			}
		}

		public bool doubleBuffered
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CustomRenderTexture.get_doubleBuffered_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_doubleBuffered_Injected(intPtr, value);
			}
		}

		public bool wrapUpdateZones
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CustomRenderTexture.get_wrapUpdateZones_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_wrapUpdateZones_Injected(intPtr, value);
			}
		}

		public float updatePeriod
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CustomRenderTexture.get_updatePeriod_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CustomRenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CustomRenderTexture.set_updatePeriod_Injected(intPtr, value);
			}
		}

		public CustomRenderTexture(int width, int height, RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite)
			: this(width, height, RenderTexture.GetCompatibleFormat(format, readWrite))
		{
			bool flag = this != null;
			if (flag)
			{
				base.SetShadowSamplingMode(RenderTexture.GetShadowSamplingModeForFormat(format));
			}
		}

		[ExcludeFromDocs]
		public CustomRenderTexture(int width, int height, RenderTextureFormat format)
			: this(width, height, format, RenderTextureReadWrite.Default)
		{
		}

		[ExcludeFromDocs]
		public CustomRenderTexture(int width, int height)
			: this(width, height, SystemInfo.GetGraphicsFormat(DefaultFormat.LDR))
		{
		}

		[ExcludeFromDocs]
		public CustomRenderTexture(int width, int height, [DefaultValue("DefaultFormat.LDR")] DefaultFormat defaultFormat)
			: this(width, height, RenderTexture.GetDefaultColorFormat(defaultFormat))
		{
			bool flag = defaultFormat == DefaultFormat.DepthStencil || defaultFormat == DefaultFormat.Shadow;
			if (flag)
			{
				base.depthStencilFormat = SystemInfo.GetGraphicsFormat(defaultFormat);
				base.SetShadowSamplingMode(RenderTexture.GetShadowSamplingModeForFormat(defaultFormat));
			}
		}

		[ExcludeFromDocs]
		public CustomRenderTexture(int width, int height, GraphicsFormat format)
		{
			bool flag = format != GraphicsFormat.None && !base.ValidateFormat(format, GraphicsFormatUsage.Render);
			if (!flag)
			{
				CustomRenderTexture.Internal_CreateCustomRenderTexture(this);
				this.width = width;
				this.height = height;
				base.graphicsFormat = format;
				base.SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(format));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TriggerUpdate_Injected(IntPtr _unity_self, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TriggerInitialization_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearUpdateZones_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_material_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_material_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_initializationMaterial_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_initializationMaterial_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_initializationTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_initializationTexture_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetUpdateZonesInternal_Injected(IntPtr _unity_self, object updateZones);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetUpdateZonesInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper updateZones);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDoubleBufferRenderTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnsureDoubleBufferConsistency_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CustomRenderTextureInitializationSource get_initializationSource_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_initializationSource_Injected(IntPtr _unity_self, CustomRenderTextureInitializationSource value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_initializationColor_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_initializationColor_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CustomRenderTextureUpdateMode get_updateMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updateMode_Injected(IntPtr _unity_self, CustomRenderTextureUpdateMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CustomRenderTextureUpdateMode get_initializationMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_initializationMode_Injected(IntPtr _unity_self, CustomRenderTextureUpdateMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CustomRenderTextureUpdateZoneSpace get_updateZoneSpace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updateZoneSpace_Injected(IntPtr _unity_self, CustomRenderTextureUpdateZoneSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_shaderPass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shaderPass_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_cubemapFaceMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cubemapFaceMask_Injected(IntPtr _unity_self, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_doubleBuffered_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_doubleBuffered_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_wrapUpdateZones_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wrapUpdateZones_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_updatePeriod_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updatePeriod_Injected(IntPtr _unity_self, float value);
	}
}
