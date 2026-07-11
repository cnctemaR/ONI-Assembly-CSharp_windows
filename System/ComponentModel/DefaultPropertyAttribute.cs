using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultPropertyAttribute : Attribute
	{
		public DefaultPropertyAttribute(string name)
		{
			this.property_name = name;
		}

		public string Name
		{
			get
			{
				return this.property_name;
			}
		}

		public override bool Equals(object o)
		{
			return o is DefaultPropertyAttribute && ((DefaultPropertyAttribute)o).Name == this.property_name;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private string property_name;

		public static readonly DefaultPropertyAttribute Default = new DefaultPropertyAttribute(null);
	}
}
