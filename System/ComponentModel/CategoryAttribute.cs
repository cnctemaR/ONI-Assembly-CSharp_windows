using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class CategoryAttribute : Attribute
	{
		public CategoryAttribute()
		{
			this.category = "Misc";
		}

		public CategoryAttribute(string category)
		{
			this.category = category;
		}

		public static CategoryAttribute Action
		{
			get
			{
				if (CategoryAttribute.action != null)
				{
					return CategoryAttribute.action;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.action == null)
					{
						CategoryAttribute.action = new CategoryAttribute("Action");
					}
				}
				return CategoryAttribute.action;
			}
		}

		public static CategoryAttribute Appearance
		{
			get
			{
				if (CategoryAttribute.appearance != null)
				{
					return CategoryAttribute.appearance;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.appearance == null)
					{
						CategoryAttribute.appearance = new CategoryAttribute("Appearance");
					}
				}
				return CategoryAttribute.appearance;
			}
		}

		public static CategoryAttribute Asynchronous
		{
			get
			{
				if (CategoryAttribute.behaviour != null)
				{
					return CategoryAttribute.behaviour;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.async == null)
					{
						CategoryAttribute.async = new CategoryAttribute("Asynchronous");
					}
				}
				return CategoryAttribute.async;
			}
		}

		public static CategoryAttribute Behavior
		{
			get
			{
				if (CategoryAttribute.behaviour != null)
				{
					return CategoryAttribute.behaviour;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.behaviour == null)
					{
						CategoryAttribute.behaviour = new CategoryAttribute("Behavior");
					}
				}
				return CategoryAttribute.behaviour;
			}
		}

		public static CategoryAttribute Data
		{
			get
			{
				if (CategoryAttribute.data != null)
				{
					return CategoryAttribute.data;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.data == null)
					{
						CategoryAttribute.data = new CategoryAttribute("Data");
					}
				}
				return CategoryAttribute.data;
			}
		}

		public static CategoryAttribute Default
		{
			get
			{
				if (CategoryAttribute.def != null)
				{
					return CategoryAttribute.def;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.def == null)
					{
						CategoryAttribute.def = new CategoryAttribute();
					}
				}
				return CategoryAttribute.def;
			}
		}

		public static CategoryAttribute Design
		{
			get
			{
				if (CategoryAttribute.design != null)
				{
					return CategoryAttribute.design;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.design == null)
					{
						CategoryAttribute.design = new CategoryAttribute("Design");
					}
				}
				return CategoryAttribute.design;
			}
		}

		public static CategoryAttribute DragDrop
		{
			get
			{
				if (CategoryAttribute.drag_drop != null)
				{
					return CategoryAttribute.drag_drop;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.drag_drop == null)
					{
						CategoryAttribute.drag_drop = new CategoryAttribute("Drag Drop");
					}
				}
				return CategoryAttribute.drag_drop;
			}
		}

		public static CategoryAttribute Focus
		{
			get
			{
				if (CategoryAttribute.focus != null)
				{
					return CategoryAttribute.focus;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.focus == null)
					{
						CategoryAttribute.focus = new CategoryAttribute("Focus");
					}
				}
				return CategoryAttribute.focus;
			}
		}

		public static CategoryAttribute Format
		{
			get
			{
				if (CategoryAttribute.format != null)
				{
					return CategoryAttribute.format;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.format == null)
					{
						CategoryAttribute.format = new CategoryAttribute("Format");
					}
				}
				return CategoryAttribute.format;
			}
		}

		public static CategoryAttribute Key
		{
			get
			{
				if (CategoryAttribute.key != null)
				{
					return CategoryAttribute.key;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.key == null)
					{
						CategoryAttribute.key = new CategoryAttribute("Key");
					}
				}
				return CategoryAttribute.key;
			}
		}

		public static CategoryAttribute Layout
		{
			get
			{
				if (CategoryAttribute.layout != null)
				{
					return CategoryAttribute.layout;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.layout == null)
					{
						CategoryAttribute.layout = new CategoryAttribute("Layout");
					}
				}
				return CategoryAttribute.layout;
			}
		}

		public static CategoryAttribute Mouse
		{
			get
			{
				if (CategoryAttribute.mouse != null)
				{
					return CategoryAttribute.mouse;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.mouse == null)
					{
						CategoryAttribute.mouse = new CategoryAttribute("Mouse");
					}
				}
				return CategoryAttribute.mouse;
			}
		}

		public static CategoryAttribute WindowStyle
		{
			get
			{
				if (CategoryAttribute.window_style != null)
				{
					return CategoryAttribute.window_style;
				}
				object obj = CategoryAttribute.lockobj;
				lock (obj)
				{
					if (CategoryAttribute.window_style == null)
					{
						CategoryAttribute.window_style = new CategoryAttribute("Window Style");
					}
				}
				return CategoryAttribute.window_style;
			}
		}

		protected virtual string GetLocalizedString(string value)
		{
			return global::Locale.GetText(value);
		}

		public string Category
		{
			get
			{
				if (!this.IsLocalized)
				{
					this.IsLocalized = true;
					string localizedString = this.GetLocalizedString(this.category);
					if (localizedString != null)
					{
						this.category = localizedString;
					}
				}
				return this.category;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is CategoryAttribute && (obj == this || ((CategoryAttribute)obj).Category == this.category);
		}

		public override int GetHashCode()
		{
			return this.category.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.category == CategoryAttribute.Default.Category;
		}

		private string category;

		private bool IsLocalized;

		private static volatile CategoryAttribute action;

		private static volatile CategoryAttribute appearance;

		private static volatile CategoryAttribute behaviour;

		private static volatile CategoryAttribute data;

		private static volatile CategoryAttribute def;

		private static volatile CategoryAttribute design;

		private static volatile CategoryAttribute drag_drop;

		private static volatile CategoryAttribute focus;

		private static volatile CategoryAttribute format;

		private static volatile CategoryAttribute key;

		private static volatile CategoryAttribute layout;

		private static volatile CategoryAttribute mouse;

		private static volatile CategoryAttribute window_style;

		private static volatile CategoryAttribute async;

		private static object lockobj = new object();
	}
}
