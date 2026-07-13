using System;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct NativeKeyValueArrays<TKey, TValue> : INativeDisposable, IDisposable where TKey : struct where TValue : struct
	{
		public int Length
		{
			get
			{
				return this.Keys.Length;
			}
		}

		public NativeKeyValueArrays(int length, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options)
		{
			this.Keys = CollectionHelper.CreateNativeArray<TKey>(length, allocator, options);
			this.Values = CollectionHelper.CreateNativeArray<TValue>(length, allocator, options);
		}

		public void Dispose()
		{
			this.Keys.Dispose();
			this.Values.Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return this.Keys.Dispose(this.Values.Dispose(inputDeps));
		}

		public NativeArray<TKey> Keys;

		public NativeArray<TValue> Values;
	}
}
