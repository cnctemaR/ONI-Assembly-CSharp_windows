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
			this.DescriptionValue = description;
		}

		public virtual string Description
		{
			get
			{
				return this.DescriptionValue;
			}
		}

		protected string DescriptionValue { get; set; }

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
	}
}
