using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class DefaultValueAttribute : Attribute
	{
		public DefaultValueAttribute(Type type, string value)
		{
			try
			{
				this.value = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
			}
			catch
			{
			}
		}

		public DefaultValueAttribute(char value)
		{
			this.value = value;
		}

		public DefaultValueAttribute(byte value)
		{
			this.value = value;
		}

		public DefaultValueAttribute(short value)
		{
			this.value = value;
		}

		public DefaultValueAttribute(int value)
		{
			this.value = value;
		}

		public DefaultValueAttribute(long value)
		{
			this.value = value;
		}

		public DefaultValueAttribute(float value)
		{
			this.value = value;
		}

		public DefaultValueAttribute(double value)
		{
			this.value = value;
		}

		public DefaultValueAttribute(bool value)
		{
			this.value = value;
		}

		public DefaultValueAttribute(string value)
		{
			this.value = value;
		}

		public DefaultValueAttribute(object value)
		{
			this.value = value;
		}

		public virtual object Value
		{
			get
			{
				return this.value;
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
			this.value = value;
		}

		private object value;
	}
}
