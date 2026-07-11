using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	internal sealed class OleDbConnectionString : DbConnectionOptions
	{
		internal OleDbConnectionString(string connectionString, bool validate)
			: base(connectionString, null)
		{
		}
	}
}
