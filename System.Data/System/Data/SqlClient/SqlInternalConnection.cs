using System;
using System.Data.Common;
using System.Data.ProviderBase;
using System.Threading;
using System.Transactions;

namespace System.Data.SqlClient
{
	internal abstract class SqlInternalConnection : DbConnectionInternal
	{
		internal string CurrentDatabase { get; set; }

		internal string CurrentDataSource { get; set; }

		internal SqlDelegatedTransaction DelegatedTransaction { get; set; }

		internal SqlInternalConnection(SqlConnectionString connectionOptions)
		{
			this._connectionOptions = connectionOptions;
		}

		internal SqlConnection Connection
		{
			get
			{
				return (SqlConnection)base.Owner;
			}
		}

		internal SqlConnectionString ConnectionOptions
		{
			get
			{
				return this._connectionOptions;
			}
		}

		internal abstract SqlInternalTransaction CurrentTransaction { get; }

		internal virtual SqlInternalTransaction AvailableInternalTransaction
		{
			get
			{
				return this.CurrentTransaction;
			}
		}

		internal abstract SqlInternalTransaction PendingTransaction { get; }

		protected internal override bool IsNonPoolableTransactionRoot
		{
			get
			{
				return this.IsTransactionRoot;
			}
		}

		internal override bool IsTransactionRoot
		{
			get
			{
				SqlDelegatedTransaction delegatedTransaction = this.DelegatedTransaction;
				return delegatedTransaction != null && delegatedTransaction.IsActive;
			}
		}

		internal bool HasLocalTransaction
		{
			get
			{
				SqlInternalTransaction currentTransaction = this.CurrentTransaction;
				return currentTransaction != null && currentTransaction.IsLocal;
			}
		}

		internal bool HasLocalTransactionFromAPI
		{
			get
			{
				SqlInternalTransaction currentTransaction = this.CurrentTransaction;
				return currentTransaction != null && currentTransaction.HasParentTransaction;
			}
		}

		internal bool IsEnlistedInTransaction
		{
			get
			{
				return this._isEnlistedInTransaction;
			}
		}

		internal abstract bool IsLockedForBulkCopy { get; }

		internal abstract bool IsKatmaiOrNewer { get; }

		internal byte[] PromotedDTCToken
		{
			get
			{
				return this._promotedDTCToken;
			}
			set
			{
				this._promotedDTCToken = value;
			}
		}

		internal bool IsGlobalTransaction
		{
			get
			{
				return this._isGlobalTransaction;
			}
			set
			{
				this._isGlobalTransaction = value;
			}
		}

		internal bool IsGlobalTransactionsEnabledForServer
		{
			get
			{
				return this._isGlobalTransactionEnabledForServer;
			}
			set
			{
				this._isGlobalTransactionEnabledForServer = value;
			}
		}

		public override DbTransaction BeginTransaction(IsolationLevel iso)
		{
			return this.BeginSqlTransaction(iso, null, false);
		}

		internal virtual SqlTransaction BeginSqlTransaction(IsolationLevel iso, string transactionName, bool shouldReconnect)
		{
			SqlStatistics sqlStatistics = null;
			SqlTransaction sqlTransaction2;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Connection.Statistics);
				this.ValidateConnectionForExecute(null);
				if (this.HasLocalTransactionFromAPI)
				{
					throw ADP.ParallelTransactionsNotSupported(this.Connection);
				}
				if (iso == IsolationLevel.Unspecified)
				{
					iso = IsolationLevel.ReadCommitted;
				}
				SqlTransaction sqlTransaction = new SqlTransaction(this, this.Connection, iso, this.AvailableInternalTransaction);
				sqlTransaction.InternalTransaction.RestoreBrokenConnection = shouldReconnect;
				this.ExecuteTransaction(SqlInternalConnection.TransactionRequest.Begin, transactionName, iso, sqlTransaction.InternalTransaction, false);
				sqlTransaction.InternalTransaction.RestoreBrokenConnection = false;
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
			if (string.IsNullOrEmpty(database))
			{
				throw ADP.EmptyDatabaseName();
			}
			this.ValidateConnectionForExecute(null);
			this.ChangeDatabaseInternal(database);
		}

		protected abstract void ChangeDatabaseInternal(string database);

		protected override void CleanupTransactionOnCompletion(Transaction transaction)
		{
			SqlDelegatedTransaction delegatedTransaction = this.DelegatedTransaction;
			if (delegatedTransaction != null)
			{
				delegatedTransaction.TransactionEnded(transaction);
			}
		}

		protected override DbReferenceCollection CreateReferenceCollection()
		{
			return new SqlReferenceCollection();
		}

		protected override void Deactivate()
		{
			try
			{
				SqlReferenceCollection sqlReferenceCollection = (SqlReferenceCollection)base.ReferenceCollection;
				if (sqlReferenceCollection != null)
				{
					sqlReferenceCollection.Deactivate();
				}
				this.InternalDeactivate();
			}
			catch (Exception ex)
			{
				if (!ADP.IsCatchableExceptionType(ex))
				{
					throw;
				}
				base.DoomThisConnection();
			}
		}

		internal abstract void DisconnectTransaction(SqlInternalTransaction internalTransaction);

		public override void Dispose()
		{
			this._whereAbouts = null;
			base.Dispose();
		}

		protected void Enlist(Transaction tx)
		{
			if (null == tx)
			{
				if (this.IsEnlistedInTransaction)
				{
					this.EnlistNull();
					return;
				}
				Transaction enlistedTransaction = base.EnlistedTransaction;
				if (enlistedTransaction != null && enlistedTransaction.TransactionInformation.Status != TransactionStatus.Active)
				{
					this.EnlistNull();
					return;
				}
			}
			else if (!tx.Equals(base.EnlistedTransaction))
			{
				this.EnlistNonNull(tx);
			}
		}

		private void EnlistNonNull(Transaction tx)
		{
			bool flag = false;
			SqlDelegatedTransaction sqlDelegatedTransaction = new SqlDelegatedTransaction(this, tx);
			try
			{
				if (this._isGlobalTransaction)
				{
					if (SysTxForGlobalTransactions.EnlistPromotableSinglePhase == null)
					{
						flag = tx.EnlistPromotableSinglePhase(sqlDelegatedTransaction);
					}
					else
					{
						flag = (bool)SysTxForGlobalTransactions.EnlistPromotableSinglePhase.Invoke(tx, new object[]
						{
							sqlDelegatedTransaction,
							SqlInternalConnection._globalTransactionTMID
						});
					}
				}
				else
				{
					flag = tx.EnlistPromotableSinglePhase(sqlDelegatedTransaction);
				}
				if (flag)
				{
					this.DelegatedTransaction = sqlDelegatedTransaction;
				}
			}
			catch (SqlException ex)
			{
				if (ex.Class >= 20)
				{
					throw;
				}
				SqlInternalConnectionTds sqlInternalConnectionTds = this as SqlInternalConnectionTds;
				if (sqlInternalConnectionTds != null)
				{
					TdsParser parser = sqlInternalConnectionTds.Parser;
					if (parser == null || parser.State != TdsParserState.OpenLoggedIn)
					{
						throw;
					}
				}
			}
			if (!flag)
			{
				byte[] array;
				if (this._isGlobalTransaction)
				{
					if (SysTxForGlobalTransactions.GetPromotedToken == null)
					{
						throw SQL.UnsupportedSysTxForGlobalTransactions();
					}
					array = (byte[])SysTxForGlobalTransactions.GetPromotedToken.Invoke(tx, null);
				}
				else
				{
					if (this._whereAbouts == null)
					{
						byte[] dtcaddress = this.GetDTCAddress();
						if (dtcaddress == null)
						{
							throw SQL.CannotGetDTCAddress();
						}
						this._whereAbouts = dtcaddress;
					}
					array = SqlInternalConnection.GetTransactionCookie(tx, this._whereAbouts);
				}
				this.PropagateTransactionCookie(array);
				this._isEnlistedInTransaction = true;
			}
			base.EnlistedTransaction = tx;
		}

		internal void EnlistNull()
		{
			this.PropagateTransactionCookie(null);
			this._isEnlistedInTransaction = false;
			base.EnlistedTransaction = null;
		}

		public override void EnlistTransaction(Transaction transaction)
		{
			this.ValidateConnectionForExecute(null);
			if (this.HasLocalTransaction)
			{
				throw ADP.LocalTransactionPresent();
			}
			if (null != transaction && transaction.Equals(base.EnlistedTransaction))
			{
				return;
			}
			try
			{
				this.Enlist(transaction);
			}
			catch (OutOfMemoryException ex)
			{
				this.Connection.Abort(ex);
				throw;
			}
			catch (StackOverflowException ex2)
			{
				this.Connection.Abort(ex2);
				throw;
			}
			catch (ThreadAbortException ex3)
			{
				this.Connection.Abort(ex3);
				throw;
			}
		}

		internal abstract void ExecuteTransaction(SqlInternalConnection.TransactionRequest transactionRequest, string name, IsolationLevel iso, SqlInternalTransaction internalTransaction, bool isDelegateControlRequest);

		internal SqlDataReader FindLiveReader(SqlCommand command)
		{
			SqlDataReader sqlDataReader = null;
			SqlReferenceCollection sqlReferenceCollection = (SqlReferenceCollection)base.ReferenceCollection;
			if (sqlReferenceCollection != null)
			{
				sqlDataReader = sqlReferenceCollection.FindLiveReader(command);
			}
			return sqlDataReader;
		}

		internal SqlCommand FindLiveCommand(TdsParserStateObject stateObj)
		{
			SqlCommand sqlCommand = null;
			SqlReferenceCollection sqlReferenceCollection = (SqlReferenceCollection)base.ReferenceCollection;
			if (sqlReferenceCollection != null)
			{
				sqlCommand = sqlReferenceCollection.FindLiveCommand(stateObj);
			}
			return sqlCommand;
		}

		protected abstract byte[] GetDTCAddress();

		private static byte[] GetTransactionCookie(Transaction transaction, byte[] whereAbouts)
		{
			byte[] array = null;
			if (null != transaction)
			{
				array = TransactionInterop.GetExportCookie(transaction, whereAbouts);
			}
			return array;
		}

		protected virtual void InternalDeactivate()
		{
		}

		internal void OnError(SqlException exception, bool breakConnection, Action<Action> wrapCloseInAction = null)
		{
			if (breakConnection)
			{
				base.DoomThisConnection();
			}
			SqlConnection connection = this.Connection;
			if (connection != null)
			{
				connection.OnError(exception, breakConnection, wrapCloseInAction);
				return;
			}
			if (exception.Class >= 11)
			{
				throw exception;
			}
		}

		protected abstract void PropagateTransactionCookie(byte[] transactionCookie);

		internal abstract void ValidateConnectionForExecute(SqlCommand command);

		private readonly SqlConnectionString _connectionOptions;

		private bool _isEnlistedInTransaction;

		private byte[] _promotedDTCToken;

		private byte[] _whereAbouts;

		private bool _isGlobalTransaction;

		private bool _isGlobalTransactionEnabledForServer;

		private static readonly Guid _globalTransactionTMID = new Guid("1c742caf-6680-40ea-9c26-6b6846079764");

		internal enum TransactionRequest
		{
			Begin,
			Promote,
			Commit,
			Rollback,
			IfRollback,
			Save
		}
	}
}
