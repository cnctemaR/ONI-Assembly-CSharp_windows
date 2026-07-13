using System;
using Unity.Audio;

namespace UnityEngine.Audio
{
	internal struct MessageArguments
	{
		internal unsafe ControlHeader* Context;

		internal unsafe ProcessorInstance.Message* MessageData;

		internal Handle Self;

		internal ProcessorInstance.Response StatusReturn;
	}
}
