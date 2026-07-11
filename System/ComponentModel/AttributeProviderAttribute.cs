using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Property)]
	public class AttributeProviderAttribute : Attribute
	{
		public AttributeProviderAttribute(string typeName)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			this._typeName = typeName;
		}

		public AttributeProviderAttribute(string typeName, string propertyName)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			this._typeName = typeName;
			this._propertyName = propertyName;
		}

		public AttributeProviderAttribute(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this._typeName = type.AssemblyQualifiedName;
		}

		public string TypeName
		{
			get
			{
				return this._typeName;
			}
		}

		public string PropertyName
		{
			get
			{
				return this._propertyName;
			}
		}

		private string _typeName;

		private string _propertyName;
	}
}
