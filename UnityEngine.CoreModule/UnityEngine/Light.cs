using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Runtime/Camera/Light.h")]
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Runtime/Export/Graphics/Light.bindings.h")]
	public sealed class Light : Behaviour
	{
		[NativeProperty("LightType")]
		public LightType type
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_type_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_type_Injected(intPtr, value);
			}
		}

		[Obsolete("This property has been deprecated. Use Light.type instead.")]
		public LightShape shape { get; set; }

		public float spotAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_spotAngle_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_spotAngle_Injected(intPtr, value);
			}
		}

		public float innerSpotAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_innerSpotAngle_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_innerSpotAngle_Injected(intPtr, value);
			}
		}

		public Color color
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				Light.get_color_Injected(intPtr, out color);
				return color;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_color_Injected(intPtr, ref value);
			}
		}

		public float colorTemperature
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_colorTemperature_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_colorTemperature_Injected(intPtr, value);
			}
		}

		public bool useColorTemperature
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_useColorTemperature_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_useColorTemperature_Injected(intPtr, value);
			}
		}

		public float intensity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_intensity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_intensity_Injected(intPtr, value);
			}
		}

		public float bounceIntensity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_bounceIntensity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_bounceIntensity_Injected(intPtr, value);
			}
		}

		public LightUnit lightUnit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_lightUnit_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_lightUnit_Injected(intPtr, value);
			}
		}

		public float luxAtDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_luxAtDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_luxAtDistance_Injected(intPtr, value);
			}
		}

		public bool enableSpotReflector
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_enableSpotReflector_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_enableSpotReflector_Injected(intPtr, value);
			}
		}

		public bool useBoundingSphereOverride
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_useBoundingSphereOverride_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_useBoundingSphereOverride_Injected(intPtr, value);
			}
		}

		public Vector4 boundingSphereOverride
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector4 vector;
				Light.get_boundingSphereOverride_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_boundingSphereOverride_Injected(intPtr, ref value);
			}
		}

		public bool useViewFrustumForShadowCasterCull
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_useViewFrustumForShadowCasterCull_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_useViewFrustumForShadowCasterCull_Injected(intPtr, value);
			}
		}

		public bool forceVisible
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_forceVisible_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_forceVisible_Injected(intPtr, value);
			}
		}

		public int shadowCustomResolution
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_shadowCustomResolution_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_shadowCustomResolution_Injected(intPtr, value);
			}
		}

		public float shadowBias
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_shadowBias_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_shadowBias_Injected(intPtr, value);
			}
		}

		public float shadowNormalBias
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_shadowNormalBias_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_shadowNormalBias_Injected(intPtr, value);
			}
		}

		public float shadowNearPlane
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_shadowNearPlane_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_shadowNearPlane_Injected(intPtr, value);
			}
		}

		public bool useShadowMatrixOverride
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_useShadowMatrixOverride_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_useShadowMatrixOverride_Injected(intPtr, value);
			}
		}

		public Matrix4x4 shadowMatrixOverride
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Light.get_shadowMatrixOverride_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_shadowMatrixOverride_Injected(intPtr, ref value);
			}
		}

		public float range
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_range_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_range_Injected(intPtr, value);
			}
		}

		public float dilatedRange
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_dilatedRange_Injected(intPtr);
			}
		}

		public Flare flare
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Flare>(Light.get_flare_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_flare_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Flare>(value));
			}
		}

		public LightBakingOutput bakingOutput
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightBakingOutput lightBakingOutput;
				Light.get_bakingOutput_Injected(intPtr, out lightBakingOutput);
				return lightBakingOutput;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_bakingOutput_Injected(intPtr, ref value);
			}
		}

		public int cullingMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_cullingMask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_cullingMask_Injected(intPtr, value);
			}
		}

		public int renderingLayerMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_renderingLayerMask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_renderingLayerMask_Injected(intPtr, value);
			}
		}

		public LightShadowCasterMode lightShadowCasterMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_lightShadowCasterMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_lightShadowCasterMode_Injected(intPtr, value);
			}
		}

		public void Reset()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Light.Reset_Injected(intPtr);
		}

		public LightShadows shadows
		{
			[NativeMethod("GetShadowType")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_shadows_Injected(intPtr);
			}
			[FreeFunction("Light_Bindings::SetShadowType", HasExplicitThis = true, ThrowsException = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_shadows_Injected(intPtr, value);
			}
		}

		public float shadowStrength
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_shadowStrength_Injected(intPtr);
			}
			[FreeFunction("Light_Bindings::SetShadowStrength", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_shadowStrength_Injected(intPtr, value);
			}
		}

		public LightShadowResolution shadowResolution
		{
			get
			{
				return this.ShadowResolution;
			}
			set
			{
				bool flag = RenderPipelineManager.currentPipeline != null;
				if (flag)
				{
					Light.LogWarningOnlyBuiltIn("shadowResolution");
				}
				this.ShadowResolution = value;
			}
		}

		private static void LogWarningOnlyBuiltIn([CallerMemberName] string propertyName = "")
		{
			Debug.LogWarning("Light." + propertyName + " is compatible only with the Built-In Render Pipeline.");
		}

		private LightShadowResolution ShadowResolution
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_ShadowResolution_Injected(intPtr);
			}
			[FreeFunction("Light_Bindings::SetShadowResolution", HasExplicitThis = true, ThrowsException = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_ShadowResolution_Injected(intPtr, value);
			}
		}

		[Obsolete("Shadow softness is removed in Unity 5.0+", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public float shadowSoftness
		{
			get
			{
				return 4f;
			}
			set
			{
			}
		}

		[Obsolete("Shadow softness is removed in Unity 5.0+", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public float shadowSoftnessFade
		{
			get
			{
				return 1f;
			}
			set
			{
			}
		}

		public unsafe float[] layerShadowCullDistances
		{
			[FreeFunction("Light_Bindings::GetLayerShadowCullDistances", HasExplicitThis = true, ThrowsException = false)]
			get
			{
				float[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					Light.get_layerShadowCullDistances_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					float[] array;
					blittableArrayWrapper.Unmarshal<float>(ref array);
					array2 = array;
				}
				return array2;
			}
			[FreeFunction("Light_Bindings::SetLayerShadowCullDistances", HasExplicitThis = true, ThrowsException = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<float> span = new Span<float>(value);
				fixed (float* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Light.set_layerShadowCullDistances_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		[Obsolete("Light.cookieSize has been deprecated. Use Light.cookieSize2D instead.", false)]
		public float cookieSize
		{
			get
			{
				return this.cookieSize2D.x;
			}
			set
			{
				this.cookieSize2D = new Vector2(value, value);
			}
		}

		public Vector2 cookieSize2D
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Light.get_cookieSize2D_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_cookieSize2D_Injected(intPtr, ref value);
			}
		}

		public Texture cookie
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture>(Light.get_cookie_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_cookie_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(value));
			}
		}

		public LightRenderMode renderMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_renderMode_Injected(intPtr);
			}
			[FreeFunction("Light_Bindings::SetRenderMode", HasExplicitThis = true, ThrowsException = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_renderMode_Injected(intPtr, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("warning bakedIndex has been removed please use bakingOutput.isBaked instead.", true)]
		public int bakedIndex
		{
			get
			{
				return this.m_BakedIndex;
			}
			set
			{
				this.m_BakedIndex = value;
			}
		}

		public Vector2 areaSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Light.get_areaSize_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Light.set_areaSize_Injected(intPtr, ref value);
			}
		}

		public void AddCommandBuffer(LightEvent evt, CommandBuffer buffer)
		{
			this.AddCommandBuffer(evt, buffer, ShadowMapPass.All);
		}

		public void AddCommandBuffer(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask)
		{
			bool flag = RenderPipelineManager.currentPipeline != null;
			if (flag)
			{
				Light.LogWarningOnlyBuiltIn("AddCommandBuffer");
			}
			this.AddCommandBufferInternal(evt, buffer, shadowPassMask);
		}

		[FreeFunction("Light_Bindings::AddCommandBuffer", HasExplicitThis = true)]
		internal void AddCommandBufferInternal(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Light.AddCommandBufferInternal_Injected(intPtr, evt, (buffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(buffer), shadowPassMask);
		}

		public void AddCommandBufferAsync(LightEvent evt, CommandBuffer buffer, ComputeQueueType queueType)
		{
			this.AddCommandBufferAsync(evt, buffer, ShadowMapPass.All, queueType);
		}

		public void AddCommandBufferAsync(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask, ComputeQueueType queueType)
		{
			bool flag = RenderPipelineManager.currentPipeline != null;
			if (flag)
			{
				Light.LogWarningOnlyBuiltIn("AddCommandBufferAsync");
			}
			this.AddCommandBufferAsyncInternal(evt, buffer, shadowPassMask, queueType);
		}

		[FreeFunction("Light_Bindings::AddCommandBufferAsync", HasExplicitThis = true)]
		internal void AddCommandBufferAsyncInternal(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask, ComputeQueueType queueType)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Light.AddCommandBufferAsyncInternal_Injected(intPtr, evt, (buffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(buffer), shadowPassMask, queueType);
		}

		public void RemoveCommandBuffer(LightEvent evt, CommandBuffer buffer)
		{
			bool flag = RenderPipelineManager.currentPipeline != null;
			if (flag)
			{
				Light.LogWarningOnlyBuiltIn("RemoveCommandBuffer");
			}
			this.RemoveCommandBufferInternal(evt, buffer);
		}

		[NativeMethod("RemoveCommandBuffer")]
		internal void RemoveCommandBufferInternal(LightEvent evt, CommandBuffer buffer)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Light.RemoveCommandBufferInternal_Injected(intPtr, evt, (buffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(buffer));
		}

		public void RemoveCommandBuffers(LightEvent evt)
		{
			bool flag = RenderPipelineManager.currentPipeline != null;
			if (flag)
			{
				Light.LogWarningOnlyBuiltIn("RemoveCommandBuffers");
			}
			this.RemoveCommandBuffersInternal(evt);
		}

		[NativeMethod("RemoveCommandBuffers")]
		internal void RemoveCommandBuffersInternal(LightEvent evt)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Light.RemoveCommandBuffersInternal_Injected(intPtr, evt);
		}

		public void RemoveAllCommandBuffers()
		{
			bool flag = RenderPipelineManager.currentPipeline != null;
			if (flag)
			{
				Light.LogWarningOnlyBuiltIn("RemoveAllCommandBuffers");
			}
			this.RemoveAllCommandBuffersInternal();
		}

		[NativeMethod("RemoveAllCommandBuffers")]
		internal void RemoveAllCommandBuffersInternal()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Light.RemoveAllCommandBuffersInternal_Injected(intPtr);
		}

		public CommandBuffer[] GetCommandBuffers(LightEvent evt)
		{
			bool flag = RenderPipelineManager.currentPipeline != null;
			if (flag)
			{
				Light.LogWarningOnlyBuiltIn("GetCommandBuffers");
			}
			return this.GetCommandBuffersInternal(evt);
		}

		[FreeFunction("Light_Bindings::GetCommandBuffers", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		internal CommandBuffer[] GetCommandBuffersInternal(LightEvent evt)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Light.GetCommandBuffersInternal_Injected(intPtr, evt);
		}

		public int commandBufferCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Light>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Light.get_commandBufferCount_Injected(intPtr);
			}
		}

		[Obsolete("Use QualitySettings.pixelLightCount instead.")]
		public static int pixelLightCount
		{
			get
			{
				return QualitySettings.pixelLightCount;
			}
			set
			{
				QualitySettings.pixelLightCount = value;
			}
		}

		[Obsolete("Light.GetLights has been deprecated, use FindObjectsOfType in combination with light.cullingmask/light.type", false)]
		[FreeFunction("Light_Bindings::GetLights")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Light[] GetLights(LightType type, int layer);

		[Obsolete("light.shadowConstantBias was removed, use light.shadowBias", true)]
		public float shadowConstantBias
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("light.shadowObjectSizeBias was removed, use light.shadowBias", true)]
		public float shadowObjectSizeBias
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("light.attenuate was removed; all lights always attenuate now", true)]
		public bool attenuate
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightType get_type_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_type_Injected(IntPtr _unity_self, LightType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_spotAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_spotAngle_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_innerSpotAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_innerSpotAngle_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_color_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_color_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_colorTemperature_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colorTemperature_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useColorTemperature_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useColorTemperature_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_intensity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_intensity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_bounceIntensity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bounceIntensity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightUnit get_lightUnit_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lightUnit_Injected(IntPtr _unity_self, LightUnit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_luxAtDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_luxAtDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableSpotReflector_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableSpotReflector_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useBoundingSphereOverride_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useBoundingSphereOverride_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_boundingSphereOverride_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_boundingSphereOverride_Injected(IntPtr _unity_self, [In] ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useViewFrustumForShadowCasterCull_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useViewFrustumForShadowCasterCull_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_forceVisible_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_forceVisible_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_shadowCustomResolution_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowCustomResolution_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_shadowBias_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowBias_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_shadowNormalBias_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowNormalBias_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_shadowNearPlane_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowNearPlane_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useShadowMatrixOverride_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useShadowMatrixOverride_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_shadowMatrixOverride_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowMatrixOverride_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_range_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_range_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_dilatedRange_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_flare_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_flare_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bakingOutput_Injected(IntPtr _unity_self, out LightBakingOutput ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bakingOutput_Injected(IntPtr _unity_self, [In] ref LightBakingOutput value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_cullingMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cullingMask_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_renderingLayerMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderingLayerMask_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightShadowCasterMode get_lightShadowCasterMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lightShadowCasterMode_Injected(IntPtr _unity_self, LightShadowCasterMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Reset_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightShadows get_shadows_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadows_Injected(IntPtr _unity_self, LightShadows value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_shadowStrength_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowStrength_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightShadowResolution get_ShadowResolution_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_ShadowResolution_Injected(IntPtr _unity_self, LightShadowResolution value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_layerShadowCullDistances_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_layerShadowCullDistances_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_cookieSize2D_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cookieSize2D_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_cookie_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cookie_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightRenderMode get_renderMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderMode_Injected(IntPtr _unity_self, LightRenderMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_areaSize_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_areaSize_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddCommandBufferInternal_Injected(IntPtr _unity_self, LightEvent evt, IntPtr buffer, ShadowMapPass shadowPassMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddCommandBufferAsyncInternal_Injected(IntPtr _unity_self, LightEvent evt, IntPtr buffer, ShadowMapPass shadowPassMask, ComputeQueueType queueType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveCommandBufferInternal_Injected(IntPtr _unity_self, LightEvent evt, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveCommandBuffersInternal_Injected(IntPtr _unity_self, LightEvent evt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveAllCommandBuffersInternal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CommandBuffer[] GetCommandBuffersInternal_Injected(IntPtr _unity_self, LightEvent evt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_commandBufferCount_Injected(IntPtr _unity_self);

		private int m_BakedIndex;
	}
}
