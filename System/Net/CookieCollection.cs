using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace System.Net
{
	[Serializable]
	public class CookieCollection : ICollection, IEnumerable
	{
		internal IList<Cookie> List
		{
			get
			{
				return this.list;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
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
				return this;
			}
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)this.list).CopyTo(array, index);
		}

		public void CopyTo(Cookie[] array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public void Add(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			int num = this.SearchCookie(cookie);
			if (num == -1)
			{
				this.list.Add(cookie);
			}
			else
			{
				this.list[num] = cookie;
			}
		}

		internal void Sort()
		{
			if (this.list.Count > 0)
			{
				this.list.Sort(CookieCollection.Comparer);
			}
		}

		private int SearchCookie(Cookie cookie)
		{
			string name = cookie.Name;
			string domain = cookie.Domain;
			string path = cookie.Path;
			for (int i = this.list.Count - 1; i >= 0; i--)
			{
				Cookie cookie2 = this.list[i];
				if (cookie2.Version == cookie.Version)
				{
					if (string.Compare(domain, cookie2.Domain, true, CultureInfo.InvariantCulture) == 0)
					{
						if (string.Compare(name, cookie2.Name, true, CultureInfo.InvariantCulture) == 0)
						{
							if (string.Compare(path, cookie2.Path, true, CultureInfo.InvariantCulture) == 0)
							{
								return i;
							}
						}
					}
				}
			}
			return -1;
		}

		public void Add(CookieCollection cookies)
		{
			if (cookies == null)
			{
				throw new ArgumentNullException("cookies");
			}
			foreach (object obj in cookies)
			{
				Cookie cookie = (Cookie)obj;
				this.Add(cookie);
			}
		}

		public Cookie this[int index]
		{
			get
			{
				if (index < 0 || index >= this.list.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.list[index];
			}
		}

		public Cookie this[string name]
		{
			get
			{
				foreach (Cookie cookie in this.list)
				{
					if (string.Compare(cookie.Name, name, true, CultureInfo.InvariantCulture) == 0)
					{
						return cookie;
					}
				}
				return null;
			}
		}

		private List<Cookie> list = new List<Cookie>();

		private static CookieCollection.CookieCollectionComparer Comparer = new CookieCollection.CookieCollectionComparer();

		private sealed class CookieCollectionComparer : IComparer<Cookie>
		{
			public int Compare(Cookie x, Cookie y)
			{
				if (x == null || y == null)
				{
					return 0;
				}
				int num = x.Name.Length + x.Value.Length;
				int num2 = y.Name.Length + y.Value.Length;
				return num - num2;
			}
		}
	}
}
