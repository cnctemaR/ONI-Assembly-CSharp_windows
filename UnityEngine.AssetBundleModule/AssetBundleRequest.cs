using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadAssetOperation.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class AssetBundleRequest : AsyncOperation
	{
		public extern Object asset
		{
			[NativeMethod("GetLoadedAsset")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Object[] allAssets
		{
			[NativeMethod("GetAllLoadedAssets")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
