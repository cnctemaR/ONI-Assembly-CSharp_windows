using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices.CompensatingResourceManager
{
	[ProgId("System.EnterpriseServices.Crm.ApplicationCrmEnabledAttribute")]
	[AttributeUsage(AttributeTargets.Assembly)]
	[ComVisible(false)]
	public sealed class ApplicationCrmEnabledAttribute : Attribute
	{
		public ApplicationCrmEnabledAttribute()
		{
			this.val = true;
		}

		public ApplicationCrmEnabledAttribute(bool val)
		{
			this.val = val;
		}

		public bool Value
		{
			get
			{
				return this.val;
			}
		}

		private bool val;
	}
}
