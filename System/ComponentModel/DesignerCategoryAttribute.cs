using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class DesignerCategoryAttribute : Attribute
	{
		public DesignerCategoryAttribute()
		{
			this.category = string.Empty;
		}

		public DesignerCategoryAttribute(string category)
		{
			this.category = category;
		}

		public string Category
		{
			get
			{
				return this.category;
			}
		}

		public override object TypeId
		{
			get
			{
				if (this.typeId == null)
				{
					this.typeId = base.GetType().FullName + this.Category;
				}
				return this.typeId;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DesignerCategoryAttribute designerCategoryAttribute = obj as DesignerCategoryAttribute;
			return designerCategoryAttribute != null && designerCategoryAttribute.category == this.category;
		}

		public override int GetHashCode()
		{
			return this.category.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.category.Equals(DesignerCategoryAttribute.Default.Category);
		}

		private string category;

		private string typeId;

		public static readonly DesignerCategoryAttribute Component = new DesignerCategoryAttribute("Component");

		public static readonly DesignerCategoryAttribute Default = new DesignerCategoryAttribute();

		public static readonly DesignerCategoryAttribute Form = new DesignerCategoryAttribute("Form");

		public static readonly DesignerCategoryAttribute Generic = new DesignerCategoryAttribute("Designer");
	}
}
