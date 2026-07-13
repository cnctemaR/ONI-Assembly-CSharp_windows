using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadAssetOperation.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class AssetBundleRequest : ResourceRequest
	{
		[NativeMethod("GetLoadedAsset")]
		protected override Object GetResult()
		{
			IntPtr intPtr = AssetBundleRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Object>(AssetBundleRequest.GetResult_Injected(intPtr));
		}

		public new Object asset
		{
			get
			{
				return this.GetResult();
			}
		}

		public Object[] allAssets
		{
			[NativeMethod("GetAllLoadedAssets")]
			get
			{
				IntPtr intPtr = AssetBundleRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AssetBundleRequest.get_allAssets_Injected(intPtr);
			}
		}

		public AssetBundleRequest()
		{
		}

		private AssetBundleRequest(IntPtr ptr)
			: base(ptr)
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetResult_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object[] get_allAssets_Injected(IntPtr _unity_self);

		internal new static class BindingsMarshaller
		{
			public static AssetBundleRequest ConvertToManaged(IntPtr ptr)
			{
				return new AssetBundleRequest(ptr);
			}

			public static IntPtr ConvertToNative(AssetBundleRequest request)
			{
				return request.m_Ptr;
			}
		}
	}
}
