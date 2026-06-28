using System;

namespace System.Net.Sockets
{
	public class IPv6MulticastOption
	{
		public IPv6MulticastOption(IPAddress group)
			: this(group, 0L)
		{
		}

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
			this.group = group;
			this.ifIndex = ifindex;
		}

		public IPAddress Group
		{
			get
			{
				return this.group;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.group = value;
			}
		}

		public long InterfaceIndex
		{
			get
			{
				return this.ifIndex;
			}
			set
			{
				if (value < 0L || value > (long)((ulong)(-1)))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.ifIndex = value;
			}
		}

		private IPAddress group;

		private long ifIndex;
	}
}
