using System;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Data.SqlClient
{
	public class SqlAuthenticationToken
	{
		public SqlAuthenticationToken(string accessToken, DateTimeOffset expiresOn)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public string AccessToken
		{
			[CompilerGenerated]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public DateTimeOffset ExpiresOn
		{
			[CompilerGenerated]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(DateTimeOffset);
			}
		}
	}
}
