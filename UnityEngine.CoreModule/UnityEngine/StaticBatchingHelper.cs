using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Mesh/StaticBatching.h")]
	internal struct StaticBatchingHelper
	{
		[FreeFunction("StaticBatching::CombineMeshesForStaticBatching")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CombineMeshes(GameObject[] gos, GameObject staticBatchRoot);
	}
}
