using System;
using System.Data;
using FileHelpers.Events;

namespace FileHelpers.DataLink
{
	public abstract class DataStorage
	{
		public event EventHandler<ProgressEventArgs> Progress;

		protected void OnProgress(ProgressEventArgs e)
		{
			if (this.Progress == null)
			{
				return;
			}
			this.Progress(this, e);
		}

		public Type RecordType
		{
			get
			{
				return this.mRecordType;
			}
		}

		public abstract object[] ExtractRecords();

		public DataTable ExtractRecordsAsDT()
		{
			IRecordInfo recordInfo = RecordInfo.Resolve(this.RecordType);
			return recordInfo.Operations.RecordsToDataTable(this.ExtractRecords());
		}

		public abstract void InsertRecords(object[] records);

		public ErrorManager ErrorManager
		{
			get
			{
				return this.mErrorManager;
			}
		}

		protected void AddError(int lineNumber, Exception ex, string recordLine)
		{
			ErrorInfo errorInfo = new ErrorInfo();
			errorInfo.mLineNumber = lineNumber;
			errorInfo.mExceptionInfo = ex;
			errorInfo.mRecordString = recordLine;
			this.mErrorManager.AddError(errorInfo);
		}

		protected DataStorage(Type recordClass)
		{
			this.mRecordType = recordClass;
			this.mRecordInfo = RecordInfo.Resolve(recordClass);
		}

		protected object ValuesToRecord(object[] values)
		{
			return this.mRecordInfo.Operations.ValuesToRecord(values);
		}

		protected object[] RecordToValues(object record)
		{
			return this.mRecordInfo.Operations.RecordToValues(record);
		}

		protected int RecordFieldCount
		{
			get
			{
				return this.mRecordInfo.FieldCount;
			}
		}

		private Type mRecordType;

		internal IRecordInfo mRecordInfo;

		protected ErrorManager mErrorManager = new ErrorManager();
	}
}
