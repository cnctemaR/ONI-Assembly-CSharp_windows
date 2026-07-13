using System;
using Unity.Audio;

namespace UnityEngine.Audio
{
	internal struct DisposeArguments
	{
		internal unsafe ControlHeader* ControlContext;

		internal Handle Self;
	}
}
