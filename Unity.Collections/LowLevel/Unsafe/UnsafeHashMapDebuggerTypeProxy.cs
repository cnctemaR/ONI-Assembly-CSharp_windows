using System;
using System.Collections.Generic;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal sealed class UnsafeHashMapDebuggerTypeProxy<TKey, TValue> where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		public UnsafeHashMapDebuggerTypeProxy(UnsafeHashMap<TKey, TValue> target)
		{
			this.m_Target = target;
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
