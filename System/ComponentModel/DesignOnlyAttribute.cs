using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class DesignOnlyAttribute : Attribute
	{
		public DesignOnlyAttribute(bool isDesignOnly)
		{
			this.IsDesignOnly = isDesignOnly;
		}

		public bool IsDesignOnly { get; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DesignOnlyAttribute designOnlyAttribute = obj as DesignOnlyAttribute;
			bool? flag = ((designOnlyAttribute != null) ? new bool?(designOnlyAttribute.IsDesignOnly) : null);
			bool isDesignOnly = this.IsDesignOnly;
			return (flag.GetValueOrDefault() == isDesignOnly) & (flag != null);
		}

		public override int GetHashCode()
		{
			return this.IsDesignOnly.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.IsDesignOnly == DesignOnlyAttribute.Default.IsDesignOnly;
		}

		public static readonly DesignOnlyAttribute Yes = new DesignOnlyAttribute(true);

		public static readonly DesignOnlyAttribute No = new DesignOnlyAttribute(false);

		public static readonly DesignOnlyAttribute Default = DesignOnlyAttribute.No;
	}
}
