using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class ComponentAccessControlAttribute : Attribute
	{
		public ComponentAccessControlAttribute()
		{
			this.val = false;
		}

		public ComponentAccessControlAttribute(bool val)
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
