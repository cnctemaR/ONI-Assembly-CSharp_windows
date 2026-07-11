using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class RefreshPropertiesAttribute : Attribute
	{
		public RefreshPropertiesAttribute(RefreshProperties refresh)
		{
			this.refresh = refresh;
		}

		public RefreshProperties RefreshProperties
		{
			get
			{
				return this.refresh;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is RefreshPropertiesAttribute && (obj == this || ((RefreshPropertiesAttribute)obj).RefreshProperties == this.refresh);
		}

		public override int GetHashCode()
		{
			return this.refresh.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this == RefreshPropertiesAttribute.Default;
		}

		private RefreshProperties refresh;

		public static readonly RefreshPropertiesAttribute All = new RefreshPropertiesAttribute(RefreshProperties.All);

		public static readonly RefreshPropertiesAttribute Default = new RefreshPropertiesAttribute(RefreshProperties.None);

		public static readonly RefreshPropertiesAttribute Repaint = new RefreshPropertiesAttribute(RefreshProperties.Repaint);
	}
}
