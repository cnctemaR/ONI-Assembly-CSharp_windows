using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity;

namespace System.Text.RegularExpressions
{
	[DebuggerTypeProxy(typeof(CollectionDebuggerProxy<Capture>))]
	[DebuggerDisplay("Count = {Count}")]
	public class CaptureCollection : IList<Capture>, ICollection<Capture>, IEnumerable<Capture>, IEnumerable, IReadOnlyList<Capture>, IReadOnlyCollection<Capture>, IList, ICollection
	{
		internal CaptureCollection(Group group)
		{
			this._group = group;
			this._capcount = this._group._capcount;
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public int Count
		{
			get
			{
				return this._capcount;
			}
		}

		public Capture this[int i]
		{
			get
			{
				return this.GetCapture(i);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return new CaptureCollection.Enumerator(this);
		}

		IEnumerator<Capture> IEnumerable<Capture>.GetEnumerator()
		{
			return new CaptureCollection.Enumerator(this);
		}

		private Capture GetCapture(int i)
		{
			if (i == this._capcount - 1 && i >= 0)
			{
				return this._group;
			}
			if (i >= this._capcount || i < 0)
			{
				throw new ArgumentOutOfRangeException("i");
			}
			if (this._captures == null)
			{
				this.ForceInitialized();
			}
			return this._captures[i];
		}

		internal void ForceInitialized()
		{
			this._captures = new Capture[this._capcount];
			for (int i = 0; i < this._capcount - 1; i++)
			{
				this._captures[i] = new Capture(this._group.Text, this._group._caps[i * 2], this._group._caps[i * 2 + 1]);
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._group;
			}
		}

		public void CopyTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int num = arrayIndex;
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], num);
				num++;
			}
		}

		public void CopyTo(Capture[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (array.Length - arrayIndex < this.Count)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}
			int num = arrayIndex;
			for (int i = 0; i < this.Count; i++)
			{
				array[num] = this[i];
				num++;
			}
		}

		int IList<Capture>.IndexOf(Capture item)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (EqualityComparer<Capture>.Default.Equals(this[i], item))
				{
					return i;
				}
			}
			return -1;
		}

		void IList<Capture>.Insert(int index, Capture item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList<Capture>.RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		Capture IList<Capture>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("Collection is read-only.");
			}
		}

		void ICollection<Capture>.Add(Capture item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void ICollection<Capture>.Clear()
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		bool ICollection<Capture>.Contains(Capture item)
		{
			return ((IList<Capture>)this).IndexOf(item) >= 0;
		}

		bool ICollection<Capture>.Remove(Capture item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList.Clear()
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		bool IList.Contains(object value)
		{
			return value is Capture && ((ICollection<Capture>)this).Contains((Capture)value);
		}

		int IList.IndexOf(object value)
		{
			if (!(value is Capture))
			{
				return -1;
			}
			return ((IList<Capture>)this).IndexOf((Capture)value);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("Collection is read-only.");
			}
		}

		internal CaptureCollection()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly Group _group;

		private readonly int _capcount;

		private Capture[] _captures;

		[Serializable]
		private sealed class Enumerator : IEnumerator<Capture>, IDisposable, IEnumerator
		{
			internal Enumerator(CaptureCollection collection)
			{
				this._collection = collection;
				this._index = -1;
			}

			public bool MoveNext()
			{
				int count = this._collection.Count;
				if (this._index >= count)
				{
					return false;
				}
				this._index++;
				return this._index < count;
			}

			public Capture Current
			{
				get
				{
					if (this._index < 0 || this._index >= this._collection.Count)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._collection[this._index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				this._index = -1;
			}

			void IDisposable.Dispose()
			{
			}

			private readonly CaptureCollection _collection;

			private int _index;
		}
	}
}
