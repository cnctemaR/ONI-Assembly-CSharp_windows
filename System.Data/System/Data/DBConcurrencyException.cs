using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public sealed class DBConcurrencyException : SystemException
	{
		public DBConcurrencyException()
			: base("Concurrency violation.")
		{
		}

		public DBConcurrencyException(string message)
			: base(message)
		{
		}

		public DBConcurrencyException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public DBConcurrencyException(string message, Exception inner, DataRow[] dataRows)
			: base(message, inner)
		{
			this.rows = dataRows;
		}

		private DBConcurrencyException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}

		public DataRow Row
		{
			get
			{
				if (this.rows != null)
				{
					return this.rows[0];
				}
				return null;
			}
			set
			{
				this.rows = new DataRow[] { value };
			}
		}

		public int RowCount
		{
			get
			{
				if (this.rows != null)
				{
					return this.rows.Length;
				}
				return 0;
			}
		}

		[MonoTODO]
		public void CopyToRows(DataRow[] array)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void CopyToRows(DataRow[] array, int ArrayIndex)
		{
			throw new NotImplementedException();
		}

		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
			if (si == null)
			{
				throw new ArgumentNullException("si");
			}
			base.GetObjectData(si, context);
		}

		private DataRow[] rows;
	}
}
