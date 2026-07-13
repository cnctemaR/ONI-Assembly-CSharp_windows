using System;
using System.Diagnostics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Key = {Key}, Value = {Value}")]
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct KeyValue<TKey, TValue> where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		public static KeyValue<TKey, TValue> Null
		{
			get
			{
				return new KeyValue<TKey, TValue>
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
					return UnsafeUtility.ReadArrayElement<TKey>((void*)this.m_Buffer->keys, this.m_Index);
				}
				return default(TKey);
			}
		}

		public unsafe ref TValue Value
		{
			get
			{
				return UnsafeUtility.AsRef<TValue>((void*)(this.m_Buffer->values + UnsafeUtility.SizeOf<TValue>() * this.m_Index));
			}
		}

		public unsafe bool GetKeyValue(out TKey key, out TValue value)
		{
			if (this.m_Index != -1)
			{
				key = UnsafeUtility.ReadArrayElement<TKey>((void*)this.m_Buffer->keys, this.m_Index);
				value = UnsafeUtility.ReadArrayElement<TValue>((void*)this.m_Buffer->values, this.m_Index);
				return true;
			}
			key = default(TKey);
			value = default(TValue);
			return false;
		}

		internal unsafe UnsafeHashMapData* m_Buffer;

		internal int m_Index;

		internal int m_Next;
	}
}
