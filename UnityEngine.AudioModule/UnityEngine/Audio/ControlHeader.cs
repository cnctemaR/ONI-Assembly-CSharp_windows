using System;
using Unity.Audio;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ControlHeader.h")]
	internal struct ControlHeader
	{
		internal Handle Handle;

		internal IntPtr ManagedTransport;
	}
}
