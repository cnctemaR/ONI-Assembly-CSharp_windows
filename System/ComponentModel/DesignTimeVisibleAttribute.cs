using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public sealed class DesignTimeVisibleAttribute : Attribute
	{
		public DesignTimeVisibleAttribute()
			: this(true)
		{
		}

		public DesignTimeVisibleAttribute(bool visible)
		{
			this.visible = visible;
		}

		public bool Visible
		{
			get
			{
				return this.visible;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is DesignTimeVisibleAttribute && (obj == this || ((DesignTimeVisibleAttribute)obj).Visible == this.visible);
		}

		public override int GetHashCode()
		{
			return this.visible.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.visible == DesignTimeVisibleAttribute.Default.Visible;
		}

		private bool visible;

		public static readonly DesignTimeVisibleAttribute Default = new DesignTimeVisibleAttribute(true);

		public static readonly DesignTimeVisibleAttribute No = new DesignTimeVisibleAttribute(false);

		public static readonly DesignTimeVisibleAttribute Yes = new DesignTimeVisibleAttribute(true);
	}
}
