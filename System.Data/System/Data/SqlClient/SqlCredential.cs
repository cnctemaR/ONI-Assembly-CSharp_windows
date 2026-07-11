using System;
using System.Security;

namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlCredential
	{
		public SqlCredential(string userId, SecureString password)
		{
			if (userId == null)
			{
				throw new ArgumentNullException("userId");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			this.uid = userId;
			this.pwd = password;
		}

		public string UserId
		{
			get
			{
				return this.uid;
			}
		}

		public SecureString Password
		{
			get
			{
				return this.pwd;
			}
		}

		private string uid = "";

		private SecureString pwd;
	}
}
