using System;
using System.Data.ProviderBase;

namespace System.Data.SqlClient
{
	internal sealed class SqlReferenceCollection : DbReferenceCollection
	{
		public override void Add(object value, int tag)
		{
			base.AddItem(value, tag);
		}

		internal void Deactivate()
		{
			base.Notify(0);
		}

		internal SqlDataReader FindLiveReader(SqlCommand command)
		{
			if (command == null)
			{
				return base.FindItem<SqlDataReader>(1, (SqlDataReader dataReader) => !dataReader.IsClosed);
			}
			return base.FindItem<SqlDataReader>(1, (SqlDataReader dataReader) => !dataReader.IsClosed && command == dataReader.Command);
		}

		internal SqlCommand FindLiveCommand(TdsParserStateObject stateObj)
		{
			return base.FindItem<SqlCommand>(2, (SqlCommand command) => command.StateObject == stateObj);
		}

		protected override void NotifyItem(int message, int tag, object value)
		{
			if (tag == 1)
			{
				SqlDataReader sqlDataReader = (SqlDataReader)value;
				if (!sqlDataReader.IsClosed)
				{
					sqlDataReader.CloseReaderFromConnection();
					return;
				}
			}
			else
			{
				if (tag == 2)
				{
					((SqlCommand)value).OnConnectionClosed();
					return;
				}
				if (tag == 3)
				{
					((SqlBulkCopy)value).OnConnectionClosed();
				}
			}
		}

		public override void Remove(object value)
		{
			base.RemoveItem(value);
		}

		internal const int DataReaderTag = 1;

		internal const int CommandTag = 2;

		internal const int BulkCopyTag = 3;
	}
}
