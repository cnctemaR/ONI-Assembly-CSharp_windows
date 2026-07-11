using System;
using System.Globalization;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class DefaultValueAttribute : Attribute
	{
		public DefaultValueAttribute(bool value)
		{
			this.DefaultValue = value;
		}

		public DefaultValueAttribute(byte value)
		{
			this.DefaultValue = value;
		}

		public DefaultValueAttribute(char value)
		{
			this.DefaultValue = value;
		}

		public DefaultValueAttribute(double value)
		{
			this.DefaultValue = value;
		}

		public DefaultValueAttribute(short value)
		{
			this.DefaultValue = value;
		}

		public DefaultValueAttribute(int value)
		{
			this.DefaultValue = value;
		}

		public DefaultValueAttribute(long value)
		{
			this.DefaultValue = value;
		}

		public DefaultValueAttribute(object value)
		{
			this.DefaultValue = value;
		}

		public DefaultValueAttribute(float value)
		{
			this.DefaultValue = value;
		}

		public DefaultValueAttribute(string value)
		{
			this.DefaultValue = value;
		}

		public DefaultValueAttribute(Type type, string value)
		{
			try
			{
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				this.DefaultValue = converter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
			}
			catch
			{
			}
		}

		public virtual object Value
		{
			get
			{
				return this.DefaultValue;
			}
		}

		protected void SetValue(object value)
		{
			this.DefaultValue = value;
		}

		public override bool Equals(object obj)
		{
			DefaultValueAttribute defaultValueAttribute = obj as DefaultValueAttribute;
			if (defaultValueAttribute == null)
			{
				return false;
			}
			if (this.DefaultValue == null)
			{
				return defaultValueAttribute.Value == null;
			}
			return this.DefaultValue.Equals(defaultValueAttribute.Value);
		}

		public override int GetHashCode()
		{
			if (this.DefaultValue == null)
			{
				return base.GetHashCode();
			}
			return this.DefaultValue.GetHashCode();
		}

		private object DefaultValue;
	}
}
