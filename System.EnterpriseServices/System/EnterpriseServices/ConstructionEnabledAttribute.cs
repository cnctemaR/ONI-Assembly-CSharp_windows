using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ConstructionEnabledAttribute : Attribute
	{
		public ConstructionEnabledAttribute()
		{
			this.def = string.Empty;
			this.enabled = true;
		}

		public ConstructionEnabledAttribute(bool val)
		{
			this.def = string.Empty;
			this.enabled = val;
		}

		public string Default
		{
			get
			{
				return this.def;
			}
			set
			{
				this.def = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		private string def;

		private bool enabled;
	}
}
