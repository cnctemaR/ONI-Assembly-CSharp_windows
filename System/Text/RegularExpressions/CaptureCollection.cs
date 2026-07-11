using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class CaptureCollection : ICollection, IEnumerable
	{
		internal CaptureCollection(int n)
		{
			this.list = new Capture[n];
		}

		public int Count
		{
			get
			{
				return this.list.Length;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public Capture this[int i]
		{
			get
			{
				if (i < 0 || i >= this.Count)
				{
					throw new ArgumentOutOfRangeException("Index is out of range");
				}
				return this.list[i];
			}
		}

		internal void SetValue(Capture cap, int i)
		{
			this.list[i] = cap;
		}

		public object SyncRoot
		{
			get
			{
				return this.list;
			}
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		private Capture[] list;
	}
}
