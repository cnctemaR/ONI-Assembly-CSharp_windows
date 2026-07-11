using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbTransaction : DbTransaction
	{
		internal OleDbTransaction()
		{
		}

		public new OleDbConnection Connection
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override IsolationLevel IsolationLevel
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public OleDbTransaction Begin()
		{
			throw ADP.OleDb();
		}

		public OleDbTransaction Begin(IsolationLevel isolevel)
		{
			throw ADP.OleDb();
		}

		public override void Commit()
		{
			throw ADP.OleDb();
		}

		protected override void Dispose(bool disposing)
		{
			throw ADP.OleDb();
		}

		public override void Rollback()
		{
			throw ADP.OleDb();
		}
	}
}
