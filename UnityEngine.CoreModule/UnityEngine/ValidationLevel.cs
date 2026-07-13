using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Diagnostics/Validation.h")]
	public enum ValidationLevel
	{
		None,
		Low,
		Medium,
		High
	}
}
