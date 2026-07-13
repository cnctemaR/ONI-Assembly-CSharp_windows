using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal sealed class UnsafeParallelHashSetDebuggerTypeProxy<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType, IEquatable<T>
	{
		public UnsafeParallelHashSetDebuggerTypeProxy(UnsafeParallelHashSet<T> data)
		{
			this.Data = data;
		}

		public List<T> Items
		{
			get
			{
				List<T> list = new List<T>();
				using (NativeArray<T> nativeArray = this.Data.ToNativeArray(Allocator.Temp))
				{
					for (int i = 0; i < nativeArray.Length; i++)
					{
						list.Add(nativeArray[i]);
					}
				}
				return list;
			}
		}

		private UnsafeParallelHashSet<T> Data;
	}
}
