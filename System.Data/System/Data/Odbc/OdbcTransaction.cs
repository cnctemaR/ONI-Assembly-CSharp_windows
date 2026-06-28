using System;
using System.Data.Common;
using System.Globalization;

namespace System.Data.Odbc
{
	public sealed class OdbcTransaction : DbTransaction, IDisposable
	{
		internal OdbcTransaction(OdbcConnection conn, IsolationLevel isolationlevel)
		{
			OdbcTransaction.SetAutoCommit(conn, false);
			OdbcIsolationLevel odbcIsolationLevel = OdbcIsolationLevel.ReadCommitted;
			OdbcConnectionAttribute odbcConnectionAttribute = OdbcConnectionAttribute.TransactionIsolation;
			if (isolationlevel != IsolationLevel.Unspecified)
			{
				if (isolationlevel == IsolationLevel.Chaos)
				{
					throw new ArgumentOutOfRangeException("IsolationLevel", string.Format(CultureInfo.CurrentCulture, "The IsolationLevel enumeration value, {0}, is not supported by the .Net Framework Odbc Data Provider.", new object[] { (int)isolationlevel }));
				}
				if (isolationlevel != IsolationLevel.ReadUncommitted)
				{
					if (isolationlevel != IsolationLevel.ReadCommitted)
					{
						if (isolationlevel != IsolationLevel.RepeatableRead)
						{
							if (isolationlevel != IsolationLevel.Serializable)
							{
								if (isolationlevel != IsolationLevel.Snapshot)
								{
									throw new ArgumentOutOfRangeException("IsolationLevel", string.Format(CultureInfo.CurrentCulture, "The IsolationLevel enumeration value, {0}, is invalid.", new object[] { (int)isolationlevel }));
								}
								odbcIsolationLevel = OdbcIsolationLevel.Snapshot;
								odbcConnectionAttribute = OdbcConnectionAttribute.CoptTransactionIsolation;
							}
							else
							{
								odbcIsolationLevel = OdbcIsolationLevel.Serializable;
							}
						}
						else
						{
							odbcIsolationLevel = OdbcIsolationLevel.RepeatableRead;
						}
					}
					else
					{
						odbcIsolationLevel = OdbcIsolationLevel.ReadCommitted;
					}
				}
				else
				{
					odbcIsolationLevel = OdbcIsolationLevel.ReadUncommitted;
				}
			}
			if (isolationlevel != IsolationLevel.Unspecified)
			{
				OdbcReturn odbcReturn = libodbc.SQLSetConnectAttr(conn.hDbc, odbcConnectionAttribute, (IntPtr)((int)odbcIsolationLevel), 0);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw conn.CreateOdbcException(OdbcHandleType.Dbc, conn.hDbc);
				}
			}
			this.isolationlevel = isolationlevel;
			this.connection = conn;
			this.isOpen = true;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private static void SetAutoCommit(OdbcConnection conn, bool isAuto)
		{
			OdbcReturn odbcReturn = libodbc.SQLSetConnectAttr(conn.hDbc, OdbcConnectionAttribute.AutoCommit, (IntPtr)((!isAuto) ? 0 : 1), -5);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw conn.CreateOdbcException(OdbcHandleType.Dbc, conn.hDbc);
			}
		}

		private static IsolationLevel GetIsolationLevel(OdbcConnection conn)
		{
			int num;
			int num2;
			OdbcReturn odbcReturn = libodbc.SQLGetConnectAttr(conn.hDbc, OdbcConnectionAttribute.TransactionIsolation, out num, 0, out num2);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw conn.CreateOdbcException(OdbcHandleType.Dbc, conn.hDbc);
			}
			return OdbcTransaction.MapOdbcIsolationLevel((OdbcIsolationLevel)num);
		}

		private static IsolationLevel MapOdbcIsolationLevel(OdbcIsolationLevel odbcLevel)
		{
			IsolationLevel isolationLevel = IsolationLevel.Unspecified;
			switch (odbcLevel)
			{
			case OdbcIsolationLevel.ReadUncommitted:
				isolationLevel = IsolationLevel.ReadUncommitted;
				break;
			case OdbcIsolationLevel.ReadCommitted:
				isolationLevel = IsolationLevel.ReadCommitted;
				break;
			default:
				if (odbcLevel == OdbcIsolationLevel.Snapshot)
				{
					isolationLevel = IsolationLevel.Snapshot;
				}
				break;
			case OdbcIsolationLevel.RepeatableRead:
				isolationLevel = IsolationLevel.RepeatableRead;
				break;
			case OdbcIsolationLevel.Serializable:
				isolationLevel = IsolationLevel.Serializable;
				break;
			}
			return isolationLevel;
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.isOpen)
				{
					this.Rollback();
				}
				this.disposed = true;
			}
		}

		public override void Commit()
		{
			if (!this.isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(base.GetType());
			}
			if (this.connection.transaction != this)
			{
				throw new InvalidOperationException();
			}
			OdbcReturn odbcReturn = libodbc.SQLEndTran(2, this.connection.hDbc, 0);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.connection.CreateOdbcException(OdbcHandleType.Dbc, this.connection.hDbc);
			}
			OdbcTransaction.SetAutoCommit(this.connection, true);
			this.connection.transaction = null;
			this.connection = null;
			this.isOpen = false;
		}

		public override void Rollback()
		{
			if (!this.isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(base.GetType());
			}
			if (this.connection.transaction != this)
			{
				throw new InvalidOperationException();
			}
			OdbcReturn odbcReturn = libodbc.SQLEndTran(2, this.connection.hDbc, 1);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.connection.CreateOdbcException(OdbcHandleType.Dbc, this.connection.hDbc);
			}
			OdbcTransaction.SetAutoCommit(this.connection, true);
			this.connection.transaction = null;
			this.connection = null;
			this.isOpen = false;
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.Connection;
			}
		}

		public override IsolationLevel IsolationLevel
		{
			get
			{
				if (!this.isOpen)
				{
					throw ExceptionHelper.TransactionNotUsable(base.GetType());
				}
				if (this.isolationlevel == IsolationLevel.Unspecified)
				{
					this.isolationlevel = OdbcTransaction.GetIsolationLevel(this.Connection);
				}
				return this.isolationlevel;
			}
		}

		public new OdbcConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		private bool disposed;

		private OdbcConnection connection;

		private IsolationLevel isolationlevel;

		private bool isOpen;
	}
}
