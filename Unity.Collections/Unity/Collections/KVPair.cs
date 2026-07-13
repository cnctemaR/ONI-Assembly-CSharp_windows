using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[DebuggerDisplay("Key = {Key}, Value = {Value}")]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct KVPair<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue> where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType
	{
		public static KVPair<TKey, TValue> Null
		{
			get
			{
				return new KVPair<TKey, TValue>
				{
					m_Index = -1
				};
			}
		}

		public unsafe TKey Key
		{
			get
			{
				if (this.m_Index != -1)
				{
					return this.m_Data->Keys[(IntPtr)this.m_Index * (IntPtr)sizeof(TKey) / (IntPtr)sizeof(TKey)];
				}
				return default(TKey);
			}
		}

		public unsafe ref TValue Value
		{
			get
			{
				return UnsafeUtility.AsRef<TValue>((void*)(this.m_Data->Ptr + sizeof(TValue) * this.m_Index));
			}
		}

		public unsafe bool GetKeyValue(out TKey key, out TValue value)
		{
			if (this.m_Index != -1)
			{
				key = this.m_Data->Keys[(IntPtr)this.m_Index * (IntPtr)sizeof(TKey) / (IntPtr)sizeof(TKey)];
				value = UnsafeUtility.ReadArrayElement<TValue>((void*)this.m_Data->Ptr, this.m_Index);
				return true;
			}
			key = default(TKey);
			value = default(TValue);
			return false;
		}

		internal unsafe HashMapHelper<TKey>* m_Data;

		internal int m_Index;

		internal int m_Next;
	}
}
