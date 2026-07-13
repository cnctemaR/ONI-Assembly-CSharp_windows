using System;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	internal struct GPUDrivenLODGroupData
	{
		public NativeArray<EntityId> lodGroupID;

		public NativeArray<int> lodOffset;

		public NativeArray<int> lodCount;

		public NativeArray<LODFadeMode> fadeMode;

		public NativeArray<Vector3> worldSpaceReferencePoint;

		public NativeArray<float> worldSpaceSize;

		public NativeArray<short> renderersCount;

		public NativeArray<bool> lastLODIsBillboard;

		public NativeArray<byte> forceLODMask;

		public NativeArray<EntityId> invalidLODGroupID;

		public NativeArray<short> lodRenderersCount;

		public NativeArray<float> lodScreenRelativeTransitionHeight;

		public NativeArray<float> lodFadeTransitionWidth;
	}
}
