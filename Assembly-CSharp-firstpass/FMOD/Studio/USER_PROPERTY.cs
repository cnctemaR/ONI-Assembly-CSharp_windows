using System;

namespace FMOD.Studio
{
	public struct USER_PROPERTY
	{
		public int intValue()
		{
			return (this.type != USER_PROPERTY_TYPE.INTEGER) ? (-1) : this.value.intvalue;
		}

		public bool boolValue()
		{
			return this.type == USER_PROPERTY_TYPE.BOOLEAN && this.value.boolvalue;
		}

		public float floatValue()
		{
			return (this.type != USER_PROPERTY_TYPE.FLOAT) ? (-1f) : this.value.floatvalue;
		}

		public string stringValue()
		{
			return (this.type != USER_PROPERTY_TYPE.STRING) ? string.Empty : this.value.stringvalue;
		}

		public StringWrapper name;

		public USER_PROPERTY_TYPE type;

		private Union_IntBoolFloatString value;
	}
}
