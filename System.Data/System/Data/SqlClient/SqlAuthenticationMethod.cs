using System;

namespace System.Data.SqlClient
{
	public enum SqlAuthenticationMethod
	{
		NotSpecified,
		SqlPassword,
		ActiveDirectoryPassword,
		ActiveDirectoryIntegrated,
		ActiveDirectoryInteractive
	}
}
