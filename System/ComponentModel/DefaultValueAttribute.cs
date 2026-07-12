using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class DefaultValueAttribute : Attribute
	{
		public DefaultValueAttribute(Type type, string value)
		{
			try
			{
				object obj;
				if (DefaultValueAttribute.<.ctor>g__TryConvertFromInvariantString|2_0(type, value, out obj))
				{
					this._value = obj;
				}
				else if (type.IsSubclassOf(typeof(Enum)))
				{
					this._value = Enum.Parse(type, value, true);
				}
				else if (type == typeof(TimeSpan))
				{
					this._value = TimeSpan.Parse(value);
				}
				else
				{
					this._value = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
				}
			}
			catch
			{
			}
		}

		public DefaultValueAttribute(char value)
		{
			this._value = value;
		}

		public DefaultValueAttribute(byte value)
		{
			this._value = value;
		}

		public DefaultValueAttribute(short value)
		{
			this._value = value;
		}

		public DefaultValueAttribute(int value)
		{
			this._value = value;
		}

		public DefaultValueAttribute(long value)
		{
			this._value = value;
		}

		public DefaultValueAttribute(float value)
		{
			this._value = value;
		}

		public DefaultValueAttribute(double value)
		{
			this._value = value;
		}

		public DefaultValueAttribute(bool value)
		{
			this._value = value;
		}

		public DefaultValueAttribute(string value)
		{
			this._value = value;
		}

		public DefaultValueAttribute(object value)
		{
			this._value = value;
		}

		[CLSCompliant(false)]
		public DefaultValueAttribute(sbyte value)
		{
			this._value = value;
		}

		[CLSCompliant(false)]
		public DefaultValueAttribute(ushort value)
		{
			this._value = value;
		}

		[CLSCompliant(false)]
		public DefaultValueAttribute(uint value)
		{
			this._value = value;
		}

		[CLSCompliant(false)]
		public DefaultValueAttribute(ulong value)
		{
			this._value = value;
		}

		public virtual object Value
		{
			get
			{
				return this._value;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DefaultValueAttribute defaultValueAttribute = obj as DefaultValueAttribute;
			if (defaultValueAttribute == null)
			{
				return false;
			}
			if (this.Value != null)
			{
				return this.Value.Equals(defaultValueAttribute.Value);
			}
			return defaultValueAttribute.Value == null;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		protected void SetValue(object value)
		{
			this._value = value;
		}

		[CompilerGenerated]
		internal static bool <.ctor>g__TryConvertFromInvariantString|2_0(Type typeToConvert, string stringValue, out object conversionResult)
		{
			conversionResult = null;
			if (DefaultValueAttribute.s_convertFromInvariantString == null)
			{
				Type type = Type.GetType("System.ComponentModel.TypeDescriptor, System.ComponentModel.TypeConverter", false);
				Volatile.Write<object>(ref DefaultValueAttribute.s_convertFromInvariantString, (type == null) ? new object() : Delegate.CreateDelegate(typeof(Func<Type, string, object>), type, "ConvertFromInvariantString", false));
			}
			Func<Type, string, object> func = DefaultValueAttribute.s_convertFromInvariantString as Func<Type, string, object>;
			if (func == null)
			{
				return false;
			}
			conversionResult = func(typeToConvert, stringValue);
			return true;
		}

		private object _value;

		private static object s_convertFromInvariantString;
	}
}
