using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class GroupCollection : ICollection, IEnumerable
	{
		internal GroupCollection(int n, int gap)
		{
			this.list = new Group[n];
			this.gap = gap;
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

		public Group this[int i]
		{
			get
			{
				if (i >= this.gap)
				{
					Match match = (Match)this.list[0];
					i = ((match != Match.Empty) ? match.Regex.GetGroupIndex(i) : (-1));
				}
				return (i >= 0) ? this.list[i] : Group.Fail;
			}
		}

		internal void SetValue(Group g, int i)
		{
			this.list[i] = g;
		}

		public Group this[string groupName]
		{
			get
			{
				Match match = (Match)this.list[0];
				if (match != Match.Empty)
				{
					int num = match.Regex.GroupNumberFromName(groupName);
					if (num != -1)
					{
						return this[num];
					}
				}
				return Group.Fail;
			}
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

		private Group[] list;

		private int gap;
	}
}
