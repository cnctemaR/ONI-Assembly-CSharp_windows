using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Data.ProviderBase
{
	internal sealed class DbConnectionPool
	{
		internal DbConnectionPool(DbConnectionFactory connectionFactory, DbConnectionPoolGroup connectionPoolGroup, DbConnectionPoolIdentity identity, DbConnectionPoolProviderInfo connectionPoolProviderInfo)
		{
			if (identity != null && identity.IsRestricted)
			{
				throw ADP.InternalError(ADP.InternalErrorCode.AttemptingToPoolOnRestrictedToken);
			}
			this._state = DbConnectionPool.State.Initializing;
			Random random = DbConnectionPool.s_random;
			lock (random)
			{
				this._cleanupWait = DbConnectionPool.s_random.Next(12, 24) * 10 * 1000;
			}
			this._connectionFactory = connectionFactory;
			this._connectionPoolGroup = connectionPoolGroup;
			this._connectionPoolGroupOptions = connectionPoolGroup.PoolGroupOptions;
			this._connectionPoolProviderInfo = connectionPoolProviderInfo;
			this._identity = identity;
			this._waitHandles = new DbConnectionPool.PoolWaitHandles();
			this._errorWait = 5000;
			this._errorTimer = null;
			this._objectList = new List<DbConnectionInternal>(this.MaxPoolSize);
			if (ADP.IsPlatformNT5)
			{
				this._transactedConnectionPool = new DbConnectionPool.TransactedConnectionPool(this);
			}
			this._poolCreateRequest = new WaitCallback(this.PoolCreateRequest);
			this._state = DbConnectionPool.State.Running;
		}

		private int CreationTimeout
		{
			get
			{
				return this.PoolGroupOptions.CreationTimeout;
			}
		}

		internal int Count
		{
			get
			{
				return this._totalObjects;
			}
		}

		internal DbConnectionFactory ConnectionFactory
		{
			get
			{
				return this._connectionFactory;
			}
		}

		internal bool ErrorOccurred
		{
			get
			{
				return this._errorOccurred;
			}
		}

		private bool HasTransactionAffinity
		{
			get
			{
				return this.PoolGroupOptions.HasTransactionAffinity;
			}
		}

		internal TimeSpan LoadBalanceTimeout
		{
			get
			{
				return this.PoolGroupOptions.LoadBalanceTimeout;
			}
		}

		private bool NeedToReplenish
		{
			get
			{
				if (DbConnectionPool.State.Running != this._state)
				{
					return false;
				}
				int count = this.Count;
				if (count >= this.MaxPoolSize)
				{
					return false;
				}
				if (count < this.MinPoolSize)
				{
					return true;
				}
				int num = this._stackNew.Count + this._stackOld.Count;
				int waitCount = this._waitCount;
				return num < waitCount || (num == waitCount && count > 1);
			}
		}

		internal DbConnectionPoolIdentity Identity
		{
			get
			{
				return this._identity;
			}
		}

		internal bool IsRunning
		{
			get
			{
				return DbConnectionPool.State.Running == this._state;
			}
		}

		private int MaxPoolSize
		{
			get
			{
				return this.PoolGroupOptions.MaxPoolSize;
			}
		}

		private int MinPoolSize
		{
			get
			{
				return this.PoolGroupOptions.MinPoolSize;
			}
		}

		internal DbConnectionPoolGroup PoolGroup
		{
			get
			{
				return this._connectionPoolGroup;
			}
		}

		internal DbConnectionPoolGroupOptions PoolGroupOptions
		{
			get
			{
				return this._connectionPoolGroupOptions;
			}
		}

		internal DbConnectionPoolProviderInfo ProviderInfo
		{
			get
			{
				return this._connectionPoolProviderInfo;
			}
		}

		internal bool UseLoadBalancing
		{
			get
			{
				return this.PoolGroupOptions.UseLoadBalancing;
			}
		}

		private bool UsingIntegrateSecurity
		{
			get
			{
				return this._identity != null && DbConnectionPoolIdentity.NoIdentity != this._identity;
			}
		}

		private void CleanupCallback(object state)
		{
			while (this.Count > this.MinPoolSize && this._waitHandles.PoolSemaphore.WaitOne(0))
			{
				DbConnectionInternal dbConnectionInternal;
				if (!this._stackOld.TryPop(out dbConnectionInternal))
				{
					this._waitHandles.PoolSemaphore.Release(1);
					break;
				}
				bool flag = true;
				DbConnectionInternal dbConnectionInternal2 = dbConnectionInternal;
				lock (dbConnectionInternal2)
				{
					if (dbConnectionInternal.IsTransactionRoot)
					{
						flag = false;
					}
				}
				if (flag)
				{
					this.DestroyObject(dbConnectionInternal);
				}
				else
				{
					dbConnectionInternal.SetInStasis();
				}
			}
			if (this._waitHandles.PoolSemaphore.WaitOne(0))
			{
				DbConnectionInternal dbConnectionInternal3;
				while (this._stackNew.TryPop(out dbConnectionInternal3))
				{
					this._stackOld.Push(dbConnectionInternal3);
				}
				this._waitHandles.PoolSemaphore.Release(1);
			}
			this.QueuePoolCreateRequest();
		}

		internal void Clear()
		{
			List<DbConnectionInternal> objectList = this._objectList;
			DbConnectionInternal dbConnectionInternal;
			lock (objectList)
			{
				int count = this._objectList.Count;
				for (int i = 0; i < count; i++)
				{
					dbConnectionInternal = this._objectList[i];
					if (dbConnectionInternal != null)
					{
						dbConnectionInternal.DoNotPoolThisConnection();
					}
				}
				goto IL_0057;
			}
			IL_0050:
			this.DestroyObject(dbConnectionInternal);
			IL_0057:
			if (!this._stackNew.TryPop(out dbConnectionInternal))
			{
				while (this._stackOld.TryPop(out dbConnectionInternal))
				{
					this.DestroyObject(dbConnectionInternal);
				}
				this.ReclaimEmancipatedObjects();
				return;
			}
			goto IL_0050;
		}

		private Timer CreateCleanupTimer()
		{
			return new Timer(new TimerCallback(this.CleanupCallback), null, this._cleanupWait, this._cleanupWait);
		}

		private DbConnectionInternal CreateObject(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
		{
			DbConnectionInternal dbConnectionInternal = null;
			try
			{
				dbConnectionInternal = this._connectionFactory.CreatePooledConnection(this, owningObject, this._connectionPoolGroup.ConnectionOptions, this._connectionPoolGroup.PoolKey, userOptions);
				if (dbConnectionInternal == null)
				{
					throw ADP.InternalError(ADP.InternalErrorCode.CreateObjectReturnedNull);
				}
				if (!dbConnectionInternal.CanBePooled)
				{
					throw ADP.InternalError(ADP.InternalErrorCode.NewObjectCannotBePooled);
				}
				dbConnectionInternal.PrePush(null);
				List<DbConnectionInternal> list = this._objectList;
				lock (list)
				{
					if (oldConnection != null && oldConnection.Pool == this)
					{
						this._objectList.Remove(oldConnection);
					}
					this._objectList.Add(dbConnectionInternal);
					this._totalObjects = this._objectList.Count;
				}
				if (oldConnection != null)
				{
					DbConnectionPool pool = oldConnection.Pool;
					if (pool != null && pool != this)
					{
						list = pool._objectList;
						lock (list)
						{
							pool._objectList.Remove(oldConnection);
							pool._totalObjects = pool._objectList.Count;
						}
					}
				}
				this._errorWait = 5000;
			}
			catch (Exception ex)
			{
				if (!ADP.IsCatchableExceptionType(ex))
				{
					throw;
				}
				dbConnectionInternal = null;
				this._resError = ex;
				Timer timer = new Timer(new TimerCallback(this.ErrorCallback), null, -1, -1);
				try
				{
				}
				finally
				{
					this._waitHandles.ErrorEvent.Set();
					this._errorOccurred = true;
					this._errorTimer = timer;
					timer.Change(this._errorWait, this._errorWait);
				}
				if (30000 < this._errorWait)
				{
					this._errorWait = 60000;
				}
				else
				{
					this._errorWait *= 2;
				}
				throw;
			}
			return dbConnectionInternal;
		}

		private void DeactivateObject(DbConnectionInternal obj)
		{
			obj.DeactivateConnection();
			bool flag = false;
			bool flag2 = false;
			if (obj.IsConnectionDoomed)
			{
				flag2 = true;
			}
			else
			{
				lock (obj)
				{
					if (this._state == DbConnectionPool.State.ShuttingDown)
					{
						if (obj.IsTransactionRoot)
						{
							obj.SetInStasis();
						}
						else
						{
							flag2 = true;
						}
					}
					else if (obj.IsNonPoolableTransactionRoot)
					{
						obj.SetInStasis();
					}
					else if (obj.CanBePooled)
					{
						Transaction enlistedTransaction = obj.EnlistedTransaction;
						if (null != enlistedTransaction)
						{
							this._transactedConnectionPool.PutTransactedObject(enlistedTransaction, obj);
						}
						else
						{
							flag = true;
						}
					}
					else if (obj.IsTransactionRoot && !obj.IsConnectionDoomed)
					{
						obj.SetInStasis();
					}
					else
					{
						flag2 = true;
					}
				}
			}
			if (flag)
			{
				this.PutNewObject(obj);
				return;
			}
			if (flag2)
			{
				this.DestroyObject(obj);
				this.QueuePoolCreateRequest();
			}
		}

		internal void DestroyObject(DbConnectionInternal obj)
		{
			if (!obj.IsTxRootWaitingForTxEnd)
			{
				List<DbConnectionInternal> objectList = this._objectList;
				lock (objectList)
				{
					this._objectList.Remove(obj);
					this._totalObjects = this._objectList.Count;
				}
				obj.Dispose();
			}
		}

		private void ErrorCallback(object state)
		{
			this._errorOccurred = false;
			this._waitHandles.ErrorEvent.Reset();
			Timer errorTimer = this._errorTimer;
			this._errorTimer = null;
			if (errorTimer != null)
			{
				errorTimer.Dispose();
			}
		}

		private Exception TryCloneCachedException()
		{
			if (this._resError == null)
			{
				return null;
			}
			SqlException ex = this._resError as SqlException;
			if (ex != null)
			{
				return ex.InternalClone();
			}
			return this._resError;
		}

		private void WaitForPendingOpen()
		{
			DbConnectionPool.PendingGetConnection pendingGetConnection;
			do
			{
				bool flag = false;
				try
				{
					try
					{
					}
					finally
					{
						flag = Interlocked.CompareExchange(ref this._pendingOpensWaiting, 1, 0) == 0;
					}
					if (!flag)
					{
						break;
					}
					while (this._pendingOpens.TryDequeue(out pendingGetConnection))
					{
						if (!pendingGetConnection.Completion.Task.IsCompleted)
						{
							uint num;
							if (pendingGetConnection.DueTime == -1L)
							{
								num = uint.MaxValue;
							}
							else
							{
								num = (uint)Math.Max(ADP.TimerRemainingMilliseconds(pendingGetConnection.DueTime), 0L);
							}
							DbConnectionInternal dbConnectionInternal = null;
							bool flag2 = false;
							Exception ex = null;
							try
							{
								bool flag3 = true;
								bool flag4 = false;
								ADP.SetCurrentTransaction(pendingGetConnection.Completion.Task.AsyncState as Transaction);
								flag2 = !this.TryGetConnection(pendingGetConnection.Owner, num, flag3, flag4, pendingGetConnection.UserOptions, out dbConnectionInternal);
							}
							catch (Exception ex)
							{
							}
							if (ex != null)
							{
								pendingGetConnection.Completion.TrySetException(ex);
							}
							else if (flag2)
							{
								pendingGetConnection.Completion.TrySetException(ADP.ExceptionWithStackTrace(ADP.PooledOpenTimeout()));
							}
							else if (!pendingGetConnection.Completion.TrySetResult(dbConnectionInternal))
							{
								this.PutObject(dbConnectionInternal, pendingGetConnection.Owner);
							}
						}
					}
				}
				finally
				{
					if (flag)
					{
						Interlocked.Exchange(ref this._pendingOpensWaiting, 0);
					}
				}
			}
			while (this._pendingOpens.TryPeek(out pendingGetConnection));
		}

		internal bool TryGetConnection(DbConnection owningObject, TaskCompletionSource<DbConnectionInternal> retry, DbConnectionOptions userOptions, out DbConnectionInternal connection)
		{
			uint num = 0U;
			bool flag = false;
			if (retry == null)
			{
				num = (uint)this.CreationTimeout;
				if (num == 0U)
				{
					num = uint.MaxValue;
				}
				flag = true;
			}
			if (this._state != DbConnectionPool.State.Running)
			{
				connection = null;
				return true;
			}
			bool flag2 = true;
			if (this.TryGetConnection(owningObject, num, flag, flag2, userOptions, out connection))
			{
				return true;
			}
			if (retry == null)
			{
				return true;
			}
			DbConnectionPool.PendingGetConnection pendingGetConnection = new DbConnectionPool.PendingGetConnection((this.CreationTimeout == 0) ? (-1L) : (ADP.TimerCurrent() + ADP.TimerFromSeconds(this.CreationTimeout / 1000)), owningObject, retry, userOptions);
			this._pendingOpens.Enqueue(pendingGetConnection);
			if (this._pendingOpensWaiting == 0)
			{
				new Thread(new ThreadStart(this.WaitForPendingOpen))
				{
					IsBackground = true
				}.Start();
			}
			connection = null;
			return false;
		}

		private bool TryGetConnection(DbConnection owningObject, uint waitForMultipleObjectsTimeout, bool allowCreate, bool onlyOneCheckConnection, DbConnectionOptions userOptions, out DbConnectionInternal connection)
		{
			DbConnectionInternal dbConnectionInternal = null;
			Transaction transaction = null;
			if (this.HasTransactionAffinity)
			{
				dbConnectionInternal = this.GetFromTransactedPool(out transaction);
			}
			if (dbConnectionInternal == null)
			{
				Interlocked.Increment(ref this._waitCount);
				for (;;)
				{
					int num = 3;
					try
					{
						try
						{
						}
						finally
						{
							num = WaitHandle.WaitAny(this._waitHandles.GetHandles(allowCreate), (int)waitForMultipleObjectsTimeout);
						}
						switch (num)
						{
						case 0:
							Interlocked.Decrement(ref this._waitCount);
							dbConnectionInternal = this.GetFromGeneralPool();
							if (dbConnectionInternal != null && !dbConnectionInternal.IsConnectionAlive(false))
							{
								this.DestroyObject(dbConnectionInternal);
								dbConnectionInternal = null;
								if (onlyOneCheckConnection)
								{
									if (this._waitHandles.CreationSemaphore.WaitOne((int)waitForMultipleObjectsTimeout))
									{
										try
										{
											dbConnectionInternal = this.UserCreateRequest(owningObject, userOptions, null);
											break;
										}
										finally
										{
											this._waitHandles.CreationSemaphore.Release(1);
										}
									}
									connection = null;
									return false;
								}
							}
							break;
						case 1:
							Interlocked.Decrement(ref this._waitCount);
							throw this.TryCloneCachedException();
						case 2:
							try
							{
								dbConnectionInternal = this.UserCreateRequest(owningObject, userOptions, null);
							}
							catch
							{
								if (dbConnectionInternal == null)
								{
									Interlocked.Decrement(ref this._waitCount);
								}
								throw;
							}
							finally
							{
								if (dbConnectionInternal != null)
								{
									Interlocked.Decrement(ref this._waitCount);
								}
							}
							if (dbConnectionInternal == null && this.Count >= this.MaxPoolSize && this.MaxPoolSize != 0 && !this.ReclaimEmancipatedObjects())
							{
								allowCreate = false;
							}
							break;
						default:
							if (num == 258)
							{
								Interlocked.Decrement(ref this._waitCount);
								connection = null;
								return false;
							}
							Interlocked.Decrement(ref this._waitCount);
							throw ADP.InternalError(ADP.InternalErrorCode.UnexpectedWaitAnyResult);
						}
					}
					finally
					{
						if (2 == num)
						{
							this._waitHandles.CreationSemaphore.Release(1);
						}
					}
					if (dbConnectionInternal != null)
					{
						goto IL_0185;
					}
				}
				bool flag;
				return flag;
			}
			IL_0185:
			if (dbConnectionInternal != null)
			{
				this.PrepareConnection(owningObject, dbConnectionInternal, transaction);
			}
			connection = dbConnectionInternal;
			return true;
		}

		private void PrepareConnection(DbConnection owningObject, DbConnectionInternal obj, Transaction transaction)
		{
			lock (obj)
			{
				obj.PostPop(owningObject);
			}
			try
			{
				obj.ActivateConnection(transaction);
			}
			catch
			{
				this.PutObject(obj, owningObject);
				throw;
			}
		}

		internal DbConnectionInternal ReplaceConnection(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
		{
			DbConnectionInternal dbConnectionInternal = this.UserCreateRequest(owningObject, userOptions, oldConnection);
			if (dbConnectionInternal != null)
			{
				this.PrepareConnection(owningObject, dbConnectionInternal, oldConnection.EnlistedTransaction);
				oldConnection.PrepareForReplaceConnection();
				oldConnection.DeactivateConnection();
				oldConnection.Dispose();
			}
			return dbConnectionInternal;
		}

		private DbConnectionInternal GetFromGeneralPool()
		{
			DbConnectionInternal dbConnectionInternal = null;
			if (!this._stackNew.TryPop(out dbConnectionInternal) && !this._stackOld.TryPop(out dbConnectionInternal))
			{
				dbConnectionInternal = null;
			}
			return dbConnectionInternal;
		}

		private DbConnectionInternal GetFromTransactedPool(out Transaction transaction)
		{
			transaction = ADP.GetCurrentTransaction();
			DbConnectionInternal dbConnectionInternal = null;
			if (null != transaction && this._transactedConnectionPool != null)
			{
				dbConnectionInternal = this._transactedConnectionPool.GetTransactedObject(transaction);
				if (dbConnectionInternal != null)
				{
					if (dbConnectionInternal.IsTransactionRoot)
					{
						try
						{
							dbConnectionInternal.IsConnectionAlive(true);
							return dbConnectionInternal;
						}
						catch
						{
							this.DestroyObject(dbConnectionInternal);
							throw;
						}
					}
					if (!dbConnectionInternal.IsConnectionAlive(false))
					{
						this.DestroyObject(dbConnectionInternal);
						dbConnectionInternal = null;
					}
				}
			}
			return dbConnectionInternal;
		}

		private void PoolCreateRequest(object state)
		{
			if (DbConnectionPool.State.Running == this._state)
			{
				if (!this._pendingOpens.IsEmpty && this._pendingOpensWaiting == 0)
				{
					new Thread(new ThreadStart(this.WaitForPendingOpen))
					{
						IsBackground = true
					}.Start();
				}
				this.ReclaimEmancipatedObjects();
				if (!this.ErrorOccurred && this.NeedToReplenish)
				{
					if (this.UsingIntegrateSecurity && !this._identity.Equals(DbConnectionPoolIdentity.GetCurrent()))
					{
						return;
					}
					int num = 3;
					try
					{
						try
						{
						}
						finally
						{
							num = WaitHandle.WaitAny(this._waitHandles.GetHandles(true), this.CreationTimeout);
						}
						if (2 == num)
						{
							if (!this.ErrorOccurred)
							{
								while (this.NeedToReplenish)
								{
									DbConnectionInternal dbConnectionInternal = this.CreateObject(null, null, null);
									if (dbConnectionInternal == null)
									{
										break;
									}
									this.PutNewObject(dbConnectionInternal);
								}
							}
						}
						else if (258 == num)
						{
							this.QueuePoolCreateRequest();
						}
					}
					finally
					{
						if (2 == num)
						{
							this._waitHandles.CreationSemaphore.Release(1);
						}
					}
				}
			}
		}

		internal void PutNewObject(DbConnectionInternal obj)
		{
			this._stackNew.Push(obj);
			this._waitHandles.PoolSemaphore.Release(1);
		}

		internal void PutObject(DbConnectionInternal obj, object owningObject)
		{
			lock (obj)
			{
				obj.PrePush(owningObject);
			}
			this.DeactivateObject(obj);
		}

		internal void PutObjectFromTransactedPool(DbConnectionInternal obj)
		{
			if (this._state == DbConnectionPool.State.Running && obj.CanBePooled)
			{
				this.PutNewObject(obj);
				return;
			}
			this.DestroyObject(obj);
			this.QueuePoolCreateRequest();
		}

		private void QueuePoolCreateRequest()
		{
			if (DbConnectionPool.State.Running == this._state)
			{
				ThreadPool.QueueUserWorkItem(this._poolCreateRequest);
			}
		}

		private bool ReclaimEmancipatedObjects()
		{
			bool flag = false;
			List<DbConnectionInternal> list = new List<DbConnectionInternal>();
			List<DbConnectionInternal> objectList = this._objectList;
			int num;
			lock (objectList)
			{
				num = this._objectList.Count;
				for (int i = 0; i < num; i++)
				{
					DbConnectionInternal dbConnectionInternal = this._objectList[i];
					if (dbConnectionInternal != null)
					{
						bool flag3 = false;
						try
						{
							Monitor.TryEnter(dbConnectionInternal, ref flag3);
							if (flag3 && dbConnectionInternal.IsEmancipated)
							{
								dbConnectionInternal.PrePush(null);
								list.Add(dbConnectionInternal);
							}
						}
						finally
						{
							if (flag3)
							{
								Monitor.Exit(dbConnectionInternal);
							}
						}
					}
				}
			}
			num = list.Count;
			for (int j = 0; j < num; j++)
			{
				DbConnectionInternal dbConnectionInternal2 = list[j];
				flag = true;
				dbConnectionInternal2.DetachCurrentTransactionIfEnded();
				this.DeactivateObject(dbConnectionInternal2);
			}
			return flag;
		}

		internal void Startup()
		{
			this._cleanupTimer = this.CreateCleanupTimer();
			if (this.NeedToReplenish)
			{
				this.QueuePoolCreateRequest();
			}
		}

		internal void Shutdown()
		{
			this._state = DbConnectionPool.State.ShuttingDown;
			Timer cleanupTimer = this._cleanupTimer;
			this._cleanupTimer = null;
			if (cleanupTimer != null)
			{
				cleanupTimer.Dispose();
			}
		}

		internal void TransactionEnded(Transaction transaction, DbConnectionInternal transactedObject)
		{
			DbConnectionPool.TransactedConnectionPool transactedConnectionPool = this._transactedConnectionPool;
			if (transactedConnectionPool != null)
			{
				transactedConnectionPool.TransactionEnded(transaction, transactedObject);
			}
		}

		private DbConnectionInternal UserCreateRequest(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection = null)
		{
			DbConnectionInternal dbConnectionInternal = null;
			if (this.ErrorOccurred)
			{
				throw this.TryCloneCachedException();
			}
			if ((oldConnection != null || this.Count < this.MaxPoolSize || this.MaxPoolSize == 0) && (oldConnection != null || (this.Count & 1) == 1 || !this.ReclaimEmancipatedObjects()))
			{
				dbConnectionInternal = this.CreateObject(owningObject, userOptions, oldConnection);
			}
			return dbConnectionInternal;
		}

		private const int MAX_Q_SIZE = 1048576;

		private const int SEMAPHORE_HANDLE = 0;

		private const int ERROR_HANDLE = 1;

		private const int CREATION_HANDLE = 2;

		private const int BOGUS_HANDLE = 3;

		private const int ERROR_WAIT_DEFAULT = 5000;

		private static readonly Random s_random = new Random(5101977);

		private readonly int _cleanupWait;

		private readonly DbConnectionPoolIdentity _identity;

		private readonly DbConnectionFactory _connectionFactory;

		private readonly DbConnectionPoolGroup _connectionPoolGroup;

		private readonly DbConnectionPoolGroupOptions _connectionPoolGroupOptions;

		private DbConnectionPoolProviderInfo _connectionPoolProviderInfo;

		private DbConnectionPool.State _state;

		private readonly ConcurrentStack<DbConnectionInternal> _stackOld = new ConcurrentStack<DbConnectionInternal>();

		private readonly ConcurrentStack<DbConnectionInternal> _stackNew = new ConcurrentStack<DbConnectionInternal>();

		private readonly ConcurrentQueue<DbConnectionPool.PendingGetConnection> _pendingOpens = new ConcurrentQueue<DbConnectionPool.PendingGetConnection>();

		private int _pendingOpensWaiting;

		private readonly WaitCallback _poolCreateRequest;

		private int _waitCount;

		private readonly DbConnectionPool.PoolWaitHandles _waitHandles;

		private Exception _resError;

		private volatile bool _errorOccurred;

		private int _errorWait;

		private Timer _errorTimer;

		private Timer _cleanupTimer;

		private readonly DbConnectionPool.TransactedConnectionPool _transactedConnectionPool;

		private readonly List<DbConnectionInternal> _objectList;

		private int _totalObjects;

		private enum State
		{
			Initializing,
			Running,
			ShuttingDown
		}

		private sealed class TransactedConnectionList : List<DbConnectionInternal>
		{
			internal TransactedConnectionList(int initialAllocation, Transaction tx)
				: base(initialAllocation)
			{
				this._transaction = tx;
			}

			internal void Dispose()
			{
				if (null != this._transaction)
				{
					this._transaction.Dispose();
				}
			}

			private Transaction _transaction;
		}

		private sealed class PendingGetConnection
		{
			public PendingGetConnection(long dueTime, DbConnection owner, TaskCompletionSource<DbConnectionInternal> completion, DbConnectionOptions userOptions)
			{
				this.DueTime = dueTime;
				this.Owner = owner;
				this.Completion = completion;
			}

			public long DueTime { get; private set; }

			public DbConnection Owner { get; private set; }

			public TaskCompletionSource<DbConnectionInternal> Completion { get; private set; }

			public DbConnectionOptions UserOptions { get; private set; }
		}

		private sealed class TransactedConnectionPool
		{
			internal TransactedConnectionPool(DbConnectionPool pool)
			{
				this._pool = pool;
				this._transactedCxns = new Dictionary<Transaction, DbConnectionPool.TransactedConnectionList>();
			}

			internal int ObjectID
			{
				get
				{
					return this._objectID;
				}
			}

			internal DbConnectionPool Pool
			{
				get
				{
					return this._pool;
				}
			}

			internal DbConnectionInternal GetTransactedObject(Transaction transaction)
			{
				DbConnectionInternal dbConnectionInternal = null;
				bool flag = false;
				Dictionary<Transaction, DbConnectionPool.TransactedConnectionList> transactedCxns = this._transactedCxns;
				DbConnectionPool.TransactedConnectionList transactedConnectionList;
				lock (transactedCxns)
				{
					flag = this._transactedCxns.TryGetValue(transaction, out transactedConnectionList);
				}
				if (flag)
				{
					DbConnectionPool.TransactedConnectionList transactedConnectionList2 = transactedConnectionList;
					lock (transactedConnectionList2)
					{
						int num = transactedConnectionList.Count - 1;
						if (0 <= num)
						{
							dbConnectionInternal = transactedConnectionList[num];
							transactedConnectionList.RemoveAt(num);
						}
					}
				}
				return dbConnectionInternal;
			}

			internal void PutTransactedObject(Transaction transaction, DbConnectionInternal transactedObject)
			{
				bool flag = false;
				Dictionary<Transaction, DbConnectionPool.TransactedConnectionList> dictionary = this._transactedCxns;
				lock (dictionary)
				{
					DbConnectionPool.TransactedConnectionList transactedConnectionList;
					if (flag = this._transactedCxns.TryGetValue(transaction, out transactedConnectionList))
					{
						DbConnectionPool.TransactedConnectionList transactedConnectionList2 = transactedConnectionList;
						lock (transactedConnectionList2)
						{
							transactedConnectionList.Add(transactedObject);
						}
					}
				}
				if (!flag)
				{
					Transaction transaction2 = null;
					DbConnectionPool.TransactedConnectionList transactedConnectionList3 = null;
					try
					{
						transaction2 = transaction.Clone();
						transactedConnectionList3 = new DbConnectionPool.TransactedConnectionList(2, transaction2);
						dictionary = this._transactedCxns;
						lock (dictionary)
						{
							DbConnectionPool.TransactedConnectionList transactedConnectionList;
							if (flag = this._transactedCxns.TryGetValue(transaction, out transactedConnectionList))
							{
								DbConnectionPool.TransactedConnectionList transactedConnectionList2 = transactedConnectionList;
								lock (transactedConnectionList2)
								{
									transactedConnectionList.Add(transactedObject);
									return;
								}
							}
							transactedConnectionList3.Add(transactedObject);
							this._transactedCxns.Add(transaction2, transactedConnectionList3);
							transaction2 = null;
						}
					}
					finally
					{
						if (null != transaction2)
						{
							if (transactedConnectionList3 != null)
							{
								transactedConnectionList3.Dispose();
							}
							else
							{
								transaction2.Dispose();
							}
						}
					}
				}
			}

			internal void TransactionEnded(Transaction transaction, DbConnectionInternal transactedObject)
			{
				int num = -1;
				Dictionary<Transaction, DbConnectionPool.TransactedConnectionList> transactedCxns = this._transactedCxns;
				lock (transactedCxns)
				{
					DbConnectionPool.TransactedConnectionList transactedConnectionList;
					if (this._transactedCxns.TryGetValue(transaction, out transactedConnectionList))
					{
						bool flag2 = false;
						DbConnectionPool.TransactedConnectionList transactedConnectionList2 = transactedConnectionList;
						lock (transactedConnectionList2)
						{
							num = transactedConnectionList.IndexOf(transactedObject);
							if (num >= 0)
							{
								transactedConnectionList.RemoveAt(num);
							}
							if (0 >= transactedConnectionList.Count)
							{
								this._transactedCxns.Remove(transaction);
								flag2 = true;
							}
						}
						if (flag2)
						{
							transactedConnectionList.Dispose();
						}
					}
				}
				if (0 <= num)
				{
					this.Pool.PutObjectFromTransactedPool(transactedObject);
				}
			}

			private Dictionary<Transaction, DbConnectionPool.TransactedConnectionList> _transactedCxns;

			private DbConnectionPool _pool;

			private static int _objectTypeCount;

			internal readonly int _objectID = Interlocked.Increment(ref DbConnectionPool.TransactedConnectionPool._objectTypeCount);
		}

		private sealed class PoolWaitHandles
		{
			internal PoolWaitHandles()
			{
				this._poolSemaphore = new Semaphore(0, 1048576);
				this._errorEvent = new ManualResetEvent(false);
				this._creationSemaphore = new Semaphore(1, 1);
				this._handlesWithCreate = new WaitHandle[] { this._poolSemaphore, this._errorEvent, this._creationSemaphore };
				this._handlesWithoutCreate = new WaitHandle[] { this._poolSemaphore, this._errorEvent };
			}

			internal Semaphore CreationSemaphore
			{
				get
				{
					return this._creationSemaphore;
				}
			}

			internal ManualResetEvent ErrorEvent
			{
				get
				{
					return this._errorEvent;
				}
			}

			internal Semaphore PoolSemaphore
			{
				get
				{
					return this._poolSemaphore;
				}
			}

			internal WaitHandle[] GetHandles(bool withCreate)
			{
				if (!withCreate)
				{
					return this._handlesWithoutCreate;
				}
				return this._handlesWithCreate;
			}

			private readonly Semaphore _poolSemaphore;

			private readonly ManualResetEvent _errorEvent;

			private readonly Semaphore _creationSemaphore;

			private readonly WaitHandle[] _handlesWithCreate;

			private readonly WaitHandle[] _handlesWithoutCreate;
		}
	}
}
