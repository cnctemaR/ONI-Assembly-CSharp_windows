using System;
using UnityEngine.Bindings;

namespace Unity.Profiling
{
	[NativeHeader("Runtime/Profiler/ScriptBindings/ProfilerMarker.bindings.h")]
	[Flags]
	internal enum MarkerFlags
	{
		Default = 0,
		AvailabilityEditor = 4,
		AvailabilityNonDevelopment = 8,
		Warning = 16,
		VerbosityDebug = 1024,
		VerbosityInternal = 2048,
		VerbosityAdvanced = 4096
	}
}
