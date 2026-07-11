using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Net.NetworkInformation
{
	public class IPAddressCollection : ICollection<IPAddress>, IEnumerable<IPAddress>, IEnumerable
	{
		protected internal IPAddressCollection()
		{
		}

		public virtual void CopyTo(IPAddress[] array, int offset)
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

		public virtual void Add(IPAddress address)
		{
			throw new NotSupportedException(global::SR.GetString("The collection is read-only."));
		}

		internal void InternalAdd(IPAddress address)
		{
			this.addresses.Add(address);
		}

		public virtual bool Contains(IPAddress address)
		{
			return this.addresses.Contains(address);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public virtual IEnumerator<IPAddress> GetEnumerator()
		{
			return this.addresses.GetEnumerator();
		}

		public virtual IPAddress this[int index]
		{
			get
			{
				return this.addresses[index];
			}
		}

		public virtual bool Remove(IPAddress address)
		{
			throw new NotSupportedException(global::SR.GetString("The collection is read-only."));
		}

		public virtual void Clear()
		{
			throw new NotSupportedException(global::SR.GetString("The collection is read-only."));
		}

		private Collection<IPAddress> addresses = new Collection<IPAddress>();
	}
}
