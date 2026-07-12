using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Internal
{
	[ExcludeFromDocs]
	[NativeHeader("Runtime/GfxDevice/HDROutputSettings.h")]
	internal static class InternalHDROutputFaking
	{
		[FreeFunction("HDROutputSettingsBindings::SetFakeHDROutputEnabled")]
		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEnabled(bool enabled);
	}
}
