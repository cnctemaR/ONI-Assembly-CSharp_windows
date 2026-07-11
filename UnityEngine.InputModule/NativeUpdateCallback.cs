using System;

namespace UnityEngineInternal.Input
{
	public unsafe delegate void NativeUpdateCallback(NativeInputUpdateType updateType, NativeInputEventBuffer* buffer);
}
