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

		public override bool Equals(object value)
		{
			return value is RefreshPropertiesAttribute && ((RefreshPropertiesAttribute)value).RefreshProperties == this.refresh;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(RefreshPropertiesAttribute.Default);
		}

		public static readonly RefreshPropertiesAttribute All = new RefreshPropertiesAttribute(RefreshProperties.All);

		public static readonly RefreshPropertiesAttribute Repaint = new RefreshPropertiesAttribute(RefreshProperties.Repaint);

		public static readonly RefreshPropertiesAttribute Default = new RefreshPropertiesAttribute(RefreshProperties.None);

		private RefreshProperties refresh;
	}
}
