using System;
using System.Collections;

namespace System.Net
{
	[Serializable]
	internal class PathList
	{
		public int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		public int GetCookiesCount()
		{
			int num = 0;
			object syncRoot = this.SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in this.m_list.Values)
				{
					CookieCollection cookieCollection = (CookieCollection)obj;
					num += cookieCollection.Count;
				}
			}
			return num;
		}

		public ICollection Values
		{
			get
			{
				return this.m_list.Values;
			}
		}

		public object this[string s]
		{
			get
			{
				return this.m_list[s];
			}
			set
			{
				object syncRoot = this.SyncRoot;
				lock (syncRoot)
				{
					this.m_list[s] = value;
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_list.GetEnumerator();
		}

		public object SyncRoot
		{
			get
			{
				return this.m_list.SyncRoot;
			}
		}

		private SortedList m_list = SortedList.Synchronized(new SortedList(PathList.PathListComparer.StaticInstance));

		[Serializable]
		private class PathListComparer : IComparer
		{
			int IComparer.Compare(object ol, object or)
			{
				string text = CookieParser.CheckQuoted((string)ol);
				string text2 = CookieParser.CheckQuoted((string)or);
				int length = text.Length;
				int length2 = text2.Length;
				int num = Math.Min(length, length2);
				for (int i = 0; i < num; i++)
				{
					if (text[i] != text2[i])
					{
						return (int)(text[i] - text2[i]);
					}
				}
				return length2 - length;
			}

			internal static readonly PathList.PathListComparer StaticInstance = new PathList.PathListComparer();
		}
	}
}
