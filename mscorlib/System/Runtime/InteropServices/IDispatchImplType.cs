using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Obsolete("The IDispatchImplAttribute is deprecated.", false)]
	[Serializable]
	public enum IDispatchImplType
	{
		SystemDefinedImpl,
		InternalImpl,
		CompatibleImpl
	}
}
