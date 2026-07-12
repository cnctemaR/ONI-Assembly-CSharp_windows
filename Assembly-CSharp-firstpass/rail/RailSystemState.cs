using System;

namespace rail
{
	public enum RailSystemState
	{
		kSystemStateUnknown,
		kSystemStatePlatformOnline,
		kSystemStatePlatformOffline,
		kSystemStatePlatformExit,
		kSystemStatePlayerOwnershipExpired = 20,
		kSystemStatePlayerOwnershipActivated,
		kSystemStatePlayerOwnershipBanned,
		kSystemStateGameExitByAntiAddiction = 40
	}
}
