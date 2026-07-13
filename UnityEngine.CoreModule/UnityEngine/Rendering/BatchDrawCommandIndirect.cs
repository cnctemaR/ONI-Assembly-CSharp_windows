using System;

namespace UnityEngine.Rendering
{
	public struct BatchDrawCommandIndirect
	{
		public BatchDrawCommandFlags flags;

		public BatchID batchID;

		public BatchMaterialID materialID;

		public ushort splitVisibilityMask;

		public ushort lightmapIndex;

		public int sortingPosition;

		public uint visibleOffset;

		public BatchMeshID meshID;

		public MeshTopology topology;

		public GraphicsBufferHandle visibleInstancesBufferHandle;

		public uint visibleInstancesBufferWindowOffset;

		public uint visibleInstancesBufferWindowSizeBytes;

		public GraphicsBufferHandle indirectArgsBufferHandle;

		public uint indirectArgsBufferOffset;
	}
}
