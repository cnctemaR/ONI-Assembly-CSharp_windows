using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultPropertyAttribute : Attribute
	{
		public DefaultPropertyAttribute(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public override bool Equals(object obj)
		{
			DefaultPropertyAttribute defaultPropertyAttribute = obj as DefaultPropertyAttribute;
			return defaultPropertyAttribute != null && defaultPropertyAttribute.Name == this.name;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private readonly string name;

		public static readonly DefaultPropertyAttribute Default = new DefaultPropertyAttribute(null);
	}
}
