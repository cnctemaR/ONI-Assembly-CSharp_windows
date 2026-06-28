using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class COMTIIntrinsicsAttribute : Attribute
	{
		public COMTIIntrinsicsAttribute()
		{
			this.val = false;
		}

		public COMTIIntrinsicsAttribute(bool val)
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
