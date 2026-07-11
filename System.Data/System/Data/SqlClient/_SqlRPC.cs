using System;

namespace System.Data.SqlClient
{
	internal sealed class _SqlRPC
	{
		internal string GetCommandTextOrRpcName()
		{
			if (10 == this.ProcID)
			{
				return (string)this.parameters[0].Value;
			}
			return this.rpcName;
		}

		internal string rpcName;

		internal ushort ProcID;

		internal ushort options;

		internal SqlParameter[] parameters;

		internal byte[] paramoptions;

		internal int? recordsAffected;

		internal int cumulativeRecordsAffected;

		internal int errorsIndexStart;

		internal int errorsIndexEnd;

		internal SqlErrorCollection errors;

		internal int warningsIndexStart;

		internal int warningsIndexEnd;

		internal SqlErrorCollection warnings;
	}
}
