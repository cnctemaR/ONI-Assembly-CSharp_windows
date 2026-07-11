using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public sealed class DBConcurrencyException : SystemException
	{
		public DBConcurrencyException()
		{
		}

		public DBConcurrencyException(string message)
		{
		}

		public DBConcurrencyException(string message, Exception inner)
		{
		}

		public DBConcurrencyException(string message, Exception inner, DataRow[] dataRows)
		{
		}

		public DataRow Row
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public int RowCount
		{
			get
			{
				throw null;
			}
		}

		[MonoTODO]
		public void CopyToRows(DataRow[] array)
		{
		}

		[MonoTODO]
		public void CopyToRows(DataRow[] array, int arrayIndex)
		{
		}

		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}
}
