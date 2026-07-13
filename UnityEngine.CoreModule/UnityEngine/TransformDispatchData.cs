using System;
using Unity.Collections;

namespace UnityEngine
{
	internal struct TransformDispatchData : IDisposable
	{
		public void Dispose()
		{
			this.transformedID.Dispose();
			this.parentID.Dispose();
			this.localToWorldMatrices.Dispose();
			this.positions.Dispose();
			this.rotations.Dispose();
			this.scales.Dispose();
		}

		public NativeArray<EntityId> transformedID;

		public NativeArray<EntityId> parentID;

		public NativeArray<Matrix4x4> localToWorldMatrices;

		public NativeArray<Vector3> positions;

		public NativeArray<Quaternion> rotations;

		public NativeArray<Vector3> scales;
	}
}
