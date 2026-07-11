using System;
using System.Collections;
using Unity;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class GroupCollection : ICollection, IEnumerable
	{
		internal GroupCollection(Match match, Hashtable caps)
		{
			this._match = match;
			this._captureMap = caps;
		}

		public object SyncRoot
		{
			get
			{
				return this._match;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
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
				return this._match._matchcount.Length;
			}
		}

		public Group this[int groupnum]
		{
			get
			{
				return this.GetGroup(groupnum);
			}
		}

		public Group this[string groupname]
		{
			get
			{
				if (this._match._regex == null)
				{
					return Group._emptygroup;
				}
				return this.GetGroup(this._match._regex.GroupNumberFromName(groupname));
			}
		}

		internal Group GetGroup(int groupnum)
		{
			if (this._captureMap != null)
			{
				object obj = this._captureMap[groupnum];
				if (obj == null)
				{
					return Group._emptygroup;
				}
				return this.GetGroupImpl((int)obj);
			}
			else
			{
				if (groupnum >= this._match._matchcount.Length || groupnum < 0)
				{
					return Group._emptygroup;
				}
				return this.GetGroupImpl(groupnum);
			}
		}

		internal Group GetGroupImpl(int groupnum)
		{
			if (groupnum == 0)
			{
				return this._match;
			}
			if (this._groups == null)
			{
				this._groups = new Group[this._match._matchcount.Length - 1];
				for (int i = 0; i < this._groups.Length; i++)
				{
					string text = this._match._regex.GroupNameFromNumber(i + 1);
					this._groups[i] = new Group(this._match._text, this._match._matches[i + 1], this._match._matchcount[i + 1], text);
				}
			}
			return this._groups[groupnum - 1];
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

		public IEnumerator GetEnumerator()
		{
			return new GroupEnumerator(this);
		}

		internal GroupCollection()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		internal Match _match;

		internal Hashtable _captureMap;

		internal Group[] _groups;
	}
}
