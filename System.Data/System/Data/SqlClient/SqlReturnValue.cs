using System;

namespace System.Data.SqlClient
{
	internal sealed class SqlReturnValue : SqlMetaDataPriv
	{
		internal SqlReturnValue()
		{
			this.value = new SqlBuffer();
		}

		internal string parameter;

		internal readonly SqlBuffer value;
	}
}
