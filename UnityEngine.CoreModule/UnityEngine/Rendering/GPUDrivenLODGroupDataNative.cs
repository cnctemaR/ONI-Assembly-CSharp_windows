using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	internal struct GPUDrivenLODGroupDataNative
	{
		public unsafe EntityId* lodGroupID;

		public unsafe int* lodOffset;

		public unsafe int* lodCount;

		public unsafe LODFadeMode* fadeMode;

		public unsafe Vector3* worldSpaceReferencePoint;

		public unsafe float* worldSpaceSize;

		public unsafe short* renderersCount;

		public unsafe bool* lastLODIsBillboard;

		public unsafe byte* forceLODMask;

		public int lodGroupCount;

		public unsafe EntityId* invalidLODGroupID;

		public int invalidLODGroupCount;

		public unsafe short* lodRenderersCount;

		public unsafe float* lodScreenRelativeTransitionHeight;

		public unsafe float* lodFadeTransitionWidth;

		public int lodDataCount;
	}
}
