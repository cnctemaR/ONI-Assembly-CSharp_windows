using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class LocalizableAttribute : Attribute
	{
		public LocalizableAttribute(bool localizable)
		{
			this.localizable = localizable;
		}

		public bool IsLocalizable
		{
			get
			{
				return this.localizable;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is LocalizableAttribute && (obj == this || ((LocalizableAttribute)obj).IsLocalizable == this.localizable);
		}

		public override int GetHashCode()
		{
			return this.localizable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.localizable == LocalizableAttribute.Default.IsLocalizable;
		}

		private bool localizable;

		public static readonly LocalizableAttribute Default = new LocalizableAttribute(false);

		public static readonly LocalizableAttribute No = new LocalizableAttribute(false);

		public static readonly LocalizableAttribute Yes = new LocalizableAttribute(true);
	}
}
