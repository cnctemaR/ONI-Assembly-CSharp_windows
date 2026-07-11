using System;
using System.Threading;

namespace System.Dynamic.Utils
{
	internal sealed class CacheDict<TKey, TValue>
	{
		internal CacheDict(int size)
		{
			int num = CacheDict<TKey, TValue>.AlignSize(size);
			this._mask = num - 1;
			this._entries = new CacheDict<TKey, TValue>.Entry[num];
		}

		private static int AlignSize(int size)
		{
			size--;
			size |= size >> 1;
			size |= size >> 2;
			size |= size >> 4;
			size |= size >> 8;
			size |= size >> 16;
			size++;
			return size;
		}

		internal bool TryGetValue(TKey key, out TValue value)
		{
			int hashCode = key.GetHashCode();
			int num = hashCode & this._mask;
			CacheDict<TKey, TValue>.Entry entry = Volatile.Read<CacheDict<TKey, TValue>.Entry>(ref this._entries[num]);
			if (entry != null && entry._hash == hashCode)
			{
				TKey key2 = entry._key;
				if (key2.Equals(key))
				{
					value = entry._value;
					return true;
				}
			}
			value = default(TValue);
			return false;
		}

		internal void Add(TKey key, TValue value)
		{
			int hashCode = key.GetHashCode();
			int num = hashCode & this._mask;
			CacheDict<TKey, TValue>.Entry entry = Volatile.Read<CacheDict<TKey, TValue>.Entry>(ref this._entries[num]);
			if (entry != null && entry._hash == hashCode)
			{
				TKey key2 = entry._key;
				if (key2.Equals(key))
				{
					return;
				}
			}
			Volatile.Write<CacheDict<TKey, TValue>.Entry>(ref this._entries[num], new CacheDict<TKey, TValue>.Entry(hashCode, key, value));
		}

		internal TValue this[TKey key]
		{
			set
			{
				this.Add(key, value);
			}
		}

		private readonly int _mask;

		private readonly CacheDict<TKey, TValue>.Entry[] _entries;

		private sealed class Entry
		{
			internal Entry(int hash, TKey key, TValue value)
			{
				this._hash = hash;
				this._key = key;
				this._value = value;
			}

			internal readonly int _hash;

			internal readonly TKey _key;

			internal readonly TValue _value;
		}
	}
}
