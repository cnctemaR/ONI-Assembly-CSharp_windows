using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.ProviderBase;
using System.Globalization;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Data.SqlClient
{
	internal sealed class SqlInternalConnectionTds : SqlInternalConnection, IDisposable
	{
		internal SessionData CurrentSessionData
		{
			get
			{
				if (this._currentSessionData != null)
				{
					this._currentSessionData._database = base.CurrentDatabase;
					this._currentSessionData._language = this._currentLanguage;
				}
				return this._currentSessionData;
			}
		}

		internal SqlConnectionTimeoutErrorInternal TimeoutErrorInternal
		{
			get
			{
				return this._timeoutErrorInternal;
			}
		}

		internal SqlInternalConnectionTds(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, object providerInfo, string newPassword, SecureString newSecurePassword, bool redirectedUserInstance, SqlConnectionString userConnectionOptions = null, SessionData reconnectSessionData = null, bool applyTransientFaultHandling = false, string accessToken = null)
			: base(connectionOptions)
		{
			if (connectionOptions.ConnectRetryCount > 0)
			{
				this._recoverySessionData = reconnectSessionData;
				if (reconnectSessionData == null)
				{
					this._currentSessionData = new SessionData();
				}
				else
				{
					this._currentSessionData = new SessionData(this._recoverySessionData);
					this._originalDatabase = this._recoverySessionData._initialDatabase;
					this._originalLanguage = this._recoverySessionData._initialLanguage;
				}
			}
			if (accessToken != null)
			{
				this._accessTokenInBytes = Encoding.Unicode.GetBytes(accessToken);
			}
			this._identity = identity;
			this._poolGroupProviderInfo = (SqlConnectionPoolGroupProviderInfo)providerInfo;
			this._fResetConnection = connectionOptions.ConnectionReset;
			if (this._fResetConnection && this._recoverySessionData == null)
			{
				this._originalDatabase = connectionOptions.InitialCatalog;
				this._originalLanguage = connectionOptions.CurrentLanguage;
			}
			this._timeoutErrorInternal = new SqlConnectionTimeoutErrorInternal();
			this._credential = credential;
			this._parserLock.Wait(false);
			this.ThreadHasParserLockForClose = true;
			try
			{
				this._timeout = TimeoutTimer.StartSecondsTimeout(connectionOptions.ConnectTimeout);
				int num = (applyTransientFaultHandling ? (connectionOptions.ConnectRetryCount + 1) : 1);
				int num2 = connectionOptions.ConnectRetryInterval * 1000;
				for (int i = 0; i < num; i++)
				{
					try
					{
						this.OpenLoginEnlist(this._timeout, connectionOptions, credential, newPassword, newSecurePassword, redirectedUserInstance);
						break;
					}
					catch (SqlException ex)
					{
						if (i + 1 == num || !applyTransientFaultHandling || this._timeout.IsExpired || this._timeout.MillisecondsRemaining < (long)num2 || !this.IsTransientError(ex))
						{
							throw ex;
						}
						Thread.Sleep(num2);
					}
				}
			}
			finally
			{
				this.ThreadHasParserLockForClose = false;
				this._parserLock.Release();
			}
		}

		private bool IsTransientError(SqlException exc)
		{
			if (exc == null)
			{
				return false;
			}
			foreach (object obj in exc.Errors)
			{
				SqlError sqlError = (SqlError)obj;
				if (SqlInternalConnectionTds.s_transientErrors.Contains(sqlError.Number))
				{
					return true;
				}
			}
			return false;
		}

		internal Guid ClientConnectionId
		{
			get
			{
				return this._clientConnectionId;
			}
		}

		internal Guid OriginalClientConnectionId
		{
			get
			{
				return this._originalClientConnectionId;
			}
		}

		internal string RoutingDestination
		{
			get
			{
				return this._routingDestination;
			}
		}

		internal override SqlInternalTransaction CurrentTransaction
		{
			get
			{
				return this._parser.CurrentTransaction;
			}
		}

		internal override SqlInternalTransaction AvailableInternalTransaction
		{
			get
			{
				if (!this._parser._fResetConnection)
				{
					return this.CurrentTransaction;
				}
				return null;
			}
		}

		internal override SqlInternalTransaction PendingTransaction
		{
			get
			{
				return this._parser.PendingTransaction;
			}
		}

		internal DbConnectionPoolIdentity Identity
		{
			get
			{
				return this._identity;
			}
		}

		internal string InstanceName
		{
			get
			{
				return this._instanceName;
			}
		}

		internal override bool IsLockedForBulkCopy
		{
			get
			{
				return !this.Parser.MARSOn && this.Parser._physicalStateObj.BcpLock;
			}
		}

		protected internal override bool IsNonPoolableTransactionRoot
		{
			get
			{
				return this.IsTransactionRoot && (!this.IsKatmaiOrNewer || base.Pool == null);
			}
		}

		internal override bool IsKatmaiOrNewer
		{
			get
			{
				return this._parser.IsKatmaiOrNewer;
			}
		}

		internal int PacketSize
		{
			get
			{
				return this._currentPacketSize;
			}
		}

		internal TdsParser Parser
		{
			get
			{
				return this._parser;
			}
		}

		internal string ServerProvidedFailOverPartner
		{
			get
			{
				return this._currentFailoverPartner;
			}
		}

		internal SqlConnectionPoolGroupProviderInfo PoolGroupProviderInfo
		{
			get
			{
				return this._poolGroupProviderInfo;
			}
		}

		protected override bool ReadyToPrepareTransaction
		{
			get
			{
				return base.FindLiveReader(null) == null;
			}
		}

		public override string ServerVersion
		{
			get
			{
				return string.Format(null, "{0:00}.{1:00}.{2:0000}", this._loginAck.majorVersion, (short)this._loginAck.minorVersion, this._loginAck.buildNum);
			}
		}

		protected override bool UnbindOnTransactionCompletion
		{
			get
			{
				return false;
			}
		}

		protected override void ChangeDatabaseInternal(string database)
		{
			database = SqlConnection.FixupDatabaseTransactionName(database);
			this._parser.TdsExecuteSQLBatch("use " + database, base.ConnectionOptions.ConnectTimeout, null, this._parser._physicalStateObj, true, false);
			this._parser.Run(RunBehavior.UntilDone, null, null, null, this._parser._physicalStateObj);
		}

		public override void Dispose()
		{
			try
			{
				TdsParser tdsParser = Interlocked.Exchange<TdsParser>(ref this._parser, null);
				if (tdsParser != null)
				{
					tdsParser.Disconnect();
				}
			}
			finally
			{
				this._loginAck = null;
				this._fConnectionOpen = false;
			}
			base.Dispose();
		}

		internal override void ValidateConnectionForExecute(SqlCommand command)
		{
			TdsParser parser = this._parser;
			if (parser == null || parser.State == TdsParserState.Broken || parser.State == TdsParserState.Closed)
			{
				throw ADP.ClosedConnectionError();
			}
			SqlDataReader sqlDataReader = null;
			if (parser.MARSOn)
			{
				if (command != null)
				{
					sqlDataReader = base.FindLiveReader(command);
				}
			}
			else
			{
				if (this._asyncCommandCount > 0)
				{
					throw SQL.MARSUnspportedOnConnection();
				}
				sqlDataReader = base.FindLiveReader(null);
			}
			if (sqlDataReader != null)
			{
				throw ADP.OpenReaderExists();
			}
			if (!parser.MARSOn && parser._physicalStateObj._pendingData)
			{
				parser.DrainData(parser._physicalStateObj);
			}
			parser.RollbackOrphanedAPITransactions();
		}

		internal void CheckEnlistedTransactionBinding()
		{
			Transaction enlistedTransaction = base.EnlistedTransaction;
			if (enlistedTransaction != null)
			{
				if (base.ConnectionOptions.TransactionBinding == SqlConnectionString.TransactionBindingEnum.ExplicitUnbind)
				{
					Transaction transaction = Transaction.Current;
					if (enlistedTransaction.TransactionInformation.Status != TransactionStatus.Active || !enlistedTransaction.Equals(transaction))
					{
						throw ADP.TransactionConnectionMismatch();
					}
				}
				else if (enlistedTransaction.TransactionInformation.Status != TransactionStatus.Active)
				{
					if (base.EnlistedTransactionDisposed)
					{
						base.DetachTransaction(enlistedTransaction, true);
						return;
					}
					throw ADP.TransactionCompletedButNotDisposed();
				}
			}
		}

		internal override bool IsConnectionAlive(bool throwOnException)
		{
			return this._parser._physicalStateObj.IsConnectionAlive(throwOnException);
		}

		protected override void Activate(Transaction transaction)
		{
			if (null != transaction)
			{
				if (base.ConnectionOptions.Enlist)
				{
					base.Enlist(transaction);
					return;
				}
			}
			else
			{
				base.Enlist(null);
			}
		}

		protected override void InternalDeactivate()
		{
			if (this._asyncCommandCount != 0)
			{
				base.DoomThisConnection();
			}
			if (!this.IsNonPoolableTransactionRoot && this._parser != null)
			{
				this._parser.Deactivate(base.IsConnectionDoomed);
				if (!base.IsConnectionDoomed)
				{
					this.ResetConnection();
				}
			}
		}

		private void ResetConnection()
		{
			if (this._fResetConnection)
			{
				this._parser.PrepareResetConnection(this.IsTransactionRoot && !this.IsNonPoolableTransactionRoot);
				base.CurrentDatabase = this._originalDatabase;
				this._currentLanguage = this._originalLanguage;
			}
		}

		internal void DecrementAsyncCount()
		{
			Interlocked.Decrement(ref this._asyncCommandCount);
		}

		internal void IncrementAsyncCount()
		{
			Interlocked.Increment(ref this._asyncCommandCount);
		}

		internal override void DisconnectTransaction(SqlInternalTransaction internalTransaction)
		{
			TdsParser parser = this.Parser;
			if (parser != null)
			{
				parser.DisconnectTransaction(internalTransaction);
			}
		}

		internal void ExecuteTransaction(SqlInternalConnection.TransactionRequest transactionRequest, string name, IsolationLevel iso)
		{
			this.ExecuteTransaction(transactionRequest, name, iso, null, false);
		}

		internal override void ExecuteTransaction(SqlInternalConnection.TransactionRequest transactionRequest, string name, IsolationLevel iso, SqlInternalTransaction internalTransaction, bool isDelegateControlRequest)
		{
			if (base.IsConnectionDoomed)
			{
				if (transactionRequest == SqlInternalConnection.TransactionRequest.Rollback || transactionRequest == SqlInternalConnection.TransactionRequest.IfRollback)
				{
					return;
				}
				throw SQL.ConnectionDoomed();
			}
			else
			{
				if ((transactionRequest == SqlInternalConnection.TransactionRequest.Commit || transactionRequest == SqlInternalConnection.TransactionRequest.Rollback || transactionRequest == SqlInternalConnection.TransactionRequest.IfRollback) && !this.Parser.MARSOn && this.Parser._physicalStateObj.BcpLock)
				{
					throw SQL.ConnectionLockedForBcpEvent();
				}
				string text = ((name == null) ? string.Empty : name);
				this.ExecuteTransactionYukon(transactionRequest, text, iso, internalTransaction, isDelegateControlRequest);
				return;
			}
		}

		internal void ExecuteTransactionYukon(SqlInternalConnection.TransactionRequest transactionRequest, string transactionName, IsolationLevel iso, SqlInternalTransaction internalTransaction, bool isDelegateControlRequest)
		{
			TdsEnums.TransactionManagerRequestType transactionManagerRequestType = TdsEnums.TransactionManagerRequestType.Begin;
			if (iso <= IsolationLevel.ReadUncommitted)
			{
				if (iso == IsolationLevel.Unspecified)
				{
					TdsEnums.TransactionManagerIsolationLevel transactionManagerIsolationLevel = TdsEnums.TransactionManagerIsolationLevel.Unspecified;
					goto IL_007E;
				}
				if (iso == IsolationLevel.Chaos)
				{
					throw SQL.NotSupportedIsolationLevel(iso);
				}
				if (iso == IsolationLevel.ReadUncommitted)
				{
					TdsEnums.TransactionManagerIsolationLevel transactionManagerIsolationLevel = TdsEnums.TransactionManagerIsolationLevel.ReadUncommitted;
					goto IL_007E;
				}
			}
			else if (iso <= IsolationLevel.RepeatableRead)
			{
				if (iso == IsolationLevel.ReadCommitted)
				{
					TdsEnums.TransactionManagerIsolationLevel transactionManagerIsolationLevel = TdsEnums.TransactionManagerIsolationLevel.ReadCommitted;
					goto IL_007E;
				}
				if (iso == IsolationLevel.RepeatableRead)
				{
					TdsEnums.TransactionManagerIsolationLevel transactionManagerIsolationLevel = TdsEnums.TransactionManagerIsolationLevel.RepeatableRead;
					goto IL_007E;
				}
			}
			else
			{
				if (iso == IsolationLevel.Serializable)
				{
					TdsEnums.TransactionManagerIsolationLevel transactionManagerIsolationLevel = TdsEnums.TransactionManagerIsolationLevel.Serializable;
					goto IL_007E;
				}
				if (iso == IsolationLevel.Snapshot)
				{
					TdsEnums.TransactionManagerIsolationLevel transactionManagerIsolationLevel = TdsEnums.TransactionManagerIsolationLevel.Snapshot;
					goto IL_007E;
				}
			}
			throw ADP.InvalidIsolationLevel(iso);
			IL_007E:
			TdsParserStateObject tdsParserStateObject = this._parser._physicalStateObj;
			TdsParser parser = this._parser;
			bool flag = false;
			bool releaseConnectionLock = false;
			if (!this.ThreadHasParserLockForClose)
			{
				this._parserLock.Wait(false);
				this.ThreadHasParserLockForClose = true;
				releaseConnectionLock = true;
			}
			try
			{
				switch (transactionRequest)
				{
				case SqlInternalConnection.TransactionRequest.Begin:
					transactionManagerRequestType = TdsEnums.TransactionManagerRequestType.Begin;
					break;
				case SqlInternalConnection.TransactionRequest.Promote:
					transactionManagerRequestType = TdsEnums.TransactionManagerRequestType.Promote;
					break;
				case SqlInternalConnection.TransactionRequest.Commit:
					transactionManagerRequestType = TdsEnums.TransactionManagerRequestType.Commit;
					break;
				case SqlInternalConnection.TransactionRequest.Rollback:
				case SqlInternalConnection.TransactionRequest.IfRollback:
					transactionManagerRequestType = TdsEnums.TransactionManagerRequestType.Rollback;
					break;
				case SqlInternalConnection.TransactionRequest.Save:
					transactionManagerRequestType = TdsEnums.TransactionManagerRequestType.Save;
					break;
				}
				if ((internalTransaction != null && internalTransaction.RestoreBrokenConnection) & releaseConnectionLock)
				{
					Task task = internalTransaction.Parent.Connection.ValidateAndReconnect(delegate
					{
						this.ThreadHasParserLockForClose = false;
						this._parserLock.Release();
						releaseConnectionLock = false;
					}, 0);
					if (task != null)
					{
						AsyncHelper.WaitForCompletion(task, 0, null, true);
						internalTransaction.ConnectionHasBeenRestored = true;
						return;
					}
				}
				if (internalTransaction != null && internalTransaction.IsDelegated)
				{
					if (this._parser.MARSOn)
					{
						tdsParserStateObject = this._parser.GetSession(this);
						flag = true;
					}
					else
					{
						int openResultsCount = internalTransaction.OpenResultsCount;
					}
				}
				TdsEnums.TransactionManagerIsolationLevel transactionManagerIsolationLevel;
				this._parser.TdsExecuteTransactionManagerRequest(null, transactionManagerRequestType, transactionName, transactionManagerIsolationLevel, base.ConnectionOptions.ConnectTimeout, internalTransaction, tdsParserStateObject, isDelegateControlRequest);
			}
			finally
			{
				if (flag)
				{
					parser.PutSession(tdsParserStateObject);
				}
				if (releaseConnectionLock)
				{
					this.ThreadHasParserLockForClose = false;
					this._parserLock.Release();
				}
			}
		}

		internal override void DelegatedTransactionEnded()
		{
			base.DelegatedTransactionEnded();
		}

		protected override byte[] GetDTCAddress()
		{
			return this._parser.GetDTCAddress(base.ConnectionOptions.ConnectTimeout, this._parser.GetSession(this));
		}

		protected override void PropagateTransactionCookie(byte[] cookie)
		{
			this._parser.PropagateDistributedTransaction(cookie, base.ConnectionOptions.ConnectTimeout, this._parser._physicalStateObj);
		}

		private void CompleteLogin(bool enlistOK)
		{
			this._parser.Run(RunBehavior.UntilDone, null, null, null, this._parser._physicalStateObj);
			if (this._routingInfo == null)
			{
				if (this._federatedAuthenticationRequested && !this._federatedAuthenticationAcknowledged)
				{
					throw SQL.ParsingError(ParsingErrorState.FedAuthNotAcknowledged);
				}
				if (!this._sessionRecoveryAcknowledged)
				{
					this._currentSessionData = null;
					if (this._recoverySessionData != null)
					{
						throw SQL.CR_NoCRAckAtReconnection(this);
					}
				}
				if (this._currentSessionData != null && this._recoverySessionData == null)
				{
					this._currentSessionData._initialDatabase = base.CurrentDatabase;
					this._currentSessionData._initialCollation = this._currentSessionData._collation;
					this._currentSessionData._initialLanguage = this._currentLanguage;
				}
				bool flag = this._parser.EncryptionOptions == EncryptionOptions.ON;
				if (this._recoverySessionData != null && this._recoverySessionData._encrypted != flag)
				{
					throw SQL.CR_EncryptionChanged(this);
				}
				if (this._currentSessionData != null)
				{
					this._currentSessionData._encrypted = flag;
				}
				this._recoverySessionData = null;
			}
			this._parser._physicalStateObj.SniContext = SniContext.Snix_EnableMars;
			this._parser.EnableMars();
			this._fConnectionOpen = true;
			if (enlistOK && base.ConnectionOptions.Enlist)
			{
				this._parser._physicalStateObj.SniContext = SniContext.Snix_AutoEnlist;
				Transaction currentTransaction = ADP.GetCurrentTransaction();
				base.Enlist(currentTransaction);
			}
			this._parser._physicalStateObj.SniContext = SniContext.Snix_Login;
		}

		private void Login(ServerInfo server, TimeoutTimer timeout, string newPassword, SecureString newSecurePassword)
		{
			SqlLogin sqlLogin = new SqlLogin();
			base.CurrentDatabase = server.ResolvedDatabaseName;
			this._currentPacketSize = base.ConnectionOptions.PacketSize;
			this._currentLanguage = base.ConnectionOptions.CurrentLanguage;
			int num = 0;
			if (!timeout.IsInfinite)
			{
				long num2 = timeout.MillisecondsRemaining / 1000L;
				if (2147483647L > num2)
				{
					num = (int)num2;
				}
			}
			sqlLogin.timeout = num;
			sqlLogin.userInstance = base.ConnectionOptions.UserInstance;
			sqlLogin.hostName = base.ConnectionOptions.ObtainWorkstationId();
			sqlLogin.userName = base.ConnectionOptions.UserID;
			sqlLogin.password = base.ConnectionOptions.Password;
			sqlLogin.applicationName = base.ConnectionOptions.ApplicationName;
			sqlLogin.language = this._currentLanguage;
			if (!sqlLogin.userInstance)
			{
				sqlLogin.database = base.CurrentDatabase;
				sqlLogin.attachDBFilename = base.ConnectionOptions.AttachDBFilename;
			}
			sqlLogin.serverName = server.UserServerName;
			sqlLogin.useReplication = base.ConnectionOptions.Replication;
			sqlLogin.useSSPI = base.ConnectionOptions.IntegratedSecurity;
			sqlLogin.packetSize = this._currentPacketSize;
			sqlLogin.newPassword = newPassword;
			sqlLogin.readOnlyIntent = base.ConnectionOptions.ApplicationIntent == ApplicationIntent.ReadOnly;
			sqlLogin.credential = this._credential;
			if (newSecurePassword != null)
			{
				sqlLogin.newSecurePassword = newSecurePassword;
			}
			TdsEnums.FeatureExtension featureExtension = TdsEnums.FeatureExtension.None;
			if (base.ConnectionOptions.ConnectRetryCount > 0)
			{
				featureExtension |= TdsEnums.FeatureExtension.SessionRecovery;
				this._sessionRecoveryRequested = true;
			}
			if (this._accessTokenInBytes != null)
			{
				featureExtension |= TdsEnums.FeatureExtension.FedAuth;
				this._fedAuthFeatureExtensionData = new FederatedAuthenticationFeatureExtensionData?(new FederatedAuthenticationFeatureExtensionData
				{
					libraryType = TdsEnums.FedAuthLibrary.SecurityToken,
					fedAuthRequiredPreLoginResponse = this._fedAuthRequired,
					accessToken = this._accessTokenInBytes
				});
				this._federatedAuthenticationRequested = true;
			}
			featureExtension |= TdsEnums.FeatureExtension.GlobalTransactions;
			this._parser.TdsLogin(sqlLogin, featureExtension, this._recoverySessionData, this._fedAuthFeatureExtensionData);
		}

		private void LoginFailure()
		{
			if (this._parser != null)
			{
				this._parser.Disconnect();
			}
		}

		private void OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, string newPassword, SecureString newSecurePassword, bool redirectedUserInstance)
		{
			ServerInfo serverInfo = new ServerInfo(connectionOptions);
			bool flag;
			string text;
			if (this.PoolGroupProviderInfo != null)
			{
				flag = this.PoolGroupProviderInfo.UseFailoverPartner;
				text = this.PoolGroupProviderInfo.FailoverPartner;
			}
			else
			{
				flag = false;
				text = base.ConnectionOptions.FailoverPartner;
			}
			this._timeoutErrorInternal.SetInternalSourceType(flag ? SqlConnectionInternalSourceType.Failover : SqlConnectionInternalSourceType.Principle);
			bool flag2 = !string.IsNullOrEmpty(text);
			try
			{
				this._timeoutErrorInternal.SetAndBeginPhase(SqlConnectionTimeoutErrorPhase.PreLoginBegin);
				if (flag2)
				{
					this._timeoutErrorInternal.SetFailoverScenario(true);
					this.LoginWithFailover(flag, serverInfo, text, newPassword, newSecurePassword, redirectedUserInstance, connectionOptions, credential, timeout);
				}
				else
				{
					this._timeoutErrorInternal.SetFailoverScenario(false);
					this.LoginNoFailover(serverInfo, newPassword, newSecurePassword, redirectedUserInstance, connectionOptions, credential, timeout);
				}
				this._timeoutErrorInternal.EndPhase(SqlConnectionTimeoutErrorPhase.PostLogin);
			}
			catch (Exception ex)
			{
				if (ADP.IsCatchableExceptionType(ex))
				{
					this.LoginFailure();
				}
				throw;
			}
			this._timeoutErrorInternal.SetAllCompleteMarker();
		}

		private bool IsDoNotRetryConnectError(SqlException exc)
		{
			return 18456 == exc.Number || 18488 == exc.Number || 1346 == exc.Number || exc._doNotReconnect;
		}

		private void LoginNoFailover(ServerInfo serverInfo, string newPassword, SecureString newSecurePassword, bool redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
		{
			int num = 0;
			ServerInfo serverInfo2 = serverInfo;
			int num2 = 100;
			this.ResolveExtendedServerName(serverInfo, !redirectedUserInstance, connectionOptions);
			long num3 = 0L;
			int num4;
			TimeoutTimer timeoutTimer;
			checked
			{
				if (connectionOptions.MultiSubnetFailover)
				{
					if (timeout.IsInfinite)
					{
						num3 = 1200L;
					}
					else
					{
						num3 = (long)(unchecked(0.08f * (float)timeout.MillisecondsRemaining));
					}
				}
				num4 = 0;
				timeoutTimer = null;
			}
			for (;;)
			{
				if (connectionOptions.MultiSubnetFailover)
				{
					num4++;
					checked
					{
						long num5 = num3 * unchecked((long)num4);
						long millisecondsRemaining = timeout.MillisecondsRemaining;
						if (num5 > millisecondsRemaining)
						{
							num5 = millisecondsRemaining;
						}
						timeoutTimer = TimeoutTimer.StartMillisecondsTimeout(num5);
					}
				}
				if (this._parser != null)
				{
					this._parser.Disconnect();
				}
				this._parser = new TdsParser(base.ConnectionOptions.MARS, base.ConnectionOptions.Asynchronous);
				try
				{
					this.AttemptOneLogin(serverInfo, newPassword, newSecurePassword, !connectionOptions.MultiSubnetFailover, connectionOptions.MultiSubnetFailover ? timeoutTimer : timeout, false);
					if (connectionOptions.MultiSubnetFailover && this.ServerProvidedFailOverPartner != null)
					{
						throw SQL.MultiSubnetFailoverWithFailoverPartner(true, this);
					}
					if (this._routingInfo == null)
					{
						goto IL_0271;
					}
					if (num > 0)
					{
						throw SQL.ROR_RecursiveRoutingNotSupported(this);
					}
					if (timeout.IsExpired)
					{
						throw SQL.ROR_TimeoutAfterRoutingInfo(this);
					}
					serverInfo = new ServerInfo(base.ConnectionOptions, this._routingInfo, serverInfo.ResolvedServerName);
					this._timeoutErrorInternal.SetInternalSourceType(SqlConnectionInternalSourceType.RoutingDestination);
					this._originalClientConnectionId = this._clientConnectionId;
					this._routingDestination = serverInfo.UserServerName;
					this._currentPacketSize = base.ConnectionOptions.PacketSize;
					this._currentLanguage = (this._originalLanguage = base.ConnectionOptions.CurrentLanguage);
					base.CurrentDatabase = (this._originalDatabase = base.ConnectionOptions.InitialCatalog);
					this._currentFailoverPartner = null;
					this._instanceName = string.Empty;
					num++;
					continue;
				}
				catch (SqlException ex)
				{
					if (this._parser == null || this._parser.State != TdsParserState.Closed || this.IsDoNotRetryConnectError(ex) || timeout.IsExpired)
					{
						throw;
					}
					if (timeout.MillisecondsRemaining <= (long)num2)
					{
						throw;
					}
				}
				if (this.ServerProvidedFailOverPartner != null)
				{
					break;
				}
				Thread.Sleep(num2);
				num2 = ((num2 < 500) ? (num2 * 2) : 1000);
			}
			if (connectionOptions.MultiSubnetFailover)
			{
				throw SQL.MultiSubnetFailoverWithFailoverPartner(true, this);
			}
			this._timeoutErrorInternal.ResetAndRestartPhase();
			this._timeoutErrorInternal.SetAndBeginPhase(SqlConnectionTimeoutErrorPhase.PreLoginBegin);
			this._timeoutErrorInternal.SetInternalSourceType(SqlConnectionInternalSourceType.Failover);
			this._timeoutErrorInternal.SetFailoverScenario(true);
			this.LoginWithFailover(true, serverInfo, this.ServerProvidedFailOverPartner, newPassword, newSecurePassword, redirectedUserInstance, connectionOptions, credential, timeout);
			return;
			IL_0271:
			if (this.PoolGroupProviderInfo != null)
			{
				this.PoolGroupProviderInfo.FailoverCheck(this, false, connectionOptions, this.ServerProvidedFailOverPartner);
			}
			base.CurrentDataSource = serverInfo2.UserServerName;
		}

		private void LoginWithFailover(bool useFailoverHost, ServerInfo primaryServerInfo, string failoverHost, string newPassword, SecureString newSecurePassword, bool redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
		{
			int num = 100;
			ServerInfo serverInfo = new ServerInfo(connectionOptions, failoverHost);
			this.ResolveExtendedServerName(primaryServerInfo, !redirectedUserInstance, connectionOptions);
			if (this.ServerProvidedFailOverPartner == null)
			{
				this.ResolveExtendedServerName(serverInfo, !redirectedUserInstance && failoverHost != primaryServerInfo.UserServerName, connectionOptions);
			}
			checked
			{
				long num2;
				if (timeout.IsInfinite)
				{
					num2 = (long)(unchecked(0.08f * (float)ADP.TimerFromSeconds(15)));
				}
				else
				{
					num2 = (long)(unchecked(0.08f * (float)timeout.MillisecondsRemaining));
				}
				int num3 = 0;
				for (;;)
				{
					long num4 = num2 * unchecked((long)(checked(num3 / 2 + 1)));
					long millisecondsRemaining = timeout.MillisecondsRemaining;
					if (num4 > millisecondsRemaining)
					{
						num4 = millisecondsRemaining;
					}
					TimeoutTimer timeoutTimer = TimeoutTimer.StartMillisecondsTimeout(num4);
					if (this._parser != null)
					{
						this._parser.Disconnect();
					}
					this._parser = new TdsParser(base.ConnectionOptions.MARS, base.ConnectionOptions.Asynchronous);
					ServerInfo serverInfo2;
					if (useFailoverHost)
					{
						if (this.ServerProvidedFailOverPartner != null && serverInfo.ResolvedServerName != this.ServerProvidedFailOverPartner)
						{
							serverInfo.SetDerivedNames(string.Empty, this.ServerProvidedFailOverPartner);
						}
						serverInfo2 = serverInfo;
						this._timeoutErrorInternal.SetInternalSourceType(SqlConnectionInternalSourceType.Failover);
					}
					else
					{
						serverInfo2 = primaryServerInfo;
						this._timeoutErrorInternal.SetInternalSourceType(SqlConnectionInternalSourceType.Principle);
					}
					unchecked
					{
						try
						{
							this.AttemptOneLogin(serverInfo2, newPassword, newSecurePassword, false, timeoutTimer, true);
							if (this._routingInfo != null)
							{
								throw SQL.ROR_UnexpectedRoutingInfo(this);
							}
							break;
						}
						catch (SqlException ex)
						{
							if (this.IsDoNotRetryConnectError(ex) || timeout.IsExpired)
							{
								throw;
							}
							if (base.IsConnectionDoomed)
							{
								throw;
							}
							if (1 == num3 % 2 && timeout.MillisecondsRemaining <= (long)num)
							{
								throw;
							}
						}
						if (1 == num3 % 2)
						{
							Thread.Sleep(num);
							num = ((num < 500) ? (num * 2) : 1000);
						}
						num3++;
						useFailoverHost = !useFailoverHost;
					}
				}
				if (useFailoverHost && this.ServerProvidedFailOverPartner == null)
				{
					throw SQL.InvalidPartnerConfiguration(failoverHost, base.CurrentDatabase);
				}
				if (this.PoolGroupProviderInfo != null)
				{
					this.PoolGroupProviderInfo.FailoverCheck(this, useFailoverHost, connectionOptions, this.ServerProvidedFailOverPartner);
				}
				base.CurrentDataSource = (useFailoverHost ? failoverHost : primaryServerInfo.UserServerName);
			}
		}

		private void ResolveExtendedServerName(ServerInfo serverInfo, bool aliasLookup, SqlConnectionString options)
		{
			if (serverInfo.ExtendedServerName == null)
			{
				string userServerName = serverInfo.UserServerName;
				string userProtocol = serverInfo.UserProtocol;
				serverInfo.SetDerivedNames(userProtocol, userServerName);
			}
		}

		private void AttemptOneLogin(ServerInfo serverInfo, string newPassword, SecureString newSecurePassword, bool ignoreSniOpenTimeout, TimeoutTimer timeout, bool withFailover = false)
		{
			this._routingInfo = null;
			this._parser._physicalStateObj.SniContext = SniContext.Snix_Connect;
			this._parser.Connect(serverInfo, this, ignoreSniOpenTimeout, timeout.LegacyTimerExpire, base.ConnectionOptions.Encrypt, base.ConnectionOptions.TrustServerCertificate, base.ConnectionOptions.IntegratedSecurity, withFailover);
			this._timeoutErrorInternal.EndPhase(SqlConnectionTimeoutErrorPhase.ConsumePreLoginHandshake);
			this._timeoutErrorInternal.SetAndBeginPhase(SqlConnectionTimeoutErrorPhase.LoginBegin);
			this._parser._physicalStateObj.SniContext = SniContext.Snix_Login;
			this.Login(serverInfo, timeout, newPassword, newSecurePassword);
			this._timeoutErrorInternal.EndPhase(SqlConnectionTimeoutErrorPhase.ProcessConnectionAuth);
			this._timeoutErrorInternal.SetAndBeginPhase(SqlConnectionTimeoutErrorPhase.PostLogin);
			this.CompleteLogin(!base.ConnectionOptions.Pooling);
			this._timeoutErrorInternal.EndPhase(SqlConnectionTimeoutErrorPhase.PostLogin);
		}

		protected override object ObtainAdditionalLocksForClose()
		{
			bool flag = !this.ThreadHasParserLockForClose;
			if (flag)
			{
				this._parserLock.Wait(false);
				this.ThreadHasParserLockForClose = true;
			}
			return flag;
		}

		protected override void ReleaseAdditionalLocksForClose(object lockToken)
		{
			if ((bool)lockToken)
			{
				this.ThreadHasParserLockForClose = false;
				this._parserLock.Release();
			}
		}

		internal bool GetSessionAndReconnectIfNeeded(SqlConnection parent, int timeout = 0)
		{
			if (this.ThreadHasParserLockForClose)
			{
				return false;
			}
			this._parserLock.Wait(false);
			this.ThreadHasParserLockForClose = true;
			bool releaseConnectionLock = true;
			bool flag;
			try
			{
				Task task = parent.ValidateAndReconnect(delegate
				{
					this.ThreadHasParserLockForClose = false;
					this._parserLock.Release();
					releaseConnectionLock = false;
				}, timeout);
				if (task != null)
				{
					AsyncHelper.WaitForCompletion(task, timeout, null, true);
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			finally
			{
				if (releaseConnectionLock)
				{
					this.ThreadHasParserLockForClose = false;
					this._parserLock.Release();
				}
			}
			return flag;
		}

		internal void BreakConnection()
		{
			SqlConnection connection = base.Connection;
			base.DoomThisConnection();
			if (connection != null)
			{
				connection.Close();
			}
		}

		internal bool IgnoreEnvChange
		{
			get
			{
				return this._routingInfo != null;
			}
		}

		internal void OnEnvChange(SqlEnvChange rec)
		{
			switch (rec.type)
			{
			case 1:
				if (!this._fConnectionOpen && this._recoverySessionData == null)
				{
					this._originalDatabase = rec.newValue;
				}
				base.CurrentDatabase = rec.newValue;
				return;
			case 2:
				if (!this._fConnectionOpen && this._recoverySessionData == null)
				{
					this._originalLanguage = rec.newValue;
				}
				this._currentLanguage = rec.newValue;
				return;
			case 3:
			case 5:
			case 6:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 14:
			case 16:
			case 17:
				break;
			case 4:
				this._currentPacketSize = int.Parse(rec.newValue, CultureInfo.InvariantCulture);
				return;
			case 7:
				if (this._currentSessionData != null)
				{
					this._currentSessionData._collation = rec.newCollation;
					return;
				}
				break;
			case 13:
				if (base.ConnectionOptions.ApplicationIntent == ApplicationIntent.ReadOnly)
				{
					throw SQL.ROR_FailoverNotSupportedServer(this);
				}
				this._currentFailoverPartner = rec.newValue;
				return;
			case 15:
				base.PromotedDTCToken = rec.newBinValue;
				return;
			case 18:
				if (this._currentSessionData != null)
				{
					this._currentSessionData.Reset();
					return;
				}
				break;
			case 19:
				this._instanceName = rec.newValue;
				return;
			case 20:
				if (string.IsNullOrEmpty(rec.newRoutingInfo.ServerName) || rec.newRoutingInfo.Protocol != 0 || rec.newRoutingInfo.Port == 0)
				{
					throw SQL.ROR_InvalidRoutingInfo(this);
				}
				this._routingInfo = rec.newRoutingInfo;
				break;
			default:
				return;
			}
		}

		internal void OnLoginAck(SqlLoginAck rec)
		{
			this._loginAck = rec;
			if (this._recoverySessionData != null && this._recoverySessionData._tdsVersion != rec.tdsVersion)
			{
				throw SQL.CR_TDSVersionNotPreserved(this);
			}
			if (this._currentSessionData != null)
			{
				this._currentSessionData._tdsVersion = rec.tdsVersion;
			}
		}

		internal void OnFeatureExtAck(int featureId, byte[] data)
		{
			if (this._routingInfo != null)
			{
				return;
			}
			switch (featureId)
			{
			case 1:
			{
				if (!this._sessionRecoveryRequested)
				{
					throw SQL.ParsingError();
				}
				this._sessionRecoveryAcknowledged = true;
				int i = 0;
				while (i < data.Length)
				{
					byte b = data[i];
					i++;
					byte b2 = data[i];
					i++;
					int num;
					if (b2 == 255)
					{
						num = BitConverter.ToInt32(data, i);
						i += 4;
					}
					else
					{
						num = (int)b2;
					}
					byte[] array = new byte[num];
					Buffer.BlockCopy(data, i, array, 0, num);
					i += num;
					if (this._recoverySessionData == null)
					{
						this._currentSessionData._initialState[(int)b] = array;
					}
					else
					{
						this._currentSessionData._delta[(int)b] = new SessionStateRecord
						{
							_data = array,
							_dataLength = num,
							_recoverable = true,
							_version = 0U
						};
						this._currentSessionData._deltaDirty = true;
					}
				}
				return;
			}
			case 2:
				if (!this._federatedAuthenticationRequested)
				{
					throw SQL.ParsingErrorFeatureId(ParsingErrorState.UnrequestedFeatureAckReceived, featureId);
				}
				if (this._fedAuthFeatureExtensionData.Value.libraryType != TdsEnums.FedAuthLibrary.SecurityToken)
				{
					throw SQL.ParsingErrorLibraryType(ParsingErrorState.FedAuthFeatureAckUnknownLibraryType, (int)this._fedAuthFeatureExtensionData.Value.libraryType);
				}
				if (data.Length != 0)
				{
					throw SQL.ParsingError(ParsingErrorState.FedAuthFeatureAckContainsExtraData);
				}
				this._federatedAuthenticationAcknowledged = true;
				return;
			case 5:
				if (data.Length < 1)
				{
					throw SQL.ParsingError();
				}
				base.IsGlobalTransaction = true;
				if (1 == data[0])
				{
					base.IsGlobalTransactionsEnabledForServer = true;
					return;
				}
				return;
			}
			throw SQL.ParsingError();
		}

		internal bool ThreadHasParserLockForClose
		{
			get
			{
				return this._threadIdOwningParserLock == Thread.CurrentThread.ManagedThreadId;
			}
			set
			{
				if (value)
				{
					this._threadIdOwningParserLock = Thread.CurrentThread.ManagedThreadId;
					return;
				}
				if (this._threadIdOwningParserLock == Thread.CurrentThread.ManagedThreadId)
				{
					this._threadIdOwningParserLock = -1;
				}
			}
		}

		internal override bool TryReplaceConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, DbConnectionOptions userOptions)
		{
			return base.TryOpenConnectionInternal(outerConnection, connectionFactory, retry, userOptions);
		}

		private readonly SqlConnectionPoolGroupProviderInfo _poolGroupProviderInfo;

		private TdsParser _parser;

		private SqlLoginAck _loginAck;

		private SqlCredential _credential;

		private FederatedAuthenticationFeatureExtensionData? _fedAuthFeatureExtensionData;

		private bool _sessionRecoveryRequested;

		internal bool _sessionRecoveryAcknowledged;

		internal SessionData _currentSessionData;

		private SessionData _recoverySessionData;

		internal bool _fedAuthRequired;

		internal bool _federatedAuthenticationRequested;

		internal bool _federatedAuthenticationAcknowledged;

		internal byte[] _accessTokenInBytes;

		private static readonly HashSet<int> s_transientErrors = new HashSet<int> { 4060, 10928, 10929, 40197, 40501, 40613 };

		private bool _fConnectionOpen;

		private bool _fResetConnection;

		private string _originalDatabase;

		private string _currentFailoverPartner;

		private string _originalLanguage;

		private string _currentLanguage;

		private int _currentPacketSize;

		private int _asyncCommandCount;

		private string _instanceName = string.Empty;

		private DbConnectionPoolIdentity _identity;

		internal SqlInternalConnectionTds.SyncAsyncLock _parserLock = new SqlInternalConnectionTds.SyncAsyncLock();

		private int _threadIdOwningParserLock = -1;

		private SqlConnectionTimeoutErrorInternal _timeoutErrorInternal;

		internal Guid _clientConnectionId = Guid.Empty;

		private RoutingInfo _routingInfo;

		private Guid _originalClientConnectionId = Guid.Empty;

		private string _routingDestination;

		private readonly TimeoutTimer _timeout;

		internal class SyncAsyncLock
		{
			internal void Wait(bool canReleaseFromAnyThread)
			{
				Monitor.Enter(this._semaphore);
				if (canReleaseFromAnyThread || this._semaphore.CurrentCount == 0)
				{
					this._semaphore.Wait();
					if (canReleaseFromAnyThread)
					{
						Monitor.Exit(this._semaphore);
						return;
					}
					this._semaphore.Release();
				}
			}

			internal void Wait(bool canReleaseFromAnyThread, int timeout, ref bool lockTaken)
			{
				lockTaken = false;
				bool flag = false;
				try
				{
					Monitor.TryEnter(this._semaphore, timeout, ref flag);
					if (flag)
					{
						if (canReleaseFromAnyThread || this._semaphore.CurrentCount == 0)
						{
							if (this._semaphore.Wait(timeout))
							{
								if (canReleaseFromAnyThread)
								{
									Monitor.Exit(this._semaphore);
									flag = false;
								}
								else
								{
									this._semaphore.Release();
								}
								lockTaken = true;
							}
						}
						else
						{
							lockTaken = true;
						}
					}
				}
				finally
				{
					if (!lockTaken && flag)
					{
						Monitor.Exit(this._semaphore);
					}
				}
			}

			internal void Release()
			{
				if (this._semaphore.CurrentCount == 0)
				{
					this._semaphore.Release();
					return;
				}
				Monitor.Exit(this._semaphore);
			}

			internal bool CanBeReleasedFromAnyThread
			{
				get
				{
					return this._semaphore.CurrentCount == 0;
				}
			}

			internal bool ThreadMayHaveLock()
			{
				return Monitor.IsEntered(this._semaphore) || this._semaphore.CurrentCount == 0;
			}

			private SemaphoreSlim _semaphore = new SemaphoreSlim(1);
		}
	}
}
