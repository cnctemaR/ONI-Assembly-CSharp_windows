using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbEnumerator
	{
		public DataTable GetElements()
		{
			throw ADP.OleDb();
		}

		public static OleDbDataReader GetEnumerator(Type type)
		{
			throw ADP.OleDb();
		}

		public static OleDbDataReader GetRootEnumerator()
		{
			throw ADP.OleDb();
		}
	}
}
