using System;

namespace KMod
{
	public enum EventType
	{
		LoadError,
		NotFound,
		InstallInfoInaccessible,
		OutOfOrder,
		ExpectedActive,
		ExpectedInactive,
		ActiveDuringCrash,
		InstallFailed,
		Installed,
		Uninstalled,
		CannotInstall,
		VersionUpdate,
		AvailableContentChanged,
		RestartRequested,
		BadWorldGen,
		Deactivated,
		DisabledEarlyAccess,
		DownloadFailed
	}
}
