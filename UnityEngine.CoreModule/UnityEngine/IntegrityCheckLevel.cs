using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Diagnostics/IntegrityCheck.h")]
	public enum IntegrityCheckLevel
	{
		Low = 1,
		Medium,
		High
	}
}
