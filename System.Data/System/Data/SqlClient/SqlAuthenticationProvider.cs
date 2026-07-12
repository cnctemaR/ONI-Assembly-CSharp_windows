using System;
using System.Threading.Tasks;
using Unity;

namespace System.Data.SqlClient
{
	public abstract class SqlAuthenticationProvider
	{
		protected SqlAuthenticationProvider()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public abstract Task<SqlAuthenticationToken> AcquireTokenAsync(SqlAuthenticationParameters parameters);

		public virtual void BeforeLoad(SqlAuthenticationMethod authenticationMethod)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public virtual void BeforeUnload(SqlAuthenticationMethod authenticationMethod)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public static SqlAuthenticationProvider GetProvider(SqlAuthenticationMethod authenticationMethod)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public abstract bool IsSupported(SqlAuthenticationMethod authenticationMethod);

		public static bool SetProvider(SqlAuthenticationMethod authenticationMethod, SqlAuthenticationProvider provider)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}
	}
}
