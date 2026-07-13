using System;

namespace UnityEngine.Tilemaps
{
	[Flags]
	public enum TileAnimationFlags
	{
		None = 0,
		LoopOnce = 1,
		PauseAnimation = 2,
		UpdatePhysics = 4,
		UnscaledTime = 8,
		SyncAnimation = 16
	}
}
