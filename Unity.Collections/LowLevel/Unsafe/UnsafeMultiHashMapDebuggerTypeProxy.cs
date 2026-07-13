using System;
using System.Collections.Generic;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal sealed class UnsafeMultiHashMapDebuggerTypeProxy<TKey, TValue> where TKey : struct, IEquatable<TKey>, IComparable<TKey> where TValue : struct
	{
		public UnsafeMultiHashMapDebuggerTypeProxy(UnsafeMultiHashMap<TKey, TValue> target)
		{
			this.m_Target = target;
		}

		public static ValueTuple<NativeArray<TKey>, int> GetUniqueKeyArray(ref UnsafeMultiHashMap<TKey, TValue> hashMap, AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<TKey> keyArray = hashMap.GetKeyArray(allocator);
			keyArray.Sort<TKey>();
			int num = keyArray.Unique<TKey>();
			return new ValueTuple<NativeArray<TKey>, int>(keyArray, num);
		}

		public List<ListPair<TKey, List<TValue>>> Items
		{
			get
			{
				List<ListPair<TKey, List<TValue>>> list = new List<ListPair<TKey, List<TValue>>>();
				ValueTuple<NativeArray<TKey>, int> uniqueKeyArray = UnsafeMultiHashMapDebuggerTypeProxy<TKey, TValue>.GetUniqueKeyArray(ref this.m_Target, Allocator.Temp);
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

		private UnsafeMultiHashMap<TKey, TValue> m_Target;
	}
}
