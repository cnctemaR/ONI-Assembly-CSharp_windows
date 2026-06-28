using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
	public class DisplayNameAttribute : Attribute
	{
		public DisplayNameAttribute()
		{
			this.attributeDisplayName = string.Empty;
		}

		public DisplayNameAttribute(string displayName)
		{
			this.attributeDisplayName = displayName;
		}

		public override bool IsDefaultAttribute()
		{
			return this.attributeDisplayName != null && this.attributeDisplayName.Length == 0;
		}

		public override int GetHashCode()
		{
			return this.attributeDisplayName.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DisplayNameAttribute displayNameAttribute = obj as DisplayNameAttribute;
			return displayNameAttribute != null && displayNameAttribute.DisplayName == this.attributeDisplayName;
		}

		public virtual string DisplayName
		{
			get
			{
				return this.attributeDisplayName;
			}
		}

		protected string DisplayNameValue
		{
			get
			{
				return this.attributeDisplayName;
			}
			set
			{
				this.attributeDisplayName = value;
			}
		}

		public static readonly DisplayNameAttribute Default = new DisplayNameAttribute();

		private string attributeDisplayName;
	}
}
