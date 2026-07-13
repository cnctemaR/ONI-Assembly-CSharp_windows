using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Renderer.h")]
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	[UsedByNativeCode]
	public class Renderer : Component
	{
		[Obsolete("Use shadowCastingMode instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool castShadows
		{
			get
			{
				return this.shadowCastingMode > ShadowCastingMode.Off;
			}
			set
			{
				this.shadowCastingMode = (value ? ShadowCastingMode.On : ShadowCastingMode.Off);
			}
		}

		[Obsolete("Use motionVectorGenerationMode instead.", false)]
		public bool motionVectors
		{
			get
			{
				return this.motionVectorGenerationMode == MotionVectorGenerationMode.Object;
			}
			set
			{
				this.motionVectorGenerationMode = (value ? MotionVectorGenerationMode.Object : MotionVectorGenerationMode.Camera);
			}
		}

		[Obsolete("Use lightProbeUsage instead.", false)]
		public bool useLightProbes
		{
			get
			{
				return this.lightProbeUsage > LightProbeUsage.Off;
			}
			set
			{
				this.lightProbeUsage = (value ? LightProbeUsage.BlendProbes : LightProbeUsage.Off);
			}
		}

		public Bounds bounds
		{
			[FreeFunction(Name = "RendererScripting::GetWorldBounds", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				Renderer.get_bounds_Injected(intPtr, out bounds);
				return bounds;
			}
			[NativeName("SetWorldAABB")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_bounds_Injected(intPtr, ref value);
			}
		}

		public Bounds localBounds
		{
			[FreeFunction(Name = "RendererScripting::GetLocalBounds", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				Renderer.get_localBounds_Injected(intPtr, out bounds);
				return bounds;
			}
			[NativeName("SetLocalAABB")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_localBounds_Injected(intPtr, ref value);
			}
		}

		[NativeName("ResetWorldAABB")]
		public void ResetBounds()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.ResetBounds_Injected(intPtr);
		}

		[NativeName("ResetLocalAABB")]
		public void ResetLocalBounds()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.ResetLocalBounds_Injected(intPtr);
		}

		[NativeName("HasCustomWorldAABB")]
		internal bool Internal_HasCustomBounds()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Renderer.Internal_HasCustomBounds_Injected(intPtr);
		}

		[NativeName("HasCustomLocalAABB")]
		internal bool Internal_HasCustomLocalBounds()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Renderer.Internal_HasCustomLocalBounds_Injected(intPtr);
		}

		[FreeFunction(Name = "RendererScripting::SetStaticLightmapST", HasExplicitThis = true)]
		private void SetStaticLightmapST(Vector4 st)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.SetStaticLightmapST_Injected(intPtr, ref st);
		}

		[FreeFunction(Name = "RendererScripting::GetMaterial", HasExplicitThis = true)]
		private Material GetMaterial()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(Renderer.GetMaterial_Injected(intPtr));
		}

		[FreeFunction(Name = "RendererScripting::GetSharedMaterial", HasExplicitThis = true)]
		private Material GetSharedMaterial()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(Renderer.GetSharedMaterial_Injected(intPtr));
		}

		[FreeFunction(Name = "RendererScripting::SetMaterial", HasExplicitThis = true)]
		private void SetMaterial(Material m)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.SetMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(m));
		}

		[FreeFunction(Name = "RendererScripting::GetMaterialArray", HasExplicitThis = true)]
		private Material[] GetMaterialArray()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Renderer.GetMaterialArray_Injected(intPtr);
		}

		[FreeFunction(Name = "RendererScripting::GetMaterialArray", HasExplicitThis = true)]
		private void CopyMaterialArray([Out] Material[] m)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.CopyMaterialArray_Injected(intPtr, m);
		}

		[FreeFunction(Name = "RendererScripting::GetSharedMaterialArray", HasExplicitThis = true)]
		private void CopySharedMaterialArray([Out] Material[] m)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.CopySharedMaterialArray_Injected(intPtr, m);
		}

		[FreeFunction(Name = "RendererScripting::SetMaterialArray", HasExplicitThis = true)]
		private void SetMaterialArray([NotNull] Material[] m, int length)
		{
			if (m == null)
			{
				ThrowHelper.ThrowArgumentNullException(m, "m");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.SetMaterialArray_Injected(intPtr, m, length);
		}

		private void SetMaterialArray(Material[] m)
		{
			this.SetMaterialArray(m, (m != null) ? m.Length : 0);
		}

		[FreeFunction(Name = "RendererScripting::SetPropertyBlock", HasExplicitThis = true)]
		internal void Internal_SetPropertyBlock(MaterialPropertyBlock properties)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.Internal_SetPropertyBlock_Injected(intPtr, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[FreeFunction(Name = "RendererScripting::GetPropertyBlock", HasExplicitThis = true)]
		internal void Internal_GetPropertyBlock([NotNull] MaterialPropertyBlock dest)
		{
			if (dest == null)
			{
				ThrowHelper.ThrowArgumentNullException(dest, "dest");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(dest);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(dest, "dest");
			}
			Renderer.Internal_GetPropertyBlock_Injected(intPtr, intPtr2);
		}

		[FreeFunction(Name = "RendererScripting::SetPropertyBlockMaterialIndex", HasExplicitThis = true)]
		internal void Internal_SetPropertyBlockMaterialIndex(MaterialPropertyBlock properties, int materialIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.Internal_SetPropertyBlockMaterialIndex_Injected(intPtr, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), materialIndex);
		}

		[FreeFunction(Name = "RendererScripting::GetPropertyBlockMaterialIndex", HasExplicitThis = true)]
		internal void Internal_GetPropertyBlockMaterialIndex([NotNull] MaterialPropertyBlock dest, int materialIndex)
		{
			if (dest == null)
			{
				ThrowHelper.ThrowArgumentNullException(dest, "dest");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(dest);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(dest, "dest");
			}
			Renderer.Internal_GetPropertyBlockMaterialIndex_Injected(intPtr, intPtr2, materialIndex);
		}

		[FreeFunction(Name = "RendererScripting::HasPropertyBlock", HasExplicitThis = true)]
		public bool HasPropertyBlock()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Renderer.HasPropertyBlock_Injected(intPtr);
		}

		public void SetPropertyBlock(MaterialPropertyBlock properties)
		{
			this.Internal_SetPropertyBlock(properties);
		}

		public void SetPropertyBlock(MaterialPropertyBlock properties, int materialIndex)
		{
			this.Internal_SetPropertyBlockMaterialIndex(properties, materialIndex);
		}

		public void GetPropertyBlock(MaterialPropertyBlock properties)
		{
			this.Internal_GetPropertyBlock(properties);
		}

		public void GetPropertyBlock(MaterialPropertyBlock properties, int materialIndex)
		{
			this.Internal_GetPropertyBlockMaterialIndex(properties, materialIndex);
		}

		[FreeFunction(Name = "RendererScripting::GetClosestReflectionProbes", HasExplicitThis = true)]
		private void GetClosestReflectionProbesInternal(object result)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.GetClosestReflectionProbesInternal_Injected(intPtr, result);
		}

		[NativeName("Renderer::GetMaskInteraction")]
		internal SpriteMaskInteraction Internal_GetSpriteMaskInteraction()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Renderer.Internal_GetSpriteMaskInteraction_Injected(intPtr);
		}

		[NativeName("Renderer::SetMaskInteraction")]
		internal void Internal_SetSpriteMaskInteraction(SpriteMaskInteraction maskInteraction)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.Internal_SetSpriteMaskInteraction_Injected(intPtr, maskInteraction);
		}

		public bool enabled
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_enabled_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_enabled_Injected(intPtr, value);
			}
		}

		public bool isVisible
		{
			[NativeName("IsVisibleInScene")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_isVisible_Injected(intPtr);
			}
		}

		public ShadowCastingMode shadowCastingMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_shadowCastingMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_shadowCastingMode_Injected(intPtr, value);
			}
		}

		public bool receiveShadows
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_receiveShadows_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_receiveShadows_Injected(intPtr, value);
			}
		}

		public bool forceRenderingOff
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_forceRenderingOff_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_forceRenderingOff_Injected(intPtr, value);
			}
		}

		internal bool allowGPUDrivenRendering
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_allowGPUDrivenRendering_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_allowGPUDrivenRendering_Injected(intPtr, value);
			}
		}

		internal bool smallMeshCulling
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_smallMeshCulling_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_smallMeshCulling_Injected(intPtr, value);
			}
		}

		[NativeName("GetIsStaticShadowCaster")]
		private bool GetIsStaticShadowCaster()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Renderer.GetIsStaticShadowCaster_Injected(intPtr);
		}

		[NativeName("SetIsStaticShadowCaster")]
		private void SetIsStaticShadowCaster(bool value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.SetIsStaticShadowCaster_Injected(intPtr, value);
		}

		public bool staticShadowCaster
		{
			get
			{
				return this.GetIsStaticShadowCaster();
			}
			set
			{
				this.SetIsStaticShadowCaster(value);
			}
		}

		public MotionVectorGenerationMode motionVectorGenerationMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_motionVectorGenerationMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_motionVectorGenerationMode_Injected(intPtr, value);
			}
		}

		public LightProbeUsage lightProbeUsage
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_lightProbeUsage_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_lightProbeUsage_Injected(intPtr, value);
			}
		}

		public ReflectionProbeUsage reflectionProbeUsage
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_reflectionProbeUsage_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_reflectionProbeUsage_Injected(intPtr, value);
			}
		}

		public uint renderingLayerMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_renderingLayerMask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_renderingLayerMask_Injected(intPtr, value);
			}
		}

		public int rendererPriority
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_rendererPriority_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_rendererPriority_Injected(intPtr, value);
			}
		}

		public RayTracingMode rayTracingMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_rayTracingMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_rayTracingMode_Injected(intPtr, value);
			}
		}

		public RayTracingAccelerationStructureBuildFlags rayTracingAccelerationStructureBuildFlags
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_rayTracingAccelerationStructureBuildFlags_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_rayTracingAccelerationStructureBuildFlags_Injected(intPtr, value);
			}
		}

		public bool rayTracingAccelerationStructureBuildFlagsOverride
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_rayTracingAccelerationStructureBuildFlagsOverride_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_rayTracingAccelerationStructureBuildFlagsOverride_Injected(intPtr, value);
			}
		}

		public unsafe string sortingLayerName
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					Renderer.get_sortingLayerName_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			set
			{
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					Renderer.set_sortingLayerName_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public int sortingLayerID
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_sortingLayerID_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_sortingLayerID_Injected(intPtr, value);
			}
		}

		public int sortingOrder
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_sortingOrder_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_sortingOrder_Injected(intPtr, value);
			}
		}

		internal uint sortingKey
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_sortingKey_Injected(intPtr);
			}
		}

		internal int sortingGroupID
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_sortingGroupID_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_sortingGroupID_Injected(intPtr, value);
			}
		}

		internal int sortingGroupOrder
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_sortingGroupOrder_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_sortingGroupOrder_Injected(intPtr, value);
			}
		}

		internal uint sortingGroupKey
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_sortingGroupKey_Injected(intPtr);
			}
		}

		public bool isLOD0
		{
			[NativeName("IsLOD0")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_isLOD0_Injected(intPtr);
			}
		}

		[NativeProperty("IsDynamicOccludee")]
		public bool allowOcclusionWhenDynamic
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_allowOcclusionWhenDynamic_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_allowOcclusionWhenDynamic_Injected(intPtr, value);
			}
		}

		[NativeProperty("ForceMeshLod")]
		public short forceMeshLod
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_forceMeshLod_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_forceMeshLod_Injected(intPtr, value);
			}
		}

		[NativeProperty("MeshLodSelectionBias")]
		public float meshLodSelectionBias
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_meshLodSelectionBias_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_meshLodSelectionBias_Injected(intPtr, value);
			}
		}

		[NativeProperty("StaticBatchRoot")]
		internal Transform staticBatchRootTransform
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Transform>(Renderer.get_staticBatchRootTransform_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_staticBatchRootTransform_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(value));
			}
		}

		internal int staticBatchIndex
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_staticBatchIndex_Injected(intPtr);
			}
		}

		internal void SetStaticBatchInfo(int firstSubMesh, int subMeshCount)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.SetStaticBatchInfo_Injected(intPtr, firstSubMesh, subMeshCount);
		}

		public bool isPartOfStaticBatch
		{
			[NativeName("IsPartOfStaticBatch")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Renderer.get_isPartOfStaticBatch_Injected(intPtr);
			}
		}

		public Matrix4x4 worldToLocalMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Renderer.get_worldToLocalMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
		}

		public Matrix4x4 localToWorldMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Renderer.get_localToWorldMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
		}

		public GameObject lightProbeProxyVolumeOverride
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<GameObject>(Renderer.get_lightProbeProxyVolumeOverride_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_lightProbeProxyVolumeOverride_Injected(intPtr, Object.MarshalledUnityObject.Marshal<GameObject>(value));
			}
		}

		public Transform probeAnchor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Transform>(Renderer.get_probeAnchor_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Renderer.set_probeAnchor_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(value));
			}
		}

		[NativeName("GetLightmapIndexInt")]
		private int GetLightmapIndex(LightmapType lt)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Renderer.GetLightmapIndex_Injected(intPtr, lt);
		}

		[NativeName("SetLightmapIndexInt")]
		private void SetLightmapIndex(int index, LightmapType lt)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.SetLightmapIndex_Injected(intPtr, index, lt);
		}

		[NativeName("GetLightmapST")]
		private Vector4 GetLightmapST(LightmapType lt)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector4 vector;
			Renderer.GetLightmapST_Injected(intPtr, lt, out vector);
			return vector;
		}

		[NativeName("SetLightmapST")]
		private void SetLightmapST(Vector4 st, LightmapType lt)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Renderer.SetLightmapST_Injected(intPtr, ref st, lt);
		}

		public int lightmapIndex
		{
			get
			{
				return this.GetLightmapIndex(LightmapType.StaticLightmap);
			}
			set
			{
				this.SetLightmapIndex(value, LightmapType.StaticLightmap);
			}
		}

		public int realtimeLightmapIndex
		{
			get
			{
				return this.GetLightmapIndex(LightmapType.DynamicLightmap);
			}
			set
			{
				this.SetLightmapIndex(value, LightmapType.DynamicLightmap);
			}
		}

		public Vector4 lightmapScaleOffset
		{
			get
			{
				return this.GetLightmapST(LightmapType.StaticLightmap);
			}
			set
			{
				this.SetStaticLightmapST(value);
			}
		}

		public Vector4 realtimeLightmapScaleOffset
		{
			get
			{
				return this.GetLightmapST(LightmapType.DynamicLightmap);
			}
			set
			{
				this.SetLightmapST(value, LightmapType.DynamicLightmap);
			}
		}

		private int GetMaterialCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Renderer.GetMaterialCount_Injected(intPtr);
		}

		[NativeName("GetMaterialArray")]
		private Material[] GetSharedMaterialArray()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Renderer.GetSharedMaterialArray_Injected(intPtr);
		}

		public Material[] materials
		{
			get
			{
				return this.GetMaterialArray();
			}
			set
			{
				this.SetMaterialArray(value);
			}
		}

		public Material material
		{
			get
			{
				return this.GetMaterial();
			}
			set
			{
				this.SetMaterial(value);
			}
		}

		public Material sharedMaterial
		{
			get
			{
				return this.GetSharedMaterial();
			}
			set
			{
				this.SetMaterial(value);
			}
		}

		public Material[] sharedMaterials
		{
			get
			{
				return this.GetSharedMaterialArray();
			}
			set
			{
				this.SetMaterialArray(value);
			}
		}

		public void GetMaterials(List<Material> m)
		{
			bool flag = m == null;
			if (flag)
			{
				throw new ArgumentNullException("The result material list cannot be null.", "m");
			}
			NoAllocHelpers.EnsureListElemCount<Material>(m, this.GetMaterialCount());
			this.CopyMaterialArray(NoAllocHelpers.ExtractArrayFromList<Material>(m));
		}

		public void SetSharedMaterials(List<Material> materials)
		{
			bool flag = materials == null;
			if (flag)
			{
				throw new ArgumentNullException("The material list to set cannot be null.", "materials");
			}
			this.SetMaterialArray(NoAllocHelpers.ExtractArrayFromList<Material>(materials), materials.Count);
		}

		public void SetMaterials(List<Material> materials)
		{
			bool flag = materials == null;
			if (flag)
			{
				throw new ArgumentNullException("The material list to set cannot be null.", "materials");
			}
			this.SetMaterialArray(NoAllocHelpers.ExtractArrayFromList<Material>(materials), materials.Count);
		}

		public void GetSharedMaterials(List<Material> m)
		{
			bool flag = m == null;
			if (flag)
			{
				throw new ArgumentNullException("The result material list cannot be null.", "m");
			}
			NoAllocHelpers.EnsureListElemCount<Material>(m, this.GetMaterialCount());
			this.CopySharedMaterialArray(NoAllocHelpers.ExtractArrayFromList<Material>(m));
		}

		public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
		{
			this.GetClosestReflectionProbesInternal(result);
		}

		public LODGroup LODGroup
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<LODGroup>(Renderer.get_LODGroup_Injected(intPtr));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bounds_Injected(IntPtr _unity_self, [In] ref Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localBounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_localBounds_Injected(IntPtr _unity_self, [In] ref Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetBounds_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetLocalBounds_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_HasCustomBounds_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_HasCustomLocalBounds_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStaticLightmapST_Injected(IntPtr _unity_self, [In] ref Vector4 st);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetMaterial_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSharedMaterial_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMaterial_Injected(IntPtr _unity_self, IntPtr m);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Material[] GetMaterialArray_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyMaterialArray_Injected(IntPtr _unity_self, [Out] Material[] m);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopySharedMaterialArray_Injected(IntPtr _unity_self, [Out] Material[] m);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMaterialArray_Injected(IntPtr _unity_self, Material[] m, int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetPropertyBlock_Injected(IntPtr _unity_self, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetPropertyBlock_Injected(IntPtr _unity_self, IntPtr dest);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetPropertyBlockMaterialIndex_Injected(IntPtr _unity_self, IntPtr properties, int materialIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetPropertyBlockMaterialIndex_Injected(IntPtr _unity_self, IntPtr dest, int materialIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasPropertyBlock_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetClosestReflectionProbesInternal_Injected(IntPtr _unity_self, object result);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteMaskInteraction Internal_GetSpriteMaskInteraction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetSpriteMaskInteraction_Injected(IntPtr _unity_self, SpriteMaskInteraction maskInteraction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enabled_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isVisible_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ShadowCastingMode get_shadowCastingMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowCastingMode_Injected(IntPtr _unity_self, ShadowCastingMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_receiveShadows_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_receiveShadows_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_forceRenderingOff_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_forceRenderingOff_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_allowGPUDrivenRendering_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_allowGPUDrivenRendering_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_smallMeshCulling_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_smallMeshCulling_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsStaticShadowCaster_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIsStaticShadowCaster_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern MotionVectorGenerationMode get_motionVectorGenerationMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_motionVectorGenerationMode_Injected(IntPtr _unity_self, MotionVectorGenerationMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightProbeUsage get_lightProbeUsage_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lightProbeUsage_Injected(IntPtr _unity_self, LightProbeUsage value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ReflectionProbeUsage get_reflectionProbeUsage_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_reflectionProbeUsage_Injected(IntPtr _unity_self, ReflectionProbeUsage value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_renderingLayerMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderingLayerMask_Injected(IntPtr _unity_self, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_rendererPriority_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rendererPriority_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RayTracingMode get_rayTracingMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rayTracingMode_Injected(IntPtr _unity_self, RayTracingMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RayTracingAccelerationStructureBuildFlags get_rayTracingAccelerationStructureBuildFlags_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rayTracingAccelerationStructureBuildFlags_Injected(IntPtr _unity_self, RayTracingAccelerationStructureBuildFlags value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_rayTracingAccelerationStructureBuildFlagsOverride_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rayTracingAccelerationStructureBuildFlagsOverride_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sortingLayerName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingLayerName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingLayerID_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingLayerID_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingOrder_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingOrder_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_sortingKey_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingGroupID_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingGroupID_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingGroupOrder_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingGroupOrder_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_sortingGroupKey_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isLOD0_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_allowOcclusionWhenDynamic_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_allowOcclusionWhenDynamic_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern short get_forceMeshLod_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_forceMeshLod_Injected(IntPtr _unity_self, short value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_meshLodSelectionBias_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_meshLodSelectionBias_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_staticBatchRootTransform_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_staticBatchRootTransform_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_staticBatchIndex_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStaticBatchInfo_Injected(IntPtr _unity_self, int firstSubMesh, int subMeshCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPartOfStaticBatch_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_worldToLocalMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_lightProbeProxyVolumeOverride_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lightProbeProxyVolumeOverride_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_probeAnchor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_probeAnchor_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLightmapIndex_Injected(IntPtr _unity_self, LightmapType lt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLightmapIndex_Injected(IntPtr _unity_self, int index, LightmapType lt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLightmapST_Injected(IntPtr _unity_self, LightmapType lt, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLightmapST_Injected(IntPtr _unity_self, [In] ref Vector4 st, LightmapType lt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMaterialCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Material[] GetSharedMaterialArray_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_LODGroup_Injected(IntPtr _unity_self);
	}
}
