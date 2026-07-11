using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public enum LoaderOptimization
	{
		NotSpecified,
		SingleDomain,
		MultiDomain,
		MultiDomainHost,
		[Obsolete]
		DomainMask = 3,
		[Obsolete]
		DisallowBindings
	}
}
