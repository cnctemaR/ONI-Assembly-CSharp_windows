using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public class SqlFunctionAttribute : Attribute
	{
		public DataAccessKind DataAccess
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public bool IsDeterministic
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public bool IsPrecise
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public SystemDataAccessKind SystemDataAccess
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}
	}
}
