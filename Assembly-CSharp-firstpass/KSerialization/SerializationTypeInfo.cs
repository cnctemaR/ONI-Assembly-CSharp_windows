using System;

namespace KSerialization
{
	public enum SerializationTypeInfo : byte
	{
		UserDefined,
		SByte,
		Byte,
		Boolean,
		Int16,
		UInt16,
		Int32,
		UInt32,
		Int64,
		UInt64,
		Single,
		Double,
		String,
		Enumeration,
		Vector2I,
		Vector2,
		Vector3,
		Array,
		Pair,
		Dictionary,
		List,
		HashSet,
		Colour,
		IS_GENERIC_TYPE = 128,
		IS_VALUE_TYPE = 64,
		VALUE_MASK = 63
	}
}
