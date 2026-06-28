using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum FieldAttributes
	{
		FieldAccessMask = 7,
		PrivateScope = 0,
		Private = 1,
		FamANDAssem = 2,
		Assembly = 3,
		Family = 4,
		FamORAssem = 5,
		Public = 6,
		Static = 16,
		InitOnly = 32,
		Literal = 64,
		NotSerialized = 128,
		HasFieldRVA = 256,
		SpecialName = 512,
		RTSpecialName = 1024,
		HasFieldMarshal = 4096,
		PinvokeImpl = 8192,
		HasDefault = 32768,
		ReservedMask = 38144
	}
}
