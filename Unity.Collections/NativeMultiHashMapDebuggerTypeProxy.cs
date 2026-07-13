using System;
using System.Collections.Generic;

namespace Unity.Collections
{
	internal sealed class NativeMultiHashMapDebuggerTypeProxy<TKey, TValue> where TKey : struct, IEquatable<TKey>, IComparable<TKey> where TValue : struct
	{
		public NativeMultiHashMapDebuggerTypeProxy(NativeMultiHashMap<TKey, TValue> target)
		{
			this.m_Target = target;
		}

		public List<ListPair<TKey, List<TValue>>> Items
		{
			get
			{
				List<ListPair<TKey, List<TValue>>> list = new List<ListPair<TKey, List<TValue>>>();
				ValueTuple<NativeArray<TKey>, int> uniqueKeyArray = this.m_Target.GetUniqueKeyArray<TKey, TValue>(Allocator.Temp);
				using (uniqueKeyArray.Item1)
				{
					for (int i = 0; i < uniqueKeyArray.Item2; i++)
					{
						List<TValue> list2 = new List<TValue>();
						TValue tvalue;
						NativeMultiHashMapIterator<TKey> nativeMultiHashMapIterator;
						if (this.m_Target.TryGetFirstValue(uniqueKeyArray.Item1[i], out tvalue, out nativeMultiHashMapIterator))
						{
							do
							{
								list2.Add(tvalue);
							}
							while (this.m_Target.TryGetNextValue(out tvalue, ref nativeMultiHashMapIterator));
						}
						list.Add(new ListPair<TKey, List<TValue>>(uniqueKeyArray.Item1[i], list2));
					}
				}
				return list;
			}
		}

		private NativeMultiHashMap<TKey, TValue> m_Target;
	}
}
