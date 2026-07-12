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
	public class AssetBundleRequest : ResourceRequest
	{
		[NativeMethod("GetLoadedAsset")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		protected override extern Object GetResult();

		public new Object asset
		{
			get
			{
				return this.GetResult();
			}
		}

		public extern Object[] allAssets
		{
			[NativeMethod("GetAllLoadedAssets")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
