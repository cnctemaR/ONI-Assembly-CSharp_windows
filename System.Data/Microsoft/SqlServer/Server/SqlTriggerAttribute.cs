using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public sealed class SqlTriggerAttribute : Attribute
	{
		public SqlTriggerAttribute()
		{
			this.m_fName = null;
			this.m_fTarget = null;
			this.m_fEvent = null;
		}

		public string Name
		{
			get
			{
				return this.m_fName;
			}
			set
			{
				this.m_fName = value;
			}
		}

		public string Target
		{
			get
			{
				return this.m_fTarget;
			}
			set
			{
				this.m_fTarget = value;
			}
		}

		public string Event
		{
			get
			{
				return this.m_fEvent;
			}
			set
			{
				this.m_fEvent = value;
			}
		}

		private string m_fName;

		private string m_fTarget;

		private string m_fEvent;
	}
}
