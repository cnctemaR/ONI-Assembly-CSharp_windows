using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class DesignOnlyAttribute : Attribute
	{
		public DesignOnlyAttribute(bool design_only)
		{
			this.design_only = design_only;
		}

		public bool IsDesignOnly
		{
			get
			{
				return this.design_only;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is DesignOnlyAttribute && (obj == this || ((DesignOnlyAttribute)obj).IsDesignOnly == this.design_only);
		}

		public override int GetHashCode()
		{
			return this.design_only.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.design_only == DesignOnlyAttribute.Default.IsDesignOnly;
		}

		private bool design_only;

		public static readonly DesignOnlyAttribute Default = new DesignOnlyAttribute(false);

		public static readonly DesignOnlyAttribute No = new DesignOnlyAttribute(false);

		public static readonly DesignOnlyAttribute Yes = new DesignOnlyAttribute(true);
	}
}
