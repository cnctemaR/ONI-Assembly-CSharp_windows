using System;
using System.Data.SqlClient;

namespace Microsoft.SqlServer.Server
{
	public sealed class SqlPipe
	{
		private SqlPipe()
		{
		}

		public bool IsSendingResults
		{
			get
			{
				return false;
			}
		}

		public void ExecuteAndSend(SqlCommand command)
		{
			throw new NotImplementedException();
		}

		public void Send(string message)
		{
			throw new NotImplementedException();
		}

		public void Send(SqlDataReader reader)
		{
			throw new NotImplementedException();
		}

		public void Send(SqlDataRecord record)
		{
			throw new NotImplementedException();
		}

		public void SendResultsStart(SqlDataRecord record)
		{
			throw new NotImplementedException();
		}

		public void SendResultsRow(SqlDataRecord record)
		{
			throw new NotImplementedException();
		}

		public void SendResultsEnd()
		{
			throw new NotImplementedException();
		}
	}
}
