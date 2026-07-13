using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Camera/RenderManager.h")]
	[NativeHeader("Runtime/GfxDevice/GfxDeviceTypes.h")]
	[NativeHeader("Runtime/Shaders/Shader.h")]
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Runtime/Camera/Camera.h")]
	[NativeHeader("Runtime/Graphics/RenderTexture.h")]
	[NativeHeader("Runtime/Graphics/CommandBuffer/RenderingCommandBuffer.h")]
	[NativeHeader("Runtime/Misc/GameObjectUtility.h")]
	[UsedByNativeCode]
	public sealed class Camera : Behaviour
	{
		[NativeProperty("Near")]
		public float nearClipPlane
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_nearClipPlane_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_nearClipPlane_Injected(intPtr, value);
			}
		}

		[NativeProperty("Far")]
		public float farClipPlane
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_farClipPlane_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_farClipPlane_Injected(intPtr, value);
			}
		}

		[NativeProperty("VerticalFieldOfView")]
		public float fieldOfView
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_fieldOfView_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_fieldOfView_Injected(intPtr, value);
			}
		}

		public RenderingPath renderingPath
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_renderingPath_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_renderingPath_Injected(intPtr, value);
			}
		}

		public RenderingPath actualRenderingPath
		{
			[NativeName("CalculateRenderingPath")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_actualRenderingPath_Injected(intPtr);
			}
		}

		public void Reset()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.Reset_Injected(intPtr);
		}

		public bool allowHDR
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_allowHDR_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_allowHDR_Injected(intPtr, value);
			}
		}

		public bool allowMSAA
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_allowMSAA_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_allowMSAA_Injected(intPtr, value);
			}
		}

		public bool allowDynamicResolution
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_allowDynamicResolution_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_allowDynamicResolution_Injected(intPtr, value);
			}
		}

		[NativeProperty("ForceIntoRT")]
		public bool forceIntoRenderTexture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_forceIntoRenderTexture_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_forceIntoRenderTexture_Injected(intPtr, value);
			}
		}

		public float orthographicSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_orthographicSize_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_orthographicSize_Injected(intPtr, value);
			}
		}

		public bool orthographic
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_orthographic_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_orthographic_Injected(intPtr, value);
			}
		}

		public OpaqueSortMode opaqueSortMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_opaqueSortMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_opaqueSortMode_Injected(intPtr, value);
			}
		}

		public TransparencySortMode transparencySortMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_transparencySortMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_transparencySortMode_Injected(intPtr, value);
			}
		}

		public Vector3 transparencySortAxis
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Camera.get_transparencySortAxis_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_transparencySortAxis_Injected(intPtr, ref value);
			}
		}

		public void ResetTransparencySortSettings()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.ResetTransparencySortSettings_Injected(intPtr);
		}

		public float depth
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_depth_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_depth_Injected(intPtr, value);
			}
		}

		public float aspect
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_aspect_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_aspect_Injected(intPtr, value);
			}
		}

		public void ResetAspect()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.ResetAspect_Injected(intPtr);
		}

		public Vector3 velocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Camera.get_velocity_Injected(intPtr, out vector);
				return vector;
			}
		}

		public int cullingMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_cullingMask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_cullingMask_Injected(intPtr, value);
			}
		}

		public int eventMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_eventMask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_eventMask_Injected(intPtr, value);
			}
		}

		public bool layerCullSpherical
		{
			get
			{
				return this.layerCullSphericalInternal;
			}
			set
			{
				bool flag = GraphicsSettings.currentRenderPipeline != null;
				if (flag)
				{
					Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.layerCullSpherical only with the built-in renderer.");
				}
				this.layerCullSphericalInternal = value;
			}
		}

		[NativeProperty("LayerCullSpherical")]
		internal bool layerCullSphericalInternal
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_layerCullSphericalInternal_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_layerCullSphericalInternal_Injected(intPtr, value);
			}
		}

		public CameraType cameraType
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_cameraType_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_cameraType_Injected(intPtr, value);
			}
		}

		internal Material skyboxMaterial
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Material>(Camera.get_skyboxMaterial_Injected(intPtr));
			}
		}

		[NativeConditional("UNITY_EDITOR")]
		public ulong overrideSceneCullingMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_overrideSceneCullingMask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_overrideSceneCullingMask_Injected(intPtr, value);
			}
		}

		[NativeConditional("UNITY_EDITOR")]
		internal ulong sceneCullingMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_sceneCullingMask_Injected(intPtr);
			}
		}

		[NativeConditional("UNITY_EDITOR")]
		internal bool useInteractiveLightBakingData
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_useInteractiveLightBakingData_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_useInteractiveLightBakingData_Injected(intPtr, value);
			}
		}

		[FreeFunction("CameraScripting::GetLayerCullDistances", HasExplicitThis = true)]
		private float[] GetLayerCullDistances()
		{
			float[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Camera.GetLayerCullDistances_Injected(intPtr, out blittableArrayWrapper);
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

		[FreeFunction("CameraScripting::SetLayerCullDistances", HasExplicitThis = true)]
		private unsafe void SetLayerCullDistances([NotNull] float[] d)
		{
			if (d == null)
			{
				ThrowHelper.ThrowArgumentNullException(d, "d");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(d);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Camera.SetLayerCullDistances_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		public float[] layerCullDistances
		{
			get
			{
				return this.GetLayerCullDistances();
			}
			set
			{
				bool flag = value.Length != 32;
				if (flag)
				{
					throw new UnityException("Array needs to contain exactly 32 floats for layerCullDistances.");
				}
				this.SetLayerCullDistances(value);
			}
		}

		[Obsolete("PreviewCullingLayer is obsolete. Use scene culling masks instead.", false)]
		internal static int PreviewCullingLayer
		{
			get
			{
				return 31;
			}
		}

		public bool useOcclusionCulling
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_useOcclusionCulling_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_useOcclusionCulling_Injected(intPtr, value);
			}
		}

		public Matrix4x4 cullingMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Camera.get_cullingMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_cullingMatrix_Injected(intPtr, ref value);
			}
		}

		public void ResetCullingMatrix()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.ResetCullingMatrix_Injected(intPtr);
		}

		public Color backgroundColor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				Camera.get_backgroundColor_Injected(intPtr, out color);
				return color;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_backgroundColor_Injected(intPtr, ref value);
			}
		}

		public CameraClearFlags clearFlags
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_clearFlags_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_clearFlags_Injected(intPtr, value);
			}
		}

		public DepthTextureMode depthTextureMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_depthTextureMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_depthTextureMode_Injected(intPtr, value);
			}
		}

		public bool clearStencilAfterLightingPass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_clearStencilAfterLightingPass_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_clearStencilAfterLightingPass_Injected(intPtr, value);
			}
		}

		public unsafe void SetReplacementShader(Shader shader, string replacementTag)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.Marshal<Shader>(shader);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(replacementTag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = replacementTag.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Camera.SetReplacementShader_Injected(intPtr, intPtr2, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void ResetReplacementShader()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.ResetReplacementShader_Injected(intPtr);
		}

		internal Camera.ProjectionMatrixMode projectionMatrixMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_projectionMatrixMode_Injected(intPtr);
			}
		}

		public bool usePhysicalProperties
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_usePhysicalProperties_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_usePhysicalProperties_Injected(intPtr, value);
			}
		}

		public int iso
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_iso_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_iso_Injected(intPtr, value);
			}
		}

		public float shutterSpeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_shutterSpeed_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_shutterSpeed_Injected(intPtr, value);
			}
		}

		public float aperture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_aperture_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_aperture_Injected(intPtr, value);
			}
		}

		public float focusDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_focusDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_focusDistance_Injected(intPtr, value);
			}
		}

		public float focalLength
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_focalLength_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_focalLength_Injected(intPtr, value);
			}
		}

		public int bladeCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_bladeCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_bladeCount_Injected(intPtr, value);
			}
		}

		public Vector2 curvature
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Camera.get_curvature_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_curvature_Injected(intPtr, ref value);
			}
		}

		public float barrelClipping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_barrelClipping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_barrelClipping_Injected(intPtr, value);
			}
		}

		public float anamorphism
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_anamorphism_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_anamorphism_Injected(intPtr, value);
			}
		}

		public Vector2 sensorSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Camera.get_sensorSize_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_sensorSize_Injected(intPtr, ref value);
			}
		}

		public Vector2 lensShift
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Camera.get_lensShift_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_lensShift_Injected(intPtr, ref value);
			}
		}

		public Camera.GateFitMode gateFit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_gateFit_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_gateFit_Injected(intPtr, value);
			}
		}

		public float GetGateFittedFieldOfView()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Camera.GetGateFittedFieldOfView_Injected(intPtr);
		}

		public Vector2 GetGateFittedLensShift()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			Camera.GetGateFittedLensShift_Injected(intPtr, out vector);
			return vector;
		}

		internal Vector3 GetLocalSpaceAim()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Camera.GetLocalSpaceAim_Injected(intPtr, out vector);
			return vector;
		}

		[NativeProperty("NormalizedViewportRect")]
		public Rect rect
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rect rect;
				Camera.get_rect_Injected(intPtr, out rect);
				return rect;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_rect_Injected(intPtr, ref value);
			}
		}

		[NativeProperty("ScreenViewportRect")]
		public Rect pixelRect
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rect rect;
				Camera.get_pixelRect_Injected(intPtr, out rect);
				return rect;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_pixelRect_Injected(intPtr, ref value);
			}
		}

		public int pixelWidth
		{
			[FreeFunction("CameraScripting::GetPixelWidth", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_pixelWidth_Injected(intPtr);
			}
		}

		public int pixelHeight
		{
			[FreeFunction("CameraScripting::GetPixelHeight", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_pixelHeight_Injected(intPtr);
			}
		}

		public int scaledPixelWidth
		{
			[FreeFunction("CameraScripting::GetScaledPixelWidth", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_scaledPixelWidth_Injected(intPtr);
			}
		}

		public int scaledPixelHeight
		{
			[FreeFunction("CameraScripting::GetScaledPixelHeight", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_scaledPixelHeight_Injected(intPtr);
			}
		}

		public RenderTexture targetTexture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<RenderTexture>(Camera.get_targetTexture_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_targetTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(value));
			}
		}

		public RenderTexture activeTexture
		{
			[NativeName("GetCurrentTargetTexture")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<RenderTexture>(Camera.get_activeTexture_Injected(intPtr));
			}
		}

		public int targetDisplay
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_targetDisplay_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_targetDisplay_Injected(intPtr, value);
			}
		}

		[FreeFunction("CameraScripting::SetTargetBuffers", HasExplicitThis = true)]
		private void SetTargetBuffersImpl(RenderBuffer color, RenderBuffer depth)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.SetTargetBuffersImpl_Injected(intPtr, ref color, ref depth);
		}

		public void SetTargetBuffers(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
		{
			this.SetTargetBuffersImpl(colorBuffer, depthBuffer);
		}

		[FreeFunction("CameraScripting::SetTargetBuffers", HasExplicitThis = true)]
		private unsafe void SetTargetBuffersMRTImpl(RenderBuffer[] color, RenderBuffer depth)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<RenderBuffer> span = new Span<RenderBuffer>(color);
			fixed (RenderBuffer* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Camera.SetTargetBuffersMRTImpl_Injected(intPtr, ref managedSpanWrapper, ref depth);
			}
		}

		public void SetTargetBuffers(RenderBuffer[] colorBuffer, RenderBuffer depthBuffer)
		{
			this.SetTargetBuffersMRTImpl(colorBuffer, depthBuffer);
		}

		internal string[] GetCameraBufferWarnings()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Camera.GetCameraBufferWarnings_Injected(intPtr);
		}

		public Matrix4x4 cameraToWorldMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Camera.get_cameraToWorldMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
		}

		public Matrix4x4 worldToCameraMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Camera.get_worldToCameraMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_worldToCameraMatrix_Injected(intPtr, ref value);
			}
		}

		public Matrix4x4 projectionMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Camera.get_projectionMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_projectionMatrix_Injected(intPtr, ref value);
			}
		}

		public Matrix4x4 nonJitteredProjectionMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Camera.get_nonJitteredProjectionMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_nonJitteredProjectionMatrix_Injected(intPtr, ref value);
			}
		}

		[NativeProperty("UseJitteredProjectionMatrixForTransparent")]
		public bool useJitteredProjectionMatrixForTransparentRendering
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_useJitteredProjectionMatrixForTransparentRendering_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_useJitteredProjectionMatrixForTransparentRendering_Injected(intPtr, value);
			}
		}

		public Matrix4x4 previousViewProjectionMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Camera.get_previousViewProjectionMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
		}

		public void ResetWorldToCameraMatrix()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.ResetWorldToCameraMatrix_Injected(intPtr);
		}

		public void ResetProjectionMatrix()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.ResetProjectionMatrix_Injected(intPtr);
		}

		[FreeFunction("CameraScripting::CalculateObliqueMatrix", HasExplicitThis = true)]
		public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			Camera.CalculateObliqueMatrix_Injected(intPtr, ref clipPlane, out matrix4x);
			return matrix4x;
		}

		public Vector3 WorldToScreenPoint(Vector3 position, Camera.MonoOrStereoscopicEye eye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Camera.WorldToScreenPoint_Injected(intPtr, ref position, eye, out vector);
			return vector;
		}

		public Vector3 WorldToViewportPoint(Vector3 position, Camera.MonoOrStereoscopicEye eye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Camera.WorldToViewportPoint_Injected(intPtr, ref position, eye, out vector);
			return vector;
		}

		public Vector3 ViewportToWorldPoint(Vector3 position, Camera.MonoOrStereoscopicEye eye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Camera.ViewportToWorldPoint_Injected(intPtr, ref position, eye, out vector);
			return vector;
		}

		public Vector3 ScreenToWorldPoint(Vector3 position, Camera.MonoOrStereoscopicEye eye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Camera.ScreenToWorldPoint_Injected(intPtr, ref position, eye, out vector);
			return vector;
		}

		public Vector3 WorldToScreenPoint(Vector3 position)
		{
			return this.WorldToScreenPoint(position, Camera.MonoOrStereoscopicEye.Mono);
		}

		public Vector3 WorldToViewportPoint(Vector3 position)
		{
			return this.WorldToViewportPoint(position, Camera.MonoOrStereoscopicEye.Mono);
		}

		public Vector3 ViewportToWorldPoint(Vector3 position)
		{
			return this.ViewportToWorldPoint(position, Camera.MonoOrStereoscopicEye.Mono);
		}

		public Vector3 ScreenToWorldPoint(Vector3 position)
		{
			return this.ScreenToWorldPoint(position, Camera.MonoOrStereoscopicEye.Mono);
		}

		public Vector3 ScreenToViewportPoint(Vector3 position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Camera.ScreenToViewportPoint_Injected(intPtr, ref position, out vector);
			return vector;
		}

		public Vector3 ViewportToScreenPoint(Vector3 position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Camera.ViewportToScreenPoint_Injected(intPtr, ref position, out vector);
			return vector;
		}

		internal Vector2 GetFrustumPlaneSizeAt(float distance)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			Camera.GetFrustumPlaneSizeAt_Injected(intPtr, distance, out vector);
			return vector;
		}

		private Ray ViewportPointToRay(Vector2 pos, Camera.MonoOrStereoscopicEye eye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Ray ray;
			Camera.ViewportPointToRay_Injected(intPtr, ref pos, eye, out ray);
			return ray;
		}

		public Ray ViewportPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye)
		{
			return this.ViewportPointToRay(pos, eye);
		}

		public Ray ViewportPointToRay(Vector3 pos)
		{
			return this.ViewportPointToRay(pos, Camera.MonoOrStereoscopicEye.Mono);
		}

		private Ray ScreenPointToRay(Vector2 pos, Camera.MonoOrStereoscopicEye eye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Ray ray;
			Camera.ScreenPointToRay_Injected(intPtr, ref pos, eye, out ray);
			return ray;
		}

		public Ray ScreenPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye)
		{
			return this.ScreenPointToRay(pos, eye);
		}

		public Ray ScreenPointToRay(Vector3 pos)
		{
			return this.ScreenPointToRay(pos, Camera.MonoOrStereoscopicEye.Mono);
		}

		[FreeFunction("CameraScripting::CalculateViewportRayVectors", HasExplicitThis = true)]
		private unsafe void CalculateFrustumCornersInternal(Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, [Out] Vector3[] outCorners)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (outCorners != null)
				{
					fixed (Vector3[] array = outCorners)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Camera.CalculateFrustumCornersInternal_Injected(intPtr, ref viewport, z, eye, out blittableArrayWrapper);
			}
			finally
			{
				Vector3[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Vector3>(ref array);
			}
		}

		public void CalculateFrustumCorners(Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, Vector3[] outCorners)
		{
			bool flag = outCorners == null;
			if (flag)
			{
				throw new ArgumentNullException("outCorners");
			}
			bool flag2 = outCorners.Length < 4;
			if (flag2)
			{
				throw new ArgumentException("outCorners minimum size is 4", "outCorners");
			}
			this.CalculateFrustumCornersInternal(viewport, z, eye, outCorners);
		}

		[NativeName("CalculateProjectionMatrixFromPhysicalProperties")]
		private static void CalculateProjectionMatrixFromPhysicalPropertiesInternal(out Matrix4x4 output, float focalLength, Vector2 sensorSize, Vector2 lensShift, float nearClip, float farClip, float gateAspect, Camera.GateFitMode gateFitMode)
		{
			Camera.CalculateProjectionMatrixFromPhysicalPropertiesInternal_Injected(out output, focalLength, ref sensorSize, ref lensShift, nearClip, farClip, gateAspect, gateFitMode);
		}

		public static void CalculateProjectionMatrixFromPhysicalProperties(out Matrix4x4 output, float focalLength, Vector2 sensorSize, Vector2 lensShift, float nearClip, float farClip, Camera.GateFitParameters gateFitParameters = default(Camera.GateFitParameters))
		{
			Camera.CalculateProjectionMatrixFromPhysicalPropertiesInternal(out output, focalLength, sensorSize, lensShift, nearClip, farClip, gateFitParameters.aspect, gateFitParameters.mode);
		}

		[NativeName("FocalLengthToFieldOfView_Safe")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float FocalLengthToFieldOfView(float focalLength, float sensorSize);

		[NativeName("FieldOfViewToFocalLength_Safe")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float FieldOfViewToFocalLength(float fieldOfView, float sensorSize);

		[NativeName("HorizontalToVerticalFieldOfView_Safe")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float HorizontalToVerticalFieldOfView(float horizontalFieldOfView, float aspectRatio);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float VerticalToHorizontalFieldOfView(float verticalFieldOfView, float aspectRatio);

		public static Camera main
		{
			[FreeFunction("FindMainCamera")]
			get
			{
				return Unmarshal.UnmarshalUnityObject<Camera>(Camera.get_main_Injected());
			}
		}

		public static Camera current
		{
			get
			{
				return Camera.currentInternal;
			}
		}

		private static Camera currentInternal
		{
			[FreeFunction("GetCurrentCameraPPtr")]
			get
			{
				return Unmarshal.UnmarshalUnityObject<Camera>(Camera.get_currentInternal_Injected());
			}
		}

		public Scene scene
		{
			[FreeFunction("CameraScripting::GetScene", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Scene scene;
				Camera.get_scene_Injected(intPtr, out scene);
				return scene;
			}
			[FreeFunction("CameraScripting::SetScene", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_scene_Injected(intPtr, ref value);
			}
		}

		public bool stereoEnabled
		{
			[NativeMethod("GetStereoEnabledForBuiltInOrSRP")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_stereoEnabled_Injected(intPtr);
			}
		}

		public float stereoSeparation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_stereoSeparation_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_stereoSeparation_Injected(intPtr, value);
			}
		}

		public float stereoConvergence
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_stereoConvergence_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_stereoConvergence_Injected(intPtr, value);
			}
		}

		public bool areVRStereoViewMatricesWithinSingleCullTolerance
		{
			[NativeName("AreVRStereoViewMatricesWithinSingleCullTolerance")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_areVRStereoViewMatricesWithinSingleCullTolerance_Injected(intPtr);
			}
		}

		public StereoTargetEyeMask stereoTargetEye
		{
			get
			{
				return this.stereoTargetEyeInternal;
			}
			set
			{
				bool flag = GraphicsSettings.currentRenderPipeline != null;
				if (flag)
				{
					Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.stereoTargetEye only with the built-in renderer.");
				}
				this.stereoTargetEyeInternal = value;
			}
		}

		[NativeProperty("StereoTargetEye")]
		internal StereoTargetEyeMask stereoTargetEyeInternal
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_stereoTargetEyeInternal_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_stereoTargetEyeInternal_Injected(intPtr, value);
			}
		}

		public Camera.MonoOrStereoscopicEye stereoActiveEye
		{
			[FreeFunction("CameraScripting::GetStereoActiveEye", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_stereoActiveEye_Injected(intPtr);
			}
		}

		public Matrix4x4 GetStereoNonJitteredProjectionMatrix(Camera.StereoscopicEye eye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			Camera.GetStereoNonJitteredProjectionMatrix_Injected(intPtr, eye, out matrix4x);
			return matrix4x;
		}

		[FreeFunction("CameraScripting::GetStereoViewMatrix", HasExplicitThis = true)]
		public Matrix4x4 GetStereoViewMatrix(Camera.StereoscopicEye eye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			Camera.GetStereoViewMatrix_Injected(intPtr, eye, out matrix4x);
			return matrix4x;
		}

		public void CopyStereoDeviceProjectionMatrixToNonJittered(Camera.StereoscopicEye eye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.CopyStereoDeviceProjectionMatrixToNonJittered_Injected(intPtr, eye);
		}

		[FreeFunction("CameraScripting::GetStereoProjectionMatrix", HasExplicitThis = true)]
		public Matrix4x4 GetStereoProjectionMatrix(Camera.StereoscopicEye eye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			Camera.GetStereoProjectionMatrix_Injected(intPtr, eye, out matrix4x);
			return matrix4x;
		}

		public void SetStereoProjectionMatrix(Camera.StereoscopicEye eye, Matrix4x4 matrix)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.SetStereoProjectionMatrix_Injected(intPtr, eye, ref matrix);
		}

		public void ResetStereoProjectionMatrices()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.ResetStereoProjectionMatrices_Injected(intPtr);
		}

		public void SetStereoViewMatrix(Camera.StereoscopicEye eye, Matrix4x4 matrix)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.SetStereoViewMatrix_Injected(intPtr, eye, ref matrix);
		}

		public void ResetStereoViewMatrices()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.ResetStereoViewMatrices_Injected(intPtr);
		}

		[FreeFunction("CameraScripting::GetAllCamerasCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAllCamerasCount();

		[FreeFunction("CameraScripting::GetAllCameras")]
		private static int GetAllCamerasImpl([NotNull] [Out] Camera[] cam)
		{
			if (cam == null)
			{
				ThrowHelper.ThrowArgumentNullException(cam, "cam");
			}
			return Camera.GetAllCamerasImpl_Injected(cam);
		}

		public static int allCamerasCount
		{
			get
			{
				return Camera.GetAllCamerasCount();
			}
		}

		public static Camera[] allCameras
		{
			get
			{
				Camera[] array = new Camera[Camera.allCamerasCount];
				Camera.GetAllCamerasImpl(array);
				return array;
			}
		}

		public static int GetAllCameras(Camera[] cameras)
		{
			bool flag = cameras == null;
			if (flag)
			{
				throw new NullReferenceException();
			}
			bool flag2 = cameras.Length < Camera.allCamerasCount;
			if (flag2)
			{
				throw new ArgumentException("Passed in array to fill with cameras is to small to hold the number of cameras. Use Camera.allCamerasCount to get the needed size.");
			}
			return Camera.GetAllCamerasImpl(cameras);
		}

		[FreeFunction("CameraScripting::RenderToCubemap", HasExplicitThis = true)]
		private bool RenderToCubemapImpl(Texture tex, [DefaultValue("63")] int faceMask)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Camera.RenderToCubemapImpl_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(tex), faceMask);
		}

		public bool RenderToCubemap(Cubemap cubemap, int faceMask)
		{
			return this.RenderToCubemapImpl(cubemap, faceMask);
		}

		public bool RenderToCubemap(Cubemap cubemap)
		{
			return this.RenderToCubemapImpl(cubemap, 63);
		}

		public bool RenderToCubemap(RenderTexture cubemap, int faceMask)
		{
			return this.RenderToCubemapImpl(cubemap, faceMask);
		}

		public bool RenderToCubemap(RenderTexture cubemap)
		{
			return this.RenderToCubemapImpl(cubemap, 63);
		}

		[NativeConditional("UNITY_EDITOR")]
		private int GetFilterMode()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Camera.GetFilterMode_Injected(intPtr);
		}

		[NativeConditional("UNITY_EDITOR")]
		public Camera.SceneViewFilterMode sceneViewFilterMode
		{
			get
			{
				return (Camera.SceneViewFilterMode)this.GetFilterMode();
			}
		}

		[NativeConditional("UNITY_EDITOR")]
		public bool renderCloudsInSceneView
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_renderCloudsInSceneView_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Camera.set_renderCloudsInSceneView_Injected(intPtr, value);
			}
		}

		[NativeName("RenderToCubemap")]
		private bool RenderToCubemapEyeImpl(RenderTexture cubemap, int faceMask, Camera.MonoOrStereoscopicEye stereoEye)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Camera.RenderToCubemapEyeImpl_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(cubemap), faceMask, stereoEye);
		}

		public bool RenderToCubemap(RenderTexture cubemap, int faceMask, Camera.MonoOrStereoscopicEye stereoEye)
		{
			return this.RenderToCubemapEyeImpl(cubemap, faceMask, stereoEye);
		}

		[FreeFunction("CameraScripting::Render", HasExplicitThis = true)]
		public void Render()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.Render_Injected(intPtr);
		}

		[FreeFunction("CameraScripting::RenderWithShader", HasExplicitThis = true)]
		public unsafe void RenderWithShader(Shader shader, string replacementTag)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.Marshal<Shader>(shader);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(replacementTag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = replacementTag.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Camera.RenderWithShader_Injected(intPtr, intPtr2, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("CameraScripting::RenderDontRestore", HasExplicitThis = true)]
		public void RenderDontRestore()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.RenderDontRestore_Injected(intPtr);
		}

		public void SubmitRenderRequest<RequestData>(RequestData renderRequest)
		{
			bool flag = renderRequest == null;
			if (flag)
			{
				throw new ArgumentException("SubmitRenderRequest is invoked with invalid renderRequests");
			}
			ObjectIdRequest objectIdRequest = renderRequest as ObjectIdRequest;
			bool flag2 = objectIdRequest != null;
			if (flag2)
			{
				bool flag3 = objectIdRequest.destination.depthStencilFormat == GraphicsFormat.None;
				if (flag3)
				{
					Debug.LogWarning("ObjectId Render Request submitted without a depth stencil, which can produce results that are not depth tested correctly");
				}
				bool flag4 = GraphicsSettings.currentRenderPipeline == null || !RenderPipelineManager.currentPipeline.IsRenderRequestSupported<ObjectIdRequest>(this, objectIdRequest);
				if (flag4)
				{
					throw new ArgumentException((GraphicsSettings.currentRenderPipeline == null) ? "The Built-In Render Pipeline does not support ObjectIdRequest outside of the editor." : "The current render pipeline does not support ObjectIdRequest, and the fallback implementation of the Built-In Render Pipeline is not available outside of the editor.");
				}
			}
			bool flag5 = GraphicsSettings.currentRenderPipeline == null;
			if (flag5)
			{
				Debug.LogWarning("Trying to invoke 'SubmitRenderRequest' when no SRP is set. A scriptable render pipeline is needed for this function call");
			}
			else
			{
				this.SubmitRenderRequestsInternal(renderRequest);
			}
		}

		[FreeFunction("CameraScripting::SubmitRenderRequests", HasExplicitThis = true)]
		private void SubmitRenderRequestsInternal(object requests)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.SubmitRenderRequestsInternal_Injected(intPtr, requests);
		}

		[NativeConditional("UNITY_EDITOR")]
		[FreeFunction("CameraScripting::SubmitBuiltInObjectIDRenderRequest", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		private Object[] SubmitBuiltInObjectIDRenderRequest(RenderTexture target, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Camera.SubmitBuiltInObjectIDRenderRequest_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(target), mipLevel, cubemapFace, depthSlice);
		}

		public bool isProcessingRenderRequest
		{
			[NativeMethod("IsProcessingRenderRequest")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_isProcessingRenderRequest_Injected(intPtr);
			}
		}

		[FreeFunction("CameraScripting::SetupCurrent")]
		public static void SetupCurrent(Camera cur)
		{
			Camera.SetupCurrent_Injected(Object.MarshalledUnityObject.Marshal<Camera>(cur));
		}

		[FreeFunction("CameraScripting::CopyFrom", HasExplicitThis = true)]
		public void CopyFrom(Camera other)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.CopyFrom_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Camera>(other));
		}

		public int commandBufferCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Camera.get_commandBufferCount_Injected(intPtr);
			}
		}

		[NativeName("RemoveCommandBuffers")]
		private void RemoveCommandBuffersImpl(CameraEvent evt)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.RemoveCommandBuffersImpl_Injected(intPtr, evt);
		}

		[NativeName("RemoveAllCommandBuffers")]
		private void RemoveAllCommandBuffersImpl()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Camera.RemoveAllCommandBuffersImpl_Injected(intPtr);
		}

		public void RemoveCommandBuffers(CameraEvent evt)
		{
			bool flag = RenderPipelineManager.currentPipeline != null;
			if (flag)
			{
				Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.RemoveCommandBuffers only with the built-in renderer.");
			}
			else
			{
				this.m_NonSerializedVersion += 1U;
				this.RemoveCommandBuffersImpl(evt);
			}
		}

		public void RemoveAllCommandBuffers()
		{
			bool flag = RenderPipelineManager.currentPipeline != null;
			if (flag)
			{
				Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.RemoveAllCommandBuffers only with the built-in renderer.");
			}
			else
			{
				this.m_NonSerializedVersion += 1U;
				this.RemoveAllCommandBuffersImpl();
			}
		}

		[NativeName("AddCommandBuffer")]
		private void AddCommandBufferImpl(CameraEvent evt, [NotNull] CommandBuffer buffer)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = CommandBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			Camera.AddCommandBufferImpl_Injected(intPtr, evt, intPtr2);
		}

		[NativeName("AddCommandBufferAsync")]
		private void AddCommandBufferAsyncImpl(CameraEvent evt, [NotNull] CommandBuffer buffer, ComputeQueueType queueType)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = CommandBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			Camera.AddCommandBufferAsyncImpl_Injected(intPtr, evt, intPtr2, queueType);
		}

		[NativeName("RemoveCommandBuffer")]
		private void RemoveCommandBufferImpl(CameraEvent evt, [NotNull] CommandBuffer buffer)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = CommandBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			Camera.RemoveCommandBufferImpl_Injected(intPtr, evt, intPtr2);
		}

		public void AddCommandBuffer(CameraEvent evt, CommandBuffer buffer)
		{
			bool flag = !CameraEventUtils.IsValid(evt);
			if (flag)
			{
				throw new ArgumentException(string.Format("Invalid CameraEvent value \"{0}\".", (int)evt), "evt");
			}
			bool flag2 = buffer == null;
			if (flag2)
			{
				throw new NullReferenceException("buffer is null");
			}
			bool flag3 = RenderPipelineManager.currentPipeline != null;
			if (flag3)
			{
				Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.AddCommandBuffer only with the built-in renderer.");
			}
			else
			{
				this.AddCommandBufferImpl(evt, buffer);
				this.m_NonSerializedVersion += 1U;
			}
		}

		public void AddCommandBufferAsync(CameraEvent evt, CommandBuffer buffer, ComputeQueueType queueType)
		{
			bool flag = !CameraEventUtils.IsValid(evt);
			if (flag)
			{
				throw new ArgumentException(string.Format("Invalid CameraEvent value \"{0}\".", (int)evt), "evt");
			}
			bool flag2 = buffer == null;
			if (flag2)
			{
				throw new NullReferenceException("buffer is null");
			}
			bool flag3 = RenderPipelineManager.currentPipeline != null;
			if (flag3)
			{
				Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.AddCommandBufferAsync only with the built-in renderer.");
			}
			else
			{
				this.AddCommandBufferAsyncImpl(evt, buffer, queueType);
				this.m_NonSerializedVersion += 1U;
			}
		}

		public void RemoveCommandBuffer(CameraEvent evt, CommandBuffer buffer)
		{
			bool flag = !CameraEventUtils.IsValid(evt);
			if (flag)
			{
				throw new ArgumentException(string.Format("Invalid CameraEvent value \"{0}\".", (int)evt), "evt");
			}
			bool flag2 = buffer == null;
			if (flag2)
			{
				throw new NullReferenceException("buffer is null");
			}
			bool flag3 = RenderPipelineManager.currentPipeline != null;
			if (flag3)
			{
				Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.RemoveCommandBuffer only with the built-in renderer.");
			}
			else
			{
				this.RemoveCommandBufferImpl(evt, buffer);
				this.m_NonSerializedVersion += 1U;
			}
		}

		public CommandBuffer[] GetCommandBuffers(CameraEvent evt)
		{
			bool flag = RenderPipelineManager.currentPipeline != null;
			if (flag)
			{
				Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.GetCommandBuffers only with the built-in renderer.");
			}
			return this.GetCommandBuffersImpl(evt);
		}

		[FreeFunction("CameraScripting::GetCommandBuffers", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		internal CommandBuffer[] GetCommandBuffersImpl(CameraEvent evt)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Camera.GetCommandBuffersImpl_Injected(intPtr, evt);
		}

		[RequiredByNativeCode]
		private static void FireOnPreCull(Camera cam)
		{
			bool flag = Camera.onPreCull != null;
			if (flag)
			{
				Camera.onPreCull(cam);
			}
		}

		[RequiredByNativeCode]
		private static void FireOnPreRender(Camera cam)
		{
			bool flag = Camera.onPreRender != null;
			if (flag)
			{
				Camera.onPreRender(cam);
			}
		}

		[RequiredByNativeCode]
		private static void FireOnPostRender(Camera cam)
		{
			bool flag = Camera.onPostRender != null;
			if (flag)
			{
				Camera.onPostRender(cam);
			}
		}

		[RequiredByNativeCode]
		private static void BumpNonSerializedVersion(Camera cam)
		{
			cam.m_NonSerializedVersion += 1U;
		}

		internal void OnlyUsedForTesting1()
		{
		}

		internal void OnlyUsedForTesting2()
		{
		}

		public unsafe bool TryGetCullingParameters(out ScriptableCullingParameters cullingParameters)
		{
			return Camera.GetCullingParameters_Internal(this, false, out cullingParameters, sizeof(ScriptableCullingParameters));
		}

		public unsafe bool TryGetCullingParameters(bool stereoAware, out ScriptableCullingParameters cullingParameters)
		{
			return Camera.GetCullingParameters_Internal(this, stereoAware, out cullingParameters, sizeof(ScriptableCullingParameters));
		}

		[NativeHeader("Runtime/Export/RenderPipeline/ScriptableRenderPipeline.bindings.h")]
		[FreeFunction("ScriptableRenderPipeline_Bindings::GetCullingParameters_Internal")]
		private static bool GetCullingParameters_Internal(Camera camera, bool stereoAware, out ScriptableCullingParameters cullingParameters, int managedCullingParametersSize)
		{
			return Camera.GetCullingParameters_Internal_Injected(Object.MarshalledUnityObject.Marshal<Camera>(camera), stereoAware, out cullingParameters, managedCullingParametersSize);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_nearClipPlane_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_nearClipPlane_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_farClipPlane_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_farClipPlane_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_fieldOfView_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_fieldOfView_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RenderingPath get_renderingPath_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderingPath_Injected(IntPtr _unity_self, RenderingPath value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RenderingPath get_actualRenderingPath_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Reset_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_allowHDR_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_allowHDR_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_allowMSAA_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_allowMSAA_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_allowDynamicResolution_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_allowDynamicResolution_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_forceIntoRenderTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_forceIntoRenderTexture_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_orthographicSize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_orthographicSize_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_orthographic_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_orthographic_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern OpaqueSortMode get_opaqueSortMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_opaqueSortMode_Injected(IntPtr _unity_self, OpaqueSortMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TransparencySortMode get_transparencySortMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_transparencySortMode_Injected(IntPtr _unity_self, TransparencySortMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_transparencySortAxis_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_transparencySortAxis_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetTransparencySortSettings_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_depth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_depth_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_aspect_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_aspect_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetAspect_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_velocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_cullingMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cullingMask_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_eventMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_eventMask_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_layerCullSphericalInternal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_layerCullSphericalInternal_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CameraType get_cameraType_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cameraType_Injected(IntPtr _unity_self, CameraType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_skyboxMaterial_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong get_overrideSceneCullingMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_overrideSceneCullingMask_Injected(IntPtr _unity_self, ulong value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong get_sceneCullingMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useInteractiveLightBakingData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useInteractiveLightBakingData_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLayerCullDistances_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLayerCullDistances_Injected(IntPtr _unity_self, ref ManagedSpanWrapper d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useOcclusionCulling_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useOcclusionCulling_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_cullingMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cullingMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetCullingMatrix_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_backgroundColor_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_backgroundColor_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CameraClearFlags get_clearFlags_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_clearFlags_Injected(IntPtr _unity_self, CameraClearFlags value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DepthTextureMode get_depthTextureMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_depthTextureMode_Injected(IntPtr _unity_self, DepthTextureMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_clearStencilAfterLightingPass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_clearStencilAfterLightingPass_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetReplacementShader_Injected(IntPtr _unity_self, IntPtr shader, ref ManagedSpanWrapper replacementTag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetReplacementShader_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Camera.ProjectionMatrixMode get_projectionMatrixMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_usePhysicalProperties_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_usePhysicalProperties_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_iso_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_iso_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_shutterSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shutterSpeed_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_aperture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_aperture_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_focusDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_focusDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_focalLength_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_focalLength_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_bladeCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bladeCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_curvature_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_curvature_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_barrelClipping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_barrelClipping_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_anamorphism_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_anamorphism_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sensorSize_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sensorSize_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_lensShift_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lensShift_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Camera.GateFitMode get_gateFit_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gateFit_Injected(IntPtr _unity_self, Camera.GateFitMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetGateFittedFieldOfView_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGateFittedLensShift_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalSpaceAim_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rect_Injected(IntPtr _unity_self, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rect_Injected(IntPtr _unity_self, [In] ref Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_pixelRect_Injected(IntPtr _unity_self, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_pixelRect_Injected(IntPtr _unity_self, [In] ref Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_pixelWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_pixelHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_scaledPixelWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_scaledPixelHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_targetTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetTexture_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_activeTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_targetDisplay_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetDisplay_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTargetBuffersImpl_Injected(IntPtr _unity_self, [In] ref RenderBuffer color, [In] ref RenderBuffer depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTargetBuffersMRTImpl_Injected(IntPtr _unity_self, ref ManagedSpanWrapper color, [In] ref RenderBuffer depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetCameraBufferWarnings_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_cameraToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_worldToCameraMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_worldToCameraMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_projectionMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_projectionMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_nonJitteredProjectionMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_nonJitteredProjectionMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useJitteredProjectionMatrixForTransparentRendering_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useJitteredProjectionMatrixForTransparentRendering_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_previousViewProjectionMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetWorldToCameraMatrix_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetProjectionMatrix_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateObliqueMatrix_Injected(IntPtr _unity_self, [In] ref Vector4 clipPlane, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WorldToScreenPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, Camera.MonoOrStereoscopicEye eye, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WorldToViewportPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, Camera.MonoOrStereoscopicEye eye, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ViewportToWorldPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, Camera.MonoOrStereoscopicEye eye, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScreenToWorldPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, Camera.MonoOrStereoscopicEye eye, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScreenToViewportPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ViewportToScreenPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetFrustumPlaneSizeAt_Injected(IntPtr _unity_self, float distance, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ViewportPointToRay_Injected(IntPtr _unity_self, [In] ref Vector2 pos, Camera.MonoOrStereoscopicEye eye, out Ray ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScreenPointToRay_Injected(IntPtr _unity_self, [In] ref Vector2 pos, Camera.MonoOrStereoscopicEye eye, out Ray ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateFrustumCornersInternal_Injected(IntPtr _unity_self, [In] ref Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, out BlittableArrayWrapper outCorners);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateProjectionMatrixFromPhysicalPropertiesInternal_Injected(out Matrix4x4 output, float focalLength, [In] ref Vector2 sensorSize, [In] ref Vector2 lensShift, float nearClip, float farClip, float gateAspect, Camera.GateFitMode gateFitMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_main_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_currentInternal_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_scene_Injected(IntPtr _unity_self, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_scene_Injected(IntPtr _unity_self, [In] ref Scene value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_stereoEnabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_stereoSeparation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_stereoSeparation_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_stereoConvergence_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_stereoConvergence_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_areVRStereoViewMatricesWithinSingleCullTolerance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern StereoTargetEyeMask get_stereoTargetEyeInternal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_stereoTargetEyeInternal_Injected(IntPtr _unity_self, StereoTargetEyeMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Camera.MonoOrStereoscopicEye get_stereoActiveEye_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetStereoNonJitteredProjectionMatrix_Injected(IntPtr _unity_self, Camera.StereoscopicEye eye, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetStereoViewMatrix_Injected(IntPtr _unity_self, Camera.StereoscopicEye eye, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyStereoDeviceProjectionMatrixToNonJittered_Injected(IntPtr _unity_self, Camera.StereoscopicEye eye);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetStereoProjectionMatrix_Injected(IntPtr _unity_self, Camera.StereoscopicEye eye, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStereoProjectionMatrix_Injected(IntPtr _unity_self, Camera.StereoscopicEye eye, [In] ref Matrix4x4 matrix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetStereoProjectionMatrices_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStereoViewMatrix_Injected(IntPtr _unity_self, Camera.StereoscopicEye eye, [In] ref Matrix4x4 matrix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetStereoViewMatrices_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAllCamerasImpl_Injected([Out] Camera[] cam);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RenderToCubemapImpl_Injected(IntPtr _unity_self, IntPtr tex, [DefaultValue("63")] int faceMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetFilterMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_renderCloudsInSceneView_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderCloudsInSceneView_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RenderToCubemapEyeImpl_Injected(IntPtr _unity_self, IntPtr cubemap, int faceMask, Camera.MonoOrStereoscopicEye stereoEye);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Render_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RenderWithShader_Injected(IntPtr _unity_self, IntPtr shader, ref ManagedSpanWrapper replacementTag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RenderDontRestore_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SubmitRenderRequestsInternal_Injected(IntPtr _unity_self, object requests);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object[] SubmitBuiltInObjectIDRenderRequest_Injected(IntPtr _unity_self, IntPtr target, int mipLevel, CubemapFace cubemapFace, int depthSlice);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isProcessingRenderRequest_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetupCurrent_Injected(IntPtr cur);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyFrom_Injected(IntPtr _unity_self, IntPtr other);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_commandBufferCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveCommandBuffersImpl_Injected(IntPtr _unity_self, CameraEvent evt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveAllCommandBuffersImpl_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddCommandBufferImpl_Injected(IntPtr _unity_self, CameraEvent evt, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddCommandBufferAsyncImpl_Injected(IntPtr _unity_self, CameraEvent evt, IntPtr buffer, ComputeQueueType queueType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveCommandBufferImpl_Injected(IntPtr _unity_self, CameraEvent evt, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CommandBuffer[] GetCommandBuffersImpl_Injected(IntPtr _unity_self, CameraEvent evt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetCullingParameters_Internal_Injected(IntPtr camera, bool stereoAware, out ScriptableCullingParameters cullingParameters, int managedCullingParametersSize);

		public const float kMinAperture = 0.7f;

		public const float kMaxAperture = 32f;

		public const int kMinBladeCount = 3;

		public const int kMaxBladeCount = 11;

		internal uint m_NonSerializedVersion;

		public static Camera.CameraCallback onPreCull;

		public static Camera.CameraCallback onPreRender;

		public static Camera.CameraCallback onPostRender;

		internal enum ProjectionMatrixMode
		{
			Explicit,
			Implicit,
			PhysicalPropertiesBased
		}

		public enum GateFitMode
		{
			Vertical = 1,
			Horizontal,
			Fill,
			Overscan,
			None = 0
		}

		public enum FieldOfViewAxis
		{
			Vertical,
			Horizontal
		}

		public struct GateFitParameters
		{
			public Camera.GateFitMode mode { readonly get; set; }

			public float aspect { readonly get; set; }

			public GateFitParameters(Camera.GateFitMode mode, float aspect)
			{
				this.mode = mode;
				this.aspect = aspect;
			}
		}

		public enum StereoscopicEye
		{
			Left,
			Right
		}

		public enum MonoOrStereoscopicEye
		{
			Left,
			Right,
			Mono
		}

		public enum SceneViewFilterMode
		{
			Off,
			ShowFiltered
		}

		public delegate void CameraCallback(Camera cam);
	}
}
