using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultEventAttribute : Attribute
	{
		public DefaultEventAttribute(string name)
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
			DefaultEventAttribute defaultEventAttribute = obj as DefaultEventAttribute;
			return defaultEventAttribute != null && defaultEventAttribute.Name == this.name;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private readonly string name;

		public static readonly DefaultEventAttribute Default = new DefaultEventAttribute(null);
	}
}
