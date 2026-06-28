using System;

namespace System.Data
{
	public interface IDbCommand : IDisposable
	{
		void Cancel();

		IDbDataParameter CreateParameter();

		int ExecuteNonQuery();

		IDataReader ExecuteReader();

		IDataReader ExecuteReader(CommandBehavior behavior);

		object ExecuteScalar();

		void Prepare();

		string CommandText { get; set; }

		int CommandTimeout { get; set; }

		CommandType CommandType { get; set; }

		IDbConnection Connection { get; set; }

		IDataParameterCollection Parameters { get; }

		IDbTransaction Transaction { get; set; }

		UpdateRowSource UpdatedRowSource { get; set; }
	}
}
