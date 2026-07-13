using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[ExcludeFromPreset]
	[NativeHeader("Modules/Terrain/Public/Tree.h")]
	public sealed class Tree : Component
	{
		[NativeProperty("TreeData")]
		public ScriptableObject data
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tree>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<ScriptableObject>(Tree.get_data_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tree>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Tree.set_data_Injected(intPtr, Object.MarshalledUnityObject.Marshal<ScriptableObject>(value));
			}
		}

		public bool hasSpeedTreeWind
		{
			[NativeMethod("HasSpeedTreeWind")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tree>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Tree.get_hasSpeedTreeWind_Injected(intPtr);
			}
		}

		[NativeProperty("SpeedTreeWindAsset")]
		public SpeedTreeWindAsset windAsset
		{
			[NativeMethod("GetSpeedTreeWind")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tree>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<SpeedTreeWindAsset>(Tree.get_windAsset_Injected(intPtr));
			}
			[NativeMethod("SetSpeedTreeWind")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tree>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Tree.set_windAsset_Injected(intPtr, Object.MarshalledUnityObject.Marshal<SpeedTreeWindAsset>(value));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_data_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_data_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasSpeedTreeWind_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_windAsset_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_windAsset_Injected(IntPtr _unity_self, IntPtr value);
	}
}
