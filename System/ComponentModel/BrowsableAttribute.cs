using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class BrowsableAttribute : Attribute
	{
		public BrowsableAttribute(bool browsable)
		{
			this.browsable = browsable;
		}

		public bool Browsable
		{
			get
			{
				return this.browsable;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is BrowsableAttribute && (obj == this || ((BrowsableAttribute)obj).Browsable == this.browsable);
		}

		public override int GetHashCode()
		{
			return this.browsable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.browsable == BrowsableAttribute.Default.Browsable;
		}

		private bool browsable;

		public static readonly BrowsableAttribute Default = new BrowsableAttribute(true);

		public static readonly BrowsableAttribute No = new BrowsableAttribute(false);

		public static readonly BrowsableAttribute Yes = new BrowsableAttribute(true);
	}
}
