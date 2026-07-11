using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class)]
	[Serializable]
	public sealed class AttributeUsageAttribute : Attribute
	{
		public AttributeUsageAttribute(AttributeTargets validOn)
		{
			this.valid_on = validOn;
		}

		public bool AllowMultiple
		{
			get
			{
				return this.allow_multiple;
			}
			set
			{
				this.allow_multiple = value;
			}
		}

		public bool Inherited
		{
			get
			{
				return this.inherited;
			}
			set
			{
				this.inherited = value;
			}
		}

		public AttributeTargets ValidOn
		{
			get
			{
				return this.valid_on;
			}
		}

		private AttributeTargets valid_on;

		private bool allow_multiple;

		private bool inherited = true;
	}
}
