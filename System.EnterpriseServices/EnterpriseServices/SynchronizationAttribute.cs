using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class SynchronizationAttribute : Attribute
	{
		public SynchronizationAttribute()
			: this(SynchronizationOption.Required)
		{
		}

		public SynchronizationAttribute(SynchronizationOption val)
		{
			this.val = val;
		}

		public SynchronizationOption Value
		{
			get
			{
				return this.val;
			}
		}

		private SynchronizationOption val;
	}
}
