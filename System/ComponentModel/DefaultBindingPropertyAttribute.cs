using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultBindingPropertyAttribute : Attribute
	{
		public DefaultBindingPropertyAttribute()
		{
		}

		public DefaultBindingPropertyAttribute(string name)
		{
			this.name = name;
		}

		public override bool Equals(object obj)
		{
			DefaultBindingPropertyAttribute defaultBindingPropertyAttribute = obj as DefaultBindingPropertyAttribute;
			return obj != null && this.name == defaultBindingPropertyAttribute.Name;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public static readonly DefaultBindingPropertyAttribute Default = new DefaultBindingPropertyAttribute();

		private string name;
	}
}
