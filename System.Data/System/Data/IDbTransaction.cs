using System;

namespace System.Data
{
	public interface IDbTransaction : IDisposable
	{
		void Commit();

		void Rollback();

		IDbConnection Connection { get; }

		IsolationLevel IsolationLevel { get; }
	}
}
