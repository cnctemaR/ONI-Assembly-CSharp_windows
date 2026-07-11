using System;

namespace System.Net.Sockets
{
	public class IPv6MulticastOption
	{
		public IPv6MulticastOption(IPAddress group, long ifindex)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (ifindex < 0L || ifindex > (long)((ulong)(-1)))
			{
				throw new ArgumentOutOfRangeException("ifindex");
			}
			this.Group = group;
			this.InterfaceIndex = ifindex;
		}

		public IPv6MulticastOption(IPAddress group)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			this.Group = group;
			this.InterfaceIndex = 0L;
		}

		public IPAddress Group
		{
			get
			{
				return this.m_Group;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_Group = value;
			}
		}

		public long InterfaceIndex
		{
			get
			{
				return this.m_Interface;
			}
			set
			{
				if (value < 0L || value > (long)((ulong)(-1)))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.m_Interface = value;
			}
		}

		private IPAddress m_Group;

		private long m_Interface;
	}
}
