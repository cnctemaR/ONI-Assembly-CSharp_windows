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
		internal BatchCullingContext(NativeArray<Plane> inCullingPlanes, NativeArray<CullingSplit> inCullingSplits, LODParameters inLodParameters, Matrix4x4 inLocalToWorldMatrix, BatchCullingViewType inViewType, BatchCullingProjectionType inProjectionType, BatchCullingFlags inBatchCullingFlags, ulong inViewID, uint inCullingLayerMask, ulong inSceneCullingMask, int inReceiverPlaneOffset, int inReceiverPlaneCount)
		{
			this.cullingPlanes = inCullingPlanes;
			this.cullingSplits = inCullingSplits;
			this.lodParameters = inLodParameters;
			this.localToWorldMatrix = inLocalToWorldMatrix;
			this.viewType = inViewType;
			this.projectionType = inProjectionType;
			this.cullingFlags = inBatchCullingFlags;
			this.viewID = new BatchPackedCullingViewID
			{
				handle = inViewID
			};
			this.cullingLayerMask = inCullingLayerMask;
			this.sceneCullingMask = inSceneCullingMask;
			this.receiverPlaneOffset = inReceiverPlaneOffset;
			this.receiverPlaneCount = inReceiverPlaneCount;
			this.isOrthographic = 0;
		}

		public readonly NativeArray<Plane> cullingPlanes;

		public readonly NativeArray<CullingSplit> cullingSplits;

		public readonly LODParameters lodParameters;

		public readonly Matrix4x4 localToWorldMatrix;

		public readonly BatchCullingViewType viewType;

		public readonly BatchCullingProjectionType projectionType;

		public readonly BatchCullingFlags cullingFlags;

		public readonly BatchPackedCullingViewID viewID;

		public readonly uint cullingLayerMask;

		public readonly ulong sceneCullingMask;

		[Obsolete("BatchCullingContext.isOrthographic is deprecated. Use BatchCullingContext.projectionType instead.")]
		public readonly byte isOrthographic;

		public readonly int receiverPlaneOffset;

		public readonly int receiverPlaneCount;
	}
}
