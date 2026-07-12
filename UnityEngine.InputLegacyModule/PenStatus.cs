using System;

namespace UnityEngine
{
	[Flags]
	public enum PenStatus
	{
		None = 0,
		Contact = 1,
		Barrel = 2,
		Inverted = 4,
		Eraser = 8
	}
}
