using System;
using UnityEngine.Scripting;

namespace Unity.Collections
{
	[UsedByNativeCode]
	public enum NativeLeakDetectionMode
	{
		Disabled = 1,
		Enabled,
		EnabledWithStackTrace
	}
}
