using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Data
{
	public class InternalDataCollectionBase : ICollection, IEnumerable
	{
		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				return this.List.Count;
			}
		}

		public virtual void CopyTo(Array ar, int index)
		{
			this.List.CopyTo(ar, index);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this.List.GetEnumerator();
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		[Browsable(false)]
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		internal int NamesEqual(string s1, string s2, bool fCaseSensitive, CultureInfo locale)
		{
			if (fCaseSensitive)
			{
				if (string.Compare(s1, s2, false, locale) != 0)
				{
					return 0;
				}
				return 1;
			}
			else
			{
				if (locale.CompareInfo.Compare(s1, s2, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) != 0)
				{
					return 0;
				}
				if (string.Compare(s1, s2, false, locale) != 0)
				{
					return -1;
				}
				return 1;
			}
		}

		[Browsable(false)]
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		protected virtual ArrayList List
		{
			get
			{
				return null;
			}
		}

		internal static readonly CollectionChangeEventArgs s_refreshEventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null);
	}
}
