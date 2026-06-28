using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public sealed class SqlTriggerAttribute : Attribute
	{
		public SqlTriggerAttribute()
		{
			this.triggerEvent = null;
			this.name = null;
			this.target = null;
		}

		public string Event
		{
			get
			{
				return this.triggerEvent;
			}
			set
			{
				this.triggerEvent = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Target
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
			}
		}

		private string triggerEvent;

		private string name;

		private string target;
	}
}
