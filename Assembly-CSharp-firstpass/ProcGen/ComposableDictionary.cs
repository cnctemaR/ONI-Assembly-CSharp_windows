using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class ComposableDictionary<Key, Value> : IMerge<ComposableDictionary<Key, Value>>
	{
		public ComposableDictionary()
		{
			this.add = new Dictionary<Key, Value>();
			this.remove = new List<Key>();
		}

		public Dictionary<Key, Value> add { get; private set; }

		public List<Key> remove { get; private set; }

		private void VerifyConsolidated()
		{
			DebugUtil.Assert(this.remove.Count == 0, "needs to be Consolidate()d before being used");
		}

		public Value this[Key key]
		{
			get
			{
				this.VerifyConsolidated();
				return this.add[key];
			}
			set
			{
				this.add[key] = value;
			}
		}

		public ICollection<Key> Keys
		{
			get
			{
				this.VerifyConsolidated();
				return this.add.Keys;
			}
		}

		public ICollection<Value> Values
		{
			get
			{
				this.VerifyConsolidated();
				return this.add.Values;
			}
		}

		public void Add(Key key, Value value)
		{
			this.add.Add(key, value);
		}

		public void Add(KeyValuePair<Key, Value> pair)
		{
			this.Add(pair.Key, pair.Value);
		}

		public bool Remove(Key key)
		{
			this.add.Remove(key);
			return true;
		}

		public void Clear()
		{
			this.add.Clear();
		}

		public bool ContainsKey(Key key)
		{
			this.VerifyConsolidated();
			return this.add.ContainsKey(key);
		}

		public bool TryGetValue(Key key, out Value value)
		{
			this.VerifyConsolidated();
			return this.add.TryGetValue(key, out value);
		}

		public int Count
		{
			get
			{
				this.VerifyConsolidated();
				return this.add.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
		{
			this.VerifyConsolidated();
			return this.add.GetEnumerator();
		}

		public void Merge(ComposableDictionary<Key, Value> other)
		{
			this.VerifyConsolidated();
			foreach (Key key in other.remove)
			{
				this.add.Remove(key);
			}
			foreach (KeyValuePair<Key, Value> keyValuePair in other.add)
			{
				if (this.add.ContainsKey(keyValuePair.Key))
				{
					object[] array = new object[2];
					array[0] = "Overwriting entry {0}";
					int num = 1;
					Key key2 = keyValuePair.Key;
					array[num] = key2.ToString();
					DebugUtil.LogArgs(array);
				}
				this.add.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}
	}
}
