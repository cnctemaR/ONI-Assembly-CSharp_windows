using System;

namespace FMOD.Studio
{
	public struct USER_PROPERTY
	{
		public int intValue()
		{
			if (this.type != USER_PROPERTY_TYPE.INTEGER)
			{
				return -1;
			}
			return this.value.intvalue;
		}

		public bool boolValue()
		{
			return this.type == USER_PROPERTY_TYPE.BOOLEAN && this.value.boolvalue;
		}

		public float floatValue()
		{
			if (this.type != USER_PROPERTY_TYPE.FLOAT)
			{
				return -1f;
			}
			return this.value.floatvalue;
		}

		public string stringValue()
		{
			if (this.type != USER_PROPERTY_TYPE.STRING)
			{
				return "";
			}
			return this.value.stringvalue;
		}

		public StringWrapper name;

		public USER_PROPERTY_TYPE type;

		private Union_IntBoolFloatString value;
	}
}
