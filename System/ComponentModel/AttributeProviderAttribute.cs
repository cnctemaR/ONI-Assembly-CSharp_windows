using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class AttributeProviderAttribute : Attribute
	{
		public AttributeProviderAttribute(Type type)
		{
			this.type_name = type.AssemblyQualifiedName;
		}

		public AttributeProviderAttribute(string typeName, string propertyName)
		{
			this.type_name = typeName;
			this.property_name = propertyName;
		}

		public AttributeProviderAttribute(string typeName)
		{
			this.type_name = typeName;
		}

		public string PropertyName
		{
			get
			{
				return this.property_name;
			}
		}

		public string TypeName
		{
			get
			{
				return this.type_name;
			}
		}

		private string type_name;

		private string property_name;
	}
}
