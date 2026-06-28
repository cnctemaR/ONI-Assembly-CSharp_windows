using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class EventTrackingEnabledAttribute : Attribute
	{
		public EventTrackingEnabledAttribute()
		{
			this.val = true;
		}

		public EventTrackingEnabledAttribute(bool val)
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
