using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public sealed class SqlMethodAttribute : SqlFunctionAttribute
	{
		public SqlMethodAttribute()
		{
			this.isMutator = false;
			this.onNullCall = false;
		}

		public bool IsMutator
		{
			get
			{
				return this.isMutator;
			}
			set
			{
				this.isMutator = value;
			}
		}

		public bool OnNullCall
		{
			get
			{
				return this.onNullCall;
			}
			set
			{
				this.onNullCall = value;
			}
		}

		private bool isMutator;

		private bool onNullCall;
	}
}
