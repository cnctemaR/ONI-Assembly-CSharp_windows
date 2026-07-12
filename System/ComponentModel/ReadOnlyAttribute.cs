using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ReadOnlyAttribute : Attribute
	{
		public ReadOnlyAttribute(bool isReadOnly)
		{
			this.IsReadOnly = isReadOnly;
		}

		public bool IsReadOnly { get; }

		public override bool Equals(object value)
		{
			if (this == value)
			{
				return true;
			}
			ReadOnlyAttribute readOnlyAttribute = value as ReadOnlyAttribute;
			bool? flag = ((readOnlyAttribute != null) ? new bool?(readOnlyAttribute.IsReadOnly) : null);
			bool isReadOnly = this.IsReadOnly;
			return (flag.GetValueOrDefault() == isReadOnly) & (flag != null);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.IsReadOnly == ReadOnlyAttribute.Default.IsReadOnly;
		}

		public static readonly ReadOnlyAttribute Yes = new ReadOnlyAttribute(true);

		public static readonly ReadOnlyAttribute No = new ReadOnlyAttribute(false);

		public static readonly ReadOnlyAttribute Default = ReadOnlyAttribute.No;
	}
}
