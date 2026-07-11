using System;
using System.Collections;
using System.Runtime.Serialization;

namespace System.Net
{
	[Serializable]
	public class CookieCollection : ICollection, IEnumerable
	{
		public CookieCollection()
		{
			this.m_IsReadOnly = true;
		}

		internal CookieCollection(bool IsReadOnly)
		{
			this.m_IsReadOnly = IsReadOnly;
		}

		public bool IsReadOnly
		{
			get
			{
				return this.m_IsReadOnly;
			}
		}

		public Cookie this[int index]
		{
			get
			{
				if (index < 0 || index >= this.m_list.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (Cookie)this.m_list[index];
			}
		}

		public Cookie this[string name]
		{
			get
			{
				foreach (object obj in this.m_list)
				{
					Cookie cookie = (Cookie)obj;
					if (string.Compare(cookie.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return cookie;
					}
				}
				return null;
			}
		}

		public void Add(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			this.m_version++;
			int num = this.IndexOf(cookie);
			if (num == -1)
			{
				this.m_list.Add(cookie);
				return;
			}
			this.m_list[num] = cookie;
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

		public int Count
		{
			get
			{
				return this.m_list.Count;
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
			this.m_list.CopyTo(array, index);
		}

		public void CopyTo(Cookie[] array, int index)
		{
			this.m_list.CopyTo(array, index);
		}

		internal DateTime TimeStamp(CookieCollection.Stamp how)
		{
			switch (how)
			{
			case CookieCollection.Stamp.Set:
				this.m_TimeStamp = DateTime.Now;
				break;
			case CookieCollection.Stamp.SetToUnused:
				this.m_TimeStamp = DateTime.MinValue;
				break;
			case CookieCollection.Stamp.SetToMaxUsed:
				this.m_TimeStamp = DateTime.MaxValue;
				break;
			}
			return this.m_TimeStamp;
		}

		internal bool IsOtherVersionSeen
		{
			get
			{
				return this.m_has_other_versions;
			}
		}

		internal int InternalAdd(Cookie cookie, bool isStrict)
		{
			int num = 1;
			if (isStrict)
			{
				IComparer comparer = Cookie.GetComparer();
				int num2 = 0;
				foreach (object obj in this.m_list)
				{
					Cookie cookie2 = (Cookie)obj;
					if (comparer.Compare(cookie, cookie2) == 0)
					{
						num = 0;
						if (cookie2.Variant <= cookie.Variant)
						{
							this.m_list[num2] = cookie;
							break;
						}
						break;
					}
					else
					{
						num2++;
					}
				}
				if (num2 == this.m_list.Count)
				{
					this.m_list.Add(cookie);
				}
			}
			else
			{
				this.m_list.Add(cookie);
			}
			if (cookie.Version != 1)
			{
				this.m_has_other_versions = true;
			}
			return num;
		}

		internal int IndexOf(Cookie cookie)
		{
			IComparer comparer = Cookie.GetComparer();
			int num = 0;
			foreach (object obj in this.m_list)
			{
				Cookie cookie2 = (Cookie)obj;
				if (comparer.Compare(cookie, cookie2) == 0)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		internal void RemoveAt(int idx)
		{
			this.m_list.RemoveAt(idx);
		}

		public IEnumerator GetEnumerator()
		{
			return new CookieCollection.CookieCollectionEnumerator(this);
		}

		internal int m_version;

		private ArrayList m_list = new ArrayList();

		private DateTime m_TimeStamp = DateTime.MinValue;

		private bool m_has_other_versions;

		[OptionalField]
		private bool m_IsReadOnly;

		internal enum Stamp
		{
			Check,
			Set,
			SetToUnused,
			SetToMaxUsed
		}

		private class CookieCollectionEnumerator : IEnumerator
		{
			internal CookieCollectionEnumerator(CookieCollection cookies)
			{
				this.m_cookies = cookies;
				this.m_count = cookies.Count;
				this.m_version = cookies.m_version;
			}

			object IEnumerator.Current
			{
				get
				{
					if (this.m_index < 0 || this.m_index >= this.m_count)
					{
						throw new InvalidOperationException(global::SR.GetString("Enumeration has either not started or has already finished."));
					}
					if (this.m_version != this.m_cookies.m_version)
					{
						throw new InvalidOperationException(global::SR.GetString("Collection was modified; enumeration operation may not execute."));
					}
					return this.m_cookies[this.m_index];
				}
			}

			bool IEnumerator.MoveNext()
			{
				if (this.m_version != this.m_cookies.m_version)
				{
					throw new InvalidOperationException(global::SR.GetString("Collection was modified; enumeration operation may not execute."));
				}
				int num = this.m_index + 1;
				this.m_index = num;
				if (num < this.m_count)
				{
					return true;
				}
				this.m_index = this.m_count;
				return false;
			}

			void IEnumerator.Reset()
			{
				this.m_index = -1;
			}

			private CookieCollection m_cookies;

			private int m_count;

			private int m_index = -1;

			private int m_version;
		}
	}
}
