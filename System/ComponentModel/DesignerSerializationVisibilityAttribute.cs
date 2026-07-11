using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event)]
	public sealed class DesignerSerializationVisibilityAttribute : Attribute
	{
		public DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility vis)
		{
			this.visibility = vis;
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
			return obj is DesignerSerializationVisibilityAttribute && (obj == this || ((DesignerSerializationVisibilityAttribute)obj).Visibility == this.visibility);
		}

		public override int GetHashCode()
		{
			return this.visibility.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.visibility == DesignerSerializationVisibilityAttribute.Default.Visibility;
		}

		private DesignerSerializationVisibility visibility;

		public static readonly DesignerSerializationVisibilityAttribute Default = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible);

		public static readonly DesignerSerializationVisibilityAttribute Content = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content);

		public static readonly DesignerSerializationVisibilityAttribute Hidden = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden);

		public static readonly DesignerSerializationVisibilityAttribute Visible = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible);
	}
}
