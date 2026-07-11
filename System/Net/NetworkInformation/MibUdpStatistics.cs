using System;
using System.Collections.Specialized;
using System.Globalization;

namespace System.Net.NetworkInformation
{
	internal class MibUdpStatistics : UdpStatistics
	{
		public MibUdpStatistics(StringDictionary dic)
		{
			this.dic = dic;
		}

		private long Get(string name)
		{
			if (this.dic[name] == null)
			{
				return 0L;
			}
			return long.Parse(this.dic[name], NumberFormatInfo.InvariantInfo);
		}

		public override long DatagramsReceived
		{
			get
			{
				return this.Get("InDatagrams");
			}
		}

		public override long DatagramsSent
		{
			get
			{
				return this.Get("OutDatagrams");
			}
		}

		public override long IncomingDatagramsDiscarded
		{
			get
			{
				return this.Get("NoPorts");
			}
		}

		public override long IncomingDatagramsWithErrors
		{
			get
			{
				return this.Get("InErrors");
			}
		}

		public override int UdpListeners
		{
			get
			{
				return (int)this.Get("NumAddrs");
			}
		}

		private StringDictionary dic;
	}
}
