using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Data structure for cache. Please refer to See Also:Caching.AddCache for more information.</para>
	/// </summary>
	[NativeHeader("Runtime/Misc/Cache.h")]
	[StaticAccessor("CacheWrapper", StaticAccessorType.DoubleColon)]
	public struct Cache : IEquatable<Cache>
	{
		internal int handle
		{
			get
			{
				return this.m_Handle;
			}
		}

		public static bool operator ==(Cache lhs, Cache rhs)
		{
			return lhs.handle == rhs.handle;
		}

		public static bool operator !=(Cache lhs, Cache rhs)
		{
			return lhs.handle != rhs.handle;
		}

		public override int GetHashCode()
		{
			return this.m_Handle;
		}

		public override bool Equals(object other)
		{
			return other is Cache && this.Equals((Cache)other);
		}

		public bool Equals(Cache other)
		{
			return this.handle == other.handle;
		}

		/// <summary>
		///   <para>Returns true if the cache is valid.</para>
		/// </summary>
		public bool valid
		{
			get
			{
				return Cache.Cache_IsValid(this.m_Handle);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Cache_IsValid(int handle);

		/// <summary>
		///   <para>Returns true if the cache is ready.</para>
		/// </summary>
		public bool ready
		{
			get
			{
				return Cache.Cache_IsReady(this.m_Handle);
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Cache_IsReady(int handle);

		/// <summary>
		///   <para>Returns true if the cache is readonly.</para>
		/// </summary>
		public bool readOnly
		{
			get
			{
				return Cache.Cache_IsReadonly(this.m_Handle);
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Cache_IsReadonly(int handle);

		/// <summary>
		///   <para>Returns the path of the cache.</para>
		/// </summary>
		public string path
		{
			get
			{
				return Cache.Cache_GetPath(this.m_Handle);
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string Cache_GetPath(int handle);

		/// <summary>
		///   <para>Returns the index of the cache in the cache list.</para>
		/// </summary>
		public int index
		{
			get
			{
				return Cache.Cache_GetIndex(this.m_Handle);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Cache_GetIndex(int handle);

		/// <summary>
		///   <para>Returns the number of currently unused bytes in the cache.</para>
		/// </summary>
		public long spaceFree
		{
			get
			{
				return Cache.Cache_GetSpaceFree(this.m_Handle);
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long Cache_GetSpaceFree(int handle);

		/// <summary>
		///   <para>Allows you to specify the total number of bytes that can be allocated for the cache.</para>
		/// </summary>
		public long maximumAvailableStorageSpace
		{
			get
			{
				return Cache.Cache_GetMaximumDiskSpaceAvailable(this.m_Handle);
			}
			set
			{
				Cache.Cache_SetMaximumDiskSpaceAvailable(this.m_Handle, value);
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long Cache_GetMaximumDiskSpaceAvailable(int handle);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Cache_SetMaximumDiskSpaceAvailable(int handle, long value);

		/// <summary>
		///   <para>Returns the used disk space in bytes.</para>
		/// </summary>
		public long spaceOccupied
		{
			get
			{
				return Cache.Cache_GetCachingDiskSpaceUsed(this.m_Handle);
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long Cache_GetCachingDiskSpaceUsed(int handle);

		/// <summary>
		///   <para>The number of seconds that an AssetBundle may remain unused in the cache before it is automatically deleted.</para>
		/// </summary>
		public int expirationDelay
		{
			get
			{
				return Cache.Cache_GetExpirationDelay(this.m_Handle);
			}
			set
			{
				Cache.Cache_SetExpirationDelay(this.m_Handle, value);
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Cache_GetExpirationDelay(int handle);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Cache_SetExpirationDelay(int handle, int value);

		/// <summary>
		///   <para>Removes all cached content in the cache that has been cached by the current application.</para>
		/// </summary>
		/// <param name="expiration">The number of seconds that AssetBundles may remain unused in the cache.</param>
		/// <returns>
		///   <para>Returns True when cache clearing succeeded.</para>
		/// </returns>
		public bool ClearCache()
		{
			return Cache.Cache_ClearCache(this.m_Handle);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Cache_ClearCache(int handle);

		/// <summary>
		///   <para>Removes all cached content in the cache that has been cached by the current application.</para>
		/// </summary>
		/// <param name="expiration">The number of seconds that AssetBundles may remain unused in the cache.</param>
		/// <returns>
		///   <para>Returns True when cache clearing succeeded.</para>
		/// </returns>
		public bool ClearCache(int expiration)
		{
			return Cache.Cache_ClearCache_Expiration(this.m_Handle, expiration);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Cache_ClearCache_Expiration(int handle, int expiration);

		private int m_Handle;
	}
}
