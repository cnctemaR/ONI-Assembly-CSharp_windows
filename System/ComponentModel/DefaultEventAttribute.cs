using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultEventAttribute : Attribute
	{
		public DefaultEventAttribute(string name)
		{
			this.Name = name;
		}

		public string Name { get; }

		public override bool Equals(object obj)
		{
			DefaultEventAttribute defaultEventAttribute = obj as DefaultEventAttribute;
			return defaultEventAttribute != null && defaultEventAttribute.Name == this.Name;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static readonly DefaultEventAttribute Default = new DefaultEventAttribute(null);
	}
}
