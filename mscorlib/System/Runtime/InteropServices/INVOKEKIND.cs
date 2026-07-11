using System;

namespace System.Runtime.InteropServices
{
	[Obsolete]
	[Serializable]
	public enum INVOKEKIND
	{
		INVOKE_FUNC = 1,
		INVOKE_PROPERTYGET,
		INVOKE_PROPERTYPUT = 4,
		INVOKE_PROPERTYPUTREF = 8
	}
}
