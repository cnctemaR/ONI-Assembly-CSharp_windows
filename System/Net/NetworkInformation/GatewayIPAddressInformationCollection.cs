using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	public class GatewayIPAddressInformationCollection : IEnumerable, IEnumerable<GatewayIPAddressInformation>, ICollection<GatewayIPAddressInformation>
	{
		protected GatewayIPAddressInformationCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public virtual void Add(GatewayIPAddressInformation address)
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

		public virtual bool Contains(GatewayIPAddressInformation address)
		{
			return this.list.Contains(address);
		}

		public virtual void CopyTo(GatewayIPAddressInformation[] array, int offset)
		{
			this.list.CopyTo(array, offset);
		}

		public virtual IEnumerator<GatewayIPAddressInformation> GetEnumerator()
		{
			return ((IEnumerable<GatewayIPAddressInformation>)this.list).GetEnumerator();
		}

		public virtual bool Remove(GatewayIPAddressInformation address)
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

		public virtual GatewayIPAddressInformation this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		private List<GatewayIPAddressInformation> list = new List<GatewayIPAddressInformation>();
	}
}
