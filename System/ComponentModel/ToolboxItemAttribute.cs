using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class ToolboxItemAttribute : Attribute
	{
		public ToolboxItemAttribute(bool defaultType)
		{
			if (defaultType)
			{
				this.itemTypeName = "System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
			}
		}

		public ToolboxItemAttribute(string toolboxItemName)
		{
			this.itemTypeName = toolboxItemName;
		}

		public ToolboxItemAttribute(Type toolboxItemType)
		{
			this.itemType = toolboxItemType;
		}

		public Type ToolboxItemType
		{
			get
			{
				if (this.itemType == null && this.itemTypeName != null)
				{
					try
					{
						this.itemType = Type.GetType(this.itemTypeName, true);
					}
					catch (Exception ex)
					{
						throw new ArgumentException("Failed to create ToolboxItem of type: " + this.itemTypeName, ex);
					}
				}
				return this.itemType;
			}
		}

		public string ToolboxItemTypeName
		{
			get
			{
				if (this.itemTypeName == null)
				{
					if (this.itemType == null)
					{
						return string.Empty;
					}
					this.itemTypeName = this.itemType.AssemblyQualifiedName;
				}
				return this.itemTypeName;
			}
		}

		public override bool Equals(object o)
		{
			ToolboxItemAttribute toolboxItemAttribute = o as ToolboxItemAttribute;
			return toolboxItemAttribute != null && toolboxItemAttribute.ToolboxItemTypeName == this.ToolboxItemTypeName;
		}

		public override int GetHashCode()
		{
			if (this.itemTypeName != null)
			{
				return this.itemTypeName.GetHashCode();
			}
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(ToolboxItemAttribute.Default);
		}

		private const string defaultItemType = "System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

		public static readonly ToolboxItemAttribute Default = new ToolboxItemAttribute("System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");

		public static readonly ToolboxItemAttribute None = new ToolboxItemAttribute(false);

		private Type itemType;

		private string itemTypeName;
	}
}
