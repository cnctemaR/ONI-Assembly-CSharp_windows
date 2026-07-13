using System;

namespace UnityEngine.Rendering
{
	public struct BatchDrawCommandProcedural
	{
		public BatchDrawCommandFlags flags;

		public BatchID batchID;

		public BatchMaterialID materialID;

		public ushort splitVisibilityMask;

		public ushort lightmapIndex;

		public int sortingPosition;

		public uint visibleOffset;

		public uint visibleCount;

		public MeshTopology topology;

		public GraphicsBufferHandle indexBufferHandle;

		public uint baseVertex;

		public uint indexOffsetBytes;

		public uint elementCount;
	}
}
