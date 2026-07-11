using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	[RequiredByNativeCode]
	internal class ScriptableRuntimeReflectionSystemWrapper
	{
		internal IScriptableRuntimeReflectionSystem implementation { get; set; }

		[RequiredByNativeCode]
		private unsafe void Internal_ScriptableRuntimeReflectionSystemWrapper_TickRealtimeProbes(IntPtr result)
		{
			*(byte*)(void*)result = ((this.implementation != null && this.implementation.TickRealtimeProbes()) ? 1 : 0);
		}
	}
}
