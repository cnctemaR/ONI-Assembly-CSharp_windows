using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	public class IPAddressCollection : IEnumerable, ICollection<IPAddress>, IEnumerable<IPAddress>
	{
		protected internal IPAddressCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		internal void SetReadOnly()
		{
			if (!this.IsReadOnly)
			{
				this.list = ((List<IPAddress>)this.list).AsReadOnly();
			}
		}

		public virtual void Add(IPAddress address)
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

		public virtual bool Contains(IPAddress address)
		{
			return this.list.Contains(address);
		}

		public virtual void CopyTo(IPAddress[] array, int offset)
		{
			this.list.CopyTo(array, offset);
		}

		public virtual IEnumerator<IPAddress> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public virtual bool Remove(IPAddress address)
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
				return this.list.IsReadOnly;
			}
		}

		public virtual IPAddress this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		private IList<IPAddress> list = new List<IPAddress>();
	}
}
