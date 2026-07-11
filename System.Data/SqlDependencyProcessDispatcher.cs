using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.ProviderBase;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Threading;
using System.Xml;

internal class SqlDependencyProcessDispatcher : MarshalByRefObject
{
	private SqlDependencyProcessDispatcher(object dummyVariable)
	{
		this._connectionContainers = new Dictionary<SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper, SqlDependencyProcessDispatcher.SqlConnectionContainer>();
		this._sqlDependencyPerAppDomainDispatchers = new Dictionary<string, SqlDependencyPerAppDomainDispatcher>();
	}

	public SqlDependencyProcessDispatcher()
	{
	}

	internal static SqlDependencyProcessDispatcher SingletonProcessDispatcher
	{
		get
		{
			return SqlDependencyProcessDispatcher.s_staticInstance;
		}
	}

	private static SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper GetHashHelper(string connectionString, out SqlConnectionStringBuilder connectionStringBuilder, out DbConnectionPoolIdentity identity, out string user, string queue)
	{
		connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
		{
			Pooling = false,
			Enlist = false,
			ConnectRetryCount = 0
		};
		if (queue != null)
		{
			connectionStringBuilder.ApplicationName = queue;
		}
		if (connectionStringBuilder.IntegratedSecurity)
		{
			identity = DbConnectionPoolIdentity.GetCurrent();
			user = null;
		}
		else
		{
			identity = null;
			user = connectionStringBuilder.UserID;
		}
		return new SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper(identity, connectionStringBuilder.ConnectionString, queue, connectionStringBuilder);
	}

	public override object InitializeLifetimeService()
	{
		return null;
	}

	private void Invalidate(string server, SqlNotification sqlNotification)
	{
		Dictionary<string, SqlDependencyPerAppDomainDispatcher> sqlDependencyPerAppDomainDispatchers = this._sqlDependencyPerAppDomainDispatchers;
		lock (sqlDependencyPerAppDomainDispatchers)
		{
			foreach (KeyValuePair<string, SqlDependencyPerAppDomainDispatcher> keyValuePair in this._sqlDependencyPerAppDomainDispatchers)
			{
				SqlDependencyPerAppDomainDispatcher value = keyValuePair.Value;
				try
				{
					value.InvalidateServer(server, sqlNotification);
				}
				catch (Exception ex)
				{
					if (!ADP.IsCatchableExceptionType(ex))
					{
						throw;
					}
					ADP.TraceExceptionWithoutRethrow(ex);
				}
			}
		}
	}

	internal void QueueAppDomainUnloading(string appDomainKey)
	{
		ThreadPool.QueueUserWorkItem(new WaitCallback(this.AppDomainUnloading), appDomainKey);
	}

	private void AppDomainUnloading(object state)
	{
		string text = (string)state;
		Dictionary<SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper, SqlDependencyProcessDispatcher.SqlConnectionContainer> connectionContainers = this._connectionContainers;
		lock (connectionContainers)
		{
			List<SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper> list = new List<SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper>();
			foreach (KeyValuePair<SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper, SqlDependencyProcessDispatcher.SqlConnectionContainer> keyValuePair in this._connectionContainers)
			{
				SqlDependencyProcessDispatcher.SqlConnectionContainer value = keyValuePair.Value;
				if (value.AppDomainUnload(text))
				{
					list.Add(value.HashHelper);
				}
			}
			foreach (SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper sqlConnectionContainerHashHelper in list)
			{
				this._connectionContainers.Remove(sqlConnectionContainerHashHelper);
			}
		}
		Dictionary<string, SqlDependencyPerAppDomainDispatcher> sqlDependencyPerAppDomainDispatchers = this._sqlDependencyPerAppDomainDispatchers;
		lock (sqlDependencyPerAppDomainDispatchers)
		{
			this._sqlDependencyPerAppDomainDispatchers.Remove(text);
		}
	}

	internal bool StartWithDefault(string connectionString, out string server, out DbConnectionPoolIdentity identity, out string user, out string database, ref string service, string appDomainKey, SqlDependencyPerAppDomainDispatcher dispatcher, out bool errorOccurred, out bool appDomainStart)
	{
		return this.Start(connectionString, out server, out identity, out user, out database, ref service, appDomainKey, dispatcher, out errorOccurred, out appDomainStart, true);
	}

	internal bool Start(string connectionString, string queue, string appDomainKey, SqlDependencyPerAppDomainDispatcher dispatcher)
	{
		string text;
		DbConnectionPoolIdentity dbConnectionPoolIdentity;
		bool flag;
		return this.Start(connectionString, out text, out dbConnectionPoolIdentity, out text, out text, ref queue, appDomainKey, dispatcher, out flag, out flag, false);
	}

	private bool Start(string connectionString, out string server, out DbConnectionPoolIdentity identity, out string user, out string database, ref string queueService, string appDomainKey, SqlDependencyPerAppDomainDispatcher dispatcher, out bool errorOccurred, out bool appDomainStart, bool useDefaults)
	{
		server = null;
		identity = null;
		user = null;
		database = null;
		errorOccurred = false;
		appDomainStart = false;
		Dictionary<string, SqlDependencyPerAppDomainDispatcher> sqlDependencyPerAppDomainDispatchers = this._sqlDependencyPerAppDomainDispatchers;
		lock (sqlDependencyPerAppDomainDispatchers)
		{
			if (!this._sqlDependencyPerAppDomainDispatchers.ContainsKey(appDomainKey))
			{
				this._sqlDependencyPerAppDomainDispatchers[appDomainKey] = dispatcher;
			}
		}
		SqlConnectionStringBuilder sqlConnectionStringBuilder;
		SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper hashHelper = SqlDependencyProcessDispatcher.GetHashHelper(connectionString, out sqlConnectionStringBuilder, out identity, out user, queueService);
		bool flag2 = false;
		SqlDependencyProcessDispatcher.SqlConnectionContainer sqlConnectionContainer = null;
		Dictionary<SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper, SqlDependencyProcessDispatcher.SqlConnectionContainer> connectionContainers = this._connectionContainers;
		lock (connectionContainers)
		{
			if (!this._connectionContainers.ContainsKey(hashHelper))
			{
				sqlConnectionContainer = new SqlDependencyProcessDispatcher.SqlConnectionContainer(hashHelper, appDomainKey, useDefaults);
				this._connectionContainers.Add(hashHelper, sqlConnectionContainer);
				flag2 = true;
				appDomainStart = true;
			}
			else
			{
				sqlConnectionContainer = this._connectionContainers[hashHelper];
				if (sqlConnectionContainer.InErrorState)
				{
					errorOccurred = true;
				}
				else
				{
					sqlConnectionContainer.IncrementStartCount(appDomainKey, out appDomainStart);
				}
			}
		}
		if (useDefaults && !errorOccurred)
		{
			server = sqlConnectionContainer.Server;
			database = sqlConnectionContainer.Database;
			queueService = sqlConnectionContainer.Queue;
		}
		return flag2;
	}

	internal bool Stop(string connectionString, out string server, out DbConnectionPoolIdentity identity, out string user, out string database, ref string queueService, string appDomainKey, out bool appDomainStop)
	{
		server = null;
		identity = null;
		user = null;
		database = null;
		appDomainStop = false;
		SqlConnectionStringBuilder sqlConnectionStringBuilder;
		SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper hashHelper = SqlDependencyProcessDispatcher.GetHashHelper(connectionString, out sqlConnectionStringBuilder, out identity, out user, queueService);
		bool flag = false;
		Dictionary<SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper, SqlDependencyProcessDispatcher.SqlConnectionContainer> connectionContainers = this._connectionContainers;
		lock (connectionContainers)
		{
			if (this._connectionContainers.ContainsKey(hashHelper))
			{
				SqlDependencyProcessDispatcher.SqlConnectionContainer sqlConnectionContainer = this._connectionContainers[hashHelper];
				server = sqlConnectionContainer.Server;
				database = sqlConnectionContainer.Database;
				queueService = sqlConnectionContainer.Queue;
				if (sqlConnectionContainer.Stop(appDomainKey, out appDomainStop))
				{
					flag = true;
					this._connectionContainers.Remove(hashHelper);
				}
			}
		}
		return flag;
	}

	private static SqlDependencyProcessDispatcher s_staticInstance = new SqlDependencyProcessDispatcher(null);

	private Dictionary<SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper, SqlDependencyProcessDispatcher.SqlConnectionContainer> _connectionContainers;

	private Dictionary<string, SqlDependencyPerAppDomainDispatcher> _sqlDependencyPerAppDomainDispatchers;

	private class SqlConnectionContainer
	{
		internal SqlConnectionContainer(SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper hashHelper, string appDomainKey, bool useDefaults)
		{
			bool flag = false;
			try
			{
				this._hashHelper = hashHelper;
				string text = null;
				if (useDefaults)
				{
					text = Guid.NewGuid().ToString();
					this._queue = "SqlQueryNotificationService-" + text;
					this._hashHelper.ConnectionStringBuilder.ApplicationName = this._queue;
				}
				else
				{
					this._queue = this._hashHelper.Queue;
				}
				this._con = new SqlConnection(this._hashHelper.ConnectionStringBuilder.ConnectionString);
				SqlConnectionString sqlConnectionString = (SqlConnectionString)this._con.ConnectionOptions;
				this._con.Open();
				this._cachedServer = this._con.DataSource;
				this._escapedQueueName = SqlConnection.FixupDatabaseTransactionName(this._queue);
				this._appDomainKeyHash = new Dictionary<string, int>();
				this._com = new SqlCommand
				{
					Connection = this._con,
					CommandText = "select is_broker_enabled from sys.databases where database_id=db_id()"
				};
				if (!(bool)this._com.ExecuteScalar())
				{
					throw SQL.SqlDependencyDatabaseBrokerDisabled();
				}
				this._conversationGuidParam = new SqlParameter("@p1", SqlDbType.UniqueIdentifier);
				this._timeoutParam = new SqlParameter("@p2", SqlDbType.Int)
				{
					Value = 0
				};
				this._com.Parameters.Add(this._timeoutParam);
				flag = true;
				this._receiveQuery = "WAITFOR(RECEIVE TOP (1) message_type_name, conversation_handle, cast(message_body AS XML) as message_body from " + this._escapedQueueName + "), TIMEOUT @p2;";
				if (useDefaults)
				{
					this._sprocName = SqlConnection.FixupDatabaseTransactionName("SqlQueryNotificationStoredProcedure-" + text);
					this.CreateQueueAndService(false);
				}
				else
				{
					this._com.CommandText = this._receiveQuery;
					this._endConversationQuery = "END CONVERSATION @p1; ";
					this._concatQuery = this._endConversationQuery + this._receiveQuery;
				}
				bool flag2;
				this.IncrementStartCount(appDomainKey, out flag2);
				this.SynchronouslyQueryServiceBrokerQueue();
				this._timeoutParam.Value = this._defaultWaitforTimeout;
				this.AsynchronouslyQueryServiceBrokerQueue();
			}
			catch (Exception ex)
			{
				if (!ADP.IsCatchableExceptionType(ex))
				{
					throw;
				}
				ADP.TraceExceptionWithoutRethrow(ex);
				if (flag)
				{
					this.TearDownAndDispose();
				}
				else
				{
					if (this._com != null)
					{
						this._com.Dispose();
						this._com = null;
					}
					if (this._con != null)
					{
						this._con.Dispose();
						this._con = null;
					}
				}
				throw;
			}
		}

		internal string Database
		{
			get
			{
				if (this._cachedDatabase == null)
				{
					this._cachedDatabase = this._con.Database;
				}
				return this._cachedDatabase;
			}
		}

		internal SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper HashHelper
		{
			get
			{
				return this._hashHelper;
			}
		}

		internal bool InErrorState
		{
			get
			{
				return this._errorState;
			}
		}

		internal string Queue
		{
			get
			{
				return this._queue;
			}
		}

		internal string Server
		{
			get
			{
				return this._cachedServer;
			}
		}

		internal bool AppDomainUnload(string appDomainKey)
		{
			Dictionary<string, int> appDomainKeyHash = this._appDomainKeyHash;
			lock (appDomainKeyHash)
			{
				if (this._appDomainKeyHash.ContainsKey(appDomainKey))
				{
					int i = this._appDomainKeyHash[appDomainKey];
					bool flag2 = false;
					while (i > 0)
					{
						this.Stop(appDomainKey, out flag2);
						i--;
					}
				}
			}
			return this._stopped;
		}

		private void AsynchronouslyQueryServiceBrokerQueue()
		{
			AsyncCallback asyncCallback = new AsyncCallback(this.AsyncResultCallback);
			this._com.BeginExecuteReader(CommandBehavior.Default, asyncCallback, null);
		}

		private void AsyncResultCallback(IAsyncResult asyncResult)
		{
			try
			{
				using (SqlDataReader sqlDataReader = this._com.EndExecuteReader(asyncResult))
				{
					this.ProcessNotificationResults(sqlDataReader);
				}
				if (!this._stop)
				{
					this.AsynchronouslyQueryServiceBrokerQueue();
				}
				else
				{
					this.TearDownAndDispose();
				}
			}
			catch (Exception ex)
			{
				if (!ADP.IsCatchableExceptionType(ex))
				{
					this._errorState = true;
					throw;
				}
				if (!this._stop)
				{
					ADP.TraceExceptionWithoutRethrow(ex);
				}
				if (this._stop)
				{
					this.TearDownAndDispose();
				}
				else
				{
					this._errorState = true;
					this.Restart(null);
				}
			}
		}

		private void CreateQueueAndService(bool restart)
		{
			SqlCommand sqlCommand = new SqlCommand
			{
				Connection = this._con
			};
			SqlTransaction sqlTransaction = null;
			try
			{
				sqlTransaction = this._con.BeginTransaction();
				sqlCommand.Transaction = sqlTransaction;
				string text = SqlServerEscapeHelper.MakeStringLiteral(this._queue);
				sqlCommand.CommandText = string.Concat(new string[]
				{
					"CREATE PROCEDURE ", this._sprocName, " AS BEGIN BEGIN TRANSACTION; RECEIVE TOP(0) conversation_handle FROM ", this._escapedQueueName, "; IF (SELECT COUNT(*) FROM ", this._escapedQueueName, " WHERE message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer') > 0 BEGIN if ((SELECT COUNT(*) FROM sys.services WHERE name = ", text, ") > 0)   DROP SERVICE ", this._escapedQueueName,
					"; if (OBJECT_ID(", text, ", 'SQ') IS NOT NULL)   DROP QUEUE ", this._escapedQueueName, "; DROP PROCEDURE ", this._sprocName, "; END COMMIT TRANSACTION; END"
				});
				if (!restart)
				{
					sqlCommand.ExecuteNonQuery();
				}
				else
				{
					try
					{
						sqlCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						if (!ADP.IsCatchableExceptionType(ex))
						{
							throw;
						}
						ADP.TraceExceptionWithoutRethrow(ex);
						try
						{
							if (sqlTransaction != null)
							{
								sqlTransaction.Rollback();
								sqlTransaction = null;
							}
						}
						catch (Exception ex2)
						{
							if (!ADP.IsCatchableExceptionType(ex2))
							{
								throw;
							}
							ADP.TraceExceptionWithoutRethrow(ex2);
						}
					}
					if (sqlTransaction == null)
					{
						sqlTransaction = this._con.BeginTransaction();
						sqlCommand.Transaction = sqlTransaction;
					}
				}
				sqlCommand.CommandText = string.Concat(new string[]
				{
					"IF OBJECT_ID(", text, ", 'SQ') IS NULL BEGIN CREATE QUEUE ", this._escapedQueueName, " WITH ACTIVATION (PROCEDURE_NAME=", this._sprocName, ", MAX_QUEUE_READERS=1, EXECUTE AS OWNER); END; IF (SELECT COUNT(*) FROM sys.services WHERE NAME=", text, ") = 0 BEGIN CREATE SERVICE ", this._escapedQueueName,
					" ON QUEUE ", this._escapedQueueName, " ([http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification]); IF (SELECT COUNT(*) FROM sys.database_principals WHERE name='sql_dependency_subscriber' AND type='R') <> 0 BEGIN GRANT SEND ON SERVICE::", this._escapedQueueName, " TO sql_dependency_subscriber; END;  END; BEGIN DIALOG @dialog_handle FROM SERVICE ", this._escapedQueueName, " TO SERVICE ", text
				});
				SqlParameter sqlParameter = new SqlParameter
				{
					ParameterName = "@dialog_handle",
					DbType = DbType.Guid,
					Direction = ParameterDirection.Output
				};
				sqlCommand.Parameters.Add(sqlParameter);
				sqlCommand.ExecuteNonQuery();
				this._dialogHandle = ((Guid)sqlParameter.Value).ToString();
				this._beginConversationQuery = "BEGIN CONVERSATION TIMER ('" + this._dialogHandle + "') TIMEOUT = 120; " + this._receiveQuery;
				this._com.CommandText = this._beginConversationQuery;
				this._endConversationQuery = "END CONVERSATION @p1; ";
				this._concatQuery = this._endConversationQuery + this._com.CommandText;
				sqlTransaction.Commit();
				sqlTransaction = null;
				this._serviceQueueCreated = true;
			}
			finally
			{
				if (sqlTransaction != null)
				{
					try
					{
						sqlTransaction.Rollback();
						sqlTransaction = null;
					}
					catch (Exception ex3)
					{
						if (!ADP.IsCatchableExceptionType(ex3))
						{
							throw;
						}
						ADP.TraceExceptionWithoutRethrow(ex3);
					}
				}
			}
		}

		internal void IncrementStartCount(string appDomainKey, out bool appDomainStart)
		{
			appDomainStart = false;
			Interlocked.Increment(ref this._startCount);
			Dictionary<string, int> appDomainKeyHash = this._appDomainKeyHash;
			lock (appDomainKeyHash)
			{
				if (this._appDomainKeyHash.ContainsKey(appDomainKey))
				{
					this._appDomainKeyHash[appDomainKey] = this._appDomainKeyHash[appDomainKey] + 1;
				}
				else
				{
					this._appDomainKeyHash[appDomainKey] = 1;
					appDomainStart = true;
				}
			}
		}

		private void ProcessNotificationResults(SqlDataReader reader)
		{
			Guid guid = Guid.Empty;
			try
			{
				if (!this._stop)
				{
					while (reader.Read())
					{
						string @string = reader.GetString(0);
						guid = reader.GetGuid(1);
						if (string.Compare(@string, "http://schemas.microsoft.com/SQL/Notifications/QueryNotification", StringComparison.OrdinalIgnoreCase) == 0)
						{
							SqlXml sqlXml = reader.GetSqlXml(2);
							if (sqlXml == null)
							{
								continue;
							}
							SqlNotification sqlNotification = SqlDependencyProcessDispatcher.SqlNotificationParser.ProcessMessage(sqlXml);
							if (sqlNotification == null)
							{
								continue;
							}
							string key = sqlNotification.Key;
							int num = key.IndexOf(';');
							if (num < 0)
							{
								continue;
							}
							string text = key.Substring(0, num);
							Dictionary<string, SqlDependencyPerAppDomainDispatcher> sqlDependencyPerAppDomainDispatchers = SqlDependencyProcessDispatcher.s_staticInstance._sqlDependencyPerAppDomainDispatchers;
							SqlDependencyPerAppDomainDispatcher sqlDependencyPerAppDomainDispatcher;
							lock (sqlDependencyPerAppDomainDispatchers)
							{
								sqlDependencyPerAppDomainDispatcher = SqlDependencyProcessDispatcher.s_staticInstance._sqlDependencyPerAppDomainDispatchers[text];
							}
							if (sqlDependencyPerAppDomainDispatcher == null)
							{
								continue;
							}
							try
							{
								sqlDependencyPerAppDomainDispatcher.InvalidateCommandID(sqlNotification);
								continue;
							}
							catch (Exception ex)
							{
								if (!ADP.IsCatchableExceptionType(ex))
								{
									throw;
								}
								ADP.TraceExceptionWithoutRethrow(ex);
								continue;
							}
						}
						guid = Guid.Empty;
					}
				}
			}
			finally
			{
				if (guid == Guid.Empty)
				{
					this._com.CommandText = this._beginConversationQuery ?? this._receiveQuery;
					if (this._com.Parameters.Count > 1)
					{
						this._com.Parameters.Remove(this._conversationGuidParam);
					}
				}
				else
				{
					this._com.CommandText = this._concatQuery;
					this._conversationGuidParam.Value = guid;
					if (this._com.Parameters.Count == 1)
					{
						this._com.Parameters.Add(this._conversationGuidParam);
					}
				}
			}
		}

		private void Restart(object unused)
		{
			try
			{
				SqlDependencyProcessDispatcher.SqlConnectionContainer sqlConnectionContainer = this;
				lock (sqlConnectionContainer)
				{
					if (!this._stop)
					{
						try
						{
							this._con.Close();
						}
						catch (Exception ex)
						{
							if (!ADP.IsCatchableExceptionType(ex))
							{
								throw;
							}
							ADP.TraceExceptionWithoutRethrow(ex);
						}
					}
				}
				sqlConnectionContainer = this;
				lock (sqlConnectionContainer)
				{
					if (!this._stop)
					{
						this._con.Open();
					}
				}
				sqlConnectionContainer = this;
				lock (sqlConnectionContainer)
				{
					if (!this._stop && this._serviceQueueCreated)
					{
						bool flag2 = false;
						try
						{
							this.CreateQueueAndService(true);
						}
						catch (Exception ex2)
						{
							if (!ADP.IsCatchableExceptionType(ex2))
							{
								throw;
							}
							ADP.TraceExceptionWithoutRethrow(ex2);
							flag2 = true;
						}
						if (flag2)
						{
							SqlDependencyProcessDispatcher.s_staticInstance.Invalidate(this.Server, new SqlNotification(SqlNotificationInfo.Error, SqlNotificationSource.Client, SqlNotificationType.Change, null));
						}
					}
				}
				sqlConnectionContainer = this;
				lock (sqlConnectionContainer)
				{
					if (!this._stop)
					{
						this._timeoutParam.Value = 0;
						this.SynchronouslyQueryServiceBrokerQueue();
						this._timeoutParam.Value = this._defaultWaitforTimeout;
						this.AsynchronouslyQueryServiceBrokerQueue();
						this._errorState = false;
						this._retryTimer = null;
					}
				}
				if (this._stop)
				{
					this.TearDownAndDispose();
				}
			}
			catch (Exception ex3)
			{
				if (!ADP.IsCatchableExceptionType(ex3))
				{
					throw;
				}
				ADP.TraceExceptionWithoutRethrow(ex3);
				try
				{
					SqlDependencyProcessDispatcher.s_staticInstance.Invalidate(this.Server, new SqlNotification(SqlNotificationInfo.Error, SqlNotificationSource.Client, SqlNotificationType.Change, null));
				}
				catch (Exception ex4)
				{
					if (!ADP.IsCatchableExceptionType(ex4))
					{
						throw;
					}
					ADP.TraceExceptionWithoutRethrow(ex4);
				}
				try
				{
					this._con.Close();
				}
				catch (Exception ex5)
				{
					if (!ADP.IsCatchableExceptionType(ex5))
					{
						throw;
					}
					ADP.TraceExceptionWithoutRethrow(ex5);
				}
				if (!this._stop)
				{
					this._retryTimer = new Timer(new TimerCallback(this.Restart), null, this._defaultWaitforTimeout, -1);
				}
			}
		}

		internal bool Stop(string appDomainKey, out bool appDomainStop)
		{
			appDomainStop = false;
			if (appDomainKey != null)
			{
				Dictionary<string, int> appDomainKeyHash = this._appDomainKeyHash;
				lock (appDomainKeyHash)
				{
					if (this._appDomainKeyHash.ContainsKey(appDomainKey))
					{
						int num = this._appDomainKeyHash[appDomainKey];
						if (num > 0)
						{
							this._appDomainKeyHash[appDomainKey] = num - 1;
						}
						if (1 == num)
						{
							this._appDomainKeyHash.Remove(appDomainKey);
							appDomainStop = true;
						}
					}
				}
			}
			if (Interlocked.Decrement(ref this._startCount) == 0)
			{
				SqlDependencyProcessDispatcher.SqlConnectionContainer sqlConnectionContainer = this;
				lock (sqlConnectionContainer)
				{
					try
					{
						this._com.Cancel();
					}
					catch (Exception ex)
					{
						if (!ADP.IsCatchableExceptionType(ex))
						{
							throw;
						}
						ADP.TraceExceptionWithoutRethrow(ex);
					}
					this._stop = true;
				}
				Stopwatch stopwatch = Stopwatch.StartNew();
				for (;;)
				{
					sqlConnectionContainer = this;
					lock (sqlConnectionContainer)
					{
						if (this._stopped)
						{
							break;
						}
						if (this._errorState || stopwatch.Elapsed.Seconds >= 30)
						{
							Timer retryTimer = this._retryTimer;
							this._retryTimer = null;
							if (retryTimer != null)
							{
								retryTimer.Dispose();
							}
							this.TearDownAndDispose();
							break;
						}
					}
					Thread.Sleep(1);
				}
			}
			return this._stopped;
		}

		private void SynchronouslyQueryServiceBrokerQueue()
		{
			using (SqlDataReader sqlDataReader = this._com.ExecuteReader())
			{
				this.ProcessNotificationResults(sqlDataReader);
			}
		}

		private void TearDownAndDispose()
		{
			lock (this)
			{
				try
				{
					if (this._con.State != ConnectionState.Closed && ConnectionState.Broken != this._con.State)
					{
						if (this._com.Parameters.Count > 1)
						{
							try
							{
								this._com.CommandText = this._endConversationQuery;
								this._com.Parameters.Remove(this._timeoutParam);
								this._com.ExecuteNonQuery();
							}
							catch (Exception ex)
							{
								if (!ADP.IsCatchableExceptionType(ex))
								{
									throw;
								}
								ADP.TraceExceptionWithoutRethrow(ex);
							}
						}
						if (this._serviceQueueCreated && !this._errorState)
						{
							this._com.CommandText = string.Concat(new string[] { "BEGIN TRANSACTION; DROP SERVICE ", this._escapedQueueName, "; DROP QUEUE ", this._escapedQueueName, "; DROP PROCEDURE ", this._sprocName, "; COMMIT TRANSACTION;" });
							try
							{
								this._com.ExecuteNonQuery();
							}
							catch (Exception ex2)
							{
								if (!ADP.IsCatchableExceptionType(ex2))
								{
									throw;
								}
								ADP.TraceExceptionWithoutRethrow(ex2);
							}
						}
					}
				}
				finally
				{
					this._stopped = true;
					this._con.Dispose();
				}
			}
		}

		private SqlConnection _con;

		private SqlCommand _com;

		private SqlParameter _conversationGuidParam;

		private SqlParameter _timeoutParam;

		private SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper _hashHelper;

		private string _queue;

		private string _receiveQuery;

		private string _beginConversationQuery;

		private string _endConversationQuery;

		private string _concatQuery;

		private readonly int _defaultWaitforTimeout = 60000;

		private string _escapedQueueName;

		private string _sprocName;

		private string _dialogHandle;

		private string _cachedServer;

		private string _cachedDatabase;

		private volatile bool _errorState;

		private volatile bool _stop;

		private volatile bool _stopped;

		private volatile bool _serviceQueueCreated;

		private int _startCount;

		private Timer _retryTimer;

		private Dictionary<string, int> _appDomainKeyHash;
	}

	private class SqlNotificationParser
	{
		internal static SqlNotification ProcessMessage(SqlXml xmlMessage)
		{
			SqlNotification sqlNotification;
			using (XmlReader xmlReader = xmlMessage.CreateReader())
			{
				string empty = string.Empty;
				SqlDependencyProcessDispatcher.SqlNotificationParser.MessageAttributes messageAttributes = SqlDependencyProcessDispatcher.SqlNotificationParser.MessageAttributes.None;
				SqlNotificationType sqlNotificationType = SqlNotificationType.Unknown;
				SqlNotificationInfo sqlNotificationInfo = SqlNotificationInfo.Unknown;
				SqlNotificationSource sqlNotificationSource = SqlNotificationSource.Unknown;
				string text = string.Empty;
				xmlReader.Read();
				if (XmlNodeType.Element == xmlReader.NodeType && "QueryNotification" == xmlReader.LocalName && 3 <= xmlReader.AttributeCount)
				{
					while (SqlDependencyProcessDispatcher.SqlNotificationParser.MessageAttributes.All != messageAttributes && xmlReader.MoveToNextAttribute())
					{
						try
						{
							string localName = xmlReader.LocalName;
							if (!(localName == "type"))
							{
								if (!(localName == "source"))
								{
									if (localName == "info")
									{
										try
										{
											string value = xmlReader.Value;
											if (!(value == "set options"))
											{
												if (!(value == "previous invalid"))
												{
													if (!(value == "query template limit"))
													{
														SqlNotificationInfo sqlNotificationInfo2 = (SqlNotificationInfo)Enum.Parse(typeof(SqlNotificationInfo), value, true);
														if (Enum.IsDefined(typeof(SqlNotificationInfo), sqlNotificationInfo2))
														{
															sqlNotificationInfo = sqlNotificationInfo2;
														}
													}
													else
													{
														sqlNotificationInfo = SqlNotificationInfo.TemplateLimit;
													}
												}
												else
												{
													sqlNotificationInfo = SqlNotificationInfo.PreviousFire;
												}
											}
											else
											{
												sqlNotificationInfo = SqlNotificationInfo.Options;
											}
										}
										catch (Exception ex)
										{
											if (!ADP.IsCatchableExceptionType(ex))
											{
												throw;
											}
											ADP.TraceExceptionWithoutRethrow(ex);
										}
										messageAttributes |= SqlDependencyProcessDispatcher.SqlNotificationParser.MessageAttributes.Info;
									}
								}
								else
								{
									try
									{
										SqlNotificationSource sqlNotificationSource2 = (SqlNotificationSource)Enum.Parse(typeof(SqlNotificationSource), xmlReader.Value, true);
										if (Enum.IsDefined(typeof(SqlNotificationSource), sqlNotificationSource2))
										{
											sqlNotificationSource = sqlNotificationSource2;
										}
									}
									catch (Exception ex2)
									{
										if (!ADP.IsCatchableExceptionType(ex2))
										{
											throw;
										}
										ADP.TraceExceptionWithoutRethrow(ex2);
									}
									messageAttributes |= SqlDependencyProcessDispatcher.SqlNotificationParser.MessageAttributes.Source;
								}
							}
							else
							{
								try
								{
									SqlNotificationType sqlNotificationType2 = (SqlNotificationType)Enum.Parse(typeof(SqlNotificationType), xmlReader.Value, true);
									if (Enum.IsDefined(typeof(SqlNotificationType), sqlNotificationType2))
									{
										sqlNotificationType = sqlNotificationType2;
									}
								}
								catch (Exception ex3)
								{
									if (!ADP.IsCatchableExceptionType(ex3))
									{
										throw;
									}
									ADP.TraceExceptionWithoutRethrow(ex3);
								}
								messageAttributes |= SqlDependencyProcessDispatcher.SqlNotificationParser.MessageAttributes.Type;
							}
						}
						catch (ArgumentException ex4)
						{
							ADP.TraceExceptionWithoutRethrow(ex4);
							return null;
						}
					}
					if (SqlDependencyProcessDispatcher.SqlNotificationParser.MessageAttributes.All != messageAttributes)
					{
						sqlNotification = null;
					}
					else if (!xmlReader.Read())
					{
						sqlNotification = null;
					}
					else if (XmlNodeType.Element != xmlReader.NodeType || string.Compare(xmlReader.LocalName, "Message", StringComparison.OrdinalIgnoreCase) != 0)
					{
						sqlNotification = null;
					}
					else if (!xmlReader.Read())
					{
						sqlNotification = null;
					}
					else if (xmlReader.NodeType != XmlNodeType.Text)
					{
						sqlNotification = null;
					}
					else
					{
						using (XmlTextReader xmlTextReader = new XmlTextReader(xmlReader.Value, XmlNodeType.Element, null))
						{
							if (!xmlTextReader.Read())
							{
								return null;
							}
							if (xmlTextReader.NodeType != XmlNodeType.Text)
							{
								return null;
							}
							text = xmlTextReader.Value;
							xmlTextReader.Close();
						}
						sqlNotification = new SqlNotification(sqlNotificationInfo, sqlNotificationSource, sqlNotificationType, text);
					}
				}
				else
				{
					sqlNotification = null;
				}
			}
			return sqlNotification;
		}

		private const string RootNode = "QueryNotification";

		private const string MessageNode = "Message";

		private const string InfoAttribute = "info";

		private const string SourceAttribute = "source";

		private const string TypeAttribute = "type";

		[Flags]
		private enum MessageAttributes
		{
			None = 0,
			Type = 1,
			Source = 2,
			Info = 4,
			All = 7
		}
	}

	private class SqlConnectionContainerHashHelper
	{
		internal SqlConnectionContainerHashHelper(DbConnectionPoolIdentity identity, string connectionString, string queue, SqlConnectionStringBuilder connectionStringBuilder)
		{
			this._identity = identity;
			this._connectionString = connectionString;
			this._queue = queue;
			this._connectionStringBuilder = connectionStringBuilder;
		}

		internal SqlConnectionStringBuilder ConnectionStringBuilder
		{
			get
			{
				return this._connectionStringBuilder;
			}
		}

		internal DbConnectionPoolIdentity Identity
		{
			get
			{
				return this._identity;
			}
		}

		internal string Queue
		{
			get
			{
				return this._queue;
			}
		}

		public override bool Equals(object value)
		{
			SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper sqlConnectionContainerHashHelper = (SqlDependencyProcessDispatcher.SqlConnectionContainerHashHelper)value;
			bool flag;
			if (sqlConnectionContainerHashHelper == null)
			{
				flag = false;
			}
			else if (this == sqlConnectionContainerHashHelper)
			{
				flag = true;
			}
			else if ((this._identity != null && sqlConnectionContainerHashHelper._identity == null) || (this._identity == null && sqlConnectionContainerHashHelper._identity != null))
			{
				flag = false;
			}
			else if (this._identity == null && sqlConnectionContainerHashHelper._identity == null)
			{
				flag = sqlConnectionContainerHashHelper._connectionString == this._connectionString && string.Equals(sqlConnectionContainerHashHelper._queue, this._queue, StringComparison.OrdinalIgnoreCase);
			}
			else
			{
				flag = sqlConnectionContainerHashHelper._identity.Equals(this._identity) && sqlConnectionContainerHashHelper._connectionString == this._connectionString && string.Equals(sqlConnectionContainerHashHelper._queue, this._queue, StringComparison.OrdinalIgnoreCase);
			}
			return flag;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (this._identity != null)
			{
				num = this._identity.GetHashCode();
			}
			if (this._queue != null)
			{
				num = this._connectionString.GetHashCode() + this._queue.GetHashCode() + num;
			}
			else
			{
				num = this._connectionString.GetHashCode() + num;
			}
			return num;
		}

		private DbConnectionPoolIdentity _identity;

		private string _connectionString;

		private string _queue;

		private SqlConnectionStringBuilder _connectionStringBuilder;
	}
}
