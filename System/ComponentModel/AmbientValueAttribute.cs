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
				this.value = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
			}
			catch
			{
			}
		}

		public AmbientValueAttribute(char value)
		{
			this.value = value;
		}

		public AmbientValueAttribute(byte value)
		{
			this.value = value;
		}

		public AmbientValueAttribute(short value)
		{
			this.value = value;
		}

		public AmbientValueAttribute(int value)
		{
			this.value = value;
		}

		public AmbientValueAttribute(long value)
		{
			this.value = value;
		}

		public AmbientValueAttribute(float value)
		{
			this.value = value;
		}

		public AmbientValueAttribute(double value)
		{
			this.value = value;
		}

		public AmbientValueAttribute(bool value)
		{
			this.value = value;
		}

		public AmbientValueAttribute(string value)
		{
			this.value = value;
		}

		public AmbientValueAttribute(object value)
		{
			this.value = value;
		}

		public object Value
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
			AmbientValueAttribute ambientValueAttribute = obj as AmbientValueAttribute;
			if (ambientValueAttribute == null)
			{
				return false;
			}
			if (this.value != null)
			{
				return this.value.Equals(ambientValueAttribute.Value);
			}
			return ambientValueAttribute.Value == null;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private readonly object value;
	}
}
