using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ReadOnlyAttribute : Attribute
	{
		public ReadOnlyAttribute(bool read_only)
		{
			this.read_only = read_only;
		}

		public bool IsReadOnly
		{
			get
			{
				return this.read_only;
			}
		}

		public override int GetHashCode()
		{
			return this.read_only.GetHashCode();
		}

		public override bool Equals(object o)
		{
			return o is ReadOnlyAttribute && ((ReadOnlyAttribute)o).IsReadOnly.Equals(this.read_only);
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(ReadOnlyAttribute.Default);
		}

		private bool read_only;

		public static readonly ReadOnlyAttribute No = new ReadOnlyAttribute(false);

		public static readonly ReadOnlyAttribute Yes = new ReadOnlyAttribute(true);

		public static readonly ReadOnlyAttribute Default = new ReadOnlyAttribute(false);
	}
}
