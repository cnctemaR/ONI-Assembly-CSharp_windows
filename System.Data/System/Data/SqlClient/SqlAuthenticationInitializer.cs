using System;
using Unity;

namespace System.Data.SqlClient
{
	public abstract class SqlAuthenticationInitializer
	{
		protected SqlAuthenticationInitializer()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public abstract void Initialize();
	}
}
