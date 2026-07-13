using System;
using Unity.Audio;

namespace UnityEngine.Audio
{
	internal struct ProcessorRealtimeUpdateArguments
	{
		internal readonly RealtimeAccess Access;

		internal unsafe readonly ProcessorInstance.AvailableData.Element* Head;

		internal readonly Handle Self;
	}
}
