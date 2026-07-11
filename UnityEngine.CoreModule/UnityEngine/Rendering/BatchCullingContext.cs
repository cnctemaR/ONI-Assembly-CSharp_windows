using System;
using Unity.Collections;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
	[UsedByNativeCode]
	public struct BatchCullingContext
	{
		public BatchCullingContext(NativeArray<Plane> inCullingPlanes, NativeArray<BatchVisibility> inOutBatchVisibility, NativeArray<int> outVisibleIndices, LODParameters inLodParameters)
		{
			this.cullingPlanes = inCullingPlanes;
			this.batchVisibility = inOutBatchVisibility;
			this.visibleIndices = outVisibleIndices;
			this.lodParameters = inLodParameters;
		}

		public readonly NativeArray<Plane> cullingPlanes;

		public NativeArray<BatchVisibility> batchVisibility;

		public NativeArray<int> visibleIndices;

		public readonly LODParameters lodParameters;
	}
}
