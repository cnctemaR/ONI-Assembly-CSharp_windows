using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices.CompensatingResourceManager
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[ComVisible(false)]
	[ProgId("System.EnterpriseServices.Crm.ApplicationCrmEnabledAttribute")]
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
