using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class MustRunInClientContextAttribute : Attribute
	{
		public MustRunInClientContextAttribute()
			: this(true)
		{
		}

		public MustRunInClientContextAttribute(bool val)
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
