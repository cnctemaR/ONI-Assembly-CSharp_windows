using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/ClusterRenderer/ClusterSerialization.h")]
	[Obsolete("This type is deprecated and will be removed in a future release.", false)]
	[ExcludeFromDocs]
	public static class ClusterSerialization
	{
		public static int SaveTimeManagerState(NativeArray<byte> buffer)
		{
			return ClusterSerialization.SaveTimeManagerStateInternal(buffer.GetUnsafePtr<byte>(), buffer.Length);
		}

		public static bool RestoreTimeManagerState(NativeArray<byte> buffer)
		{
			return ClusterSerialization.RestoreTimeManagerStateInternal(buffer.GetUnsafePtr<byte>(), buffer.Length);
		}

		public static int SaveInputManagerState(NativeArray<byte> buffer)
		{
			return ClusterSerialization.SaveInputManagerStateInternal(buffer.GetUnsafePtr<byte>(), buffer.Length);
		}

		public static bool RestoreInputManagerState(NativeArray<byte> buffer)
		{
			return ClusterSerialization.RestoreInputManagerStateInternal(buffer.GetUnsafePtr<byte>(), buffer.Length);
		}

		public static int SaveClusterInputState(NativeArray<byte> buffer)
		{
			return ClusterSerialization.SaveClusterInputStateInternal(buffer.GetUnsafePtr<byte>(), buffer.Length);
		}

		public static bool RestoreClusterInputState(NativeArray<byte> buffer)
		{
			return ClusterSerialization.RestoreClusterInputStateInternal(buffer.GetUnsafePtr<byte>(), buffer.Length);
		}

		[FreeFunction("ClusterSerialization::SaveTimeManagerState")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int SaveTimeManagerStateInternal(void* intBuffer, int bufferSize);

		[FreeFunction("ClusterSerialization::RestoreTimeManagerState")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool RestoreTimeManagerStateInternal(void* buffer, int bufferSize);

		[FreeFunction("ClusterSerialization::SaveInputManagerState")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int SaveInputManagerStateInternal(void* intBuffer, int bufferSize);

		[FreeFunction("ClusterSerialization::RestoreInputManagerState")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool RestoreInputManagerStateInternal(void* buffer, int bufferSize);

		[FreeFunction("ClusterSerialization::SaveClusterInputState")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int SaveClusterInputStateInternal(void* intBuffer, int bufferSize);

		[FreeFunction("ClusterSerialization::RestoreClusterInputState")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool RestoreClusterInputStateInternal(void* buffer, int bufferSize);
	}
}
