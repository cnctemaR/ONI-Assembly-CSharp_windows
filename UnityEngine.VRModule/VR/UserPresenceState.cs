using System;
using System.ComponentModel;

namespace UnityEngine.VR
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("UserPresenceState has been moved.  Use UnityEngine.XR.UserPresenceState instead (UnityUpgradable) -> UnityEngine.XR.UserPresenceState", true)]
	public enum UserPresenceState
	{
		Unsupported = -1,
		NotPresent,
		Present,
		Unknown
	}
}
