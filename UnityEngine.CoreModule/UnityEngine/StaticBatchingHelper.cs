using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Mesh/StaticBatching.h")]
	internal struct StaticBatchingHelper
	{
		[FreeFunction("StaticBatching::CombineMeshesForStaticBatching")]
		internal static void CombineMeshes(GameObject[] gos, GameObject staticBatchRoot)
		{
			StaticBatchingHelper.CombineMeshes_Injected(gos, Object.MarshalledUnityObject.Marshal<GameObject>(staticBatchRoot));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CombineMeshes_Injected(GameObject[] gos, IntPtr staticBatchRoot);
	}
}
