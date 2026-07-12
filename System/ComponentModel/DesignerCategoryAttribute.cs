using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class DesignerCategoryAttribute : Attribute
	{
		public DesignerCategoryAttribute()
		{
			this.Category = string.Empty;
		}

		public DesignerCategoryAttribute(string category)
		{
			this.Category = category;
		}

		public string Category { get; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DesignerCategoryAttribute designerCategoryAttribute = obj as DesignerCategoryAttribute;
			return designerCategoryAttribute != null && designerCategoryAttribute.Category == this.Category;
		}

		public override int GetHashCode()
		{
			return this.Category.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Category.Equals(DesignerCategoryAttribute.Default.Category);
		}

		public override object TypeId
		{
			get
			{
				return base.GetType().FullName + this.Category;
			}
		}

		public static readonly DesignerCategoryAttribute Component = new DesignerCategoryAttribute("Component");

		public static readonly DesignerCategoryAttribute Default = new DesignerCategoryAttribute();

		public static readonly DesignerCategoryAttribute Form = new DesignerCategoryAttribute("Form");

		public static readonly DesignerCategoryAttribute Generic = new DesignerCategoryAttribute("Designer");
	}
}
