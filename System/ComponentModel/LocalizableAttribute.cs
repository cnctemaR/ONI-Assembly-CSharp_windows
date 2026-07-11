using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class LocalizableAttribute : Attribute
	{
		public LocalizableAttribute(bool isLocalizable)
		{
			this.isLocalizable = isLocalizable;
		}

		public bool IsLocalizable
		{
			get
			{
				return this.isLocalizable;
			}
		}

		public override bool IsDefaultAttribute()
		{
			return this.IsLocalizable == LocalizableAttribute.Default.IsLocalizable;
		}

		public override bool Equals(object obj)
		{
			LocalizableAttribute localizableAttribute = obj as LocalizableAttribute;
			return localizableAttribute != null && localizableAttribute.IsLocalizable == this.isLocalizable;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private bool isLocalizable;

		public static readonly LocalizableAttribute Yes = new LocalizableAttribute(true);

		public static readonly LocalizableAttribute No = new LocalizableAttribute(false);

		public static readonly LocalizableAttribute Default = LocalizableAttribute.No;
	}
}
