using System;
using System.Data.Common;

namespace System.Data.Odbc
{
	internal sealed class OdbcConnectionString : DbConnectionOptions
	{
		internal OdbcConnectionString(string connectionString, bool validate)
			: base(connectionString, null, true)
		{
			if (!validate)
			{
				string text = null;
				int num = 0;
				this._expandedConnectionString = base.ExpandDataDirectories(ref text, ref num);
			}
			if ((validate || this._expandedConnectionString == null) && connectionString != null && 1024 < connectionString.Length)
			{
				throw ODBC.ConnectionStringTooLong();
			}
		}

		private readonly string _expandedConnectionString;
	}
}
