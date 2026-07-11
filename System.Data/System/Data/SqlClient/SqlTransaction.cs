using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlTransaction : DbTransaction, IDbTransaction, IDisposable
	{
		internal SqlTransaction()
		{
		}

		public new SqlConnection Connection
		{
			get
			{
				throw null;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				throw null;
			}
		}

		public override IsolationLevel IsolationLevel
		{
			get
			{
				throw null;
			}
		}

		public override void Commit()
		{
		}

		protected override void Dispose(bool disposing)
		{
		}

		public override void Rollback()
		{
		}

		public void Rollback(string transactionName)
		{
		}

		public void Save(string savePointName)
		{
		}
	}
}
