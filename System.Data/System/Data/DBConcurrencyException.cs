using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Data
{
	[Serializable]
	public sealed class DBConcurrencyException : SystemException
	{
		public DBConcurrencyException()
			: this("DB concurrency violation.", null)
		{
		}

		public DBConcurrencyException(string message)
			: this(message, null)
		{
		}

		public DBConcurrencyException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146232011;
		}

		public DBConcurrencyException(string message, Exception inner, DataRow[] dataRows)
			: base(message, inner)
		{
			base.HResult = -2146232011;
			this._dataRows = dataRows;
		}

		private DBConcurrencyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		public DataRow Row
		{
			get
			{
				DataRow[] dataRows = this._dataRows;
				if (dataRows == null || dataRows.Length == 0)
				{
					return null;
				}
				return dataRows[0];
			}
			set
			{
				this._dataRows = new DataRow[] { value };
			}
		}

		public int RowCount
		{
			get
			{
				DataRow[] dataRows = this._dataRows;
				if (dataRows == null)
				{
					return 0;
				}
				return dataRows.Length;
			}
		}

		public void CopyToRows(DataRow[] array)
		{
			this.CopyToRows(array, 0);
		}

		public void CopyToRows(DataRow[] array, int arrayIndex)
		{
			DataRow[] dataRows = this._dataRows;
			if (dataRows != null)
			{
				dataRows.CopyTo(array, arrayIndex);
			}
		}

		private DataRow[] _dataRows;
	}
}
