using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Playables
{
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	[StaticAccessor("DataPlayableBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
	[NativeHeader("Modules/Director/ScriptBindings/DataPlayable.bindings.h")]
	internal static class DataPlayableBindings
	{
		[NativeThrows]
		public static bool CreateHandleInternal(PlayableGraph graph, ref PlayableHandle handle)
		{
			return DataPlayableBindings.CreateHandleInternal_Injected(ref graph, ref handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected([In] ref PlayableGraph graph, ref PlayableHandle handle);
	}
}
