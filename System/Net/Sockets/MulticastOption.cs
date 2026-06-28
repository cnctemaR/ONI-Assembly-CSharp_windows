using System;

namespace System.Net.Sockets
{
	public class MulticastOption
	{
		public MulticastOption(IPAddress group)
			: this(group, IPAddress.Any)
		{
		}

		public MulticastOption(IPAddress group, int interfaceIndex)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (interfaceIndex < 0 || interfaceIndex > 16777215)
			{
				throw new ArgumentOutOfRangeException("interfaceIndex");
			}
			this.group = group;
			this.iface_index = interfaceIndex;
		}

		public MulticastOption(IPAddress group, IPAddress mcint)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (mcint == null)
			{
				throw new ArgumentNullException("mcint");
			}
			this.group = group;
			this.local = mcint;
		}

		public IPAddress Group
		{
			get
			{
				return this.group;
			}
			set
			{
				this.group = value;
			}
		}

		public IPAddress LocalAddress
		{
			get
			{
				return this.local;
			}
			set
			{
				this.local = value;
				this.iface_index = 0;
			}
		}

		public int InterfaceIndex
		{
			get
			{
				return this.iface_index;
			}
			set
			{
				if (value < 0 || value > 16777215)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.iface_index = value;
				this.local = null;
			}
		}

		private IPAddress group;

		private IPAddress local;

		private int iface_index;
	}
}
