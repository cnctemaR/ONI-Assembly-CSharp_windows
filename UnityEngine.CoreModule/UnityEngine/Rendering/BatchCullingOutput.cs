using System;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	public struct BatchCullingOutput
	{
		public NativeArray<BatchCullingOutputDrawCommands> drawCommands;

		public NativeArray<IntPtr> customCullingResult;
	}
}
