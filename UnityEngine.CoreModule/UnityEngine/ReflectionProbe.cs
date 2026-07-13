using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Camera/ReflectionProbes.h")]
	public sealed class ReflectionProbe : Behaviour
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("type property has been deprecated. Starting with Unity 5.4, the only supported reflection probe type is Cube.", true)]
		[NativeName("ProbeType")]
		public ReflectionProbeType type
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_type_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_type_Injected(intPtr, value);
			}
		}

		[NativeName("BoxSize")]
		public Vector3 size
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ReflectionProbe.get_size_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_size_Injected(intPtr, ref value);
			}
		}

		[NativeName("BoxOffset")]
		public Vector3 center
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ReflectionProbe.get_center_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_center_Injected(intPtr, ref value);
			}
		}

		[NativeName("Near")]
		public float nearClipPlane
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_nearClipPlane_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_nearClipPlane_Injected(intPtr, value);
			}
		}

		[NativeName("Far")]
		public float farClipPlane
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_farClipPlane_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_farClipPlane_Injected(intPtr, value);
			}
		}

		[NativeName("IntensityMultiplier")]
		public float intensity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_intensity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_intensity_Injected(intPtr, value);
			}
		}

		[NativeName("GlobalAABB")]
		public Bounds bounds
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				ReflectionProbe.get_bounds_Injected(intPtr, out bounds);
				return bounds;
			}
		}

		[NativeName("HDR")]
		public bool hdr
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_hdr_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_hdr_Injected(intPtr, value);
			}
		}

		[NativeName("RenderDynamicObjects")]
		public bool renderDynamicObjects
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_renderDynamicObjects_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_renderDynamicObjects_Injected(intPtr, value);
			}
		}

		public float shadowDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_shadowDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_shadowDistance_Injected(intPtr, value);
			}
		}

		public int resolution
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_resolution_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_resolution_Injected(intPtr, value);
			}
		}

		public int cullingMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_cullingMask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_cullingMask_Injected(intPtr, value);
			}
		}

		public ReflectionProbeClearFlags clearFlags
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_clearFlags_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_clearFlags_Injected(intPtr, value);
			}
		}

		public Color backgroundColor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				ReflectionProbe.get_backgroundColor_Injected(intPtr, out color);
				return color;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_backgroundColor_Injected(intPtr, ref value);
			}
		}

		public float blendDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_blendDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_blendDistance_Injected(intPtr, value);
			}
		}

		public bool boxProjection
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_boxProjection_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_boxProjection_Injected(intPtr, value);
			}
		}

		public ReflectionProbeMode mode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_mode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_mode_Injected(intPtr, value);
			}
		}

		public int importance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_importance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_importance_Injected(intPtr, value);
			}
		}

		public ReflectionProbeRefreshMode refreshMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_refreshMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_refreshMode_Injected(intPtr, value);
			}
		}

		public ReflectionProbeTimeSlicingMode timeSlicingMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ReflectionProbe.get_timeSlicingMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_timeSlicingMode_Injected(intPtr, value);
			}
		}

		public Texture bakedTexture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture>(ReflectionProbe.get_bakedTexture_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_bakedTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(value));
			}
		}

		public Texture customBakedTexture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture>(ReflectionProbe.get_customBakedTexture_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_customBakedTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(value));
			}
		}

		public RenderTexture realtimeTexture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<RenderTexture>(ReflectionProbe.get_realtimeTexture_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ReflectionProbe.set_realtimeTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(value));
			}
		}

		public Texture texture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture>(ReflectionProbe.get_texture_Injected(intPtr));
			}
		}

		public Vector4 textureHDRDecodeValues
		{
			[NativeName("CalculateHDRDecodeValues")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector4 vector;
				ReflectionProbe.get_textureHDRDecodeValues_Injected(intPtr, out vector);
				return vector;
			}
		}

		public void Reset()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReflectionProbe.Reset_Injected(intPtr);
		}

		public int RenderProbe()
		{
			return this.RenderProbe(null);
		}

		public int RenderProbe([DefaultValue("null")] RenderTexture targetTexture)
		{
			return this.ScheduleRender(this.timeSlicingMode, targetTexture);
		}

		public bool IsFinishedRendering(int renderId)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ReflectionProbe.IsFinishedRendering_Injected(intPtr, renderId);
		}

		private int ScheduleRender(ReflectionProbeTimeSlicingMode timeSlicingMode, RenderTexture targetTexture)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ReflectionProbe>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ReflectionProbe.ScheduleRender_Injected(intPtr, timeSlicingMode, Object.MarshalledUnityObject.Marshal<RenderTexture>(targetTexture));
		}

		[NativeHeader("Runtime/Camera/CubemapGPUUtility.h")]
		[FreeFunction("CubemapGPUBlend")]
		public static bool BlendCubemap(Texture src, Texture dst, float blend, RenderTexture target)
		{
			return ReflectionProbe.BlendCubemap_Injected(Object.MarshalledUnityObject.Marshal<Texture>(src), Object.MarshalledUnityObject.Marshal<Texture>(dst), blend, Object.MarshalledUnityObject.Marshal<RenderTexture>(target));
		}

		[NativeMethod("UpdateSampleData")]
		[StaticAccessor("GetReflectionProbes()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateCachedState();

		[StaticAccessor("GetReflectionProbes()")]
		public static extern int minBakedCubemapResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetReflectionProbes()")]
		public static extern int maxBakedCubemapResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetReflectionProbes()")]
		public static Vector4 defaultTextureHDRDecodeValues
		{
			get
			{
				Vector4 vector;
				ReflectionProbe.get_defaultTextureHDRDecodeValues_Injected(out vector);
				return vector;
			}
		}

		[StaticAccessor("GetReflectionProbes()")]
		public static Texture defaultTexture
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Texture>(ReflectionProbe.get_defaultTexture_Injected());
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ReflectionProbe, ReflectionProbe.ReflectionProbeEvent> reflectionProbeChanged;

		[RequiredByNativeCode]
		private static void CallReflectionProbeEvent(ReflectionProbe probe, ReflectionProbe.ReflectionProbeEvent probeEvent)
		{
			Action<ReflectionProbe, ReflectionProbe.ReflectionProbeEvent> action = ReflectionProbe.reflectionProbeChanged;
			bool flag = action != null;
			if (flag)
			{
				action(probe, probeEvent);
			}
		}

		[Obsolete("ReflectionProbe.defaultReflectionSet has been deprecated. Use ReflectionProbe.defaultReflectionTexture. (UnityUpgradable) -> UnityEngine.ReflectionProbe.defaultReflectionTexture", false)]
		public static event Action<Cubemap> defaultReflectionSet
		{
			add
			{
				bool flag = ReflectionProbe.registeredDefaultReflectionTextureActions.Any<Action<Texture>>((Action<Texture> h) => h.Method == value.Method);
				if (!flag)
				{
					Action<Texture> action = delegate(Texture b)
					{
						Cubemap cubemap = b as Cubemap;
						bool flag2 = cubemap != null;
						if (flag2)
						{
							value(cubemap);
						}
					};
					ReflectionProbe.defaultReflectionTexture += action;
					ReflectionProbe.registeredDefaultReflectionSetActions[value.Method.GetHashCode()] = action;
				}
			}
			remove
			{
				Action<Texture> action;
				bool flag = ReflectionProbe.registeredDefaultReflectionSetActions.TryGetValue(value.Method.GetHashCode(), out action);
				if (flag)
				{
					ReflectionProbe.defaultReflectionTexture -= action;
					ReflectionProbe.registeredDefaultReflectionSetActions.Remove(value.Method.GetHashCode());
				}
			}
		}

		public static event Action<Texture> defaultReflectionTexture
		{
			add
			{
				bool flag = ReflectionProbe.registeredDefaultReflectionTextureActions.Any<Action<Texture>>((Action<Texture> h) => h.Method == value.Method) || ReflectionProbe.registeredDefaultReflectionSetActions.ContainsKey(value.Method.GetHashCode());
				if (!flag)
				{
					ReflectionProbe.registeredDefaultReflectionTextureActions.Add(value);
				}
			}
			remove
			{
				ReflectionProbe.registeredDefaultReflectionTextureActions.Remove(value);
			}
		}

		[RequiredByNativeCode]
		private static void CallSetDefaultReflection(Texture defaultReflectionCubemap)
		{
			foreach (Action<Texture> action in ReflectionProbe.registeredDefaultReflectionTextureActions)
			{
				action(defaultReflectionCubemap);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ReflectionProbeType get_type_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_type_Injected(IntPtr _unity_self, ReflectionProbeType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_size_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_size_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_center_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_center_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_nearClipPlane_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_nearClipPlane_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_farClipPlane_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_farClipPlane_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_intensity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_intensity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hdr_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_hdr_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_renderDynamicObjects_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderDynamicObjects_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_shadowDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_resolution_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_resolution_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_cullingMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cullingMask_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ReflectionProbeClearFlags get_clearFlags_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_clearFlags_Injected(IntPtr _unity_self, ReflectionProbeClearFlags value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_backgroundColor_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_backgroundColor_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_blendDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_blendDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_boxProjection_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_boxProjection_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ReflectionProbeMode get_mode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mode_Injected(IntPtr _unity_self, ReflectionProbeMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_importance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_importance_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ReflectionProbeRefreshMode get_refreshMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_refreshMode_Injected(IntPtr _unity_self, ReflectionProbeRefreshMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ReflectionProbeTimeSlicingMode get_timeSlicingMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_timeSlicingMode_Injected(IntPtr _unity_self, ReflectionProbeTimeSlicingMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_bakedTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bakedTexture_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_customBakedTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_customBakedTexture_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_realtimeTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_realtimeTexture_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_texture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_textureHDRDecodeValues_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Reset_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsFinishedRendering_Injected(IntPtr _unity_self, int renderId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ScheduleRender_Injected(IntPtr _unity_self, ReflectionProbeTimeSlicingMode timeSlicingMode, IntPtr targetTexture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool BlendCubemap_Injected(IntPtr src, IntPtr dst, float blend, IntPtr target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_defaultTextureHDRDecodeValues_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_defaultTexture_Injected();

		private static Dictionary<int, Action<Texture>> registeredDefaultReflectionSetActions = new Dictionary<int, Action<Texture>>();

		private static List<Action<Texture>> registeredDefaultReflectionTextureActions = new List<Action<Texture>>();

		public enum ReflectionProbeEvent
		{
			ReflectionProbeAdded,
			ReflectionProbeRemoved
		}
	}
}
