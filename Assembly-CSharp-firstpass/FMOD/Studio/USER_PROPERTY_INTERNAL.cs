using System;

namespace FMOD.Studio
{
	internal struct USER_PROPERTY_INTERNAL
	{
		public USER_PROPERTY createPublic()
		{
			USER_PROPERTY user_PROPERTY = default(USER_PROPERTY);
			user_PROPERTY.name = MarshallingHelper.stringFromNativeUtf8(this.name);
			user_PROPERTY.type = this.type;
			switch (this.type)
			{
			case USER_PROPERTY_TYPE.INTEGER:
				user_PROPERTY.intValue = this.value.intValue;
				break;
			case USER_PROPERTY_TYPE.BOOLEAN:
				user_PROPERTY.boolValue = this.value.boolValue;
				break;
			case USER_PROPERTY_TYPE.FLOAT:
				user_PROPERTY.floatValue = this.value.floatValue;
				break;
			case USER_PROPERTY_TYPE.STRING:
				user_PROPERTY.stringValue = MarshallingHelper.stringFromNativeUtf8(this.value.stringValue);
				break;
			}
			return user_PROPERTY;
		}

		private IntPtr name;

		private USER_PROPERTY_TYPE type;

		private Union_IntBoolFloatString value;
	}
}
