using System;
using System.Globalization;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class TypeConverterAttribute : Attribute
	{
		public TypeConverterAttribute()
		{
			this.typeName = string.Empty;
		}

		public TypeConverterAttribute(Type type)
		{
			this.typeName = type.AssemblyQualifiedName;
		}

		public TypeConverterAttribute(string typeName)
		{
			typeName.ToUpper(CultureInfo.InvariantCulture);
			this.typeName = typeName;
		}

		public string ConverterTypeName
		{
			get
			{
				return this.typeName;
			}
		}

		public override bool Equals(object obj)
		{
			TypeConverterAttribute typeConverterAttribute = obj as TypeConverterAttribute;
			return typeConverterAttribute != null && typeConverterAttribute.ConverterTypeName == this.typeName;
		}

		public override int GetHashCode()
		{
			return this.typeName.GetHashCode();
		}

		private string typeName;

		public static readonly TypeConverterAttribute Default = new TypeConverterAttribute();
	}
}
