using System;
using System.Collections;

namespace Mono.Data.Tds.Protocol
{
	public class TdsConnectionPoolManager
	{
		public TdsConnectionPoolManager(TdsVersion version)
		{
			this.version = version;
		}

		public TdsConnectionPool GetConnectionPool(string connectionString, TdsConnectionInfo info)
		{
			TdsConnectionPool tdsConnectionPool = (TdsConnectionPool)this.pools[connectionString];
			if (tdsConnectionPool == null)
			{
				this.pools[connectionString] = new TdsConnectionPool(this, info);
				tdsConnectionPool = (TdsConnectionPool)this.pools[connectionString];
			}
			return tdsConnectionPool;
		}

		public TdsConnectionPool GetConnectionPool(string connectionString)
		{
			return (TdsConnectionPool)this.pools[connectionString];
		}

		public virtual Tds CreateConnection(TdsConnectionInfo info)
		{
			TdsVersion tdsVersion = this.version;
			if (tdsVersion == TdsVersion.tds42)
			{
				return new Tds42(info.DataSource, info.Port, info.PacketSize, info.Timeout);
			}
			if (tdsVersion == TdsVersion.tds50)
			{
				return new Tds50(info.DataSource, info.Port, info.PacketSize, info.Timeout);
			}
			if (tdsVersion == TdsVersion.tds70)
			{
				return new Tds70(info.DataSource, info.Port, info.PacketSize, info.Timeout);
			}
			if (tdsVersion != TdsVersion.tds80)
			{
				throw new NotSupportedException();
			}
			return new Tds80(info.DataSource, info.Port, info.PacketSize, info.Timeout);
		}

		public IDictionary GetConnectionPool()
		{
			return this.pools;
		}

		private Hashtable pools = Hashtable.Synchronized(new Hashtable());

		private TdsVersion version;
	}
}
