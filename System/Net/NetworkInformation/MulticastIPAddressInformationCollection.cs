using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Net.NetworkInformation
{
	public class MulticastIPAddressInformationCollection : ICollection<MulticastIPAddressInformation>, IEnumerable<MulticastIPAddressInformation>, IEnumerable
	{
		protected internal MulticastIPAddressInformationCollection()
		{
		}

		public virtual void CopyTo(MulticastIPAddressInformation[] array, int offset)
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

		public virtual void Add(MulticastIPAddressInformation address)
		{
			throw new NotSupportedException(global::SR.GetString("The collection is read-only."));
		}

		internal void InternalAdd(MulticastIPAddressInformation address)
		{
			this.addresses.Add(address);
		}

		public virtual bool Contains(MulticastIPAddressInformation address)
		{
			return this.addresses.Contains(address);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public virtual IEnumerator<MulticastIPAddressInformation> GetEnumerator()
		{
			return this.addresses.GetEnumerator();
		}

		public virtual MulticastIPAddressInformation this[int index]
		{
			get
			{
				return this.addresses[index];
			}
		}

		public virtual bool Remove(MulticastIPAddressInformation address)
		{
			throw new NotSupportedException(global::SR.GetString("The collection is read-only."));
		}

		public virtual void Clear()
		{
			throw new NotSupportedException(global::SR.GetString("The collection is read-only."));
		}

		private Collection<MulticastIPAddressInformation> addresses = new Collection<MulticastIPAddressInformation>();
	}
}
