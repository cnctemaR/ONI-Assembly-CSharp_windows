using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Obsolete]
	[Serializable]
	public enum IDispatchImplType
	{
		SystemDefinedImpl,
		InternalImpl,
		CompatibleImpl
	}
}
