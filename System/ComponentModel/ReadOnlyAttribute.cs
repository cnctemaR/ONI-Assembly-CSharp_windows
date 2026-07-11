using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ReadOnlyAttribute : Attribute
	{
		public ReadOnlyAttribute(bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;
		}

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		public override bool Equals(object value)
		{
			if (this == value)
			{
				return true;
			}
			ReadOnlyAttribute readOnlyAttribute = value as ReadOnlyAttribute;
			return readOnlyAttribute != null && readOnlyAttribute.IsReadOnly == this.IsReadOnly;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.IsReadOnly == ReadOnlyAttribute.Default.IsReadOnly;
		}

		private bool isReadOnly;

		public static readonly ReadOnlyAttribute Yes = new ReadOnlyAttribute(true);

		public static readonly ReadOnlyAttribute No = new ReadOnlyAttribute(false);

		public static readonly ReadOnlyAttribute Default = ReadOnlyAttribute.No;
	}
}
