using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public sealed class DesignTimeVisibleAttribute : Attribute
	{
		public DesignTimeVisibleAttribute(bool visible)
		{
			this.Visible = visible;
		}

		public DesignTimeVisibleAttribute()
		{
		}

		public bool Visible { get; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DesignTimeVisibleAttribute designTimeVisibleAttribute = obj as DesignTimeVisibleAttribute;
			return designTimeVisibleAttribute != null && designTimeVisibleAttribute.Visible == this.Visible;
		}

		public override int GetHashCode()
		{
			return typeof(DesignTimeVisibleAttribute).GetHashCode() ^ (this.Visible ? (-1) : 0);
		}

		public override bool IsDefaultAttribute()
		{
			return this.Visible == DesignTimeVisibleAttribute.Default.Visible;
		}

		public static readonly DesignTimeVisibleAttribute Yes = new DesignTimeVisibleAttribute(true);

		public static readonly DesignTimeVisibleAttribute No = new DesignTimeVisibleAttribute(false);

		public static readonly DesignTimeVisibleAttribute Default = DesignTimeVisibleAttribute.Yes;
	}
}
