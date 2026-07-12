using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Data.ProviderBase
{
	internal abstract class DbConnectionInternal
	{
		protected DbConnectionInternal()
			: this(ConnectionState.Open, true, false)
		{
		}

		internal DbConnectionInternal(ConnectionState state, bool hidePassword, bool allowSetConnectionString)
		{
			this._allowSetConnectionString = allowSetConnectionString;
			this._hidePassword = hidePassword;
			this._state = state;
		}

		internal bool AllowSetConnectionString
		{
			get
			{
				return this._allowSetConnectionString;
			}
		}

		internal bool CanBePooled
		{
			get
			{
				return !this._connectionIsDoomed && !this._cannotBePooled && !this._owningObject.IsAlive;
			}
		}

		protected internal bool IsConnectionDoomed
		{
			get
			{
				return this._connectionIsDoomed;
			}
		}

		internal bool IsEmancipated
		{
			get
			{
				return this._pooledCount < 1 && !this._owningObject.IsAlive;
			}
		}

		internal bool IsInPool
		{
			get
			{
				return this._pooledCount == 1;
			}
		}

		protected internal object Owner
		{
			get
			{
				return this._owningObject.Target;
			}
		}

		internal DbConnectionPool Pool
		{
			get
			{
				return this._connectionPool;
			}
		}

		protected internal DbReferenceCollection ReferenceCollection
		{
			get
			{
				return this._referenceCollection;
			}
		}

		public abstract string ServerVersion { get; }

		public virtual string ServerVersionNormalized
		{
			get
			{
				throw ADP.NotSupported();
			}
		}

		public bool ShouldHidePassword
		{
			get
			{
				return this._hidePassword;
			}
		}

		public ConnectionState State
		{
			get
			{
				return this._state;
			}
		}

		internal void AddWeakReference(object value, int tag)
		{
			if (this._referenceCollection == null)
			{
				this._referenceCollection = this.CreateReferenceCollection();
				if (this._referenceCollection == null)
				{
					throw ADP.InternalError(ADP.InternalErrorCode.CreateReferenceCollectionReturnedNull);
				}
			}
			this._referenceCollection.Add(value, tag);
		}

		public abstract DbTransaction BeginTransaction(IsolationLevel il);

		public virtual void ChangeDatabase(string value)
		{
			throw ADP.MethodNotImplemented("ChangeDatabase");
		}

		internal virtual void PrepareForReplaceConnection()
		{
		}

		protected virtual void PrepareForCloseConnection()
		{
		}

		protected virtual object ObtainAdditionalLocksForClose()
		{
			return null;
		}

		protected virtual void ReleaseAdditionalLocksForClose(object lockToken)
		{
		}

		protected virtual DbReferenceCollection CreateReferenceCollection()
		{
			throw ADP.InternalError(ADP.InternalErrorCode.AttemptingToConstructReferenceCollectionOnStaticObject);
		}

		protected abstract void Deactivate();

		internal void DeactivateConnection()
		{
			if (!this._connectionIsDoomed && this.Pool.UseLoadBalancing && DateTime.UtcNow.Ticks - this._createTime.Ticks > this.Pool.LoadBalanceTimeout.Ticks)
			{
				this.DoNotPoolThisConnection();
			}
			this.Deactivate();
		}

		protected internal void DoNotPoolThisConnection()
		{
			this._cannotBePooled = true;
		}

		protected internal void DoomThisConnection()
		{
			this._connectionIsDoomed = true;
		}

		protected internal virtual DataTable GetSchema(DbConnectionFactory factory, DbConnectionPoolGroup poolGroup, DbConnection outerConnection, string collectionName, string[] restrictions)
		{
			return factory.GetMetaDataFactory(poolGroup, this).GetSchema(outerConnection, collectionName, restrictions);
		}

		internal void MakeNonPooledObject(object owningObject)
		{
			this._connectionPool = null;
			this._owningObject.Target = owningObject;
			this._pooledCount = -1;
		}

		internal void MakePooledConnection(DbConnectionPool connectionPool)
		{
			this._createTime = DateTime.UtcNow;
			this._connectionPool = connectionPool;
		}

		internal void NotifyWeakReference(int message)
		{
			DbReferenceCollection referenceCollection = this.ReferenceCollection;
			if (referenceCollection != null)
			{
				referenceCollection.Notify(message);
			}
		}

		internal virtual void OpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory)
		{
			if (!this.TryOpenConnection(outerConnection, connectionFactory, null, null))
			{
				throw ADP.InternalError(ADP.InternalErrorCode.SynchronousConnectReturnedPending);
			}
		}

		internal virtual bool TryOpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, DbConnectionOptions userOptions)
		{
			throw ADP.ConnectionAlreadyOpen(this.State);
		}

		internal virtual bool TryReplaceConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, DbConnectionOptions userOptions)
		{
			throw ADP.MethodNotImplemented("TryReplaceConnection");
		}

		protected bool TryOpenConnectionInternal(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, DbConnectionOptions userOptions)
		{
			if (connectionFactory.SetInnerConnectionFrom(outerConnection, DbConnectionClosedConnecting.SingletonInstance, this))
			{
				DbConnectionInternal dbConnectionInternal = null;
				try
				{
					connectionFactory.PermissionDemand(outerConnection);
					if (!connectionFactory.TryGetConnection(outerConnection, retry, userOptions, this, out dbConnectionInternal))
					{
						return false;
					}
				}
				catch
				{
					connectionFactory.SetInnerConnectionTo(outerConnection, this);
					throw;
				}
				if (dbConnectionInternal == null)
				{
					connectionFactory.SetInnerConnectionTo(outerConnection, this);
					throw ADP.InternalConnectionError(ADP.ConnectionError.GetConnectionReturnsNull);
				}
				connectionFactory.SetInnerConnectionEvent(outerConnection, dbConnectionInternal);
				return true;
			}
			return true;
		}

		internal void PrePush(object expectedOwner)
		{
			if (expectedOwner == null)
			{
				if (this._owningObject.Target != null)
				{
					throw ADP.InternalError(ADP.InternalErrorCode.UnpooledObjectHasOwner);
				}
			}
			else if (this._owningObject.Target != expectedOwner)
			{
				throw ADP.InternalError(ADP.InternalErrorCode.UnpooledObjectHasWrongOwner);
			}
			if (this._pooledCount != 0)
			{
				throw ADP.InternalError(ADP.InternalErrorCode.PushingObjectSecondTime);
			}
			this._pooledCount++;
			this._owningObject.Target = null;
		}

		internal void PostPop(object newOwner)
		{
			if (this._owningObject.Target != null)
			{
				throw ADP.InternalError(ADP.InternalErrorCode.PooledObjectHasOwner);
			}
			this._owningObject.Target = newOwner;
			this._pooledCount--;
			if (this.Pool != null)
			{
				if (this._pooledCount != 0)
				{
					throw ADP.InternalError(ADP.InternalErrorCode.PooledObjectInPoolMoreThanOnce);
				}
			}
			else if (-1 != this._pooledCount)
			{
				throw ADP.InternalError(ADP.InternalErrorCode.NonPooledObjectUsedMoreThanOnce);
			}
		}

		internal void RemoveWeakReference(object value)
		{
			DbReferenceCollection referenceCollection = this.ReferenceCollection;
			if (referenceCollection != null)
			{
				referenceCollection.Remove(value);
			}
		}

		internal virtual bool IsConnectionAlive(bool throwOnException = false)
		{
			return true;
		}

		protected internal Transaction EnlistedTransaction
		{
			get
			{
				return this._enlistedTransaction;
			}
			set
			{
				Transaction enlistedTransaction = this._enlistedTransaction;
				if ((null == enlistedTransaction && null != value) || (null != enlistedTransaction && !enlistedTransaction.Equals(value)))
				{
					Transaction transaction = null;
					Transaction transaction2 = null;
					try
					{
						if (null != value)
						{
							transaction = value.Clone();
						}
						lock (this)
						{
							transaction2 = Interlocked.Exchange<Transaction>(ref this._enlistedTransaction, transaction);
							this._enlistedTransactionOriginal = value;
							value = transaction;
							transaction = null;
						}
					}
					finally
					{
						if (null != transaction2 && transaction2 != this._enlistedTransaction)
						{
							transaction2.Dispose();
						}
						if (null != transaction && transaction != this._enlistedTransaction)
						{
							transaction.Dispose();
						}
					}
					if (null != value)
					{
						this.TransactionOutcomeEnlist(value);
					}
				}
			}
		}

		protected bool EnlistedTransactionDisposed
		{
			get
			{
				bool flag2;
				try
				{
					Transaction enlistedTransactionOriginal = this._enlistedTransactionOriginal;
					bool flag = enlistedTransactionOriginal != null && enlistedTransactionOriginal.TransactionInformation == null;
					flag2 = flag;
				}
				catch (ObjectDisposedException)
				{
					flag2 = true;
				}
				return flag2;
			}
		}

		internal bool IsTxRootWaitingForTxEnd
		{
			get
			{
				return this._isInStasis;
			}
		}

		protected virtual bool UnbindOnTransactionCompletion
		{
			get
			{
				return true;
			}
		}

		protected internal virtual bool IsNonPoolableTransactionRoot
		{
			get
			{
				return false;
			}
		}

		internal virtual bool IsTransactionRoot
		{
			get
			{
				return false;
			}
		}

		protected virtual bool ReadyToPrepareTransaction
		{
			get
			{
				return true;
			}
		}

		protected abstract void Activate(Transaction transaction);

		internal void ActivateConnection(Transaction transaction)
		{
			this.Activate(transaction);
		}

		internal virtual void CloseConnection(DbConnection owningObject, DbConnectionFactory connectionFactory)
		{
			if (connectionFactory.SetInnerConnectionFrom(owningObject, DbConnectionOpenBusy.SingletonInstance, this))
			{
				lock (this)
				{
					object obj = this.ObtainAdditionalLocksForClose();
					try
					{
						this.PrepareForCloseConnection();
						DbConnectionPool pool = this.Pool;
						this.DetachCurrentTransactionIfEnded();
						if (pool != null)
						{
							pool.PutObject(this, owningObject);
						}
						else
						{
							this.Deactivate();
							this._owningObject.Target = null;
							if (this.IsTransactionRoot)
							{
								this.SetInStasis();
							}
							else
							{
								this.Dispose();
							}
						}
					}
					finally
					{
						this.ReleaseAdditionalLocksForClose(obj);
						connectionFactory.SetInnerConnectionEvent(owningObject, DbConnectionClosedPreviouslyOpened.SingletonInstance);
					}
				}
			}
		}

		internal virtual void DelegatedTransactionEnded()
		{
			if (1 != this._pooledCount)
			{
				if (-1 == this._pooledCount && !this._owningObject.IsAlive)
				{
					this.TerminateStasis(false);
					this.Deactivate();
					this.Dispose();
				}
				return;
			}
			this.TerminateStasis(true);
			this.Deactivate();
			DbConnectionPool pool = this.Pool;
			if (pool == null)
			{
				throw ADP.InternalError(ADP.InternalErrorCode.PooledObjectWithoutPool);
			}
			pool.PutObjectFromTransactedPool(this);
		}

		public virtual void Dispose()
		{
			this._connectionPool = null;
			this._connectionIsDoomed = true;
			this._enlistedTransactionOriginal = null;
			Transaction transaction = Interlocked.Exchange<Transaction>(ref this._enlistedTransaction, null);
			if (transaction != null)
			{
				transaction.Dispose();
			}
		}

		public abstract void EnlistTransaction(Transaction transaction);

		protected virtual void CleanupTransactionOnCompletion(Transaction transaction)
		{
		}

		internal void DetachCurrentTransactionIfEnded()
		{
			Transaction enlistedTransaction = this.EnlistedTransaction;
			if (enlistedTransaction != null)
			{
				bool flag;
				try
				{
					flag = enlistedTransaction.TransactionInformation.Status > TransactionStatus.Active;
				}
				catch (TransactionException)
				{
					flag = true;
				}
				if (flag)
				{
					this.DetachTransaction(enlistedTransaction, true);
				}
			}
		}

		internal void DetachTransaction(Transaction transaction, bool isExplicitlyReleasing)
		{
			lock (this)
			{
				DbConnection dbConnection = (DbConnection)this.Owner;
				if (isExplicitlyReleasing || this.UnbindOnTransactionCompletion || dbConnection == null)
				{
					Transaction enlistedTransaction = this._enlistedTransaction;
					if (enlistedTransaction != null && transaction.Equals(enlistedTransaction))
					{
						this.EnlistedTransaction = null;
						if (this.IsTxRootWaitingForTxEnd)
						{
							this.DelegatedTransactionEnded();
						}
					}
				}
			}
		}

		internal void CleanupConnectionOnTransactionCompletion(Transaction transaction)
		{
			this.DetachTransaction(transaction, false);
			DbConnectionPool pool = this.Pool;
			if (pool != null)
			{
				pool.TransactionEnded(transaction, this);
			}
		}

		private void TransactionCompletedEvent(object sender, TransactionEventArgs e)
		{
			Transaction transaction = e.Transaction;
			this.CleanupTransactionOnCompletion(transaction);
			this.CleanupConnectionOnTransactionCompletion(transaction);
		}

		private void TransactionOutcomeEnlist(Transaction transaction)
		{
			transaction.TransactionCompleted += this.TransactionCompletedEvent;
		}

		internal void SetInStasis()
		{
			this._isInStasis = true;
		}

		private void TerminateStasis(bool returningToPool)
		{
			this._isInStasis = false;
		}

		internal static readonly StateChangeEventArgs StateChangeClosed = new StateChangeEventArgs(ConnectionState.Open, ConnectionState.Closed);

		internal static readonly StateChangeEventArgs StateChangeOpen = new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open);

		private readonly bool _allowSetConnectionString;

		private readonly bool _hidePassword;

		private readonly ConnectionState _state;

		private readonly WeakReference _owningObject = new WeakReference(null, false);

		private DbConnectionPool _connectionPool;

		private DbReferenceCollection _referenceCollection;

		private int _pooledCount;

		private bool _connectionIsDoomed;

		private bool _cannotBePooled;

		private DateTime _createTime;

		private bool _isInStasis;

		private Transaction _enlistedTransaction;

		private Transaction _enlistedTransactionOriginal;
	}
}
