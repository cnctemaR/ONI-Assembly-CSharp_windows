using System;
using System.Data;
using System.Data.OleDb;

namespace FileHelpers.DataLink
{
	public sealed class OleDbStorage : DatabaseStorage
	{
		public OleDbStorage(Type recordType, string oleDbConnString)
			: base(recordType)
		{
			base.ConnectionString = oleDbConnString;
		}

		protected sealed override IDbConnection CreateConnection()
		{
			if (base.ConnectionString == null || base.ConnectionString == string.Empty)
			{
				throw new BadUsageException("The OleDb Connection string can't be null or empty.");
			}
			return new OleDbConnection(base.ConnectionString);
		}
	}
}
