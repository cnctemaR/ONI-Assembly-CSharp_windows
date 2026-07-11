using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class TypeConverterAttribute : Attribute
	{
		public TypeConverterAttribute()
		{
			this.converter_type = string.Empty;
		}

		public TypeConverterAttribute(string typeName)
		{
			this.converter_type = typeName;
		}

		public TypeConverterAttribute(Type type)
		{
			this.converter_type = type.AssemblyQualifiedName;
		}

		public override bool Equals(object obj)
		{
			return obj is TypeConverterAttribute && ((TypeConverterAttribute)obj).ConverterTypeName == this.converter_type;
		}

		public override int GetHashCode()
		{
			return this.converter_type.GetHashCode();
		}

		public string ConverterTypeName
		{
			get
			{
				return this.converter_type;
			}
		}

		public static readonly TypeConverterAttribute Default = new TypeConverterAttribute();

		private string converter_type;
	}
}
