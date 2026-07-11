using System;

namespace System.Collections.Generic
{
	[Serializable]
	public class SortedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>
	{
		public SortedDictionary()
			: this(null)
		{
		}

		public SortedDictionary(IComparer<TKey> comparer)
		{
			this.hlp = SortedDictionary<TKey, TValue>.NodeHelper.GetHelper(comparer);
			this.tree = new RBTree(this.hlp);
		}

		public SortedDictionary(IDictionary<TKey, TValue> dic)
			: this(dic, null)
		{
		}

		public SortedDictionary(IDictionary<TKey, TValue> dic, IComparer<TKey> comparer)
			: this(comparer)
		{
			if (dic == null)
			{
				throw new ArgumentNullException();
			}
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dic)
			{
				this.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection(this);
			}
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection(this);
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			TValue tvalue;
			return this.TryGetValue(item.Key, out tvalue) && EqualityComparer<TValue>.Default.Equals(item.Value, tvalue);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			TValue tvalue;
			return this.TryGetValue(item.Key, out tvalue) && EqualityComparer<TValue>.Default.Equals(item.Value, tvalue) && this.Remove(item.Key);
		}

		void IDictionary.Add(object key, object value)
		{
			this.Add(this.ToKey(key), this.ToValue(value));
		}

		bool IDictionary.Contains(object key)
		{
			return this.ContainsKey(this.ToKey(key));
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this);
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection(this);
			}
		}

		void IDictionary.Remove(object key)
		{
			this.Remove(this.ToKey(key));
		}

		ICollection IDictionary.Values
		{
			get
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection(this);
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return this[this.ToKey(key)];
			}
			set
			{
				this[this.ToKey(key)] = this.ToValue(value);
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (this.Count == 0)
			{
				return;
			}
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			if (index < 0 || array.Length <= index)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array.Length - index < this.Count)
			{
				throw new ArgumentException();
			}
			foreach (RBTree.Node node in this.tree)
			{
				SortedDictionary<TKey, TValue>.Node node2 = (SortedDictionary<TKey, TValue>.Node)node;
				array.SetValue(node2.AsDE(), index++);
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this);
		}

		public IComparer<TKey> Comparer
		{
			get
			{
				return this.hlp.cmp;
			}
		}

		public int Count
		{
			get
			{
				return this.tree.Count;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				SortedDictionary<TKey, TValue>.Node node = (SortedDictionary<TKey, TValue>.Node)this.tree.Lookup<TKey>(key);
				if (node == null)
				{
					throw new KeyNotFoundException();
				}
				return node.value;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				SortedDictionary<TKey, TValue>.Node node = (SortedDictionary<TKey, TValue>.Node)this.tree.Intern<TKey>(key, null);
				node.value = value;
			}
		}

		public SortedDictionary<TKey, TValue>.KeyCollection Keys
		{
			get
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection(this);
			}
		}

		public SortedDictionary<TKey, TValue>.ValueCollection Values
		{
			get
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection(this);
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			RBTree.Node node = new SortedDictionary<TKey, TValue>.Node(key, value);
			if (this.tree.Intern<TKey>(key, node) != node)
			{
				throw new ArgumentException("key already present in dictionary", "key");
			}
		}

		public void Clear()
		{
			this.tree.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			return this.tree.Lookup<TKey>(key) != null;
		}

		public bool ContainsValue(TValue value)
		{
			IEqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			foreach (RBTree.Node node in this.tree)
			{
				SortedDictionary<TKey, TValue>.Node node2 = (SortedDictionary<TKey, TValue>.Node)node;
				if (@default.Equals(value, node2.value))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (this.Count == 0)
			{
				return;
			}
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			if (arrayIndex < 0 || array.Length <= arrayIndex)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array.Length - arrayIndex < this.Count)
			{
				throw new ArgumentException();
			}
			foreach (RBTree.Node node in this.tree)
			{
				SortedDictionary<TKey, TValue>.Node node2 = (SortedDictionary<TKey, TValue>.Node)node;
				array[arrayIndex++] = node2.AsKV();
			}
		}

		public SortedDictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this);
		}

		public bool Remove(TKey key)
		{
			return this.tree.Remove<TKey>(key) != null;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			SortedDictionary<TKey, TValue>.Node node = (SortedDictionary<TKey, TValue>.Node)this.tree.Lookup<TKey>(key);
			value = ((node != null) ? node.value : default(TValue));
			return node != null;
		}

		private TKey ToKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!(key is TKey))
			{
				throw new ArgumentException(string.Format("Key \"{0}\" cannot be converted to the key type {1}.", key, typeof(TKey)));
			}
			return (TKey)((object)key);
		}

		private TValue ToValue(object value)
		{
			if (!(value is TValue) && (value != null || typeof(TValue).IsValueType))
			{
				throw new ArgumentException(string.Format("Value \"{0}\" cannot be converted to the value type {1}.", value, typeof(TValue)));
			}
			return (TValue)((object)value);
		}

		private RBTree tree;

		private SortedDictionary<TKey, TValue>.NodeHelper hlp;

		private class Node : RBTree.Node
		{
			public Node(TKey key)
			{
				this.key = key;
			}

			public Node(TKey key, TValue value)
			{
				this.key = key;
				this.value = value;
			}

			public override void SwapValue(RBTree.Node other)
			{
				SortedDictionary<TKey, TValue>.Node node = (SortedDictionary<TKey, TValue>.Node)other;
				TKey tkey = this.key;
				this.key = node.key;
				node.key = tkey;
				TValue tvalue = this.value;
				this.value = node.value;
				node.value = tvalue;
			}

			public KeyValuePair<TKey, TValue> AsKV()
			{
				return new KeyValuePair<TKey, TValue>(this.key, this.value);
			}

			public DictionaryEntry AsDE()
			{
				return new DictionaryEntry(this.key, this.value);
			}

			public TKey key;

			public TValue value;
		}

		private class NodeHelper : RBTree.INodeHelper<TKey>
		{
			private NodeHelper(IComparer<TKey> cmp)
			{
				this.cmp = cmp;
			}

			public int Compare(TKey key, RBTree.Node node)
			{
				return this.cmp.Compare(key, ((SortedDictionary<TKey, TValue>.Node)node).key);
			}

			public RBTree.Node CreateNode(TKey key)
			{
				return new SortedDictionary<TKey, TValue>.Node(key);
			}

			public static SortedDictionary<TKey, TValue>.NodeHelper GetHelper(IComparer<TKey> cmp)
			{
				if (cmp == null || cmp == Comparer<TKey>.Default)
				{
					return SortedDictionary<TKey, TValue>.NodeHelper.Default;
				}
				return new SortedDictionary<TKey, TValue>.NodeHelper(cmp);
			}

			public IComparer<TKey> cmp;

			private static SortedDictionary<TKey, TValue>.NodeHelper Default = new SortedDictionary<TKey, TValue>.NodeHelper(Comparer<TKey>.Default);
		}

		[Serializable]
		public sealed class ValueCollection : ICollection, IEnumerable, ICollection<TValue>, IEnumerable<TValue>
		{
			public ValueCollection(SortedDictionary<TKey, TValue> dic)
			{
				this._dic = dic;
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				return this._dic.ContainsValue(item);
			}

			bool ICollection<TValue>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException();
			}

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (this.Count == 0)
				{
					return;
				}
				if (array == null)
				{
					throw new ArgumentNullException();
				}
				if (index < 0 || array.Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (array.Length - index < this.Count)
				{
					throw new ArgumentException();
				}
				foreach (RBTree.Node node in this._dic.tree)
				{
					SortedDictionary<TKey, TValue>.Node node2 = (SortedDictionary<TKey, TValue>.Node)node;
					array.SetValue(node2.value, index++);
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this._dic;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator(this._dic);
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				if (this.Count == 0)
				{
					return;
				}
				if (array == null)
				{
					throw new ArgumentNullException();
				}
				if (arrayIndex < 0 || array.Length <= arrayIndex)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (array.Length - arrayIndex < this.Count)
				{
					throw new ArgumentException();
				}
				foreach (RBTree.Node node in this._dic.tree)
				{
					SortedDictionary<TKey, TValue>.Node node2 = (SortedDictionary<TKey, TValue>.Node)node;
					array[arrayIndex++] = node2.value;
				}
			}

			public int Count
			{
				get
				{
					return this._dic.Count;
				}
			}

			public SortedDictionary<TKey, TValue>.ValueCollection.Enumerator GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator(this._dic);
			}

			private SortedDictionary<TKey, TValue> _dic;

			public struct Enumerator : IEnumerator, IDisposable, IEnumerator<TValue>
			{
				internal Enumerator(SortedDictionary<TKey, TValue> dic)
				{
					this.host = dic.tree.GetEnumerator();
				}

				object IEnumerator.Current
				{
					get
					{
						this.host.check_current();
						return this.current;
					}
				}

				void IEnumerator.Reset()
				{
					this.host.Reset();
				}

				public TValue Current
				{
					get
					{
						return this.current;
					}
				}

				public bool MoveNext()
				{
					if (!this.host.MoveNext())
					{
						return false;
					}
					this.current = ((SortedDictionary<TKey, TValue>.Node)this.host.Current).value;
					return true;
				}

				public void Dispose()
				{
					this.host.Dispose();
				}

				private RBTree.NodeEnumerator host;

				private TValue current;
			}
		}

		[Serializable]
		public sealed class KeyCollection : ICollection, IEnumerable, ICollection<TKey>, IEnumerable<TKey>
		{
			public KeyCollection(SortedDictionary<TKey, TValue> dic)
			{
				this._dic = dic;
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				return this._dic.ContainsKey(item);
			}

			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			bool ICollection<TKey>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (this.Count == 0)
				{
					return;
				}
				if (array == null)
				{
					throw new ArgumentNullException();
				}
				if (index < 0 || array.Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (array.Length - index < this.Count)
				{
					throw new ArgumentException();
				}
				foreach (RBTree.Node node in this._dic.tree)
				{
					SortedDictionary<TKey, TValue>.Node node2 = (SortedDictionary<TKey, TValue>.Node)node;
					array.SetValue(node2.key, index++);
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this._dic;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator(this._dic);
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				if (this.Count == 0)
				{
					return;
				}
				if (array == null)
				{
					throw new ArgumentNullException();
				}
				if (arrayIndex < 0 || array.Length <= arrayIndex)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (array.Length - arrayIndex < this.Count)
				{
					throw new ArgumentException();
				}
				foreach (RBTree.Node node in this._dic.tree)
				{
					SortedDictionary<TKey, TValue>.Node node2 = (SortedDictionary<TKey, TValue>.Node)node;
					array[arrayIndex++] = node2.key;
				}
			}

			public int Count
			{
				get
				{
					return this._dic.Count;
				}
			}

			public SortedDictionary<TKey, TValue>.KeyCollection.Enumerator GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator(this._dic);
			}

			private SortedDictionary<TKey, TValue> _dic;

			public struct Enumerator : IEnumerator, IDisposable, IEnumerator<TKey>
			{
				internal Enumerator(SortedDictionary<TKey, TValue> dic)
				{
					this.host = dic.tree.GetEnumerator();
				}

				object IEnumerator.Current
				{
					get
					{
						this.host.check_current();
						return this.current;
					}
				}

				void IEnumerator.Reset()
				{
					this.host.Reset();
				}

				public TKey Current
				{
					get
					{
						return this.current;
					}
				}

				public bool MoveNext()
				{
					if (!this.host.MoveNext())
					{
						return false;
					}
					this.current = ((SortedDictionary<TKey, TValue>.Node)this.host.Current).key;
					return true;
				}

				public void Dispose()
				{
					this.host.Dispose();
				}

				private RBTree.NodeEnumerator host;

				private TKey current;
			}
		}

		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
		{
			internal Enumerator(SortedDictionary<TKey, TValue> dic)
			{
				this.host = dic.tree.GetEnumerator();
			}

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					return this.CurrentNode.AsDE();
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					return this.CurrentNode.key;
				}
			}

			object IDictionaryEnumerator.Value
			{
				get
				{
					return this.CurrentNode.value;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.CurrentNode.AsDE();
				}
			}

			void IEnumerator.Reset()
			{
				this.host.Reset();
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return this.current;
				}
			}

			public bool MoveNext()
			{
				if (!this.host.MoveNext())
				{
					return false;
				}
				this.current = ((SortedDictionary<TKey, TValue>.Node)this.host.Current).AsKV();
				return true;
			}

			public void Dispose()
			{
				this.host.Dispose();
			}

			private SortedDictionary<TKey, TValue>.Node CurrentNode
			{
				get
				{
					this.host.check_current();
					return (SortedDictionary<TKey, TValue>.Node)this.host.Current;
				}
			}

			private RBTree.NodeEnumerator host;

			private KeyValuePair<TKey, TValue> current;
		}
	}
}
