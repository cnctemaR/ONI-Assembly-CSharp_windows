using System;
using System.Data.Common;
using Unity;

namespace System.Data.SqlClient
{
	public sealed class SqlTransaction : DbTransaction
	{
		internal SqlTransaction(SqlInternalConnection internalConnection, SqlConnection con, IsolationLevel iso, SqlInternalTransaction internalTransaction)
		{
			this._isolationLevel = IsolationLevel.ReadCommitted;
			base..ctor();
			this._isolationLevel = iso;
			this._connection = con;
			if (internalTransaction == null)
			{
				this._internalTransaction = new SqlInternalTransaction(internalConnection, TransactionType.LocalFromAPI, this);
				return;
			}
			this._internalTransaction = internalTransaction;
			this._internalTransaction.InitParent(this);
		}

		public new SqlConnection Connection
		{
			get
			{
				if (this.IsZombied)
				{
					return null;
				}
				return this._connection;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.Connection;
			}
		}

		internal SqlInternalTransaction InternalTransaction
		{
			get
			{
				return this._internalTransaction;
			}
		}

		public override IsolationLevel IsolationLevel
		{
			get
			{
				this.ZombieCheck();
				return this._isolationLevel;
			}
		}

		private bool IsYukonPartialZombie
		{
			get
			{
				return this._internalTransaction != null && this._internalTransaction.IsCompleted;
			}
		}

		internal bool IsZombied
		{
			get
			{
				return this._internalTransaction == null || this._internalTransaction.IsCompleted;
			}
		}

		internal SqlStatistics Statistics
		{
			get
			{
				if (this._connection != null && this._connection.StatisticsEnabled)
				{
					return this._connection.Statistics;
				}
				return null;
			}
		}

		public override void Commit()
		{
			Exception ex = null;
			Guid guid = SqlTransaction.s_diagnosticListener.WriteTransactionCommitBefore(this._isolationLevel, this._connection, "Commit");
			this.ZombieCheck();
			SqlStatistics sqlStatistics = null;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this._isFromAPI = true;
				this._internalTransaction.Commit();
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				if (ex != null)
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionCommitError(guid, this._isolationLevel, this._connection, ex, "Commit");
				}
				else
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionCommitAfter(guid, this._isolationLevel, this._connection, "Commit");
				}
				this._isFromAPI = false;
				SqlStatistics.StopTimer(sqlStatistics);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.IsZombied && !this.IsYukonPartialZombie)
			{
				this._internalTransaction.Dispose();
			}
			base.Dispose(disposing);
		}

		public override void Rollback()
		{
			Exception ex = null;
			Guid guid = SqlTransaction.s_diagnosticListener.WriteTransactionRollbackBefore(this._isolationLevel, this._connection, null, "Rollback");
			if (this.IsYukonPartialZombie)
			{
				this._internalTransaction = null;
				return;
			}
			this.ZombieCheck();
			SqlStatistics sqlStatistics = null;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this._isFromAPI = true;
				this._internalTransaction.Rollback();
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				if (ex != null)
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionRollbackError(guid, this._isolationLevel, this._connection, null, ex, "Rollback");
				}
				else
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionRollbackAfter(guid, this._isolationLevel, this._connection, null, "Rollback");
				}
				this._isFromAPI = false;
				SqlStatistics.StopTimer(sqlStatistics);
			}
		}

		public void Rollback(string transactionName)
		{
			Exception ex = null;
			Guid guid = SqlTransaction.s_diagnosticListener.WriteTransactionRollbackBefore(this._isolationLevel, this._connection, transactionName, "Rollback");
			this.ZombieCheck();
			SqlStatistics sqlStatistics = null;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this._isFromAPI = true;
				this._internalTransaction.Rollback(transactionName);
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				if (ex != null)
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionRollbackError(guid, this._isolationLevel, this._connection, transactionName, ex, "Rollback");
				}
				else
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionRollbackAfter(guid, this._isolationLevel, this._connection, transactionName, "Rollback");
				}
				this._isFromAPI = false;
				SqlStatistics.StopTimer(sqlStatistics);
			}
		}

		public void Save(string savePointName)
		{
			this.ZombieCheck();
			SqlStatistics sqlStatistics = null;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this._internalTransaction.Save(savePointName);
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
		}

		internal void Zombie()
		{
			if (!(this._connection.InnerConnection is SqlInternalConnection) || this._isFromAPI)
			{
				this._internalTransaction = null;
			}
		}

		private void ZombieCheck()
		{
			if (this.IsZombied)
			{
				if (this.IsYukonPartialZombie)
				{
					this._internalTransaction = null;
				}
				throw ADP.TransactionZombied(this);
			}
		}

		internal SqlTransaction()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private static readonly DiagnosticListener s_diagnosticListener = new DiagnosticListener("SqlClientDiagnosticListener");

		internal readonly IsolationLevel _isolationLevel;

		private SqlInternalTransaction _internalTransaction;

		private SqlConnection _connection;

		private bool _isFromAPI;
	}
}
