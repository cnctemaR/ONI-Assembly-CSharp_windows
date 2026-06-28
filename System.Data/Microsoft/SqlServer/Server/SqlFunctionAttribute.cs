using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public class SqlFunctionAttribute : Attribute
	{
		public SqlFunctionAttribute()
		{
			this.dataAccess = DataAccessKind.None;
			this.isDeterministic = false;
			this.isPrecise = false;
			this.systemDataAccess = SystemDataAccessKind.None;
		}

		public DataAccessKind DataAccess
		{
			get
			{
				return this.dataAccess;
			}
			set
			{
				this.dataAccess = value;
			}
		}

		public bool IsDeterministic
		{
			get
			{
				return this.isDeterministic;
			}
			set
			{
				this.isDeterministic = value;
			}
		}

		public bool IsPrecise
		{
			get
			{
				return this.isPrecise;
			}
			set
			{
				this.isPrecise = value;
			}
		}

		public SystemDataAccessKind SystemDataAccess
		{
			get
			{
				return this.systemDataAccess;
			}
			set
			{
				this.systemDataAccess = value;
			}
		}

		private DataAccessKind dataAccess;

		private bool isDeterministic;

		private bool isPrecise;

		private SystemDataAccessKind systemDataAccess;
	}
}
