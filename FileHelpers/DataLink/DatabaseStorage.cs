using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using FileHelpers.Events;

namespace FileHelpers.DataLink
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public abstract class DatabaseStorage : DataStorage
	{
		protected DatabaseStorage(Type recordType)
			: base(recordType)
		{
		}

		private object FillRecord(object[] fieldValues)
		{
			if (this.FillRecordCallback == null)
			{
				throw new BadUsageException("You can't extract records with a null FillRecordCallback. Check the docs for help.");
			}
			object obj = this.mRecordInfo.Operations.CreateRecordHandler();
			this.FillRecordCallback(obj, fieldValues);
			return obj;
		}

		private string GetSelectSql()
		{
			if (this.mSelectSql == null || this.mSelectSql == string.Empty)
			{
				throw new BadUsageException("The SelectSql property is empty, please set it before trying to get the records.");
			}
			return this.mSelectSql;
		}

		public string SelectSql
		{
			get
			{
				return this.mSelectSql;
			}
			set
			{
				this.mSelectSql = value;
			}
		}

		private string GetInsertSql(object record)
		{
			if (this.mInsertSqlCallback == null)
			{
				throw new BadUsageException("You can't insert records with a null GetInsertSqlCallback. Check the docs for help.");
			}
			return this.mInsertSqlCallback(record);
		}

		protected abstract IDbConnection CreateConnection();

		private void InitConnection()
		{
			if (this.mConn == null)
			{
				this.mConn = this.CreateConnection();
			}
		}

		public override object[] ExtractRecords()
		{
			this.InitConnection();
			ArrayList arrayList = new ArrayList();
			try
			{
				if (this.mConn.State != ConnectionState.Open)
				{
					this.mConn.Open();
				}
				IDbCommand dbCommand = this.mConn.CreateCommand();
				dbCommand.Connection = this.mConn;
				dbCommand.CommandText = this.GetSelectSql();
				IDataReader dataReader = dbCommand.ExecuteReader();
				object[] array = new object[dataReader.FieldCount];
				base.OnProgress(new ProgressEventArgs(0, -1));
				int num = 0;
				while (dataReader.Read())
				{
					num++;
					base.OnProgress(new ProgressEventArgs(num, -1));
					dataReader.GetValues(array);
					object obj = this.FillRecord(array);
					arrayList.Add(obj);
				}
				dataReader.Close();
			}
			finally
			{
				if (this.mConn.State != ConnectionState.Closed)
				{
					this.mConn.Close();
				}
			}
			return (object[])arrayList.ToArray(base.RecordType);
		}

		protected virtual bool ExecuteInBatch
		{
			get
			{
				return false;
			}
		}

		public override void InsertRecords(object[] records)
		{
			IDbTransaction dbTransaction = null;
			try
			{
				this.InitConnection();
				if (this.mConn.State != ConnectionState.Open)
				{
					this.mConn.Open();
				}
				string text = string.Empty;
				dbTransaction = this.InitTransaction(this.mConn);
				base.OnProgress(new ProgressEventArgs(0, records.Length));
				int num = 0;
				int num2 = 0;
				foreach (object obj in records)
				{
					num++;
					num2++;
					base.OnProgress(new ProgressEventArgs(num, records.Length));
					text = text + this.GetInsertSql(obj) + " ";
					if (this.ExecuteInBatch)
					{
						if (num2 >= this.mExecuteInBatchSize)
						{
							this.ExecuteAndLeaveOpen(text);
							text = string.Empty;
							num2 = 0;
						}
					}
					else
					{
						this.ExecuteAndLeaveOpen(text);
						text = string.Empty;
					}
				}
				if (text != null && text.Length != 0)
				{
					this.ExecuteAndLeaveOpen(text);
					text = string.Empty;
				}
				this.CommitTransaction(dbTransaction);
			}
			catch
			{
				this.RollBackTransaction(dbTransaction);
				throw;
			}
			finally
			{
				try
				{
					this.mConn.Close();
					this.mConn.Dispose();
					this.mConn = null;
				}
				catch
				{
				}
			}
		}

		private int ExecuteAndLeaveOpen(string sql)
		{
			this.InitConnection();
			IDbCommand dbCommand = this.mConn.CreateCommand();
			dbCommand.Connection = this.mConn;
			dbCommand.CommandText = sql;
			return dbCommand.ExecuteNonQuery();
		}

		private int ExecuteAndClose(string sql)
		{
			int num = -1;
			this.InitConnection();
			try
			{
				if (this.mConn.State != ConnectionState.Open)
				{
					this.mConn.Open();
				}
				IDbCommand dbCommand = this.mConn.CreateCommand();
				dbCommand.Connection = this.mConn;
				dbCommand.CommandText = sql;
				num = dbCommand.ExecuteNonQuery();
			}
			finally
			{
				if (this.mConn.State != ConnectionState.Closed)
				{
					this.mConn.Close();
				}
			}
			return num;
		}

		public InsertSqlHandler InsertSqlCallback
		{
			get
			{
				return this.mInsertSqlCallback;
			}
			set
			{
				this.mInsertSqlCallback = value;
			}
		}

		public FillRecordHandler FillRecordCallback
		{
			get
			{
				return this.mFillRecordCallback;
			}
			set
			{
				this.mFillRecordCallback = value;
			}
		}

		public int ExecuteInBatchSize
		{
			get
			{
				return this.mExecuteInBatchSize;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException("ExecuteInBatchSize", "ExecuteInBatchSize must be >= 1");
				}
				this.mExecuteInBatchSize = value;
			}
		}

		public TransactionMode TransactionMode
		{
			get
			{
				return this.mTransactionMode;
			}
			set
			{
				this.mTransactionMode = value;
			}
		}

		private IDbTransaction InitTransaction(IDbConnection conn)
		{
			if (this.mTransactionMode == TransactionMode.NoTransaction)
			{
				return null;
			}
			switch (this.mTransactionMode)
			{
			case TransactionMode.UseDefault:
				return conn.BeginTransaction();
			case TransactionMode.UseChaosLevel:
				return conn.BeginTransaction(IsolationLevel.Chaos);
			case TransactionMode.UseReadCommitted:
				return conn.BeginTransaction(IsolationLevel.ReadCommitted);
			case TransactionMode.UseReadUnCommitted:
				return conn.BeginTransaction(IsolationLevel.ReadUncommitted);
			case TransactionMode.UseRepeatableRead:
				return conn.BeginTransaction(IsolationLevel.RepeatableRead);
			case TransactionMode.UseSerializable:
				return conn.BeginTransaction(IsolationLevel.Serializable);
			default:
				return null;
			}
		}

		private void CommitTransaction(IDbTransaction trans)
		{
			if (trans == null)
			{
				return;
			}
			trans.Commit();
		}

		private void RollBackTransaction(IDbTransaction trans)
		{
			if (trans == null)
			{
				return;
			}
			trans.Rollback();
		}

		public string ConnectionString
		{
			get
			{
				return this.mConnectionString;
			}
			set
			{
				this.mConnectionString = value;
			}
		}

		private string mSelectSql = string.Empty;

		private IDbConnection mConn;

		private InsertSqlHandler mInsertSqlCallback;

		private FillRecordHandler mFillRecordCallback;

		private int mExecuteInBatchSize = 100;

		private TransactionMode mTransactionMode;

		private string mConnectionString = string.Empty;
	}
}
