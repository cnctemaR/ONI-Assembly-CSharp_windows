using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public sealed class SqlMethodAttribute : SqlFunctionAttribute
	{
		public bool IsMutator
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public bool OnNullCall
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
