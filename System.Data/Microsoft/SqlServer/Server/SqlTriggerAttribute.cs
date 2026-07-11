using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public sealed class SqlTriggerAttribute : Attribute
	{
		public string Event
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string Name
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string Target
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
