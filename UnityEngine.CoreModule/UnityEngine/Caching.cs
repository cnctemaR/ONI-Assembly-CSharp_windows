using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[StaticAccessor("GetCachingManager()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/Misc/CachingManager.h")]
	public sealed class Caching
	{
		public static extern bool compressionEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool ready
		{
			[NativeName("GetIsReady")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ClearCache();

		public static bool ClearCache(int expiration)
		{
			return Caching.ClearCache_Int(expiration);
		}

		[NativeName("ClearCache")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool ClearCache_Int(int expiration);

		public static bool ClearCachedVersion(string assetBundleName, Hash128 hash)
		{
			bool flag = string.IsNullOrEmpty(assetBundleName);
			if (flag)
			{
				throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
			}
			return Caching.ClearCachedVersionInternal(assetBundleName, hash);
		}

		[NativeName("ClearCachedVersion")]
		internal unsafe static bool ClearCachedVersionInternal(string assetBundleName, Hash128 hash)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = assetBundleName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = Caching.ClearCachedVersionInternal_Injected(ref managedSpanWrapper, ref hash);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public static bool ClearOtherCachedVersions(string assetBundleName, Hash128 hash)
		{
			bool flag = string.IsNullOrEmpty(assetBundleName);
			if (flag)
			{
				throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
			}
			return Caching.ClearCachedVersions(assetBundleName, hash, true);
		}

		public static bool ClearAllCachedVersions(string assetBundleName)
		{
			bool flag = string.IsNullOrEmpty(assetBundleName);
			if (flag)
			{
				throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
			}
			return Caching.ClearCachedVersions(assetBundleName, default(Hash128), false);
		}

		internal unsafe static bool ClearCachedVersions(string assetBundleName, Hash128 hash, bool keepInputVersion)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = assetBundleName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = Caching.ClearCachedVersions_Injected(ref managedSpanWrapper, ref hash, keepInputVersion);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static Hash128[] GetCachedVersions(string assetBundleName)
		{
			Hash128[] array2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = assetBundleName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Caching.GetCachedVersions_Injected(ref managedSpanWrapper, out blittableArrayWrapper);
			}
			finally
			{
				char* ptr = null;
				BlittableArrayWrapper blittableArrayWrapper;
				Hash128[] array;
				blittableArrayWrapper.Unmarshal<Hash128>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static void GetCachedVersions(string assetBundleName, List<Hash128> outCachedVersions)
		{
			bool flag = string.IsNullOrEmpty(assetBundleName);
			if (flag)
			{
				throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
			}
			bool flag2 = outCachedVersions == null;
			if (flag2)
			{
				throw new ArgumentNullException("Input outCachedVersions cannot be null.");
			}
			outCachedVersions.AddRange(Caching.GetCachedVersions(assetBundleName));
		}

		[Obsolete("Please use IsVersionCached with Hash128 instead.")]
		public static bool IsVersionCached(string url, int version)
		{
			return Caching.IsVersionCached(url, new Hash128(0U, 0U, 0U, (uint)version));
		}

		public static bool IsVersionCached(string url, Hash128 hash)
		{
			bool flag = string.IsNullOrEmpty(url);
			if (flag)
			{
				throw new ArgumentException("Input AssetBundle url cannot be null or empty.");
			}
			return Caching.IsVersionCached(url, "", hash);
		}

		public static bool IsVersionCached(CachedAssetBundle cachedBundle)
		{
			bool flag = string.IsNullOrEmpty(cachedBundle.name);
			if (flag)
			{
				throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
			}
			return Caching.IsVersionCached("", cachedBundle.name, cachedBundle.hash);
		}

		[NativeName("IsCached")]
		internal unsafe static bool IsVersionCached(string url, string assetBundleName, Hash128 hash)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(url, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = url.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = assetBundleName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				flag = Caching.IsVersionCached_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, ref hash);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return flag;
		}

		[Obsolete("Please use MarkAsUsed with Hash128 instead.")]
		public static bool MarkAsUsed(string url, int version)
		{
			return Caching.MarkAsUsed(url, new Hash128(0U, 0U, 0U, (uint)version));
		}

		public static bool MarkAsUsed(string url, Hash128 hash)
		{
			bool flag = string.IsNullOrEmpty(url);
			if (flag)
			{
				throw new ArgumentException("Input AssetBundle url cannot be null or empty.");
			}
			return Caching.MarkAsUsed(url, "", hash);
		}

		public static bool MarkAsUsed(CachedAssetBundle cachedBundle)
		{
			bool flag = string.IsNullOrEmpty(cachedBundle.name);
			if (flag)
			{
				throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
			}
			return Caching.MarkAsUsed("", cachedBundle.name, cachedBundle.hash);
		}

		internal unsafe static bool MarkAsUsed(string url, string assetBundleName, Hash128 hash)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(url, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = url.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = assetBundleName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				flag = Caching.MarkAsUsed_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, ref hash);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return flag;
		}

		[Obsolete("This function is obsolete and will always return -1. Use IsVersionCached instead.")]
		public static int GetVersionFromCache(string url)
		{
			return -1;
		}

		[Obsolete("Please use use Cache.spaceOccupied to get used bytes per cache.")]
		public static int spaceUsed
		{
			get
			{
				return (int)Caching.spaceOccupied;
			}
		}

		[Obsolete("This property is only used for the current cache, use Cache.spaceOccupied to get used bytes per cache.")]
		public static extern long spaceOccupied
		{
			[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
			[NativeName("GetCachingDiskSpaceUsed")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Please use use Cache.spaceOccupied to get used bytes per cache.")]
		public static int spaceAvailable
		{
			get
			{
				return (int)Caching.spaceFree;
			}
		}

		[Obsolete("This property is only used for the current cache, use Cache.spaceFree to get unused bytes per cache.")]
		public static extern long spaceFree
		{
			[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
			[NativeName("GetCachingDiskSpaceFree")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("This property is only used for the current cache, use Cache.maximumAvailableStorageSpace to access the maximum available storage space per cache.")]
		[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
		public static extern long maximumAvailableDiskSpace
		{
			[NativeName("GetMaximumDiskSpaceAvailable")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetMaximumDiskSpaceAvailable")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
		[Obsolete("This property is only used for the current cache, use Cache.expirationDelay to access the expiration delay per cache.")]
		public static extern int expirationDelay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Cache AddCache(string cachePath)
		{
			bool flag = string.IsNullOrEmpty(cachePath);
			if (flag)
			{
				throw new ArgumentNullException("Cache path cannot be null or empty.");
			}
			bool flag2 = false;
			bool flag3 = cachePath.Replace('\\', '/').StartsWith(Application.streamingAssetsPath);
			if (flag3)
			{
				flag2 = true;
			}
			else
			{
				bool flag4 = !Directory.Exists(cachePath);
				if (flag4)
				{
					throw new ArgumentException("Cache path '" + cachePath + "' doesn't exist.");
				}
				bool flag5 = (File.GetAttributes(cachePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
				if (flag5)
				{
					flag2 = true;
				}
			}
			bool valid = Caching.GetCacheByPath(cachePath).valid;
			if (valid)
			{
				throw new InvalidOperationException("Cache with path '" + cachePath + "' has already been added.");
			}
			return Caching.AddCache(cachePath, flag2);
		}

		[NativeName("AddCachePath")]
		internal unsafe static Cache AddCache(string cachePath, bool isReadonly)
		{
			Cache cache2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(cachePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = cachePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Cache cache;
				Caching.AddCache_Injected(ref managedSpanWrapper, isReadonly, out cache);
			}
			finally
			{
				char* ptr = null;
				Cache cache;
				cache2 = cache;
			}
			return cache2;
		}

		[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
		[NativeName("Caching_GetCacheHandleAt")]
		[NativeThrows]
		public static Cache GetCacheAt(int cacheIndex)
		{
			Cache cache;
			Caching.GetCacheAt_Injected(cacheIndex, out cache);
			return cache;
		}

		[NativeName("Caching_GetCacheHandleByPath")]
		[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		public unsafe static Cache GetCacheByPath(string cachePath)
		{
			Cache cache2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(cachePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = cachePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Cache cache;
				Caching.GetCacheByPath_Injected(ref managedSpanWrapper, out cache);
			}
			finally
			{
				char* ptr = null;
				Cache cache;
				cache2 = cache;
			}
			return cache2;
		}

		public static void GetAllCachePaths(List<string> cachePaths)
		{
			cachePaths.Clear();
			for (int i = 0; i < Caching.cacheCount; i++)
			{
				cachePaths.Add(Caching.GetCacheAt(i).path);
			}
		}

		[NativeThrows]
		[NativeName("Caching_RemoveCacheByHandle")]
		[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
		public static bool RemoveCache(Cache cache)
		{
			return Caching.RemoveCache_Injected(ref cache);
		}

		[NativeThrows]
		[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
		[NativeName("Caching_MoveCacheBeforeByHandle")]
		public static void MoveCacheBefore(Cache src, Cache dst)
		{
			Caching.MoveCacheBefore_Injected(ref src, ref dst);
		}

		[NativeThrows]
		[NativeName("Caching_MoveCacheAfterByHandle")]
		[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
		public static void MoveCacheAfter(Cache src, Cache dst)
		{
			Caching.MoveCacheAfter_Injected(ref src, ref dst);
		}

		public static extern int cacheCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
		public static Cache defaultCache
		{
			[NativeName("Caching_GetDefaultCacheHandle")]
			get
			{
				Cache cache;
				Caching.get_defaultCache_Injected(out cache);
				return cache;
			}
		}

		[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
		public static Cache currentCacheForWriting
		{
			[NativeName("Caching_GetCurrentCacheHandle")]
			get
			{
				Cache cache;
				Caching.get_currentCacheForWriting_Injected(out cache);
				return cache;
			}
			[NativeName("Caching_SetCurrentCacheByHandle")]
			[NativeThrows]
			set
			{
				Caching.set_currentCacheForWriting_Injected(ref value);
			}
		}

		[Obsolete("This function is obsolete. Please use ClearCache.  (UnityUpgradable) -> ClearCache()")]
		public static bool CleanCache()
		{
			return Caching.ClearCache();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ClearCachedVersionInternal_Injected(ref ManagedSpanWrapper assetBundleName, [In] ref Hash128 hash);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ClearCachedVersions_Injected(ref ManagedSpanWrapper assetBundleName, [In] ref Hash128 hash, bool keepInputVersion);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCachedVersions_Injected(ref ManagedSpanWrapper assetBundleName, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsVersionCached_Injected(ref ManagedSpanWrapper url, ref ManagedSpanWrapper assetBundleName, [In] ref Hash128 hash);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool MarkAsUsed_Injected(ref ManagedSpanWrapper url, ref ManagedSpanWrapper assetBundleName, [In] ref Hash128 hash);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddCache_Injected(ref ManagedSpanWrapper cachePath, bool isReadonly, out Cache ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCacheAt_Injected(int cacheIndex, out Cache ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCacheByPath_Injected(ref ManagedSpanWrapper cachePath, out Cache ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveCache_Injected([In] ref Cache cache);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveCacheBefore_Injected([In] ref Cache src, [In] ref Cache dst);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveCacheAfter_Injected([In] ref Cache src, [In] ref Cache dst);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_defaultCache_Injected(out Cache ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_currentCacheForWriting_Injected(out Cache ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_currentCacheForWriting_Injected([In] ref Cache value);
	}
}
