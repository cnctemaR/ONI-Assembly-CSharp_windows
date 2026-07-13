using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadFromAsyncOperation.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class AssetBundleCreateRequest : AsyncOperation
	{
		public AssetBundle assetBundle
		{
			[NativeMethod("GetAssetBundleBlocking")]
			get
			{
				IntPtr intPtr = AssetBundleCreateRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<AssetBundle>(AssetBundleCreateRequest.get_assetBundle_Injected(intPtr));
			}
		}

		[NativeMethod("SetEnableCompatibilityChecks")]
		private void SetEnableCompatibilityChecks(bool set)
		{
			IntPtr intPtr = AssetBundleCreateRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AssetBundleCreateRequest.SetEnableCompatibilityChecks_Injected(intPtr, set);
		}

		internal void DisableCompatibilityChecks()
		{
			this.SetEnableCompatibilityChecks(false);
		}

		public AssetBundleCreateRequest()
		{
		}

		private AssetBundleCreateRequest(IntPtr ptr)
			: base(ptr)
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_assetBundle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetEnableCompatibilityChecks_Injected(IntPtr _unity_self, bool set);

		internal new static class BindingsMarshaller
		{
			public static AssetBundleCreateRequest ConvertToManaged(IntPtr ptr)
			{
				return new AssetBundleCreateRequest(ptr);
			}

			public static IntPtr ConvertToNative(AssetBundleCreateRequest assetBundleCreateRequest)
			{
				return assetBundleCreateRequest.m_Ptr;
			}
		}
	}
}
