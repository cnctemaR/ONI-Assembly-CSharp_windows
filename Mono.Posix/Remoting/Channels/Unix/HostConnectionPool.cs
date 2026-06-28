using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Threading;

namespace Mono.Remoting.Channels.Unix
{
	internal class HostConnectionPool
	{
		public HostConnectionPool(string path)
		{
			this._path = path;
		}

		public UnixConnection GetConnection()
		{
			UnixConnection unixConnection = null;
			ArrayList pool = this._pool;
			lock (pool)
			{
				for (;;)
				{
					if (this._pool.Count <= 0)
					{
						goto IL_006F;
					}
					unixConnection = (UnixConnection)this._pool[this._pool.Count - 1];
					this._pool.RemoveAt(this._pool.Count - 1);
					if (unixConnection.IsAlive)
					{
						goto IL_006F;
					}
					this.CancelConnection(unixConnection);
					unixConnection = null;
					IL_009C:
					if (unixConnection != null)
					{
						break;
					}
					continue;
					IL_006F:
					if (unixConnection == null && this._activeConnections < UnixConnectionPool.MaxOpenConnections)
					{
						break;
					}
					if (unixConnection == null)
					{
						Monitor.Wait(this._pool);
						goto IL_009C;
					}
					goto IL_009C;
				}
			}
			if (unixConnection == null)
			{
				return this.CreateConnection();
			}
			return unixConnection;
		}

		private UnixConnection CreateConnection()
		{
			UnixConnection unixConnection2;
			try
			{
				ReusableUnixClient reusableUnixClient = new ReusableUnixClient(this._path);
				UnixConnection unixConnection = new UnixConnection(this, reusableUnixClient);
				this._activeConnections++;
				unixConnection2 = unixConnection;
			}
			catch (Exception ex)
			{
				throw new RemotingException(ex.Message);
			}
			return unixConnection2;
		}

		public void ReleaseConnection(UnixConnection entry)
		{
			ArrayList pool = this._pool;
			lock (pool)
			{
				entry.ControlTime = DateTime.Now;
				this._pool.Add(entry);
				Monitor.Pulse(this._pool);
			}
		}

		private void CancelConnection(UnixConnection entry)
		{
			try
			{
				entry.Stream.Close();
				this._activeConnections--;
			}
			catch
			{
			}
		}

		public void PurgeConnections()
		{
			ArrayList pool = this._pool;
			lock (pool)
			{
				for (int i = 0; i < this._pool.Count; i++)
				{
					UnixConnection unixConnection = (UnixConnection)this._pool[i];
					if ((DateTime.Now - unixConnection.ControlTime).TotalSeconds > (double)UnixConnectionPool.KeepAliveSeconds)
					{
						this.CancelConnection(unixConnection);
						this._pool.RemoveAt(i);
						i--;
					}
				}
			}
		}

		private ArrayList _pool = new ArrayList();

		private int _activeConnections;

		private string _path;
	}
}
