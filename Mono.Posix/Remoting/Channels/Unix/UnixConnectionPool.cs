using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Threading;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixConnectionPool
	{
		static UnixConnectionPool()
		{
			UnixConnectionPool._poolThread.Start();
			UnixConnectionPool._poolThread.IsBackground = true;
		}

		public static void Shutdown()
		{
			if (UnixConnectionPool._poolThread != null)
			{
				UnixConnectionPool._poolThread.Abort();
			}
		}

		public static int MaxOpenConnections
		{
			get
			{
				return UnixConnectionPool._maxOpenConnections;
			}
			set
			{
				if (value < 1)
				{
					throw new RemotingException("MaxOpenConnections must be greater than zero");
				}
				UnixConnectionPool._maxOpenConnections = value;
			}
		}

		public static int KeepAliveSeconds
		{
			get
			{
				return UnixConnectionPool._keepAliveSeconds;
			}
			set
			{
				UnixConnectionPool._keepAliveSeconds = value;
			}
		}

		public static UnixConnection GetConnection(string path)
		{
			Hashtable pools = UnixConnectionPool._pools;
			HostConnectionPool hostConnectionPool;
			lock (pools)
			{
				hostConnectionPool = (HostConnectionPool)UnixConnectionPool._pools[path];
				if (hostConnectionPool == null)
				{
					hostConnectionPool = new HostConnectionPool(path);
					UnixConnectionPool._pools[path] = hostConnectionPool;
				}
			}
			return hostConnectionPool.GetConnection();
		}

		private static void ConnectionCollector()
		{
			for (;;)
			{
				Thread.Sleep(3000);
				Hashtable pools = UnixConnectionPool._pools;
				lock (pools)
				{
					ICollection values = UnixConnectionPool._pools.Values;
					foreach (object obj in values)
					{
						HostConnectionPool hostConnectionPool = (HostConnectionPool)obj;
						hostConnectionPool.PurgeConnections();
					}
				}
			}
		}

		private static Hashtable _pools = new Hashtable();

		private static int _maxOpenConnections = int.MaxValue;

		private static int _keepAliveSeconds = 15;

		private static Thread _poolThread = new Thread(new ThreadStart(UnixConnectionPool.ConnectionCollector));
	}
}
