using System;

namespace UnityEngine.UIElements.UIR
{
	internal class MeshHandle : LinkedPoolItem<MeshHandle>
	{
		internal Alloc allocVerts;

		internal Alloc allocIndices;

		internal uint triangleCount;

		internal Page allocPage;

		internal uint allocTime;

		internal uint updateAllocID;
	}
}
