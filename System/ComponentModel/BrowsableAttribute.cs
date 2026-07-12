using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class BrowsableAttribute : Attribute
	{
		public BrowsableAttribute(bool browsable)
		{
			this.Browsable = browsable;
		}

		public bool Browsable { get; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			BrowsableAttribute browsableAttribute = obj as BrowsableAttribute;
			bool? flag = ((browsableAttribute != null) ? new bool?(browsableAttribute.Browsable) : null);
			bool browsable = this.Browsable;
			return (flag.GetValueOrDefault() == browsable) & (flag != null);
		}

		public override int GetHashCode()
		{
			return this.Browsable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(BrowsableAttribute.Default);
		}

		public static readonly BrowsableAttribute Yes = new BrowsableAttribute(true);

		public static readonly BrowsableAttribute No = new BrowsableAttribute(false);

		public static readonly BrowsableAttribute Default = BrowsableAttribute.Yes;
	}
}
