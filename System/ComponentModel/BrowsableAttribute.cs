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
			if (obj == this)
			{
				return true;
			}
			BrowsableAttribute browsableAttribute = obj as BrowsableAttribute;
			return browsableAttribute != null && browsableAttribute.Browsable == this.browsable;
		}

		public override int GetHashCode()
		{
			return this.browsable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(BrowsableAttribute.Default);
		}

		public static readonly BrowsableAttribute Yes = new BrowsableAttribute(true);

		public static readonly BrowsableAttribute No = new BrowsableAttribute(false);

		public static readonly BrowsableAttribute Default = BrowsableAttribute.Yes;

		private bool browsable = true;
	}
}
