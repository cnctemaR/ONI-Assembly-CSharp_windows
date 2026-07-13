using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Collections
{
	[UsedByNativeCode]
	[VisibleToOtherModules(new string[] { "UnityEngine.AIModule" })]
	internal enum LeakCategory
	{
		Invalid,
		Malloc,
		TempJob,
		Persistent,
		LightProbesQuery,
		NativeTest,
		MeshDataArray,
		TransformAccessArray,
		NavMeshQuery
	}
}
