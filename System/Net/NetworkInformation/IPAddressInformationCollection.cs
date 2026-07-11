using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	public class IPAddressInformationCollection : IEnumerable, IEnumerable<IPAddressInformation>, ICollection<IPAddressInformation>
	{
		internal IPAddressInformationCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public virtual void Add(IPAddressInformation address)
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

		public virtual bool Contains(IPAddressInformation address)
		{
			return this.list.Contains(address);
		}

		public virtual void CopyTo(IPAddressInformation[] array, int offset)
		{
			this.list.CopyTo(array, offset);
		}

		public virtual IEnumerator<IPAddressInformation> GetEnumerator()
		{
			return ((IEnumerable<IPAddressInformation>)this.list).GetEnumerator();
		}

		public virtual bool Remove(IPAddressInformation address)
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

		public virtual IPAddressInformation this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		private List<IPAddressInformation> list = new List<IPAddressInformation>();
	}
}
