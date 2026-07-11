using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Collections.ObjectModel
{
	[ComVisible(false)]
	[Serializable]
	public abstract class KeyedCollection<TKey, TItem> : Collection<TItem>
	{
		protected KeyedCollection()
			: this(null, 0)
		{
		}

		protected KeyedCollection(IEqualityComparer<TKey> comparer)
			: this(comparer, 0)
		{
		}

		protected KeyedCollection(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
		{
			if (comparer != null)
			{
				this.comparer = comparer;
			}
			else
			{
				this.comparer = EqualityComparer<TKey>.Default;
			}
			this.dictionaryCreationThreshold = dictionaryCreationThreshold;
			if (dictionaryCreationThreshold == 0)
			{
				this.dictionary = new Dictionary<TKey, TItem>(this.comparer);
			}
		}

		public bool Contains(TKey key)
		{
			if (this.dictionary != null)
			{
				return this.dictionary.ContainsKey(key);
			}
			return this.IndexOfKey(key) >= 0;
		}

		private int IndexOfKey(TKey key)
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				TKey keyForItem = this.GetKeyForItem(this[i]);
				if (this.comparer.Equals(key, keyForItem))
				{
					return i;
				}
			}
			return -1;
		}

		public bool Remove(TKey key)
		{
			if (this.dictionary != null)
			{
				TItem titem;
				return this.dictionary.TryGetValue(key, out titem) && base.Remove(titem);
			}
			int num = this.IndexOfKey(key);
			if (num == -1)
			{
				return false;
			}
			this.RemoveAt(num);
			return true;
		}

		public IEqualityComparer<TKey> Comparer
		{
			get
			{
				return this.comparer;
			}
		}

		public TItem this[TKey key]
		{
			get
			{
				if (this.dictionary != null)
				{
					return this.dictionary[key];
				}
				int num = this.IndexOfKey(key);
				if (num >= 0)
				{
					return base[num];
				}
				throw new KeyNotFoundException();
			}
		}

		protected void ChangeItemKey(TItem item, TKey newKey)
		{
			if (!this.Contains(item))
			{
				throw new ArgumentException();
			}
			TKey keyForItem = this.GetKeyForItem(item);
			if (this.comparer.Equals(keyForItem, newKey))
			{
				return;
			}
			if (this.Contains(newKey))
			{
				throw new ArgumentException();
			}
			if (this.dictionary != null)
			{
				if (!this.dictionary.Remove(keyForItem))
				{
					throw new ArgumentException();
				}
				this.dictionary.Add(newKey, item);
			}
		}

		protected override void ClearItems()
		{
			if (this.dictionary != null)
			{
				this.dictionary.Clear();
			}
			base.ClearItems();
		}

		protected abstract TKey GetKeyForItem(TItem item);

		protected override void InsertItem(int index, TItem item)
		{
			TKey keyForItem = this.GetKeyForItem(item);
			if (keyForItem == null)
			{
				throw new ArgumentNullException("GetKeyForItem(item)");
			}
			if (this.dictionary != null && this.dictionary.ContainsKey(keyForItem))
			{
				throw new ArgumentException("An element with the same key already exists in the dictionary.");
			}
			if (this.dictionary == null)
			{
				for (int i = 0; i < this.Count; i++)
				{
					if (this.comparer.Equals(keyForItem, this.GetKeyForItem(this[i])))
					{
						throw new ArgumentException("An element with the same key already exists in the dictionary.");
					}
				}
			}
			base.InsertItem(index, item);
			if (this.dictionary != null)
			{
				this.dictionary.Add(keyForItem, item);
			}
			else if (this.dictionaryCreationThreshold != -1 && this.Count > this.dictionaryCreationThreshold)
			{
				this.dictionary = new Dictionary<TKey, TItem>(this.comparer);
				for (int j = 0; j < this.Count; j++)
				{
					TItem titem = this[j];
					this.dictionary.Add(this.GetKeyForItem(titem), titem);
				}
			}
		}

		protected override void RemoveItem(int index)
		{
			if (this.dictionary != null)
			{
				TKey keyForItem = this.GetKeyForItem(this[index]);
				this.dictionary.Remove(keyForItem);
			}
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, TItem item)
		{
			if (this.dictionary != null)
			{
				this.dictionary.Remove(this.GetKeyForItem(this[index]));
				this.dictionary.Add(this.GetKeyForItem(item), item);
			}
			base.SetItem(index, item);
		}

		protected IDictionary<TKey, TItem> Dictionary
		{
			get
			{
				return this.dictionary;
			}
		}

		private Dictionary<TKey, TItem> dictionary;

		private IEqualityComparer<TKey> comparer;

		private int dictionaryCreationThreshold;
	}
}
