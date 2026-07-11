using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Asynchronous load request from an AssetBundle.</para>
	/// </summary>
	[RequiredByNativeCode]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadAssetOperation.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class AssetBundleRequest : AsyncOperation
	{
		/// <summary>
		///   <para>Asset object being loaded (Read Only).</para>
		/// </summary>
		public extern Object asset
		{
			[NativeMethod("GetLoadedAsset")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Asset objects with sub assets being loaded. (Read Only)</para>
		/// </summary>
		public extern Object[] allAssets
		{
			[NativeMethod("GetAllLoadedAssets")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
