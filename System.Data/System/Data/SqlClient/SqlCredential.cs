using System;
using System.Security;

namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlCredential
	{
		public SqlCredential(string user, SecureString password)
		{
		}

		public SecureString Password
		{
			get
			{
				throw null;
			}
		}

		public string UserId
		{
			get
			{
				throw null;
			}
		}
	}
}
