using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class DescriptionAttribute : Attribute
	{
		public DescriptionAttribute()
		{
			this.desc = string.Empty;
		}

		public DescriptionAttribute(string name)
		{
			this.desc = name;
		}

		public virtual string Description
		{
			get
			{
				return this.DescriptionValue;
			}
		}

		protected string DescriptionValue
		{
			get
			{
				return this.desc;
			}
			set
			{
				this.desc = value;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is DescriptionAttribute && (obj == this || ((DescriptionAttribute)obj).Description == this.desc);
		}

		public override int GetHashCode()
		{
			return this.desc.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this == DescriptionAttribute.Default;
		}

		private string desc;

		public static readonly DescriptionAttribute Default = new DescriptionAttribute();
	}
}
