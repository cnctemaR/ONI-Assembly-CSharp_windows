using System;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Transactions;

namespace System.Data.SqlClient
{
	internal sealed class SqlDelegatedTransaction : IPromotableSinglePhaseNotification, ITransactionPromoter
	{
		internal int ObjectID
		{
			get
			{
				return this._objectID;
			}
		}

		internal SqlDelegatedTransaction(SqlInternalConnection connection, Transaction tx)
		{
			this._connection = connection;
			this._atomicTransaction = tx;
			this._active = false;
			IsolationLevel isolationLevel = tx.IsolationLevel;
			switch (isolationLevel)
			{
			case IsolationLevel.Serializable:
				this._isolationLevel = IsolationLevel.Serializable;
				return;
			case IsolationLevel.RepeatableRead:
				this._isolationLevel = IsolationLevel.RepeatableRead;
				return;
			case IsolationLevel.ReadCommitted:
				this._isolationLevel = IsolationLevel.ReadCommitted;
				return;
			case IsolationLevel.ReadUncommitted:
				this._isolationLevel = IsolationLevel.ReadUncommitted;
				return;
			case IsolationLevel.Snapshot:
				this._isolationLevel = IsolationLevel.Snapshot;
				return;
			default:
				throw SQL.UnknownSysTxIsolationLevel(isolationLevel);
			}
		}

		internal Transaction Transaction
		{
			get
			{
				return this._atomicTransaction;
			}
		}

		public void Initialize()
		{
			SqlInternalConnection connection = this._connection;
			SqlConnection connection2 = connection.Connection;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				if (connection.IsEnlistedInTransaction)
				{
					connection.EnlistNull();
				}
				this._internalTransaction = new SqlInternalTransaction(connection, TransactionType.Delegated, null);
				connection.ExecuteTransaction(SqlInternalConnection.TransactionRequest.Begin, null, this._isolationLevel, this._internalTransaction, true);
				if (connection.CurrentTransaction == null)
				{
					connection.DoomThisConnection();
					throw ADP.InternalError(ADP.InternalErrorCode.UnknownTransactionFailure);
				}
				this._active = true;
			}
			catch (OutOfMemoryException ex)
			{
				connection2.Abort(ex);
				throw;
			}
			catch (StackOverflowException ex2)
			{
				connection2.Abort(ex2);
				throw;
			}
			catch (ThreadAbortException ex3)
			{
				connection2.Abort(ex3);
				throw;
			}
		}

		internal bool IsActive
		{
			get
			{
				return this._active;
			}
		}

		public byte[] Promote()
		{
			SqlInternalConnection validConnection = this.GetValidConnection();
			byte[] array = null;
			SqlConnection connection = validConnection.Connection;
			RuntimeHelpers.PrepareConstrainedRegions();
			Exception ex;
			try
			{
				SqlInternalConnection sqlInternalConnection = validConnection;
				lock (sqlInternalConnection)
				{
					try
					{
						this.ValidateActiveOnConnection(validConnection);
						validConnection.ExecuteTransaction(SqlInternalConnection.TransactionRequest.Promote, null, IsolationLevel.Unspecified, this._internalTransaction, true);
						array = this._connection.PromotedDTCToken;
						if (this._connection.IsGlobalTransaction)
						{
							if (SysTxForGlobalTransactions.SetDistributedTransactionIdentifier == null)
							{
								throw SQL.UnsupportedSysTxForGlobalTransactions();
							}
							if (!this._connection.IsGlobalTransactionsEnabledForServer)
							{
								throw SQL.GlobalTransactionsNotEnabled();
							}
							SysTxForGlobalTransactions.SetDistributedTransactionIdentifier.Invoke(this._atomicTransaction, new object[]
							{
								this,
								this.GetGlobalTxnIdentifierFromToken()
							});
						}
						ex = null;
					}
					catch (SqlException ex)
					{
						validConnection.DoomThisConnection();
					}
					catch (InvalidOperationException ex)
					{
						validConnection.DoomThisConnection();
					}
				}
			}
			catch (OutOfMemoryException ex2)
			{
				connection.Abort(ex2);
				throw;
			}
			catch (StackOverflowException ex3)
			{
				connection.Abort(ex3);
				throw;
			}
			catch (ThreadAbortException ex4)
			{
				connection.Abort(ex4);
				throw;
			}
			if (ex != null)
			{
				throw SQL.PromotionFailed(ex);
			}
			return array;
		}

		public void Rollback(SinglePhaseEnlistment enlistment)
		{
			SqlInternalConnection validConnection = this.GetValidConnection();
			SqlConnection connection = validConnection.Connection;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				SqlInternalConnection sqlInternalConnection = validConnection;
				lock (sqlInternalConnection)
				{
					try
					{
						this.ValidateActiveOnConnection(validConnection);
						this._active = false;
						this._connection = null;
						if (!this._internalTransaction.IsAborted)
						{
							validConnection.ExecuteTransaction(SqlInternalConnection.TransactionRequest.Rollback, null, IsolationLevel.Unspecified, this._internalTransaction, true);
						}
					}
					catch (SqlException)
					{
						validConnection.DoomThisConnection();
					}
					catch (InvalidOperationException)
					{
						validConnection.DoomThisConnection();
					}
				}
				validConnection.CleanupConnectionOnTransactionCompletion(this._atomicTransaction);
				enlistment.Aborted();
			}
			catch (OutOfMemoryException ex)
			{
				connection.Abort(ex);
				throw;
			}
			catch (StackOverflowException ex2)
			{
				connection.Abort(ex2);
				throw;
			}
			catch (ThreadAbortException ex3)
			{
				connection.Abort(ex3);
				throw;
			}
		}

		public void SinglePhaseCommit(SinglePhaseEnlistment enlistment)
		{
			SqlInternalConnection validConnection = this.GetValidConnection();
			SqlConnection connection = validConnection.Connection;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				if (validConnection.IsConnectionDoomed)
				{
					SqlInternalConnection sqlInternalConnection = validConnection;
					lock (sqlInternalConnection)
					{
						this._active = false;
						this._connection = null;
					}
					enlistment.Aborted(SQL.ConnectionDoomed());
				}
				else
				{
					SqlInternalConnection sqlInternalConnection = validConnection;
					Exception ex;
					lock (sqlInternalConnection)
					{
						try
						{
							this.ValidateActiveOnConnection(validConnection);
							this._active = false;
							this._connection = null;
							validConnection.ExecuteTransaction(SqlInternalConnection.TransactionRequest.Commit, null, IsolationLevel.Unspecified, this._internalTransaction, true);
							ex = null;
						}
						catch (SqlException ex)
						{
							validConnection.DoomThisConnection();
						}
						catch (InvalidOperationException ex)
						{
							validConnection.DoomThisConnection();
						}
					}
					if (ex != null)
					{
						if (this._internalTransaction.IsCommitted)
						{
							enlistment.Committed();
						}
						else if (this._internalTransaction.IsAborted)
						{
							enlistment.Aborted(ex);
						}
						else
						{
							enlistment.InDoubt(ex);
						}
					}
					validConnection.CleanupConnectionOnTransactionCompletion(this._atomicTransaction);
					if (ex == null)
					{
						enlistment.Committed();
					}
				}
			}
			catch (OutOfMemoryException ex2)
			{
				connection.Abort(ex2);
				throw;
			}
			catch (StackOverflowException ex3)
			{
				connection.Abort(ex3);
				throw;
			}
			catch (ThreadAbortException ex4)
			{
				connection.Abort(ex4);
				throw;
			}
		}

		internal void TransactionEnded(Transaction transaction)
		{
			SqlInternalConnection connection = this._connection;
			if (connection != null)
			{
				SqlInternalConnection sqlInternalConnection = connection;
				lock (sqlInternalConnection)
				{
					if (this._atomicTransaction.Equals(transaction))
					{
						this._active = false;
						this._connection = null;
					}
				}
			}
		}

		private SqlInternalConnection GetValidConnection()
		{
			SqlInternalConnection connection = this._connection;
			if (connection == null)
			{
				throw ADP.ObjectDisposed(this);
			}
			return connection;
		}

		private void ValidateActiveOnConnection(SqlInternalConnection connection)
		{
			if (!this._active || connection != this._connection || connection.DelegatedTransaction != this)
			{
				if (connection != null)
				{
					connection.DoomThisConnection();
				}
				if (connection != this._connection && this._connection != null)
				{
					this._connection.DoomThisConnection();
				}
				throw ADP.InternalError(ADP.InternalErrorCode.UnpooledObjectHasWrongOwner);
			}
		}

		private Guid GetGlobalTxnIdentifierFromToken()
		{
			byte[] array = new byte[16];
			Array.Copy(this._connection.PromotedDTCToken, 4, array, 0, array.Length);
			return new Guid(array);
		}

		private static int _objectTypeCount;

		private readonly int _objectID = Interlocked.Increment(ref SqlDelegatedTransaction._objectTypeCount);

		private const int _globalTransactionsTokenVersionSizeInBytes = 4;

		private SqlInternalConnection _connection;

		private IsolationLevel _isolationLevel;

		private SqlInternalTransaction _internalTransaction;

		private Transaction _atomicTransaction;

		private bool _active;
	}
}
