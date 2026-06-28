using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class AssetBundleManifest : Object
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllAssetBundles();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllAssetBundlesWithVariant();

		public Hash128 GetAssetBundleHash(string assetBundleName)
		{
			Hash128 hash;
			AssetBundleManifest.INTERNAL_CALL_GetAssetBundleHash(this, assetBundleName, out hash);
			return hash;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetAssetBundleHash(AssetBundleManifest self, string assetBundleName, out Hash128 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetDirectDependencies(string assetBundleName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllDependencies(string assetBundleName);
	}
}
