using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultPropertyAttribute : Attribute
	{
		public DefaultPropertyAttribute(string name)
		{
			this.Name = name;
		}

		public string Name { get; }

		public override bool Equals(object obj)
		{
			DefaultPropertyAttribute defaultPropertyAttribute = obj as DefaultPropertyAttribute;
			return defaultPropertyAttribute != null && defaultPropertyAttribute.Name == this.Name;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static readonly DefaultPropertyAttribute Default = new DefaultPropertyAttribute(null);
	}
}
