using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[Serializable]
	public abstract class DictionaryBase : IEnumerable, ICollection, IDictionary
	{
		protected DictionaryBase()
		{
			this.hashtable = new Hashtable();
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

		object IDictionary.this[object key]
		{
			get
			{
				object obj = this.hashtable[key];
				this.OnGet(key, obj);
				return obj;
			}
			set
			{
				this.OnValidate(key, value);
				object obj = this.hashtable[key];
				this.OnSet(key, obj, value);
				this.hashtable[key] = value;
				try
				{
					this.OnSetComplete(key, obj, value);
				}
				catch
				{
					this.hashtable[key] = obj;
					throw;
				}
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return this.hashtable.Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return this.hashtable.Values;
			}
		}

		void IDictionary.Add(object key, object value)
		{
			this.OnValidate(key, value);
			this.OnInsert(key, value);
			this.hashtable.Add(key, value);
			try
			{
				this.OnInsertComplete(key, value);
			}
			catch
			{
				this.hashtable.Remove(key);
				throw;
			}
		}

		void IDictionary.Remove(object key)
		{
			if (!this.hashtable.Contains(key))
			{
				return;
			}
			object obj = this.hashtable[key];
			this.OnValidate(key, obj);
			this.OnRemove(key, obj);
			this.hashtable.Remove(key);
			try
			{
				this.OnRemoveComplete(key, obj);
			}
			catch
			{
				this.hashtable[key] = obj;
				throw;
			}
		}

		bool IDictionary.Contains(object key)
		{
			return this.hashtable.Contains(key);
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.hashtable.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.hashtable.SyncRoot;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.hashtable.GetEnumerator();
		}

		public void Clear()
		{
			this.OnClear();
			this.hashtable.Clear();
			this.OnClearComplete();
		}

		public int Count
		{
			get
			{
				return this.hashtable.Count;
			}
		}

		protected IDictionary Dictionary
		{
			get
			{
				return this;
			}
		}

		protected Hashtable InnerHashtable
		{
			get
			{
				return this.hashtable;
			}
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index must be possitive");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("array is multidimensional");
			}
			int length = array.Length;
			if (index > length)
			{
				throw new ArgumentException("index is larger than array size");
			}
			if (index + this.Count > length)
			{
				throw new ArgumentException("Copy will overlflow array");
			}
			this.DoCopy(array, index);
		}

		private void DoCopy(Array array, int index)
		{
			foreach (object obj in this.hashtable)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				array.SetValue(dictionaryEntry, index++);
			}
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return this.hashtable.GetEnumerator();
		}

		protected virtual void OnClear()
		{
		}

		protected virtual void OnClearComplete()
		{
		}

		protected virtual object OnGet(object key, object currentValue)
		{
			return currentValue;
		}

		protected virtual void OnInsert(object key, object value)
		{
		}

		protected virtual void OnInsertComplete(object key, object value)
		{
		}

		protected virtual void OnSet(object key, object oldValue, object newValue)
		{
		}

		protected virtual void OnSetComplete(object key, object oldValue, object newValue)
		{
		}

		protected virtual void OnRemove(object key, object value)
		{
		}

		protected virtual void OnRemoveComplete(object key, object value)
		{
		}

		protected virtual void OnValidate(object key, object value)
		{
		}

		private Hashtable hashtable;
	}
}
