using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Camera/LightProbeProxyVolume.h")]
	public sealed class LightProbeProxyVolume : Behaviour
	{
		public static extern bool isFeatureSupported
		{
			[NativeName("IsFeatureSupported")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeName("GlobalAABB")]
		public Bounds boundsGlobal
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				LightProbeProxyVolume.get_boundsGlobal_Injected(intPtr, out bounds);
				return bounds;
			}
		}

		[NativeName("BoundingBoxSizeCustom")]
		public Vector3 sizeCustom
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				LightProbeProxyVolume.get_sizeCustom_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_sizeCustom_Injected(intPtr, ref value);
			}
		}

		[NativeName("BoundingBoxOriginCustom")]
		public Vector3 originCustom
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				LightProbeProxyVolume.get_originCustom_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_originCustom_Injected(intPtr, ref value);
			}
		}

		public float probeDensity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbeProxyVolume.get_probeDensity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_probeDensity_Injected(intPtr, value);
			}
		}

		public int gridResolutionX
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbeProxyVolume.get_gridResolutionX_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_gridResolutionX_Injected(intPtr, value);
			}
		}

		public int gridResolutionY
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbeProxyVolume.get_gridResolutionY_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_gridResolutionY_Injected(intPtr, value);
			}
		}

		public int gridResolutionZ
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbeProxyVolume.get_gridResolutionZ_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_gridResolutionZ_Injected(intPtr, value);
			}
		}

		public LightProbeProxyVolume.BoundingBoxMode boundingBoxMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbeProxyVolume.get_boundingBoxMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_boundingBoxMode_Injected(intPtr, value);
			}
		}

		public LightProbeProxyVolume.ResolutionMode resolutionMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbeProxyVolume.get_resolutionMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_resolutionMode_Injected(intPtr, value);
			}
		}

		public LightProbeProxyVolume.ProbePositionMode probePositionMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbeProxyVolume.get_probePositionMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_probePositionMode_Injected(intPtr, value);
			}
		}

		public LightProbeProxyVolume.RefreshMode refreshMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbeProxyVolume.get_refreshMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_refreshMode_Injected(intPtr, value);
			}
		}

		public LightProbeProxyVolume.QualityMode qualityMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbeProxyVolume.get_qualityMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_qualityMode_Injected(intPtr, value);
			}
		}

		public LightProbeProxyVolume.DataFormat dataFormat
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbeProxyVolume.get_dataFormat_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbeProxyVolume.set_dataFormat_Injected(intPtr, value);
			}
		}

		public void Update()
		{
			this.SetDirtyFlag(true);
		}

		private void SetDirtyFlag(bool flag)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbeProxyVolume>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			LightProbeProxyVolume.SetDirtyFlag_Injected(intPtr, flag);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_boundsGlobal_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sizeCustom_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sizeCustom_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_originCustom_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_originCustom_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_probeDensity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_probeDensity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_gridResolutionX_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gridResolutionX_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_gridResolutionY_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gridResolutionY_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_gridResolutionZ_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gridResolutionZ_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightProbeProxyVolume.BoundingBoxMode get_boundingBoxMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_boundingBoxMode_Injected(IntPtr _unity_self, LightProbeProxyVolume.BoundingBoxMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightProbeProxyVolume.ResolutionMode get_resolutionMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_resolutionMode_Injected(IntPtr _unity_self, LightProbeProxyVolume.ResolutionMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightProbeProxyVolume.ProbePositionMode get_probePositionMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_probePositionMode_Injected(IntPtr _unity_self, LightProbeProxyVolume.ProbePositionMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightProbeProxyVolume.RefreshMode get_refreshMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_refreshMode_Injected(IntPtr _unity_self, LightProbeProxyVolume.RefreshMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightProbeProxyVolume.QualityMode get_qualityMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_qualityMode_Injected(IntPtr _unity_self, LightProbeProxyVolume.QualityMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LightProbeProxyVolume.DataFormat get_dataFormat_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_dataFormat_Injected(IntPtr _unity_self, LightProbeProxyVolume.DataFormat value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDirtyFlag_Injected(IntPtr _unity_self, bool flag);

		public enum ResolutionMode
		{
			Automatic,
			Custom
		}

		public enum BoundingBoxMode
		{
			AutomaticLocal,
			AutomaticWorld,
			Custom
		}

		public enum ProbePositionMode
		{
			CellCorner,
			CellCenter
		}

		public enum RefreshMode
		{
			Automatic,
			EveryFrame,
			ViaScripting
		}

		public enum QualityMode
		{
			Low,
			Normal
		}

		public enum DataFormat
		{
			HalfFloat,
			Float
		}
	}
}
