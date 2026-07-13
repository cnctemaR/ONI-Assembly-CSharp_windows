using System;

namespace UnityEngine.Rendering
{
	public struct BatchDrawCommand
	{
		public BatchDrawCommandFlags flags;

		public BatchID batchID;

		public BatchMaterialID materialID;

		public ushort splitVisibilityMask;

		public ushort lightmapIndex;

		public int sortingPosition;

		public uint visibleOffset;

		public uint visibleCount;

		public BatchMeshID meshID;

		public ushort submeshIndex;

		public ushort activeMeshLod;
	}
}
