using System;
using System.Text;

namespace Mono.Data.Tds.Protocol
{
	public class TdsConnectionInfo
	{
		public TdsConnectionInfo(string dataSource, int port, int packetSize, int timeout, int minSize, int maxSize)
		{
			this.DataSource = dataSource;
			this.Port = port;
			this.PacketSize = packetSize;
			this.Timeout = timeout;
			this.PoolMinSize = minSize;
			this.PoolMaxSize = maxSize;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("DataSouce: {0}\n", this.DataSource);
			stringBuilder.AppendFormat("Port: {0}\n", this.Port);
			stringBuilder.AppendFormat("PacketSize: {0}\n", this.PacketSize);
			stringBuilder.AppendFormat("Timeout: {0}\n", this.Timeout);
			stringBuilder.AppendFormat("PoolMinSize: {0}\n", this.PoolMinSize);
			stringBuilder.AppendFormat("PoolMaxSize: {0}", this.PoolMaxSize);
			return stringBuilder.ToString();
		}

		public string DataSource;

		public int Port;

		public int PacketSize;

		public int Timeout;

		public int PoolMinSize;

		public int PoolMaxSize;
	}
}
