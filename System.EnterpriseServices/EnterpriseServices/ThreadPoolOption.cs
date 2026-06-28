using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[Serializable]
	public enum ThreadPoolOption
	{
		None,
		Inherit,
		STA,
		MTA
	}
}
