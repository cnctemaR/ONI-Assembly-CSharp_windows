using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public enum SecurityAction
	{
		Demand = 2,
		Assert,
		Deny,
		PermitOnly,
		LinkDemand,
		InheritanceDemand,
		RequestMinimum,
		RequestOptional,
		RequestRefuse
	}
}
