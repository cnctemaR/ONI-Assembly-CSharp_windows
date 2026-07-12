using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class TypeConverterAttribute : Attribute
	{
		public TypeConverterAttribute()
		{
			this.ConverterTypeName = string.Empty;
		}

		public TypeConverterAttribute(Type type)
		{
			this.ConverterTypeName = type.AssemblyQualifiedName;
		}

		public TypeConverterAttribute(string typeName)
		{
			this.ConverterTypeName = typeName;
		}

		public string ConverterTypeName { get; }

		public override bool Equals(object obj)
		{
			TypeConverterAttribute typeConverterAttribute = obj as TypeConverterAttribute;
			return typeConverterAttribute != null && typeConverterAttribute.ConverterTypeName == this.ConverterTypeName;
		}

		public override int GetHashCode()
		{
			return this.ConverterTypeName.GetHashCode();
		}

		public static readonly TypeConverterAttribute Default = new TypeConverterAttribute();
	}
}
