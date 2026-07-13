using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	internal sealed class NativeHashSetDebuggerTypeProxy<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType, IEquatable<T>
	{
		public NativeHashSetDebuggerTypeProxy(NativeHashSet<T> data)
		{
			this.Data = data.m_Data;
		}

		public unsafe List<T> Items
		{
			get
			{
				if (this.Data == null)
				{
					return null;
				}
				List<T> list = new List<T>();
				using (NativeArray<T> keyArray = this.Data->GetKeyArray(Allocator.Temp))
				{
					for (int i = 0; i < keyArray.Length; i++)
					{
						list.Add(keyArray[i]);
					}
				}
				return list;
			}
		}

		private unsafe HashMapHelper<T>* Data;
	}
}
