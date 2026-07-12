using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class AmbientValueAttribute : Attribute
	{
		public AmbientValueAttribute(Type type, string value)
		{
			try
			{
				this.Value = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
			}
			catch
			{
			}
		}

		public AmbientValueAttribute(char value)
		{
			this.Value = value;
		}

		public AmbientValueAttribute(byte value)
		{
			this.Value = value;
		}

		public AmbientValueAttribute(short value)
		{
			this.Value = value;
		}

		public AmbientValueAttribute(int value)
		{
			this.Value = value;
		}

		public AmbientValueAttribute(long value)
		{
			this.Value = value;
		}

		public AmbientValueAttribute(float value)
		{
			this.Value = value;
		}

		public AmbientValueAttribute(double value)
		{
			this.Value = value;
		}

		public AmbientValueAttribute(bool value)
		{
			this.Value = value;
		}

		public AmbientValueAttribute(string value)
		{
			this.Value = value;
		}

		public AmbientValueAttribute(object value)
		{
			this.Value = value;
		}

		public object Value { get; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			AmbientValueAttribute ambientValueAttribute = obj as AmbientValueAttribute;
			if (ambientValueAttribute == null)
			{
				return false;
			}
			if (this.Value == null)
			{
				return ambientValueAttribute.Value == null;
			}
			return this.Value.Equals(ambientValueAttribute.Value);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
