using System;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct NativeKeyValueArrays<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue> : INativeDisposable, IDisposable where TKey : struct, ValueType where TValue : struct, ValueType
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

		public JobHandle Dispose(JobHandle inputDeps)
		{
			return this.Keys.Dispose(this.Values.Dispose(inputDeps));
		}

		public NativeArray<TKey> Keys;

		public NativeArray<TValue> Values;
	}
}
