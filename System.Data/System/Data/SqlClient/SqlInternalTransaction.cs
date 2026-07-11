using System;
using System.Data.Common;
using System.Threading;

namespace System.Data.SqlClient
{
	internal sealed class SqlInternalTransaction
	{
		internal bool RestoreBrokenConnection { get; set; }

		internal bool ConnectionHasBeenRestored { get; set; }

		internal SqlInternalTransaction(SqlInternalConnection innerConnection, TransactionType type, SqlTransaction outerTransaction)
			: this(innerConnection, type, outerTransaction, 0L)
		{
		}

		internal SqlInternalTransaction(SqlInternalConnection innerConnection, TransactionType type, SqlTransaction outerTransaction, long transactionId)
		{
			this._innerConnection = innerConnection;
			this._transactionType = type;
			if (outerTransaction != null)
			{
				this._parent = new WeakReference(outerTransaction);
			}
			this._transactionId = transactionId;
			this.RestoreBrokenConnection = false;
			this.ConnectionHasBeenRestored = false;
		}

		internal bool HasParentTransaction
		{
			get
			{
				return TransactionType.LocalFromAPI == this._transactionType || (TransactionType.LocalFromTSQL == this._transactionType && this._parent != null);
			}
		}

		internal bool IsAborted
		{
			get
			{
				return TransactionState.Aborted == this._transactionState;
			}
		}

		internal bool IsActive
		{
			get
			{
				return TransactionState.Active == this._transactionState;
			}
		}

		internal bool IsCommitted
		{
			get
			{
				return TransactionState.Committed == this._transactionState;
			}
		}

		internal bool IsCompleted
		{
			get
			{
				return TransactionState.Aborted == this._transactionState || TransactionState.Committed == this._transactionState || TransactionState.Unknown == this._transactionState;
			}
		}

		internal bool IsDelegated
		{
			get
			{
				return TransactionType.Delegated == this._transactionType;
			}
		}

		internal bool IsDistributed
		{
			get
			{
				return TransactionType.Distributed == this._transactionType;
			}
		}

		internal bool IsLocal
		{
			get
			{
				return TransactionType.LocalFromTSQL == this._transactionType || TransactionType.LocalFromAPI == this._transactionType;
			}
		}

		internal bool IsOrphaned
		{
			get
			{
				return this._parent != null && this._parent.Target == null;
			}
		}

		internal bool IsZombied
		{
			get
			{
				return this._innerConnection == null;
			}
		}

		internal int OpenResultsCount
		{
			get
			{
				return this._openResultCount;
			}
		}

		internal SqlTransaction Parent
		{
			get
			{
				SqlTransaction sqlTransaction = null;
				if (this._parent != null)
				{
					sqlTransaction = (SqlTransaction)this._parent.Target;
				}
				return sqlTransaction;
			}
		}

		internal long TransactionId
		{
			get
			{
				return this._transactionId;
			}
			set
			{
				this._transactionId = value;
			}
		}

		internal void Activate()
		{
			this._transactionState = TransactionState.Active;
		}

		private void CheckTransactionLevelAndZombie()
		{
			try
			{
				if (!this.IsZombied && this.GetServerTransactionLevel() == 0)
				{
					this.Zombie();
				}
			}
			catch (Exception ex)
			{
				if (!ADP.IsCatchableExceptionType(ex))
				{
					throw;
				}
				this.Zombie();
			}
		}

		internal void CloseFromConnection()
		{
			SqlInternalConnection innerConnection = this._innerConnection;
			bool flag = true;
			try
			{
				innerConnection.ExecuteTransaction(SqlInternalConnection.TransactionRequest.IfRollback, null, IsolationLevel.Unspecified, null, false);
			}
			catch (Exception ex)
			{
				flag = ADP.IsCatchableExceptionType(ex);
				throw;
			}
			finally
			{
				if (flag)
				{
					this.Zombie();
				}
			}
		}

		internal void Commit()
		{
			if (this._innerConnection.IsLockedForBulkCopy)
			{
				throw SQL.ConnectionLockedForBcpEvent();
			}
			this._innerConnection.ValidateConnectionForExecute(null);
			try
			{
				this._innerConnection.ExecuteTransaction(SqlInternalConnection.TransactionRequest.Commit, null, IsolationLevel.Unspecified, null, false);
				this.ZombieParent();
			}
			catch (Exception ex)
			{
				if (ADP.IsCatchableExceptionType(ex))
				{
					this.CheckTransactionLevelAndZombie();
				}
				throw;
			}
		}

		internal void Completed(TransactionState transactionState)
		{
			this._transactionState = transactionState;
			this.Zombie();
		}

		internal int DecrementAndObtainOpenResultCount()
		{
			int num = Interlocked.Decrement(ref this._openResultCount);
			if (num < 0)
			{
				throw SQL.OpenResultCountExceeded();
			}
			return num;
		}

		internal void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && this._innerConnection != null)
			{
				this._disposing = true;
				this.Rollback();
			}
		}

		private int GetServerTransactionLevel()
		{
			int num;
			using (SqlCommand sqlCommand = new SqlCommand("set @out = @@trancount", (SqlConnection)this._innerConnection.Owner))
			{
				sqlCommand.Transaction = this.Parent;
				SqlParameter sqlParameter = new SqlParameter("@out", SqlDbType.Int);
				sqlParameter.Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add(sqlParameter);
				sqlCommand.RunExecuteReader(CommandBehavior.Default, RunBehavior.UntilDone, false, "GetServerTransactionLevel");
				num = (int)sqlParameter.Value;
			}
			return num;
		}

		internal int IncrementAndObtainOpenResultCount()
		{
			int num = Interlocked.Increment(ref this._openResultCount);
			if (num < 0)
			{
				throw SQL.OpenResultCountExceeded();
			}
			return num;
		}

		internal void InitParent(SqlTransaction transaction)
		{
			this._parent = new WeakReference(transaction);
		}

		internal void Rollback()
		{
			if (this._innerConnection.IsLockedForBulkCopy)
			{
				throw SQL.ConnectionLockedForBcpEvent();
			}
			this._innerConnection.ValidateConnectionForExecute(null);
			try
			{
				this._innerConnection.ExecuteTransaction(SqlInternalConnection.TransactionRequest.IfRollback, null, IsolationLevel.Unspecified, null, false);
				this.Zombie();
			}
			catch (Exception ex)
			{
				if (!ADP.IsCatchableExceptionType(ex))
				{
					throw;
				}
				this.CheckTransactionLevelAndZombie();
				if (!this._disposing)
				{
					throw;
				}
			}
		}

		internal void Rollback(string transactionName)
		{
			if (this._innerConnection.IsLockedForBulkCopy)
			{
				throw SQL.ConnectionLockedForBcpEvent();
			}
			this._innerConnection.ValidateConnectionForExecute(null);
			if (string.IsNullOrEmpty(transactionName))
			{
				throw SQL.NullEmptyTransactionName();
			}
			try
			{
				this._innerConnection.ExecuteTransaction(SqlInternalConnection.TransactionRequest.Rollback, transactionName, IsolationLevel.Unspecified, null, false);
			}
			catch (Exception ex)
			{
				if (ADP.IsCatchableExceptionType(ex))
				{
					this.CheckTransactionLevelAndZombie();
				}
				throw;
			}
		}

		internal void Save(string savePointName)
		{
			this._innerConnection.ValidateConnectionForExecute(null);
			if (string.IsNullOrEmpty(savePointName))
			{
				throw SQL.NullEmptyTransactionName();
			}
			try
			{
				this._innerConnection.ExecuteTransaction(SqlInternalConnection.TransactionRequest.Save, savePointName, IsolationLevel.Unspecified, null, false);
			}
			catch (Exception ex)
			{
				if (ADP.IsCatchableExceptionType(ex))
				{
					this.CheckTransactionLevelAndZombie();
				}
				throw;
			}
		}

		internal void Zombie()
		{
			this.ZombieParent();
			SqlInternalConnection innerConnection = this._innerConnection;
			this._innerConnection = null;
			if (innerConnection != null)
			{
				innerConnection.DisconnectTransaction(this);
			}
		}

		private void ZombieParent()
		{
			if (this._parent != null)
			{
				SqlTransaction sqlTransaction = (SqlTransaction)this._parent.Target;
				if (sqlTransaction != null)
				{
					sqlTransaction.Zombie();
				}
				this._parent = null;
			}
		}

		internal const long NullTransactionId = 0L;

		private TransactionState _transactionState;

		private TransactionType _transactionType;

		private long _transactionId;

		private int _openResultCount;

		private SqlInternalConnection _innerConnection;

		private bool _disposing;

		private WeakReference _parent;
	}
}
