using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal struct UnsafeHashMapDataEnumerator
	{
		internal unsafe UnsafeHashMapDataEnumerator(UnsafeHashMapData* data)
		{
			this.m_Buffer = data;
			this.m_Index = -1;
			this.m_BucketIndex = 0;
			this.m_NextIndex = -1;
		}

		internal bool MoveNext()
		{
			return UnsafeHashMapData.MoveNext(this.m_Buffer, ref this.m_BucketIndex, ref this.m_NextIndex, out this.m_Index);
		}

		internal void Reset()
		{
			this.m_Index = -1;
			this.m_BucketIndex = 0;
			this.m_NextIndex = -1;
		}

		internal KeyValue<TKey, TValue> GetCurrent<TKey, TValue>() where TKey : struct, IEquatable<TKey> where TValue : struct
		{
			return new KeyValue<TKey, TValue>
			{
				m_Buffer = this.m_Buffer,
				m_Index = this.m_Index
			};
		}

		internal unsafe TKey GetCurrentKey<TKey>() where TKey : struct, IEquatable<TKey>
		{
			if (this.m_Index != -1)
			{
				return UnsafeUtility.ReadArrayElement<TKey>((void*)this.m_Buffer->keys, this.m_Index);
			}
			return default(TKey);
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeHashMapData* m_Buffer;

		internal int m_Index;

		internal int m_BucketIndex;

		internal int m_NextIndex;
	}
}
