using System;

namespace UnityEngine
{
	public enum ScreenOrientation
	{
		Portrait = 1,
		PortraitUpsideDown,
		LandscapeLeft,
		LandscapeRight,
		AutoRotation,
		[Obsolete("Enum member Unknown has been deprecated.", false)]
		Unknown = 0,
		[Obsolete("Use LandscapeLeft instead (UnityUpgradable) -> LandscapeLeft", true)]
		Landscape = 3
	}
}
