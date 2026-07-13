using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Internal
{
	[NativeHeader("Runtime/GfxDevice/HDROutputSettings.h")]
	[ExcludeFromDocs]
	internal static class InternalHDROutputFaking
	{
		[FreeFunction("HDROutputSettingsBindings::SetFakeHDROutputEnabled")]
		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEnabled(bool enabled);
	}
}
