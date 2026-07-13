using System;
using Unity.Audio;

namespace UnityEngine.Audio
{
	internal struct UpdateArguments
	{
		internal unsafe ControlHeader* ControlContext;

		internal unsafe ProcessorInstance.AvailableData.Element* FirstElement;

		internal Handle Self;
	}
}
