using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleManifest.h")]
	public class AssetBundleManifest : Object
	{
		private AssetBundleManifest()
		{
		}

		[NativeMethod("GetAllAssetBundles")]
		public string[] GetAllAssetBundles()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundleManifest>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AssetBundleManifest.GetAllAssetBundles_Injected(intPtr);
		}

		[NativeMethod("GetAllAssetBundlesWithVariant")]
		public string[] GetAllAssetBundlesWithVariant()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundleManifest>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AssetBundleManifest.GetAllAssetBundlesWithVariant_Injected(intPtr);
		}

		[NativeMethod("GetAssetBundleHash")]
		public unsafe Hash128 GetAssetBundleHash(string assetBundleName)
		{
			Hash128 hash2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundleManifest>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = assetBundleName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Hash128 hash;
				AssetBundleManifest.GetAssetBundleHash_Injected(intPtr, ref managedSpanWrapper, out hash);
			}
			finally
			{
				char* ptr = null;
				Hash128 hash;
				hash2 = hash;
			}
			return hash2;
		}

		[NativeMethod("GetDirectDependencies")]
		public unsafe string[] GetDirectDependencies(string assetBundleName)
		{
			string[] directDependencies_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundleManifest>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = assetBundleName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				directDependencies_Injected = AssetBundleManifest.GetDirectDependencies_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return directDependencies_Injected;
		}

		[NativeMethod("GetAllDependencies")]
		public unsafe string[] GetAllDependencies(string assetBundleName)
		{
			string[] allDependencies_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundleManifest>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = assetBundleName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				allDependencies_Injected = AssetBundleManifest.GetAllDependencies_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return allDependencies_Injected;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetAllAssetBundles_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetAllAssetBundlesWithVariant_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAssetBundleHash_Injected(IntPtr _unity_self, ref ManagedSpanWrapper assetBundleName, out Hash128 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetDirectDependencies_Injected(IntPtr _unity_self, ref ManagedSpanWrapper assetBundleName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetAllDependencies_Injected(IntPtr _unity_self, ref ManagedSpanWrapper assetBundleName);
	}
}
