using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class CategoryAttribute : Attribute
	{
		public static CategoryAttribute Action
		{
			get
			{
				if (CategoryAttribute.action == null)
				{
					CategoryAttribute.action = new CategoryAttribute("Action");
				}
				return CategoryAttribute.action;
			}
		}

		public static CategoryAttribute Appearance
		{
			get
			{
				if (CategoryAttribute.appearance == null)
				{
					CategoryAttribute.appearance = new CategoryAttribute("Appearance");
				}
				return CategoryAttribute.appearance;
			}
		}

		public static CategoryAttribute Asynchronous
		{
			get
			{
				if (CategoryAttribute.asynchronous == null)
				{
					CategoryAttribute.asynchronous = new CategoryAttribute("Asynchronous");
				}
				return CategoryAttribute.asynchronous;
			}
		}

		public static CategoryAttribute Behavior
		{
			get
			{
				if (CategoryAttribute.behavior == null)
				{
					CategoryAttribute.behavior = new CategoryAttribute("Behavior");
				}
				return CategoryAttribute.behavior;
			}
		}

		public static CategoryAttribute Data
		{
			get
			{
				if (CategoryAttribute.data == null)
				{
					CategoryAttribute.data = new CategoryAttribute("Data");
				}
				return CategoryAttribute.data;
			}
		}

		public static CategoryAttribute Default
		{
			get
			{
				if (CategoryAttribute.defAttr == null)
				{
					CategoryAttribute.defAttr = new CategoryAttribute();
				}
				return CategoryAttribute.defAttr;
			}
		}

		public static CategoryAttribute Design
		{
			get
			{
				if (CategoryAttribute.design == null)
				{
					CategoryAttribute.design = new CategoryAttribute("Design");
				}
				return CategoryAttribute.design;
			}
		}

		public static CategoryAttribute DragDrop
		{
			get
			{
				if (CategoryAttribute.dragDrop == null)
				{
					CategoryAttribute.dragDrop = new CategoryAttribute("DragDrop");
				}
				return CategoryAttribute.dragDrop;
			}
		}

		public static CategoryAttribute Focus
		{
			get
			{
				if (CategoryAttribute.focus == null)
				{
					CategoryAttribute.focus = new CategoryAttribute("Focus");
				}
				return CategoryAttribute.focus;
			}
		}

		public static CategoryAttribute Format
		{
			get
			{
				if (CategoryAttribute.format == null)
				{
					CategoryAttribute.format = new CategoryAttribute("Format");
				}
				return CategoryAttribute.format;
			}
		}

		public static CategoryAttribute Key
		{
			get
			{
				if (CategoryAttribute.key == null)
				{
					CategoryAttribute.key = new CategoryAttribute("Key");
				}
				return CategoryAttribute.key;
			}
		}

		public static CategoryAttribute Layout
		{
			get
			{
				if (CategoryAttribute.layout == null)
				{
					CategoryAttribute.layout = new CategoryAttribute("Layout");
				}
				return CategoryAttribute.layout;
			}
		}

		public static CategoryAttribute Mouse
		{
			get
			{
				if (CategoryAttribute.mouse == null)
				{
					CategoryAttribute.mouse = new CategoryAttribute("Mouse");
				}
				return CategoryAttribute.mouse;
			}
		}

		public static CategoryAttribute WindowStyle
		{
			get
			{
				if (CategoryAttribute.windowStyle == null)
				{
					CategoryAttribute.windowStyle = new CategoryAttribute("WindowStyle");
				}
				return CategoryAttribute.windowStyle;
			}
		}

		public CategoryAttribute()
			: this("Default")
		{
		}

		public CategoryAttribute(string category)
		{
			this.categoryValue = category;
			this.localized = false;
		}

		public string Category
		{
			get
			{
				if (!this.localized)
				{
					this.localized = true;
					string localizedString = this.GetLocalizedString(this.categoryValue);
					if (localizedString != null)
					{
						this.categoryValue = localizedString;
					}
				}
				return this.categoryValue;
			}
		}

		public override bool Equals(object obj)
		{
			return obj == this || (obj is CategoryAttribute && this.Category.Equals(((CategoryAttribute)obj).Category));
		}

		public override int GetHashCode()
		{
			return this.Category.GetHashCode();
		}

		protected virtual string GetLocalizedString(string value)
		{
			uint num = global::<PrivateImplementationDetails>.ComputeStringHash(value);
			if (num <= 1062369733U)
			{
				if (num <= 630759034U)
				{
					if (num <= 433860734U)
					{
						if (num != 175614239U)
						{
							if (num == 433860734U)
							{
								if (value == "Default")
								{
									return "Misc";
								}
							}
						}
						else if (value == "Action")
						{
							return "Action";
						}
					}
					else if (num != 521774151U)
					{
						if (num == 630759034U)
						{
							if (value == "DragDrop")
							{
								return "Drag Drop";
							}
						}
					}
					else if (value == "Behavior")
					{
						return "Behavior";
					}
				}
				else if (num <= 723360612U)
				{
					if (num != 676498961U)
					{
						if (num == 723360612U)
						{
							if (value == "Mouse")
							{
								return "Mouse";
							}
						}
					}
					else if (value == "Scale")
					{
						return "Scale";
					}
				}
				else if (num != 822184863U)
				{
					if (num != 1041509726U)
					{
						if (num == 1062369733U)
						{
							if (value == "Data")
							{
								return "Data";
							}
						}
					}
					else if (value == "Text")
					{
						return "Text";
					}
				}
				else if (value == "Appearance")
				{
					return "Appearance";
				}
			}
			else if (num <= 2809814704U)
			{
				if (num <= 1779622119U)
				{
					if (num != 1762750224U)
					{
						if (num == 1779622119U)
						{
							if (value == "Config")
							{
								return "Configurations";
							}
						}
					}
					else if (value == "DDE")
					{
						return "DDE";
					}
				}
				else if (num != 2055433310U)
				{
					if (num != 2368288673U)
					{
						if (num == 2809814704U)
						{
							if (value == "Font")
							{
								return "Font";
							}
						}
					}
					else if (value == "List")
					{
						return "List";
					}
				}
				else if (value == "WindowStyle")
				{
					return "Window Style";
				}
			}
			else if (num <= 3441084684U)
			{
				if (num != 3159863731U)
				{
					if (num == 3441084684U)
					{
						if (value == "Key")
						{
							return "Key";
						}
					}
				}
				else if (value == "Focus")
				{
					return "Focus";
				}
			}
			else if (num != 3799987242U)
			{
				if (num != 3901555439U)
				{
					if (num == 4152902175U)
					{
						if (value == "Layout")
						{
							return "Layout";
						}
					}
				}
				else if (value == "Design")
				{
					return "Design";
				}
			}
			else if (value == "Position")
			{
				return "Position";
			}
			return value;
		}

		public override bool IsDefaultAttribute()
		{
			return this.Category.Equals(CategoryAttribute.Default.Category);
		}

		private static volatile CategoryAttribute appearance;

		private static volatile CategoryAttribute asynchronous;

		private static volatile CategoryAttribute behavior;

		private static volatile CategoryAttribute data;

		private static volatile CategoryAttribute design;

		private static volatile CategoryAttribute action;

		private static volatile CategoryAttribute format;

		private static volatile CategoryAttribute layout;

		private static volatile CategoryAttribute mouse;

		private static volatile CategoryAttribute key;

		private static volatile CategoryAttribute focus;

		private static volatile CategoryAttribute windowStyle;

		private static volatile CategoryAttribute dragDrop;

		private static volatile CategoryAttribute defAttr;

		private bool localized;

		private string categoryValue;
	}
}
