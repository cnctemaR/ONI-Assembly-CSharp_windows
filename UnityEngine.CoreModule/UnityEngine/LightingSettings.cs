using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/LightingSettings.h")]
	[PreventReadOnlyInstanceModification]
	public sealed class LightingSettings : Object
	{
		[RequiredByNativeCode]
		internal void LightingSettingsDontStripMe()
		{
		}

		public LightingSettings()
		{
			LightingSettings.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] LightingSettings self);

		[FreeFunction("GetLightingSettingsPtr")]
		internal static LightingSettings GetActiveSettings()
		{
			return Unmarshal.UnmarshalUnityObject<LightingSettings>(LightingSettings.GetActiveSettings_Injected());
		}

		[NativeName("EnableBakedLightmaps")]
		public bool bakedGI
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightingSettings>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightingSettings.get_bakedGI_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightingSettings>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightingSettings.set_bakedGI_Injected(intPtr, value);
			}
		}

		[NativeName("EnableRealtimeLightmaps")]
		public bool realtimeGI
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightingSettings>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightingSettings.get_realtimeGI_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightingSettings>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightingSettings.set_realtimeGI_Injected(intPtr, value);
			}
		}

		[NativeName("RealtimeEnvironmentLighting")]
		public bool realtimeEnvironmentLighting
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightingSettings>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightingSettings.get_realtimeEnvironmentLighting_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightingSettings>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightingSettings.set_realtimeEnvironmentLighting_Injected(intPtr, value);
			}
		}

		internal bool usingShadowmask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightingSettings>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightingSettings.get_usingShadowmask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightingSettings>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightingSettings.set_usingShadowmask_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetActiveSettings_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_bakedGI_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bakedGI_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_realtimeGI_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_realtimeGI_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_realtimeEnvironmentLighting_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_realtimeEnvironmentLighting_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_usingShadowmask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_usingShadowmask_Injected(IntPtr _unity_self, bool value);
	}
}
