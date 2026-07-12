using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class LocalizableAttribute : Attribute
	{
		public LocalizableAttribute(bool isLocalizable)
		{
			this.IsLocalizable = isLocalizable;
		}

		public bool IsLocalizable { get; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			LocalizableAttribute localizableAttribute = obj as LocalizableAttribute;
			bool? flag = ((localizableAttribute != null) ? new bool?(localizableAttribute.IsLocalizable) : null);
			bool isLocalizable = this.IsLocalizable;
			return (flag.GetValueOrDefault() == isLocalizable) & (flag != null);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.IsLocalizable == LocalizableAttribute.Default.IsLocalizable;
		}

		public static readonly LocalizableAttribute Yes = new LocalizableAttribute(true);

		public static readonly LocalizableAttribute No = new LocalizableAttribute(false);

		public static readonly LocalizableAttribute Default = LocalizableAttribute.No;
	}
}
