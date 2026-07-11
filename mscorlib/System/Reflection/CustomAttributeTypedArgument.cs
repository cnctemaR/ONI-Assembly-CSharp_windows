using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public struct CustomAttributeTypedArgument
	{
		public CustomAttributeTypedArgument(Type argumentType, object value)
		{
			if (argumentType == null)
			{
				throw new ArgumentNullException("argumentType");
			}
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

		public CustomAttributeTypedArgument(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.argumentType = value.GetType();
			this.value = value;
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
			string text = ((this.value != null) ? this.value.ToString() : string.Empty);
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
			if (!(customAttributeTypedArgument.argumentType == this.argumentType) || this.value == null)
			{
				return customAttributeTypedArgument.value == null;
			}
			return this.value.Equals(customAttributeTypedArgument.value);
		}

		public override int GetHashCode()
		{
			return (this.argumentType.GetHashCode() << 16) + ((this.value != null) ? this.value.GetHashCode() : 0);
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
