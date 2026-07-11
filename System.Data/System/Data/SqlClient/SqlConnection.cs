using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.ProviderBase;
using System.EnterpriseServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Unity;

namespace System.Data.SqlClient
{
	public sealed class SqlConnection : DbConnection, ICloneable, IDbConnection, IDisposable
	{
		public SqlConnection(string connectionString)
			: this()
		{
			this.ConnectionString = connectionString;
			this.CacheConnectionStringProperties();
		}

		private SqlConnection(SqlConnection connection)
		{
			this._reconnectLock = new object();
			this._originalConnectionId = Guid.Empty;
			base..ctor();
			GC.SuppressFinalize(this);
			this.CopyFrom(connection);
			this._connectionString = connection._connectionString;
			this.CacheConnectionStringProperties();
		}

		private void CacheConnectionStringProperties()
		{
			SqlConnectionString sqlConnectionString = this.ConnectionOptions as SqlConnectionString;
			if (sqlConnectionString != null)
			{
				this._connectRetryCount = sqlConnectionString.ConnectRetryCount;
			}
		}

		public bool StatisticsEnabled
		{
			get
			{
				return this._collectstats;
			}
			set
			{
				if (value)
				{
					if (ConnectionState.Open == this.State)
					{
						if (this._statistics == null)
						{
							this._statistics = new SqlStatistics();
							ADP.TimerCurrent(out this._statistics._openTimestamp);
						}
						this.Parser.Statistics = this._statistics;
					}
				}
				else if (this._statistics != null && ConnectionState.Open == this.State)
				{
					this.Parser.Statistics = null;
					ADP.TimerCurrent(out this._statistics._closeTimestamp);
				}
				this._collectstats = value;
			}
		}

		internal bool AsyncCommandInProgress
		{
			get
			{
				return this._AsyncCommandInProgress;
			}
			set
			{
				this._AsyncCommandInProgress = value;
			}
		}

		internal SqlConnectionString.TransactionBindingEnum TransactionBinding
		{
			get
			{
				return ((SqlConnectionString)this.ConnectionOptions).TransactionBinding;
			}
		}

		internal SqlConnectionString.TypeSystem TypeSystem
		{
			get
			{
				return ((SqlConnectionString)this.ConnectionOptions).TypeSystemVersion;
			}
		}

		internal int ConnectRetryInterval
		{
			get
			{
				return ((SqlConnectionString)this.ConnectionOptions).ConnectRetryInterval;
			}
		}

		public override string ConnectionString
		{
			get
			{
				return this.ConnectionString_Get();
			}
			set
			{
				this.ConnectionString_Set(new SqlConnectionPoolKey(value));
				this._connectionString = value;
				this.CacheConnectionStringProperties();
			}
		}

		public override int ConnectionTimeout
		{
			get
			{
				SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
				if (sqlConnectionString == null)
				{
					return 15;
				}
				return sqlConnectionString.ConnectTimeout;
			}
		}

		public override string Database
		{
			get
			{
				SqlInternalConnection sqlInternalConnection = this.InnerConnection as SqlInternalConnection;
				string text;
				if (sqlInternalConnection != null)
				{
					text = sqlInternalConnection.CurrentDatabase;
				}
				else
				{
					SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
					text = ((sqlConnectionString != null) ? sqlConnectionString.InitialCatalog : "");
				}
				return text;
			}
		}

		public override string DataSource
		{
			get
			{
				SqlInternalConnection sqlInternalConnection = this.InnerConnection as SqlInternalConnection;
				string text;
				if (sqlInternalConnection != null)
				{
					text = sqlInternalConnection.CurrentDataSource;
				}
				else
				{
					SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
					text = ((sqlConnectionString != null) ? sqlConnectionString.DataSource : "");
				}
				return text;
			}
		}

		public int PacketSize
		{
			get
			{
				SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
				int num;
				if (sqlInternalConnectionTds != null)
				{
					num = sqlInternalConnectionTds.PacketSize;
				}
				else
				{
					SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
					num = ((sqlConnectionString != null) ? sqlConnectionString.PacketSize : 8000);
				}
				return num;
			}
		}

		public Guid ClientConnectionId
		{
			get
			{
				SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
				if (sqlInternalConnectionTds != null)
				{
					return sqlInternalConnectionTds.ClientConnectionId;
				}
				Task currentReconnectionTask = this._currentReconnectionTask;
				if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
				{
					return this._originalConnectionId;
				}
				return Guid.Empty;
			}
		}

		public override string ServerVersion
		{
			get
			{
				return this.GetOpenTdsConnection().ServerVersion;
			}
		}

		public override ConnectionState State
		{
			get
			{
				Task currentReconnectionTask = this._currentReconnectionTask;
				if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
				{
					return ConnectionState.Open;
				}
				return this.InnerConnection.State;
			}
		}

		internal SqlStatistics Statistics
		{
			get
			{
				return this._statistics;
			}
		}

		public string WorkstationId
		{
			get
			{
				SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
				if (sqlConnectionString == null)
				{
					return string.Empty;
				}
				return sqlConnectionString.WorkstationId;
			}
		}

		protected override DbProviderFactory DbProviderFactory
		{
			get
			{
				return SqlClientFactory.Instance;
			}
		}

		public event SqlInfoMessageEventHandler InfoMessage;

		public bool FireInfoMessageEventOnUserErrors
		{
			get
			{
				return this._fireInfoMessageEventOnUserErrors;
			}
			set
			{
				this._fireInfoMessageEventOnUserErrors = value;
			}
		}

		internal int ReconnectCount
		{
			get
			{
				return this._reconnectCount;
			}
		}

		internal bool ForceNewConnection { get; set; }

		protected override void OnStateChange(StateChangeEventArgs stateChange)
		{
			if (!this._suppressStateChangeForReconnection)
			{
				base.OnStateChange(stateChange);
			}
		}

		public new SqlTransaction BeginTransaction()
		{
			return this.BeginTransaction(IsolationLevel.Unspecified, null);
		}

		public new SqlTransaction BeginTransaction(IsolationLevel iso)
		{
			return this.BeginTransaction(iso, null);
		}

		public SqlTransaction BeginTransaction(string transactionName)
		{
			return this.BeginTransaction(IsolationLevel.Unspecified, transactionName);
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			DbTransaction dbTransaction = this.BeginTransaction(isolationLevel);
			GC.KeepAlive(this);
			return dbTransaction;
		}

		public SqlTransaction BeginTransaction(IsolationLevel iso, string transactionName)
		{
			this.WaitForPendingReconnection();
			SqlStatistics sqlStatistics = null;
			SqlTransaction sqlTransaction2;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				bool flag = true;
				SqlTransaction sqlTransaction;
				do
				{
					sqlTransaction = this.GetOpenTdsConnection().BeginSqlTransaction(iso, transactionName, flag);
					flag = false;
				}
				while (sqlTransaction.InternalTransaction.ConnectionHasBeenRestored);
				GC.KeepAlive(this);
				sqlTransaction2 = sqlTransaction;
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
			return sqlTransaction2;
		}

		public override void ChangeDatabase(string database)
		{
			SqlStatistics sqlStatistics = null;
			this.RepairInnerConnection();
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this.InnerConnection.ChangeDatabase(database);
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
		}

		public static void ClearAllPools()
		{
			SqlConnectionFactory.SingletonInstance.ClearAllPools();
		}

		public static void ClearPool(SqlConnection connection)
		{
			ADP.CheckArgumentNull(connection, "connection");
			DbConnectionOptions userConnectionOptions = connection.UserConnectionOptions;
			if (userConnectionOptions != null)
			{
				SqlConnectionFactory.SingletonInstance.ClearPool(connection);
			}
		}

		private void CloseInnerConnection()
		{
			this.InnerConnection.CloseConnection(this, this.ConnectionFactory);
		}

		public override void Close()
		{
			ConnectionState state = this.State;
			Guid guid = default(Guid);
			Guid guid2 = default(Guid);
			if (state != ConnectionState.Closed)
			{
				guid = SqlConnection.s_diagnosticListener.WriteConnectionCloseBefore(this, "Close");
				guid2 = this.ClientConnectionId;
			}
			SqlStatistics sqlStatistics = null;
			Exception ex = null;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				Task currentReconnectionTask = this._currentReconnectionTask;
				if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
				{
					CancellationTokenSource reconnectionCancellationSource = this._reconnectionCancellationSource;
					if (reconnectionCancellationSource != null)
					{
						reconnectionCancellationSource.Cancel();
					}
					AsyncHelper.WaitForCompletion(currentReconnectionTask, 0, null, false);
					if (this.State != ConnectionState.Open)
					{
						this.OnStateChange(DbConnectionInternal.StateChangeClosed);
					}
				}
				this.CancelOpenAndWait();
				this.CloseInnerConnection();
				GC.SuppressFinalize(this);
				if (this.Statistics != null)
				{
					ADP.TimerCurrent(out this._statistics._closeTimestamp);
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
				if (state != ConnectionState.Closed)
				{
					if (ex != null)
					{
						SqlConnection.s_diagnosticListener.WriteConnectionCloseError(guid, guid2, this, ex, "Close");
					}
					else
					{
						SqlConnection.s_diagnosticListener.WriteConnectionCloseAfter(guid, guid2, this, "Close");
					}
				}
			}
		}

		public new SqlCommand CreateCommand()
		{
			return new SqlCommand(null, this);
		}

		private void DisposeMe(bool disposing)
		{
			if (!disposing)
			{
				SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
				if (sqlInternalConnectionTds != null && !sqlInternalConnectionTds.ConnectionOptions.Pooling)
				{
					TdsParser parser = sqlInternalConnectionTds.Parser;
					if (parser != null && parser._physicalStateObj != null)
					{
						parser._physicalStateObj.DecrementPendingCallbacks(false);
					}
				}
			}
		}

		public override void Open()
		{
			Guid guid = SqlConnection.s_diagnosticListener.WriteConnectionOpenBefore(this, "Open");
			this.PrepareStatisticsForNewConnection();
			SqlStatistics sqlStatistics = null;
			Exception ex = null;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				if (!this.TryOpen(null))
				{
					throw ADP.InternalError(ADP.InternalErrorCode.SynchronousConnectReturnedPending);
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
				if (ex != null)
				{
					SqlConnection.s_diagnosticListener.WriteConnectionOpenError(guid, this, ex, "Open");
				}
				else
				{
					SqlConnection.s_diagnosticListener.WriteConnectionOpenAfter(guid, this, "Open");
				}
			}
		}

		internal void RegisterWaitingForReconnect(Task waitingTask)
		{
			if (((SqlConnectionString)this.ConnectionOptions).MARS)
			{
				return;
			}
			Interlocked.CompareExchange<Task>(ref this._asyncWaitingForReconnection, waitingTask, null);
			if (this._asyncWaitingForReconnection != waitingTask)
			{
				throw SQL.MARSUnspportedOnConnection();
			}
		}

		private async Task ReconnectAsync(int timeout)
		{
			try
			{
				long commandTimeoutExpiration = 0L;
				if (timeout > 0)
				{
					commandTimeoutExpiration = ADP.TimerCurrent() + ADP.TimerFromSeconds(timeout);
				}
				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
				this._reconnectionCancellationSource = cancellationTokenSource;
				CancellationToken ctoken = cancellationTokenSource.Token;
				int retryCount = this._connectRetryCount;
				for (int attempt = 0; attempt < retryCount; attempt++)
				{
					if (ctoken.IsCancellationRequested)
					{
						return;
					}
					try
					{
						try
						{
							this.ForceNewConnection = true;
							await this.OpenAsync(ctoken).ConfigureAwait(false);
							this._reconnectCount++;
						}
						finally
						{
							this.ForceNewConnection = false;
						}
						return;
					}
					catch (SqlException ex)
					{
						if (attempt == retryCount - 1)
						{
							throw SQL.CR_AllAttemptsFailed(ex, this._originalConnectionId);
						}
						if (timeout > 0 && ADP.TimerRemaining(commandTimeoutExpiration) < ADP.TimerFromSeconds(this.ConnectRetryInterval))
						{
							throw SQL.CR_NextAttemptWillExceedQueryTimeout(ex, this._originalConnectionId);
						}
					}
					await Task.Delay(1000 * this.ConnectRetryInterval, ctoken).ConfigureAwait(false);
				}
				ctoken = default(CancellationToken);
			}
			finally
			{
				this._recoverySessionData = null;
				this._suppressStateChangeForReconnection = false;
			}
		}

		internal Task ValidateAndReconnect(Action beforeDisconnect, int timeout)
		{
			Task task = this._currentReconnectionTask;
			while (task != null && task.IsCompleted)
			{
				Interlocked.CompareExchange<Task>(ref this._currentReconnectionTask, null, task);
				task = this._currentReconnectionTask;
			}
			if (task == null)
			{
				if (this._connectRetryCount > 0)
				{
					SqlInternalConnectionTds openTdsConnection = this.GetOpenTdsConnection();
					if (openTdsConnection._sessionRecoveryAcknowledged && !openTdsConnection.Parser._physicalStateObj.ValidateSNIConnection())
					{
						if (openTdsConnection.Parser._sessionPool != null && openTdsConnection.Parser._sessionPool.ActiveSessionsCount > 0)
						{
							if (beforeDisconnect != null)
							{
								beforeDisconnect();
							}
							this.OnError(SQL.CR_UnrecoverableClient(this.ClientConnectionId), true, null);
						}
						SessionData currentSessionData = openTdsConnection.CurrentSessionData;
						if (currentSessionData._unrecoverableStatesCount == 0)
						{
							bool flag = false;
							object reconnectLock = this._reconnectLock;
							lock (reconnectLock)
							{
								openTdsConnection.CheckEnlistedTransactionBinding();
								task = this._currentReconnectionTask;
								if (task == null)
								{
									if (currentSessionData._unrecoverableStatesCount == 0)
									{
										this._originalConnectionId = this.ClientConnectionId;
										this._recoverySessionData = currentSessionData;
										if (beforeDisconnect != null)
										{
											beforeDisconnect();
										}
										try
										{
											this._suppressStateChangeForReconnection = true;
											openTdsConnection.DoomThisConnection();
										}
										catch (SqlException)
										{
										}
										task = Task.Run(() => this.ReconnectAsync(timeout));
										this._currentReconnectionTask = task;
									}
								}
								else
								{
									flag = true;
								}
							}
							if (flag && beforeDisconnect != null)
							{
								beforeDisconnect();
							}
						}
						else
						{
							if (beforeDisconnect != null)
							{
								beforeDisconnect();
							}
							this.OnError(SQL.CR_UnrecoverableServer(this.ClientConnectionId), true, null);
						}
					}
				}
			}
			else if (beforeDisconnect != null)
			{
				beforeDisconnect();
			}
			return task;
		}

		private void WaitForPendingReconnection()
		{
			Task currentReconnectionTask = this._currentReconnectionTask;
			if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
			{
				AsyncHelper.WaitForCompletion(currentReconnectionTask, 0, null, false);
			}
		}

		private void CancelOpenAndWait()
		{
			Tuple<TaskCompletionSource<DbConnectionInternal>, Task> currentCompletion = this._currentCompletion;
			if (currentCompletion != null)
			{
				currentCompletion.Item1.TrySetCanceled();
				((IAsyncResult)currentCompletion.Item2).AsyncWaitHandle.WaitOne();
			}
		}

		public override Task OpenAsync(CancellationToken cancellationToken)
		{
			Guid operationId = SqlConnection.s_diagnosticListener.WriteConnectionOpenBefore(this, "OpenAsync");
			this.PrepareStatisticsForNewConnection();
			SqlStatistics sqlStatistics = null;
			Task task;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				TaskCompletionSource<DbConnectionInternal> taskCompletionSource = new TaskCompletionSource<DbConnectionInternal>(ADP.GetCurrentTransaction());
				TaskCompletionSource<object> taskCompletionSource2 = new TaskCompletionSource<object>();
				if (SqlConnection.s_diagnosticListener.IsEnabled("System.Data.SqlClient.WriteConnectionOpenAfter") || SqlConnection.s_diagnosticListener.IsEnabled("System.Data.SqlClient.WriteConnectionOpenError"))
				{
					taskCompletionSource2.Task.ContinueWith(delegate(Task<object> t)
					{
						if (t.Exception != null)
						{
							SqlConnection.s_diagnosticListener.WriteConnectionOpenError(operationId, this, t.Exception, "OpenAsync");
							return;
						}
						SqlConnection.s_diagnosticListener.WriteConnectionOpenAfter(operationId, this, "OpenAsync");
					}, TaskScheduler.Default);
				}
				if (cancellationToken.IsCancellationRequested)
				{
					taskCompletionSource2.SetCanceled();
					task = taskCompletionSource2.Task;
				}
				else
				{
					bool flag;
					try
					{
						flag = this.TryOpen(taskCompletionSource);
					}
					catch (Exception ex)
					{
						SqlConnection.s_diagnosticListener.WriteConnectionOpenError(operationId, this, ex, "OpenAsync");
						taskCompletionSource2.SetException(ex);
						return taskCompletionSource2.Task;
					}
					if (flag)
					{
						taskCompletionSource2.SetResult(null);
						task = taskCompletionSource2.Task;
					}
					else
					{
						CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
						if (cancellationToken.CanBeCanceled)
						{
							cancellationTokenRegistration = cancellationToken.Register(delegate(object s)
							{
								((TaskCompletionSource<DbConnectionInternal>)s).TrySetCanceled();
							}, taskCompletionSource);
						}
						SqlConnection.OpenAsyncRetry openAsyncRetry = new SqlConnection.OpenAsyncRetry(this, taskCompletionSource, taskCompletionSource2, cancellationTokenRegistration);
						this._currentCompletion = new Tuple<TaskCompletionSource<DbConnectionInternal>, Task>(taskCompletionSource, taskCompletionSource2.Task);
						taskCompletionSource.Task.ContinueWith(new Action<Task<DbConnectionInternal>>(openAsyncRetry.Retry), TaskScheduler.Default);
						task = taskCompletionSource2.Task;
					}
				}
			}
			catch (Exception ex2)
			{
				SqlConnection.s_diagnosticListener.WriteConnectionOpenError(operationId, this, ex2, "OpenAsync");
				throw;
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
			return task;
		}

		public override DataTable GetSchema()
		{
			return this.GetSchema(DbMetaDataCollectionNames.MetaDataCollections, null);
		}

		public override DataTable GetSchema(string collectionName)
		{
			return this.GetSchema(collectionName, null);
		}

		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			return this.InnerConnection.GetSchema(this.ConnectionFactory, this.PoolGroup, this, collectionName, restrictionValues);
		}

		private void PrepareStatisticsForNewConnection()
		{
			if (this.StatisticsEnabled || SqlConnection.s_diagnosticListener.IsEnabled("System.Data.SqlClient.WriteCommandAfter") || SqlConnection.s_diagnosticListener.IsEnabled("System.Data.SqlClient.WriteConnectionOpenAfter"))
			{
				if (this._statistics == null)
				{
					this._statistics = new SqlStatistics();
					return;
				}
				this._statistics.ContinueOnNewConnection();
			}
		}

		private bool TryOpen(TaskCompletionSource<DbConnectionInternal> retry)
		{
			SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
			this._applyTransientFaultHandling = retry == null && sqlConnectionString != null && sqlConnectionString.ConnectRetryCount > 0;
			if (this.ForceNewConnection)
			{
				if (!this.InnerConnection.TryReplaceConnection(this, this.ConnectionFactory, retry, this.UserConnectionOptions))
				{
					return false;
				}
			}
			else if (!this.InnerConnection.TryOpenConnection(this, this.ConnectionFactory, retry, this.UserConnectionOptions))
			{
				return false;
			}
			SqlInternalConnectionTds sqlInternalConnectionTds = (SqlInternalConnectionTds)this.InnerConnection;
			if (!sqlInternalConnectionTds.ConnectionOptions.Pooling)
			{
				GC.ReRegisterForFinalize(this);
			}
			SqlStatistics statistics = this._statistics;
			if (this.StatisticsEnabled || (SqlConnection.s_diagnosticListener.IsEnabled("System.Data.SqlClient.WriteCommandAfter") && statistics != null))
			{
				ADP.TimerCurrent(out this._statistics._openTimestamp);
				sqlInternalConnectionTds.Parser.Statistics = this._statistics;
			}
			else
			{
				sqlInternalConnectionTds.Parser.Statistics = null;
				this._statistics = null;
			}
			return true;
		}

		internal bool HasLocalTransaction
		{
			get
			{
				return this.GetOpenTdsConnection().HasLocalTransaction;
			}
		}

		internal bool HasLocalTransactionFromAPI
		{
			get
			{
				Task currentReconnectionTask = this._currentReconnectionTask;
				return (currentReconnectionTask == null || currentReconnectionTask.IsCompleted) && this.GetOpenTdsConnection().HasLocalTransactionFromAPI;
			}
		}

		internal bool IsKatmaiOrNewer
		{
			get
			{
				return this._currentReconnectionTask != null || this.GetOpenTdsConnection().IsKatmaiOrNewer;
			}
		}

		internal TdsParser Parser
		{
			get
			{
				return this.GetOpenTdsConnection().Parser;
			}
		}

		internal void ValidateConnectionForExecute(string method, SqlCommand command)
		{
			Task asyncWaitingForReconnection = this._asyncWaitingForReconnection;
			if (asyncWaitingForReconnection != null)
			{
				if (!asyncWaitingForReconnection.IsCompleted)
				{
					throw SQL.MARSUnspportedOnConnection();
				}
				Interlocked.CompareExchange<Task>(ref this._asyncWaitingForReconnection, null, asyncWaitingForReconnection);
			}
			if (this._currentReconnectionTask != null)
			{
				Task currentReconnectionTask = this._currentReconnectionTask;
				if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
				{
					return;
				}
			}
			this.GetOpenTdsConnection(method).ValidateConnectionForExecute(command);
		}

		internal static string FixupDatabaseTransactionName(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				return SqlServerEscapeHelper.EscapeIdentifier(name);
			}
			return name;
		}

		internal void OnError(SqlException exception, bool breakConnection, Action<Action> wrapCloseInAction)
		{
			if (breakConnection && ConnectionState.Open == this.State)
			{
				if (wrapCloseInAction != null)
				{
					int capturedCloseCount = this._closeCount;
					Action action = delegate
					{
						if (capturedCloseCount == this._closeCount)
						{
							this.Close();
						}
					};
					wrapCloseInAction(action);
				}
				else
				{
					this.Close();
				}
			}
			if (exception.Class >= 11)
			{
				throw exception;
			}
			this.OnInfoMessage(new SqlInfoMessageEventArgs(exception));
		}

		internal SqlInternalConnectionTds GetOpenTdsConnection()
		{
			SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
			if (sqlInternalConnectionTds == null)
			{
				throw ADP.ClosedConnectionError();
			}
			return sqlInternalConnectionTds;
		}

		internal SqlInternalConnectionTds GetOpenTdsConnection(string method)
		{
			SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
			if (sqlInternalConnectionTds == null)
			{
				throw ADP.OpenConnectionRequired(method, this.InnerConnection.State);
			}
			return sqlInternalConnectionTds;
		}

		internal void OnInfoMessage(SqlInfoMessageEventArgs imevent)
		{
			bool flag;
			this.OnInfoMessage(imevent, out flag);
		}

		internal void OnInfoMessage(SqlInfoMessageEventArgs imevent, out bool notified)
		{
			SqlInfoMessageEventHandler infoMessage = this.InfoMessage;
			if (infoMessage != null)
			{
				notified = true;
				try
				{
					infoMessage(this, imevent);
					return;
				}
				catch (Exception ex)
				{
					if (!ADP.IsCatchableOrSecurityExceptionType(ex))
					{
						throw;
					}
					return;
				}
			}
			notified = false;
		}

		internal void RegisterForConnectionCloseNotification<T>(ref Task<T> outerTask, object value, int tag)
		{
			outerTask = outerTask.ContinueWith<Task<T>>(delegate(Task<T> task)
			{
				this.RemoveWeakReference(value);
				return task;
			}, TaskScheduler.Default).Unwrap<T>();
		}

		public void ResetStatistics()
		{
			if (this.Statistics != null)
			{
				this.Statistics.Reset();
				if (ConnectionState.Open == this.State)
				{
					ADP.TimerCurrent(out this._statistics._openTimestamp);
				}
			}
		}

		public IDictionary RetrieveStatistics()
		{
			if (this.Statistics != null)
			{
				this.UpdateStatistics();
				return this.Statistics.GetDictionary();
			}
			return new SqlStatistics().GetDictionary();
		}

		private void UpdateStatistics()
		{
			if (ConnectionState.Open == this.State)
			{
				ADP.TimerCurrent(out this._statistics._closeTimestamp);
			}
			this.Statistics.UpdateStatistics();
		}

		object ICloneable.Clone()
		{
			return new SqlConnection(this);
		}

		private void CopyFrom(SqlConnection connection)
		{
			ADP.CheckArgumentNull(connection, "connection");
			this._userConnectionOptions = connection.UserConnectionOptions;
			this._poolGroup = connection.PoolGroup;
			if (DbConnectionClosedNeverOpened.SingletonInstance == connection._innerConnection)
			{
				this._innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
				return;
			}
			this._innerConnection = DbConnectionClosedPreviouslyOpened.SingletonInstance;
		}

		public SqlConnection()
		{
			this._reconnectLock = new object();
			this._originalConnectionId = Guid.Empty;
			base..ctor();
			GC.SuppressFinalize(this);
			this._innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
		}

		internal int CloseCount
		{
			get
			{
				return this._closeCount;
			}
		}

		internal DbConnectionFactory ConnectionFactory
		{
			get
			{
				return SqlConnection.s_connectionFactory;
			}
		}

		internal DbConnectionOptions ConnectionOptions
		{
			get
			{
				DbConnectionPoolGroup poolGroup = this.PoolGroup;
				if (poolGroup == null)
				{
					return null;
				}
				return poolGroup.ConnectionOptions;
			}
		}

		private string ConnectionString_Get()
		{
			bool shouldHidePassword = this.InnerConnection.ShouldHidePassword;
			DbConnectionOptions userConnectionOptions = this.UserConnectionOptions;
			if (userConnectionOptions == null)
			{
				return "";
			}
			return userConnectionOptions.UsersConnectionString(shouldHidePassword);
		}

		private void ConnectionString_Set(DbConnectionPoolKey key)
		{
			DbConnectionOptions dbConnectionOptions = null;
			DbConnectionPoolGroup connectionPoolGroup = this.ConnectionFactory.GetConnectionPoolGroup(key, null, ref dbConnectionOptions);
			DbConnectionInternal innerConnection = this.InnerConnection;
			bool flag = innerConnection.AllowSetConnectionString;
			if (flag)
			{
				flag = this.SetInnerConnectionFrom(DbConnectionClosedBusy.SingletonInstance, innerConnection);
				if (flag)
				{
					this._userConnectionOptions = dbConnectionOptions;
					this._poolGroup = connectionPoolGroup;
					this._innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
				}
			}
			if (!flag)
			{
				throw ADP.OpenConnectionPropertySet("ConnectionString", innerConnection.State);
			}
		}

		internal DbConnectionInternal InnerConnection
		{
			get
			{
				return this._innerConnection;
			}
		}

		internal DbConnectionPoolGroup PoolGroup
		{
			get
			{
				return this._poolGroup;
			}
			set
			{
				this._poolGroup = value;
			}
		}

		internal DbConnectionOptions UserConnectionOptions
		{
			get
			{
				return this._userConnectionOptions;
			}
		}

		internal void Abort(Exception e)
		{
			DbConnectionInternal innerConnection = this._innerConnection;
			if (ConnectionState.Open == innerConnection.State)
			{
				Interlocked.CompareExchange<DbConnectionInternal>(ref this._innerConnection, DbConnectionClosedPreviouslyOpened.SingletonInstance, innerConnection);
				innerConnection.DoomThisConnection();
			}
		}

		internal void AddWeakReference(object value, int tag)
		{
			this.InnerConnection.AddWeakReference(value, tag);
		}

		protected override DbCommand CreateDbCommand()
		{
			DbCommand dbCommand = this.ConnectionFactory.ProviderFactory.CreateCommand();
			dbCommand.Connection = this;
			return dbCommand;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._userConnectionOptions = null;
				this._poolGroup = null;
				this.Close();
			}
			this.DisposeMe(disposing);
			base.Dispose(disposing);
		}

		private void RepairInnerConnection()
		{
			this.WaitForPendingReconnection();
			if (this._connectRetryCount == 0)
			{
				return;
			}
			SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
			if (sqlInternalConnectionTds != null)
			{
				sqlInternalConnectionTds.ValidateConnectionForExecute(null);
				sqlInternalConnectionTds.GetSessionAndReconnectIfNeeded(this, 0);
			}
		}

		public override void EnlistTransaction(Transaction transaction)
		{
			Transaction enlistedTransaction = this.InnerConnection.EnlistedTransaction;
			if (enlistedTransaction != null)
			{
				if (enlistedTransaction.Equals(transaction))
				{
					return;
				}
				if (enlistedTransaction.TransactionInformation.Status == global::System.Transactions.TransactionStatus.Active)
				{
					throw ADP.TransactionPresent();
				}
			}
			this.RepairInnerConnection();
			this.InnerConnection.EnlistTransaction(transaction);
			GC.KeepAlive(this);
		}

		internal void NotifyWeakReference(int message)
		{
			this.InnerConnection.NotifyWeakReference(message);
		}

		internal void PermissionDemand()
		{
			DbConnectionPoolGroup poolGroup = this.PoolGroup;
			DbConnectionOptions dbConnectionOptions = ((poolGroup != null) ? poolGroup.ConnectionOptions : null);
			if (dbConnectionOptions == null || dbConnectionOptions.IsEmpty)
			{
				throw ADP.NoConnectionString();
			}
			DbConnectionOptions userConnectionOptions = this.UserConnectionOptions;
		}

		internal void RemoveWeakReference(object value)
		{
			this.InnerConnection.RemoveWeakReference(value);
		}

		internal void SetInnerConnectionEvent(DbConnectionInternal to)
		{
			ConnectionState connectionState = this._innerConnection.State & ConnectionState.Open;
			ConnectionState connectionState2 = to.State & ConnectionState.Open;
			if (connectionState != connectionState2 && connectionState2 == ConnectionState.Closed)
			{
				this._closeCount++;
			}
			this._innerConnection = to;
			if (connectionState == ConnectionState.Closed && ConnectionState.Open == connectionState2)
			{
				this.OnStateChange(DbConnectionInternal.StateChangeOpen);
				return;
			}
			if (ConnectionState.Open == connectionState && connectionState2 == ConnectionState.Closed)
			{
				this.OnStateChange(DbConnectionInternal.StateChangeClosed);
				return;
			}
			if (connectionState != connectionState2)
			{
				this.OnStateChange(new StateChangeEventArgs(connectionState, connectionState2));
			}
		}

		internal bool SetInnerConnectionFrom(DbConnectionInternal to, DbConnectionInternal from)
		{
			return from == Interlocked.CompareExchange<DbConnectionInternal>(ref this._innerConnection, to, from);
		}

		internal void SetInnerConnectionTo(DbConnectionInternal to)
		{
			this._innerConnection = to;
		}

		public SqlConnection(string connectionString, SqlCredential credential)
		{
			this._reconnectLock = new object();
			this._originalConnectionId = Guid.Empty;
			base..ctor();
			this.ConnectionString = connectionString;
			this.Credentials = credential;
		}

		[MonoTODO]
		public static void ChangePassword(string connectionString, string newPassword)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public SqlCredential Credentials
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		public void EnlistDistributedTransaction(ITransaction transaction)
		{
			throw new NotImplementedException();
		}

		public string AccessToken
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		public static TimeSpan ColumnEncryptionKeyCacheTtl
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(TimeSpan);
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		public static bool ColumnEncryptionQueryMetadataCacheEnabled
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		public static IDictionary<string, IList<string>> ColumnEncryptionTrustedMasterKeyPaths
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public SqlCredential Credential
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		public static void ChangePassword(string connectionString, SqlCredential credential, SecureString newSecurePassword)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		public static void RegisterColumnEncryptionKeyStoreProviders(IDictionary<string, SqlColumnEncryptionKeyStoreProvider> customProviders)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private bool _AsyncCommandInProgress;

		internal SqlStatistics _statistics;

		private bool _collectstats;

		private bool _fireInfoMessageEventOnUserErrors;

		private Tuple<TaskCompletionSource<DbConnectionInternal>, Task> _currentCompletion;

		private string _connectionString;

		private int _connectRetryCount;

		private object _reconnectLock;

		internal Task _currentReconnectionTask;

		private Task _asyncWaitingForReconnection;

		private Guid _originalConnectionId;

		private CancellationTokenSource _reconnectionCancellationSource;

		internal SessionData _recoverySessionData;

		internal new bool _suppressStateChangeForReconnection;

		private int _reconnectCount;

		private static readonly DiagnosticListener s_diagnosticListener = new DiagnosticListener("SqlClientDiagnosticListener");

		internal bool _applyTransientFaultHandling;

		private static readonly DbConnectionFactory s_connectionFactory = SqlConnectionFactory.SingletonInstance;

		private DbConnectionOptions _userConnectionOptions;

		private DbConnectionPoolGroup _poolGroup;

		private DbConnectionInternal _innerConnection;

		private int _closeCount;

		private class OpenAsyncRetry
		{
			public OpenAsyncRetry(SqlConnection parent, TaskCompletionSource<DbConnectionInternal> retry, TaskCompletionSource<object> result, CancellationTokenRegistration registration)
			{
				this._parent = parent;
				this._retry = retry;
				this._result = result;
				this._registration = registration;
			}

			internal void Retry(Task<DbConnectionInternal> retryTask)
			{
				this._registration.Dispose();
				try
				{
					SqlStatistics sqlStatistics = null;
					try
					{
						sqlStatistics = SqlStatistics.StartTimer(this._parent.Statistics);
						if (retryTask.IsFaulted)
						{
							Exception innerException = retryTask.Exception.InnerException;
							this._parent.CloseInnerConnection();
							this._parent._currentCompletion = null;
							this._result.SetException(retryTask.Exception.InnerException);
						}
						else if (retryTask.IsCanceled)
						{
							this._parent.CloseInnerConnection();
							this._parent._currentCompletion = null;
							this._result.SetCanceled();
						}
						else
						{
							DbConnectionInternal innerConnection = this._parent.InnerConnection;
							bool flag2;
							lock (innerConnection)
							{
								flag2 = this._parent.TryOpen(this._retry);
							}
							if (flag2)
							{
								this._parent._currentCompletion = null;
								this._result.SetResult(null);
							}
							else
							{
								this._parent.CloseInnerConnection();
								this._parent._currentCompletion = null;
								this._result.SetException(ADP.ExceptionWithStackTrace(ADP.InternalError(ADP.InternalErrorCode.CompletedConnectReturnedPending)));
							}
						}
					}
					finally
					{
						SqlStatistics.StopTimer(sqlStatistics);
					}
				}
				catch (Exception ex)
				{
					this._parent.CloseInnerConnection();
					this._parent._currentCompletion = null;
					this._result.SetException(ex);
				}
			}

			private SqlConnection _parent;

			private TaskCompletionSource<DbConnectionInternal> _retry;

			private TaskCompletionSource<object> _result;

			private CancellationTokenRegistration _registration;
		}
	}
}
