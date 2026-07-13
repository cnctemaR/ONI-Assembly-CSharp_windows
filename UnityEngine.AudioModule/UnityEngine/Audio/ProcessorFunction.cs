using System;

namespace UnityEngine.Audio
{
	internal enum ProcessorFunction : uint
	{
		Process = 1U,
		Update,
		OutputProcessEarly,
		OutputProcess,
		OutputProcessEnd,
		OutputRemoved
	}
}
