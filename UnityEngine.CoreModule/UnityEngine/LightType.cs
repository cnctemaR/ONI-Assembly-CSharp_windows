using System;

namespace UnityEngine
{
	public enum LightType
	{
		Spot,
		Directional,
		Point,
		[Obsolete("Enum member LightType.Area has been deprecated. Use LightType.Rectangle instead (UnityUpgradable) -> Rectangle", true)]
		Area,
		Rectangle = 3,
		Disc,
		Pyramid,
		Box,
		Tube
	}
}
