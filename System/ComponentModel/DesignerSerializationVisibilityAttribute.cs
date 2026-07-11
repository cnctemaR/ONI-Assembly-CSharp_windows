using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event)]
	public sealed class DesignerSerializationVisibilityAttribute : Attribute
	{
		public DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility visibility)
		{
			this.visibility = visibility;
		}

		public DesignerSerializationVisibility Visibility
		{
			get
			{
				return this.visibility;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DesignerSerializationVisibilityAttribute designerSerializationVisibilityAttribute = obj as DesignerSerializationVisibilityAttribute;
			return designerSerializationVisibilityAttribute != null && designerSerializationVisibilityAttribute.Visibility == this.visibility;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(DesignerSerializationVisibilityAttribute.Default);
		}

		public static readonly DesignerSerializationVisibilityAttribute Content = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content);

		public static readonly DesignerSerializationVisibilityAttribute Hidden = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden);

		public static readonly DesignerSerializationVisibilityAttribute Visible = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible);

		public static readonly DesignerSerializationVisibilityAttribute Default = DesignerSerializationVisibilityAttribute.Visible;

		private DesignerSerializationVisibility visibility;
	}
}
