using System;
using System.Data;

namespace FileHelpers.DataLink
{
	public sealed class GenericDatabaseStorage<ConnectionClass> : DatabaseStorage where ConnectionClass : IDbConnection, new()
	{
		public GenericDatabaseStorage(Type recordType, string connectionString)
			: base(recordType)
		{
			base.ConnectionString = connectionString;
		}

		protected override bool ExecuteInBatch
		{
			get
			{
				return true;
			}
		}

		protected sealed override IDbConnection CreateConnection()
		{
			if (string.IsNullOrEmpty(base.ConnectionString))
			{
				throw new Exception("The connection cannot open because connection string is null or empty.");
			}
			ConnectionClass connectionClass = ((default(ConnectionClass) == null) ? new ConnectionClass() : default(ConnectionClass));
			connectionClass.ConnectionString = base.ConnectionString;
			return connectionClass;
		}
	}
}
