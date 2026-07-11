using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class DesignOnlyAttribute : Attribute
	{
		public DesignOnlyAttribute(bool isDesignOnly)
		{
			this.isDesignOnly = isDesignOnly;
		}

		public bool IsDesignOnly
		{
			get
			{
				return this.isDesignOnly;
			}
		}

		public override bool IsDefaultAttribute()
		{
			return this.IsDesignOnly == DesignOnlyAttribute.Default.IsDesignOnly;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DesignOnlyAttribute designOnlyAttribute = obj as DesignOnlyAttribute;
			return designOnlyAttribute != null && designOnlyAttribute.isDesignOnly == this.isDesignOnly;
		}

		public override int GetHashCode()
		{
			return this.isDesignOnly.GetHashCode();
		}

		private bool isDesignOnly;

		public static readonly DesignOnlyAttribute Yes = new DesignOnlyAttribute(true);

		public static readonly DesignOnlyAttribute No = new DesignOnlyAttribute(false);

		public static readonly DesignOnlyAttribute Default = DesignOnlyAttribute.No;
	}
}
