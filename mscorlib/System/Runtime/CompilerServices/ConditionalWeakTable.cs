using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;

namespace System.Runtime.CompilerServices
{
	public sealed class ConditionalWeakTable<TKey, TValue> where TKey : class where TValue : class
	{
		public ConditionalWeakTable()
		{
			this.data = new Ephemeron[13];
			GC.register_ephemeron_array(this.data);
		}

		~ConditionalWeakTable()
		{
		}

		private void RehashWithoutResize()
		{
			int num = this.data.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.data[i].key == GC.EPHEMERON_TOMBSTONE)
				{
					this.data[i].key = null;
				}
			}
			for (int j = 0; j < num; j++)
			{
				object key = this.data[j].key;
				if (key != null)
				{
					int num2 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num;
					while (this.data[num2].key != null)
					{
						if (this.data[num2].key == key)
						{
							goto IL_0108;
						}
						if (++num2 == num)
						{
							num2 = 0;
						}
					}
					this.data[num2].key = key;
					this.data[num2].value = this.data[j].value;
					this.data[j].key = null;
					this.data[j].value = null;
				}
				IL_0108:;
			}
		}

		private void RecomputeSize()
		{
			this.size = 0;
			for (int i = 0; i < this.data.Length; i++)
			{
				if (this.data[i].key != null)
				{
					this.size++;
				}
			}
		}

		private void Rehash()
		{
			this.RecomputeSize();
			uint prime = (uint)HashHelpers.GetPrime(((int)((float)this.size / 0.7f) << 1) | 1);
			if (prime > (float)this.data.Length * 0.5f && prime < (float)this.data.Length * 1.1f)
			{
				this.RehashWithoutResize();
				return;
			}
			Ephemeron[] array = new Ephemeron[prime];
			GC.register_ephemeron_array(array);
			this.size = 0;
			for (int i = 0; i < this.data.Length; i++)
			{
				object key = this.data[i].key;
				object value = this.data[i].value;
				if (key != null && key != GC.EPHEMERON_TOMBSTONE)
				{
					int num = array.Length;
					int num2 = -1;
					int num4;
					int num3 = (num4 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num);
					do
					{
						object key2 = array[num4].key;
						if (key2 == null || key2 == GC.EPHEMERON_TOMBSTONE)
						{
							goto IL_00D3;
						}
						if (++num4 == num)
						{
							num4 = 0;
						}
					}
					while (num4 != num3);
					IL_00ED:
					array[num2].key = key;
					array[num2].value = value;
					this.size++;
					goto IL_0118;
					IL_00D3:
					num2 = num4;
					goto IL_00ED;
				}
				IL_0118:;
			}
			this.data = array;
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("Null key", "key");
			}
			object @lock = this._lock;
			lock (@lock)
			{
				if ((float)this.size >= (float)this.data.Length * 0.7f)
				{
					this.Rehash();
				}
				int num = this.data.Length;
				int num2 = -1;
				int num4;
				int num3 = (num4 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num);
				for (;;)
				{
					object key2 = this.data[num4].key;
					if (key2 == null)
					{
						break;
					}
					if (key2 == GC.EPHEMERON_TOMBSTONE && num2 == -1)
					{
						num2 = num4;
					}
					else if (key2 == key)
					{
						goto Block_9;
					}
					if (++num4 == num)
					{
						num4 = 0;
					}
					if (num4 == num3)
					{
						goto IL_00C7;
					}
				}
				if (num2 == -1)
				{
					num2 = num4;
					goto IL_00C7;
				}
				goto IL_00C7;
				Block_9:
				throw new ArgumentException("Key already in the list", "key");
				IL_00C7:
				this.data[num2].key = key;
				this.data[num2].value = value;
				this.size++;
			}
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("Null key", "key");
			}
			object @lock = this._lock;
			lock (@lock)
			{
				int num = this.data.Length;
				int num3;
				int num2 = (num3 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num);
				for (;;)
				{
					object key2 = this.data[num3].key;
					if (key2 == key)
					{
						break;
					}
					if (key2 == null)
					{
						goto Block_5;
					}
					if (++num3 == num)
					{
						num3 = 0;
					}
					if (num3 == num2)
					{
						goto Block_7;
					}
				}
				this.data[num3].key = GC.EPHEMERON_TOMBSTONE;
				this.data[num3].value = null;
				this.size--;
				return true;
				Block_5:
				Block_7:;
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("Null key", "key");
			}
			value = default(TValue);
			object @lock = this._lock;
			lock (@lock)
			{
				int num = this.data.Length;
				int num3;
				int num2 = (num3 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num);
				for (;;)
				{
					object key2 = this.data[num3].key;
					if (key2 == key)
					{
						break;
					}
					if (key2 == null)
					{
						goto Block_5;
					}
					if (++num3 == num)
					{
						num3 = 0;
					}
					if (num3 == num2)
					{
						goto Block_7;
					}
				}
				value = (TValue)((object)this.data[num3].value);
				return true;
				Block_5:
				Block_7:;
			}
			return false;
		}

		public TValue GetOrCreateValue(TKey key)
		{
			return this.GetValue(key, (TKey k) => Activator.CreateInstance<TValue>());
		}

		public TValue GetValue(TKey key, ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback)
		{
			if (createValueCallback == null)
			{
				throw new ArgumentNullException("Null create delegate", "createValueCallback");
			}
			object @lock = this._lock;
			TValue tvalue;
			lock (@lock)
			{
				if (this.TryGetValue(key, out tvalue))
				{
					return tvalue;
				}
				tvalue = createValueCallback(key);
				this.Add(key, tvalue);
			}
			return tvalue;
		}

		[SecuritySafeCritical]
		[FriendAccessAllowed]
		internal TKey FindEquivalentKeyUnsafe(TKey key, out TValue value)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				for (int i = 0; i < this.data.Length; i++)
				{
					Ephemeron ephemeron = this.data[i];
					if (object.Equals(ephemeron.key, key))
					{
						value = (TValue)((object)ephemeron.value);
						return (TKey)((object)ephemeron.key);
					}
				}
			}
			value = default(TValue);
			return default(TKey);
		}

		[SecuritySafeCritical]
		internal void Clear()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				for (int i = 0; i < this.data.Length; i++)
				{
					this.data[i].key = GC.EPHEMERON_TOMBSTONE;
					this.data[i].value = null;
				}
				this.size = 0;
			}
		}

		internal ICollection<TKey> Keys
		{
			[SecuritySafeCritical]
			get
			{
				object ephemeron_TOMBSTONE = GC.EPHEMERON_TOMBSTONE;
				List<TKey> list = new List<TKey>(this.data.Length);
				object @lock = this._lock;
				lock (@lock)
				{
					for (int i = 0; i < this.data.Length; i++)
					{
						TKey tkey = (TKey)((object)this.data[i].key);
						if (tkey != null && tkey != ephemeron_TOMBSTONE)
						{
							list.Add(tkey);
						}
					}
				}
				return list;
			}
		}

		internal ICollection<TValue> Values
		{
			[SecuritySafeCritical]
			get
			{
				object ephemeron_TOMBSTONE = GC.EPHEMERON_TOMBSTONE;
				List<TValue> list = new List<TValue>(this.data.Length);
				object @lock = this._lock;
				lock (@lock)
				{
					for (int i = 0; i < this.data.Length; i++)
					{
						Ephemeron ephemeron = this.data[i];
						if (ephemeron.key != null && ephemeron.key != ephemeron_TOMBSTONE)
						{
							list.Add((TValue)((object)ephemeron.value));
						}
					}
				}
				return list;
			}
		}

		private const int INITIAL_SIZE = 13;

		private const float LOAD_FACTOR = 0.7f;

		private const float COMPACT_FACTOR = 0.5f;

		private const float EXPAND_FACTOR = 1.1f;

		private Ephemeron[] data;

		private object _lock = new object();

		private int size;

		public delegate TValue CreateValueCallback(TKey key);
	}
}
