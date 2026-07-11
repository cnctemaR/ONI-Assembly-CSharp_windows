using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class DescriptionAttribute : Attribute
	{
		public DescriptionAttribute()
			: this(string.Empty)
		{
		}

		public DescriptionAttribute(string description)
		{
			this.description = description;
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
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DescriptionAttribute descriptionAttribute = obj as DescriptionAttribute;
			return descriptionAttribute != null && descriptionAttribute.Description == this.Description;
		}

		public override int GetHashCode()
		{
			return this.Description.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(DescriptionAttribute.Default);
		}

		public static readonly DescriptionAttribute Default = new DescriptionAttribute();

		private string description;
	}
}
