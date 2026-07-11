using System;
using System.Collections;
using System.Threading;

namespace System.Net
{
	internal class ConnectionPool
	{
		private Mutex CreationMutex
		{
			get
			{
				return (Mutex)this.m_WaitHandles[2];
			}
		}

		private ManualResetEvent ErrorEvent
		{
			get
			{
				return (ManualResetEvent)this.m_WaitHandles[1];
			}
		}

		private Semaphore Semaphore
		{
			get
			{
				return (Semaphore)this.m_WaitHandles[0];
			}
		}

		internal ConnectionPool(ServicePoint servicePoint, int maxPoolSize, int minPoolSize, int idleTimeout, CreateConnectionDelegate createConnectionCallback)
		{
			this.m_State = ConnectionPool.State.Initializing;
			this.m_CreateConnectionCallback = createConnectionCallback;
			this.m_MaxPoolSize = maxPoolSize;
			this.m_MinPoolSize = minPoolSize;
			this.m_ServicePoint = servicePoint;
			this.Initialize();
			if (idleTimeout > 0)
			{
				this.m_CleanupQueue = TimerThread.GetOrCreateQueue((idleTimeout == 1) ? 1 : (idleTimeout / 2));
				this.m_CleanupQueue.CreateTimer(ConnectionPool.s_CleanupCallback, this);
			}
		}

		private void Initialize()
		{
			this.m_StackOld = new InterlockedStack();
			this.m_StackNew = new InterlockedStack();
			this.m_QueuedRequests = new Queue();
			this.m_WaitHandles = new WaitHandle[3];
			this.m_WaitHandles[0] = new Semaphore(0, 1048576);
			this.m_WaitHandles[1] = new ManualResetEvent(false);
			this.m_WaitHandles[2] = new Mutex();
			this.m_ErrorTimer = null;
			this.m_ObjectList = new ArrayList();
			this.m_State = ConnectionPool.State.Running;
		}

		private void QueueRequest(ConnectionPool.AsyncConnectionPoolRequest asyncRequest)
		{
			Queue queuedRequests = this.m_QueuedRequests;
			lock (queuedRequests)
			{
				this.m_QueuedRequests.Enqueue(asyncRequest);
				if (this.m_AsyncThread == null)
				{
					this.m_AsyncThread = new Thread(new ThreadStart(this.AsyncThread));
					this.m_AsyncThread.IsBackground = true;
					this.m_AsyncThread.Start();
				}
			}
		}

		private void AsyncThread()
		{
			for (;;)
			{
				Queue queue;
				if (this.m_QueuedRequests.Count <= 0)
				{
					Thread.Sleep(500);
					queue = this.m_QueuedRequests;
					lock (queue)
					{
						if (this.m_QueuedRequests.Count != 0)
						{
							continue;
						}
						this.m_AsyncThread = null;
					}
					break;
				}
				bool flag2 = true;
				ConnectionPool.AsyncConnectionPoolRequest asyncConnectionPoolRequest = null;
				queue = this.m_QueuedRequests;
				lock (queue)
				{
					asyncConnectionPoolRequest = (ConnectionPool.AsyncConnectionPoolRequest)this.m_QueuedRequests.Dequeue();
				}
				WaitHandle[] waitHandles = this.m_WaitHandles;
				PooledStream pooledStream = null;
				try
				{
					while (pooledStream == null && flag2)
					{
						int num = WaitHandle.WaitAny(waitHandles, asyncConnectionPoolRequest.CreationTimeout, false);
						pooledStream = this.Get(asyncConnectionPoolRequest.OwningObject, num, ref flag2, ref waitHandles);
					}
					pooledStream.Activate(asyncConnectionPoolRequest.OwningObject, asyncConnectionPoolRequest.AsyncCallback);
				}
				catch (Exception ex)
				{
					if (pooledStream != null)
					{
						this.PutConnection(pooledStream, asyncConnectionPoolRequest.OwningObject, asyncConnectionPoolRequest.CreationTimeout, false);
					}
					asyncConnectionPoolRequest.AsyncCallback(asyncConnectionPoolRequest.OwningObject, ex);
				}
			}
		}

		internal int Count
		{
			get
			{
				return this.m_TotalObjects;
			}
		}

		internal ServicePoint ServicePoint
		{
			get
			{
				return this.m_ServicePoint;
			}
		}

		internal int MaxPoolSize
		{
			get
			{
				return this.m_MaxPoolSize;
			}
		}

		internal int MinPoolSize
		{
			get
			{
				return this.m_MinPoolSize;
			}
		}

		private bool ErrorOccurred
		{
			get
			{
				return this.m_ErrorOccured;
			}
		}

		private static void CleanupCallbackWrapper(TimerThread.Timer timer, int timeNoticed, object context)
		{
			ConnectionPool connectionPool = (ConnectionPool)context;
			try
			{
				connectionPool.CleanupCallback();
			}
			finally
			{
				connectionPool.m_CleanupQueue.CreateTimer(ConnectionPool.s_CleanupCallback, context);
			}
		}

		internal void ForceCleanup()
		{
			if (Logging.On)
			{
			}
			while (this.Count > 0 && this.Semaphore.WaitOne(0, false))
			{
				PooledStream pooledStream = (PooledStream)this.m_StackNew.Pop();
				if (pooledStream == null)
				{
					pooledStream = (PooledStream)this.m_StackOld.Pop();
				}
				this.Destroy(pooledStream);
			}
			bool on = Logging.On;
		}

		private void CleanupCallback()
		{
			while (this.Count > this.MinPoolSize && this.Semaphore.WaitOne(0, false))
			{
				PooledStream pooledStream = (PooledStream)this.m_StackOld.Pop();
				if (pooledStream == null)
				{
					this.Semaphore.ReleaseSemaphore();
					break;
				}
				this.Destroy(pooledStream);
			}
			if (this.Semaphore.WaitOne(0, false))
			{
				for (;;)
				{
					PooledStream pooledStream2 = (PooledStream)this.m_StackNew.Pop();
					if (pooledStream2 == null)
					{
						break;
					}
					this.m_StackOld.Push(pooledStream2);
				}
				this.Semaphore.ReleaseSemaphore();
			}
		}

		private PooledStream Create(CreateConnectionDelegate createConnectionCallback)
		{
			PooledStream pooledStream = null;
			try
			{
				pooledStream = createConnectionCallback(this);
				if (pooledStream == null)
				{
					throw new InternalException();
				}
				if (!pooledStream.CanBePooled)
				{
					throw new InternalException();
				}
				pooledStream.PrePush(null);
				object syncRoot = this.m_ObjectList.SyncRoot;
				lock (syncRoot)
				{
					this.m_ObjectList.Add(pooledStream);
					this.m_TotalObjects = this.m_ObjectList.Count;
				}
			}
			catch (Exception ex)
			{
				pooledStream = null;
				this.m_ResError = ex;
				this.Abort();
			}
			return pooledStream;
		}

		private void Destroy(PooledStream pooledStream)
		{
			if (pooledStream != null)
			{
				try
				{
					object syncRoot = this.m_ObjectList.SyncRoot;
					lock (syncRoot)
					{
						this.m_ObjectList.Remove(pooledStream);
						this.m_TotalObjects = this.m_ObjectList.Count;
					}
				}
				finally
				{
					pooledStream.Dispose();
				}
			}
		}

		private static void CancelErrorCallbackWrapper(TimerThread.Timer timer, int timeNoticed, object context)
		{
			((ConnectionPool)context).CancelErrorCallback();
		}

		private void CancelErrorCallback()
		{
			TimerThread.Timer errorTimer = this.m_ErrorTimer;
			if (errorTimer != null && errorTimer.Cancel())
			{
				this.m_ErrorOccured = false;
				this.ErrorEvent.Reset();
				this.m_ErrorTimer = null;
				this.m_ResError = null;
			}
		}

		private PooledStream GetFromPool(object owningObject)
		{
			PooledStream pooledStream = (PooledStream)this.m_StackNew.Pop();
			if (pooledStream == null)
			{
				pooledStream = (PooledStream)this.m_StackOld.Pop();
			}
			if (pooledStream != null)
			{
				pooledStream.PostPop(owningObject);
			}
			return pooledStream;
		}

		private PooledStream Get(object owningObject, int result, ref bool continueLoop, ref WaitHandle[] waitHandles)
		{
			PooledStream pooledStream = null;
			if (result != 1)
			{
				if (result != 2)
				{
					if (result == 258)
					{
						Interlocked.Decrement(ref this.m_WaitCount);
						continueLoop = false;
						throw new WebException(NetRes.GetWebStatusString("net_timeout", WebExceptionStatus.ConnectFailure), WebExceptionStatus.Timeout);
					}
				}
				else
				{
					try
					{
						continueLoop = true;
						pooledStream = this.UserCreateRequest();
						if (pooledStream != null)
						{
							pooledStream.PostPop(owningObject);
							Interlocked.Decrement(ref this.m_WaitCount);
							continueLoop = false;
							return pooledStream;
						}
						if (this.Count >= this.MaxPoolSize && this.MaxPoolSize != 0 && !this.ReclaimEmancipatedObjects())
						{
							waitHandles = new WaitHandle[2];
							waitHandles[0] = this.m_WaitHandles[0];
							waitHandles[1] = this.m_WaitHandles[1];
						}
						return pooledStream;
					}
					finally
					{
						this.CreationMutex.ReleaseMutex();
					}
				}
				Interlocked.Decrement(ref this.m_WaitCount);
				pooledStream = this.GetFromPool(owningObject);
				continueLoop = false;
				return pooledStream;
			}
			bool flag = Interlocked.Decrement(ref this.m_WaitCount) != 0;
			continueLoop = false;
			Exception resError = this.m_ResError;
			if (!flag)
			{
				this.CancelErrorCallback();
			}
			throw resError;
		}

		internal void Abort()
		{
			if (this.m_ResError == null)
			{
				this.m_ResError = new WebException(NetRes.GetWebStatusString("net_requestaborted", WebExceptionStatus.RequestCanceled), WebExceptionStatus.RequestCanceled);
			}
			this.ErrorEvent.Set();
			this.m_ErrorOccured = true;
			this.m_ErrorTimer = ConnectionPool.s_CancelErrorQueue.CreateTimer(ConnectionPool.s_CancelErrorCallback, this);
		}

		internal PooledStream GetConnection(object owningObject, GeneralAsyncDelegate asyncCallback, int creationTimeout)
		{
			PooledStream pooledStream = null;
			bool flag = true;
			bool flag2 = asyncCallback != null;
			if (this.m_State != ConnectionPool.State.Running)
			{
				throw new InternalException();
			}
			Interlocked.Increment(ref this.m_WaitCount);
			WaitHandle[] waitHandles = this.m_WaitHandles;
			if (flag2)
			{
				int num = WaitHandle.WaitAny(waitHandles, 0, false);
				if (num != 258)
				{
					pooledStream = this.Get(owningObject, num, ref flag, ref waitHandles);
				}
				if (pooledStream == null)
				{
					ConnectionPool.AsyncConnectionPoolRequest asyncConnectionPoolRequest = new ConnectionPool.AsyncConnectionPoolRequest(this, owningObject, asyncCallback, creationTimeout);
					this.QueueRequest(asyncConnectionPoolRequest);
				}
			}
			else
			{
				while (pooledStream == null && flag)
				{
					int num = WaitHandle.WaitAny(waitHandles, creationTimeout, false);
					pooledStream = this.Get(owningObject, num, ref flag, ref waitHandles);
				}
			}
			if (pooledStream != null)
			{
				if (!pooledStream.IsInitalizing)
				{
					asyncCallback = null;
				}
				try
				{
					if (!pooledStream.Activate(owningObject, asyncCallback))
					{
						pooledStream = null;
					}
					return pooledStream;
				}
				catch
				{
					this.PutConnection(pooledStream, owningObject, creationTimeout, false);
					throw;
				}
			}
			if (!flag2)
			{
				throw new InternalException();
			}
			return pooledStream;
		}

		internal void PutConnection(PooledStream pooledStream, object owningObject, int creationTimeout)
		{
			this.PutConnection(pooledStream, owningObject, creationTimeout, true);
		}

		internal void PutConnection(PooledStream pooledStream, object owningObject, int creationTimeout, bool canReuse)
		{
			if (pooledStream == null)
			{
				throw new ArgumentNullException("pooledStream");
			}
			pooledStream.PrePush(owningObject);
			if (this.m_State != ConnectionPool.State.ShuttingDown)
			{
				pooledStream.Deactivate();
				if (this.m_WaitCount == 0)
				{
					this.CancelErrorCallback();
				}
				if (canReuse && pooledStream.CanBePooled)
				{
					this.PutNew(pooledStream);
					return;
				}
				try
				{
					this.Destroy(pooledStream);
					return;
				}
				finally
				{
					if (this.m_WaitCount > 0)
					{
						if (!this.CreationMutex.WaitOne(creationTimeout, false))
						{
							this.Abort();
						}
						else
						{
							try
							{
								pooledStream = this.UserCreateRequest();
								if (pooledStream != null)
								{
									this.PutNew(pooledStream);
								}
							}
							finally
							{
								this.CreationMutex.ReleaseMutex();
							}
						}
					}
				}
			}
			this.Destroy(pooledStream);
		}

		private void PutNew(PooledStream pooledStream)
		{
			this.m_StackNew.Push(pooledStream);
			this.Semaphore.ReleaseSemaphore();
		}

		private bool ReclaimEmancipatedObjects()
		{
			bool flag = false;
			object syncRoot = this.m_ObjectList.SyncRoot;
			lock (syncRoot)
			{
				object[] array = this.m_ObjectList.ToArray();
				if (array != null)
				{
					foreach (PooledStream pooledStream in array)
					{
						if (pooledStream != null)
						{
							bool flag3 = false;
							try
							{
								Monitor.TryEnter(pooledStream, ref flag3);
								if (flag3 && pooledStream.IsEmancipated)
								{
									this.PutConnection(pooledStream, null, -1);
									flag = true;
								}
							}
							finally
							{
								if (flag3)
								{
									Monitor.Exit(pooledStream);
								}
							}
						}
					}
				}
			}
			return flag;
		}

		private PooledStream UserCreateRequest()
		{
			PooledStream pooledStream = null;
			if (!this.ErrorOccurred && (this.Count < this.MaxPoolSize || this.MaxPoolSize == 0) && ((this.Count & 1) == 1 || !this.ReclaimEmancipatedObjects()))
			{
				pooledStream = this.Create(this.m_CreateConnectionCallback);
			}
			return pooledStream;
		}

		private static TimerThread.Callback s_CleanupCallback = new TimerThread.Callback(ConnectionPool.CleanupCallbackWrapper);

		private static TimerThread.Callback s_CancelErrorCallback = new TimerThread.Callback(ConnectionPool.CancelErrorCallbackWrapper);

		private static TimerThread.Queue s_CancelErrorQueue = TimerThread.GetOrCreateQueue(5000);

		private const int MaxQueueSize = 1048576;

		private const int SemaphoreHandleIndex = 0;

		private const int ErrorHandleIndex = 1;

		private const int CreationHandleIndex = 2;

		private const int WaitTimeout = 258;

		private const int WaitAbandoned = 128;

		private const int ErrorWait = 5000;

		private readonly TimerThread.Queue m_CleanupQueue;

		private ConnectionPool.State m_State;

		private InterlockedStack m_StackOld;

		private InterlockedStack m_StackNew;

		private int m_WaitCount;

		private WaitHandle[] m_WaitHandles;

		private Exception m_ResError;

		private volatile bool m_ErrorOccured;

		private TimerThread.Timer m_ErrorTimer;

		private ArrayList m_ObjectList;

		private int m_TotalObjects;

		private Queue m_QueuedRequests;

		private Thread m_AsyncThread;

		private int m_MaxPoolSize;

		private int m_MinPoolSize;

		private ServicePoint m_ServicePoint;

		private CreateConnectionDelegate m_CreateConnectionCallback;

		private enum State
		{
			Initializing,
			Running,
			ShuttingDown
		}

		private class AsyncConnectionPoolRequest
		{
			public AsyncConnectionPoolRequest(ConnectionPool pool, object owningObject, GeneralAsyncDelegate asyncCallback, int creationTimeout)
			{
				this.Pool = pool;
				this.OwningObject = owningObject;
				this.AsyncCallback = asyncCallback;
				this.CreationTimeout = creationTimeout;
			}

			public object OwningObject;

			public GeneralAsyncDelegate AsyncCallback;

			public ConnectionPool Pool;

			public int CreationTimeout;
		}
	}
}
