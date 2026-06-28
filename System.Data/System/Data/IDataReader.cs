using System;

namespace System.Data
{
	public interface IDataReader : IDisposable, IDataRecord
	{
		void Close();

		DataTable GetSchemaTable();

		bool NextResult();

		bool Read();

		int Depth { get; }

		bool IsClosed { get; }

		int RecordsAffected { get; }
	}
}
