using System;

namespace System.Net.NetworkInformation
{
	public class PingReply
	{
		internal PingReply(IPAddress address, byte[] buffer, PingOptions options, long roundtripTime, IPStatus status)
		{
			this.address = address;
			this.buffer = buffer;
			this.options = options;
			this.rtt = roundtripTime;
			this.status = status;
		}

		public IPAddress Address
		{
			get
			{
				return this.address;
			}
		}

		public byte[] Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		public PingOptions Options
		{
			get
			{
				return this.options;
			}
		}

		public long RoundtripTime
		{
			get
			{
				return this.rtt;
			}
		}

		public IPStatus Status
		{
			get
			{
				return this.status;
			}
		}

		private IPAddress address;

		private byte[] buffer;

		private PingOptions options;

		private long rtt;

		private IPStatus status;
	}
}
