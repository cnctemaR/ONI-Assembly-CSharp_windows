using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal sealed class UnsafeHashSetDebuggerTypeProxy<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType, IEquatable<T>
	{
		public UnsafeHashSetDebuggerTypeProxy(UnsafeHashSet<T> data)
		{
			this.Data = data.m_Data;
		}

		public List<T> Items
		{
			get
			{
				List<T> list = new List<T>();
				using (NativeArray<T> keyArray = this.Data.GetKeyArray(Allocator.Temp))
				{
					for (int i = 0; i < keyArray.Length; i++)
					{
						list.Add(keyArray[i]);
					}
				}
				return list;
			}
		}

		private HashMapHelper<T> Data;
	}
}
