using System;
using System.Collections;
using System.Threading;

namespace Mono.Data.Tds.Protocol
{
	public class TdsConnectionPool
	{
		public TdsConnectionPool(TdsConnectionPoolManager manager, TdsConnectionInfo info)
		{
			this.info = info;
			this.manager = manager;
			this.conns = new ArrayList(info.PoolMaxSize);
			this.available = new Queue(info.PoolMaxSize);
			this.InitializePool();
		}

		private void InitializePool()
		{
			for (int i = this.conns.Count; i < this.info.PoolMinSize; i++)
			{
				try
				{
					Tds tds = this.manager.CreateConnection(this.info);
					this.conns.Add(tds);
					this.available.Enqueue(tds);
				}
				catch
				{
				}
			}
		}

		public bool Pooling
		{
			get
			{
				return !this.no_pooling;
			}
			set
			{
				this.no_pooling = !value;
			}
		}

		public Tds GetConnection()
		{
			if (this.no_pooling)
			{
				return this.manager.CreateConnection(this.info);
			}
			Tds tds = null;
			int num = this.info.PoolMaxSize * 2;
			Exception ex;
			do
			{
				while (tds == null)
				{
					bool flag = false;
					Queue queue = this.available;
					lock (queue)
					{
						if (this.available.Count > 0)
						{
							tds = (Tds)this.available.Dequeue();
							break;
						}
						lock (this.conns)
						{
							if (this.conns.Count >= this.info.PoolMaxSize - this.in_progress)
							{
								Monitor.Exit(this.conns);
								if (!Monitor.Wait(this.available, this.info.Timeout * 1000))
								{
									throw new InvalidOperationException("Timeout expired. The timeout period elapsed before a connection could be obtained. A possible explanation is that all the connections in the pool are in use, and the maximum pool size is reached.");
								}
								if (this.available.Count > 0)
								{
									tds = (Tds)this.available.Dequeue();
									break;
								}
								continue;
							}
							else
							{
								flag = true;
								this.in_progress++;
							}
						}
					}
					if (flag)
					{
						try
						{
							tds = this.manager.CreateConnection(this.info);
							ArrayList arrayList = this.conns;
							lock (arrayList)
							{
								this.conns.Add(tds);
							}
							return tds;
						}
						finally
						{
							Queue queue2 = this.available;
							lock (queue2)
							{
								this.in_progress--;
							}
						}
					}
				}
				bool flag2 = true;
				ex = null;
				try
				{
					flag2 = !tds.IsConnected || !tds.Reset();
				}
				catch (Exception ex2)
				{
					flag2 = true;
					ex = ex2;
				}
				if (!flag2)
				{
					return tds;
				}
				ArrayList arrayList2 = this.conns;
				lock (arrayList2)
				{
					this.conns.Remove(tds);
				}
				tds.Disconnect();
				num--;
			}
			while (num != 0);
			throw ex;
		}

		public void ReleaseConnection(Tds connection)
		{
			if (connection == null)
			{
				return;
			}
			if (this.no_pooling)
			{
				connection.Disconnect();
				return;
			}
			if (connection.poolStatus == 2)
			{
				ArrayList arrayList = this.conns;
				lock (arrayList)
				{
					this.conns.Remove(connection);
				}
				connection.Disconnect();
				connection = null;
			}
			Queue queue = this.available;
			lock (queue)
			{
				if (connection != null)
				{
					this.available.Enqueue(connection);
				}
				Monitor.Pulse(this.available);
			}
		}

		public void ResetConnectionPool()
		{
			Queue queue = this.available;
			lock (queue)
			{
				ArrayList arrayList = this.conns;
				lock (arrayList)
				{
					for (int i = this.conns.Count - 1; i >= 0; i--)
					{
						Tds tds = (Tds)this.conns[i];
						tds.poolStatus = 2;
					}
					for (int i = this.available.Count - 1; i >= 0; i--)
					{
						Tds tds = (Tds)this.available.Dequeue();
						tds.Disconnect();
						this.conns.Remove(tds);
					}
					this.available.Clear();
					this.InitializePool();
				}
				Monitor.PulseAll(this.available);
			}
		}

		private TdsConnectionInfo info;

		private bool no_pooling;

		private TdsConnectionPoolManager manager;

		private Queue available;

		private ArrayList conns;

		private int in_progress;
	}
}
