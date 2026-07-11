using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public sealed class ApplicationTrustCollection : ICollection, IEnumerable
	{
		internal ApplicationTrustCollection()
		{
			this._list = new ArrayList();
		}

		public int Count
		{
			[SecuritySafeCritical]
			get
			{
				return this._list.Count;
			}
		}

		public bool IsSynchronized
		{
			[SecuritySafeCritical]
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			[SecuritySafeCritical]
			get
			{
				return this;
			}
		}

		public ApplicationTrust this[int index]
		{
			get
			{
				return (ApplicationTrust)this._list[index];
			}
		}

		public ApplicationTrust this[string appFullName]
		{
			get
			{
				for (int i = 0; i < this._list.Count; i++)
				{
					ApplicationTrust applicationTrust = this._list[i] as ApplicationTrust;
					if (applicationTrust.ApplicationIdentity.FullName == appFullName)
					{
						return applicationTrust;
					}
				}
				return null;
			}
		}

		public int Add(ApplicationTrust trust)
		{
			if (trust == null)
			{
				throw new ArgumentNullException("trust");
			}
			if (trust.ApplicationIdentity == null)
			{
				throw new ArgumentException(Locale.GetText("ApplicationTrust.ApplicationIdentity can't be null."), "trust");
			}
			return this._list.Add(trust);
		}

		public void AddRange(ApplicationTrust[] trusts)
		{
			if (trusts == null)
			{
				throw new ArgumentNullException("trusts");
			}
			foreach (ApplicationTrust applicationTrust in trusts)
			{
				if (applicationTrust.ApplicationIdentity == null)
				{
					throw new ArgumentException(Locale.GetText("ApplicationTrust.ApplicationIdentity can't be null."), "trust");
				}
				this._list.Add(applicationTrust);
			}
		}

		public void AddRange(ApplicationTrustCollection trusts)
		{
			if (trusts == null)
			{
				throw new ArgumentNullException("trusts");
			}
			foreach (ApplicationTrust applicationTrust in trusts)
			{
				if (applicationTrust.ApplicationIdentity == null)
				{
					throw new ArgumentException(Locale.GetText("ApplicationTrust.ApplicationIdentity can't be null."), "trust");
				}
				this._list.Add(applicationTrust);
			}
		}

		public void Clear()
		{
			this._list.Clear();
		}

		public void CopyTo(ApplicationTrust[] array, int index)
		{
			this._list.CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public ApplicationTrustCollection Find(ApplicationIdentity applicationIdentity, ApplicationVersionMatch versionMatch)
		{
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			string text = applicationIdentity.FullName;
			if (versionMatch != ApplicationVersionMatch.MatchExactVersion)
			{
				if (versionMatch != ApplicationVersionMatch.MatchAllVersions)
				{
					throw new ArgumentException("versionMatch");
				}
				int num = text.IndexOf(", Version=");
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
			}
			ApplicationTrustCollection applicationTrustCollection = new ApplicationTrustCollection();
			foreach (object obj in this._list)
			{
				ApplicationTrust applicationTrust = (ApplicationTrust)obj;
				if (applicationTrust.ApplicationIdentity.FullName.StartsWith(text))
				{
					applicationTrustCollection.Add(applicationTrust);
				}
			}
			return applicationTrustCollection;
		}

		public ApplicationTrustEnumerator GetEnumerator()
		{
			return new ApplicationTrustEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ApplicationTrustEnumerator(this);
		}

		public void Remove(ApplicationTrust trust)
		{
			if (trust == null)
			{
				throw new ArgumentNullException("trust");
			}
			if (trust.ApplicationIdentity == null)
			{
				throw new ArgumentException(Locale.GetText("ApplicationTrust.ApplicationIdentity can't be null."), "trust");
			}
			this.RemoveAllInstances(trust);
		}

		public void Remove(ApplicationIdentity applicationIdentity, ApplicationVersionMatch versionMatch)
		{
			foreach (ApplicationTrust applicationTrust in this.Find(applicationIdentity, versionMatch))
			{
				this.RemoveAllInstances(applicationTrust);
			}
		}

		public void RemoveRange(ApplicationTrust[] trusts)
		{
			if (trusts == null)
			{
				throw new ArgumentNullException("trusts");
			}
			foreach (ApplicationTrust applicationTrust in trusts)
			{
				this.RemoveAllInstances(applicationTrust);
			}
		}

		public void RemoveRange(ApplicationTrustCollection trusts)
		{
			if (trusts == null)
			{
				throw new ArgumentNullException("trusts");
			}
			foreach (ApplicationTrust applicationTrust in trusts)
			{
				this.RemoveAllInstances(applicationTrust);
			}
		}

		internal void RemoveAllInstances(ApplicationTrust trust)
		{
			for (int i = this._list.Count - 1; i >= 0; i--)
			{
				if (trust.Equals(this._list[i]))
				{
					this._list.RemoveAt(i);
				}
			}
		}

		private ArrayList _list;
	}
}
