using System;
using System.ComponentModel;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldNullValueAttribute : Attribute
	{
		public object NullValue { get; private set; }

		public FieldNullValueAttribute(object nullValue)
		{
			this.NullValue = nullValue;
		}

		public FieldNullValueAttribute(Type type, string nullValue)
			: this(TypeDescriptor.GetConverter(type).ConvertFromString(nullValue))
		{
		}
	}
}
