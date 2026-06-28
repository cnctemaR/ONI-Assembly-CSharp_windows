using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public struct CustomAttributeTypedArgument
	{
		internal CustomAttributeTypedArgument(Type argumentType, object value)
		{
			this.argumentType = argumentType;
			this.value = value;
			if (value is Array)
			{
				Array array = (Array)value;
				Type elementType = array.GetType().GetElementType();
				CustomAttributeTypedArgument[] array2 = new CustomAttributeTypedArgument[array.GetLength(0)];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = new CustomAttributeTypedArgument(elementType, array.GetValue(i));
				}
				this.value = new ReadOnlyCollection<CustomAttributeTypedArgument>(array2);
			}
		}

		public Type ArgumentType
		{
			get
			{
				return this.argumentType;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public override string ToString()
		{
			string text = ((this.value == null) ? string.Empty : this.value.ToString());
			if (this.argumentType == typeof(string))
			{
				return "\"" + text + "\"";
			}
			if (this.argumentType == typeof(Type))
			{
				return "typeof (" + text + ")";
			}
			if (this.argumentType.IsEnum)
			{
				return "(" + this.argumentType.Name + ")" + text;
			}
			return text;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CustomAttributeTypedArgument))
			{
				return false;
			}
			CustomAttributeTypedArgument customAttributeTypedArgument = (CustomAttributeTypedArgument)obj;
			return (customAttributeTypedArgument.argumentType != this.argumentType || this.value == null) ? (customAttributeTypedArgument.value == null) : this.value.Equals(customAttributeTypedArgument.value);
		}

		public override int GetHashCode()
		{
			return (this.argumentType.GetHashCode() << 16) + ((this.value == null) ? 0 : this.value.GetHashCode());
		}

		public static bool operator ==(CustomAttributeTypedArgument left, CustomAttributeTypedArgument right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(CustomAttributeTypedArgument left, CustomAttributeTypedArgument right)
		{
			return !left.Equals(right);
		}

		private Type argumentType;

		private object value;
	}
}
