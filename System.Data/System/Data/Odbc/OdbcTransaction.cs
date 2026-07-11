using System;
using System.Data.Common;

namespace System.Data.Odbc
{
	public sealed class OdbcTransaction : DbTransaction, IDisposable
	{
		internal OdbcTransaction()
		{
		}

		public new OdbcConnection Connection
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

		void IDisposable.Dispose()
		{
		}
	}
}
