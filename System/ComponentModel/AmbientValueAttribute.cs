using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class AmbientValueAttribute : Attribute
	{
		public AmbientValueAttribute(bool value)
		{
			this.AmbientValue = value;
		}

		public AmbientValueAttribute(byte value)
		{
			this.AmbientValue = value;
		}

		public AmbientValueAttribute(char value)
		{
			this.AmbientValue = value;
		}

		public AmbientValueAttribute(double value)
		{
			this.AmbientValue = value;
		}

		public AmbientValueAttribute(short value)
		{
			this.AmbientValue = value;
		}

		public AmbientValueAttribute(int value)
		{
			this.AmbientValue = value;
		}

		public AmbientValueAttribute(long value)
		{
			this.AmbientValue = value;
		}

		public AmbientValueAttribute(object value)
		{
			this.AmbientValue = value;
		}

		public AmbientValueAttribute(float value)
		{
			this.AmbientValue = value;
		}

		public AmbientValueAttribute(string value)
		{
			this.AmbientValue = value;
		}

		public AmbientValueAttribute(Type type, string value)
		{
			try
			{
				this.AmbientValue = Convert.ChangeType(value, type);
			}
			catch
			{
				this.AmbientValue = null;
			}
		}

		public object Value
		{
			get
			{
				return this.AmbientValue;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is AmbientValueAttribute && (obj == this || ((AmbientValueAttribute)obj).Value == this.AmbientValue);
		}

		public override int GetHashCode()
		{
			return this.AmbientValue.GetHashCode();
		}

		private object AmbientValue;
	}
}
