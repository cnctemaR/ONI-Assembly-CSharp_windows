using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
	public class DisplayNameAttribute : Attribute
	{
		public DisplayNameAttribute()
			: this(string.Empty)
		{
		}

		public DisplayNameAttribute(string displayName)
		{
			this.DisplayNameValue = displayName;
		}

		public virtual string DisplayName
		{
			get
			{
				return this.DisplayNameValue;
			}
		}

		protected string DisplayNameValue { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DisplayNameAttribute displayNameAttribute = obj as DisplayNameAttribute;
			return displayNameAttribute != null && displayNameAttribute.DisplayName == this.DisplayName;
		}

		public override int GetHashCode()
		{
			return this.DisplayName.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(DisplayNameAttribute.Default);
		}

		public static readonly DisplayNameAttribute Default = new DisplayNameAttribute();
	}
}
