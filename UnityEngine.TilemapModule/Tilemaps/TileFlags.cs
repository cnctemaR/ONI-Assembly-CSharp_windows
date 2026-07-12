using System;

namespace UnityEngine.Tilemaps
{
	[Flags]
	public enum TileFlags
	{
		None = 0,
		LockColor = 1,
		LockTransform = 2,
		InstantiateGameObjectRuntimeOnly = 4,
		KeepGameObjectRuntimeOnly = 8,
		LockAll = 3
	}
}
