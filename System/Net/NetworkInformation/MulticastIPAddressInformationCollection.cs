using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	public class MulticastIPAddressInformationCollection : IEnumerable, IEnumerable<MulticastIPAddressInformation>, ICollection<MulticastIPAddressInformation>
	{
		protected internal MulticastIPAddressInformationCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public virtual void Add(MulticastIPAddressInformation address)
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

		public virtual bool Contains(MulticastIPAddressInformation address)
		{
			return this.list.Contains(address);
		}

		public virtual void CopyTo(MulticastIPAddressInformation[] array, int offset)
		{
			this.list.CopyTo(array, offset);
		}

		public virtual IEnumerator<MulticastIPAddressInformation> GetEnumerator()
		{
			return ((IEnumerable<MulticastIPAddressInformation>)this.list).GetEnumerator();
		}

		public virtual bool Remove(MulticastIPAddressInformation address)
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

		public virtual MulticastIPAddressInformation this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		private List<MulticastIPAddressInformation> list = new List<MulticastIPAddressInformation>();
	}
}
