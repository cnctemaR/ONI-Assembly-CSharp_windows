using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	internal sealed class NativeHashMapDebuggerTypeProxy<TKey, TValue> where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		public NativeHashMapDebuggerTypeProxy(NativeHashMap<TKey, TValue> target)
		{
			this.m_Target = target.m_HashMapData;
		}

		public List<Pair<TKey, TValue>> Items
		{
			get
			{
				List<Pair<TKey, TValue>> list = new List<Pair<TKey, TValue>>();
				using (NativeKeyValueArrays<TKey, TValue> keyValueArrays = this.m_Target.GetKeyValueArrays(Allocator.Temp))
				{
					for (int i = 0; i < keyValueArrays.Length; i++)
					{
						List<Pair<TKey, TValue>> list2 = list;
						NativeArray<TKey> keys = keyValueArrays.Keys;
						TKey tkey = keys[i];
						NativeArray<TValue> values = keyValueArrays.Values;
						list2.Add(new Pair<TKey, TValue>(tkey, values[i]));
					}
				}
				return list;
			}
		}

		private UnsafeHashMap<TKey, TValue> m_Target;
	}
}
