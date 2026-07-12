using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.LowLevel;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Collections.LowLevel.Unsafe
{
	[StaticAccessor("BurstLike", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Export/BurstLike/BurstLike.bindings.h")]
	internal static class BurstLike
	{
		[ThreadSafe(ThrowsException = false)]
		[BurstAuthorizedExternalMethod]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int NativeFunctionCall_Int_IntPtr_IntPtr(IntPtr function, IntPtr p0, IntPtr p1, out int error);

		internal readonly struct SharedStatic<[IsUnmanaged] T> where T : struct, ValueType
		{
			private unsafe SharedStatic(void* buffer)
			{
				this._buffer = buffer;
			}

			public ref T Data
			{
				get
				{
					return UnsafeUtility.AsRef<T>(this._buffer);
				}
			}

			public unsafe void* UnsafeDataPointer
			{
				get
				{
					return this._buffer;
				}
			}

			public static BurstLike.SharedStatic<T> GetOrCreate<TContext>(uint alignment = 0U)
			{
				return new BurstLike.SharedStatic<T>(BurstLike.SharedStatic.GetOrCreateSharedStaticInternal(BurstRuntime.GetHashCode64<TContext>(), 0L, (uint)UnsafeUtility.SizeOf<T>(), alignment));
			}

			public static BurstLike.SharedStatic<T> GetOrCreate<TContext, TSubContext>(uint alignment = 0U)
			{
				return new BurstLike.SharedStatic<T>(BurstLike.SharedStatic.GetOrCreateSharedStaticInternal(BurstRuntime.GetHashCode64<TContext>(), BurstRuntime.GetHashCode64<TSubContext>(), (uint)UnsafeUtility.SizeOf<T>(), alignment));
			}

			private unsafe readonly void* _buffer;
		}

		internal static class SharedStatic
		{
			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private static void CheckSizeOf(uint sizeOf)
			{
				bool flag = sizeOf == 0U;
				if (flag)
				{
					throw new ArgumentException("sizeOf must be > 0", "sizeOf");
				}
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private unsafe static void CheckResult(void* result)
			{
				bool flag = result == null;
				if (flag)
				{
					throw new InvalidOperationException("Unable to create a SharedStatic for this key. This is most likely due to the size of the struct inside of the SharedStatic having changed or the same key being reused for differently sized values. To fix this the editor needs to be restarted.");
				}
			}

			[RequiredMember]
			public unsafe static void* GetOrCreateSharedStaticInternal(long getHashCode64, long getSubHashCode64, uint sizeOf, uint alignment)
			{
				Hash128 hash = new Hash128((ulong)getHashCode64, (ulong)getSubHashCode64);
				return BurstCompilerService.GetOrCreateSharedMemory(ref hash, sizeOf, (alignment == 0U) ? 4U : alignment);
			}
		}
	}
}
