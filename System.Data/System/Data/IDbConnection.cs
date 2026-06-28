using System;

namespace System.Data
{
	public interface IDbConnection : IDisposable
	{
		IDbTransaction BeginTransaction();

		IDbTransaction BeginTransaction(IsolationLevel il);

		void ChangeDatabase(string databaseName);

		void Close();

		IDbCommand CreateCommand();

		void Open();

		string ConnectionString { get; set; }

		int ConnectionTimeout { get; }

		string Database { get; }

		ConnectionState State { get; }
	}
}
