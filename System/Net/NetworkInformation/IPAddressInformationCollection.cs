using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Net.NetworkInformation
{
	public class IPAddressInformationCollection : ICollection<IPAddressInformation>, IEnumerable<IPAddressInformation>, IEnumerable
	{
		internal IPAddressInformationCollection()
		{
		}

		public virtual void CopyTo(IPAddressInformation[] array, int offset)
		{
			this.addresses.CopyTo(array, offset);
		}

		public virtual int Count
		{
			get
			{
				return this.addresses.Count;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public virtual void Add(IPAddressInformation address)
		{
			throw new NotSupportedException(global::SR.GetString("The collection is read-only."));
		}

		internal void InternalAdd(IPAddressInformation address)
		{
			this.addresses.Add(address);
		}

		public virtual bool Contains(IPAddressInformation address)
		{
			return this.addresses.Contains(address);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public virtual IEnumerator<IPAddressInformation> GetEnumerator()
		{
			return this.addresses.GetEnumerator();
		}

		public virtual IPAddressInformation this[int index]
		{
			get
			{
				return this.addresses[index];
			}
		}

		public virtual bool Remove(IPAddressInformation address)
		{
			throw new NotSupportedException(global::SR.GetString("The collection is read-only."));
		}

		public virtual void Clear()
		{
			throw new NotSupportedException(global::SR.GetString("The collection is read-only."));
		}

		private Collection<IPAddressInformation> addresses = new Collection<IPAddressInformation>();
	}
}
