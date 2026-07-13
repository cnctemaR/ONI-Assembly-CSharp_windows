using System;

namespace UnityEngine.Android
{
	public enum ExitReason
	{
		Unknown,
		ExitSelf,
		Signaled,
		LowMemory,
		Crash,
		CrashNative,
		ANR,
		InititalizationFailure,
		PermissionChange,
		ExcessiveResourceUsage,
		UserRequested,
		UserStopped,
		DependencyDied,
		Other,
		Freezer,
		PackageStateChange,
		PackageUpdated
	}
}
