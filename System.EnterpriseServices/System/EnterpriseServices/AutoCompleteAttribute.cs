using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Method)]
	[ComVisible(false)]
	public sealed class AutoCompleteAttribute : Attribute
	{
		public AutoCompleteAttribute()
		{
			this.val = true;
		}

		public AutoCompleteAttribute(bool val)
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
