using System;
using System.Data.Common;

namespace System.Data.Sql
{
	public sealed class SqlDataSourceEnumerator : DbDataSourceEnumerator
	{
		private SqlDataSourceEnumerator()
		{
		}

		[MonoTODO]
		public static SqlDataSourceEnumerator Instance
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		public override DataTable GetDataSources()
		{
			throw new NotImplementedException();
		}
	}
}
