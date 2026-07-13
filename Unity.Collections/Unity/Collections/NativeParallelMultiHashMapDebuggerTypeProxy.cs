using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	internal sealed class NativeParallelMultiHashMapDebuggerTypeProxy<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue> where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType
	{
		public NativeParallelMultiHashMapDebuggerTypeProxy(NativeParallelMultiHashMap<TKey, TValue> target)
		{
			this.m_Target = target;
		}

		public List<ListPair<TKey, List<TValue>>> Items
		{
			get
			{
				List<ListPair<TKey, List<TValue>>> list = new List<ListPair<TKey, List<TValue>>>();
				ValueTuple<NativeArray<TKey>, int> valueTuple = default(ValueTuple<NativeArray<TKey>, int>);
				using (NativeParallelHashMap<TKey, TValue> nativeParallelHashMap = new NativeParallelHashMap<TKey, TValue>(this.m_Target.Count(), Allocator.Temp))
				{
					foreach (KeyValue<TKey, TValue> keyValue in this.m_Target)
					{
						nativeParallelHashMap.TryAdd(keyValue.Key, default(TValue));
					}
					valueTuple.Item1 = nativeParallelHashMap.GetKeyArray(Allocator.Temp);
					valueTuple.Item2 = valueTuple.Item1.Length;
				}
				using (valueTuple.Item1)
				{
					for (int i = 0; i < valueTuple.Item2; i++)
					{
						List<TValue> list2 = new List<TValue>();
						TValue tvalue;
						NativeParallelMultiHashMapIterator<TKey> nativeParallelMultiHashMapIterator;
						if (this.m_Target.TryGetFirstValue(valueTuple.Item1[i], out tvalue, out nativeParallelMultiHashMapIterator))
						{
							do
							{
								list2.Add(tvalue);
							}
							while (this.m_Target.TryGetNextValue(out tvalue, ref nativeParallelMultiHashMapIterator));
						}
						list.Add(new ListPair<TKey, List<TValue>>(valueTuple.Item1[i], list2));
					}
				}
				return list;
			}
		}

		private NativeParallelMultiHashMap<TKey, TValue> m_Target;
	}
}
