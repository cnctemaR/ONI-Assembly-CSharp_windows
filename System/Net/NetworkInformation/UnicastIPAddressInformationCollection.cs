using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	public class UnicastIPAddressInformationCollection : IEnumerable, IEnumerable<UnicastIPAddressInformation>, ICollection<UnicastIPAddressInformation>
	{
		protected internal UnicastIPAddressInformationCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public virtual void Add(UnicastIPAddressInformation address)
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			this.list.Add(address);
		}

		public virtual void Clear()
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			this.list.Clear();
		}

		public virtual bool Contains(UnicastIPAddressInformation address)
		{
			return this.list.Contains(address);
		}

		public virtual void CopyTo(UnicastIPAddressInformation[] array, int offset)
		{
			this.list.CopyTo(array, offset);
		}

		public virtual IEnumerator<UnicastIPAddressInformation> GetEnumerator()
		{
			return ((IEnumerable<UnicastIPAddressInformation>)this.list).GetEnumerator();
		}

		public virtual bool Remove(UnicastIPAddressInformation address)
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			return this.list.Remove(address);
		}

		public virtual int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public virtual UnicastIPAddressInformation this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		private List<UnicastIPAddressInformation> list = new List<UnicastIPAddressInformation>();
	}
}
