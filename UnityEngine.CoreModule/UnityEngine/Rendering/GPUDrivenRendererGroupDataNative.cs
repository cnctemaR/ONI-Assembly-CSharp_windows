using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	internal struct GPUDrivenRendererGroupDataNative
	{
		public unsafe EntityId* rendererGroupID;

		public unsafe Bounds* localBounds;

		public unsafe Vector4* lightmapScaleOffset;

		public unsafe int* gameObjectLayer;

		public unsafe uint* renderingLayerMask;

		public unsafe uint* rendererUserValues;

		public unsafe EntityId* lodGroupID;

		public unsafe MotionVectorGenerationMode* motionVecGenMode;

		public unsafe GPUDrivenPackedRendererData* packedRendererData;

		public unsafe int* rendererPriority;

		public unsafe int* meshIndex;

		public unsafe short* subMeshStartIndex;

		public unsafe int* materialsOffset;

		public unsafe short* materialsCount;

		public unsafe int* instancesOffset;

		public unsafe int* instancesCount;

		public unsafe GPUDrivenRendererEditorData* editorData;

		public int rendererGroupCount;

		public unsafe EntityId* invalidRendererGroupID;

		public int invalidRendererGroupIDCount;

		public unsafe GPUDrivenRendererMeshLodData* meshLodData;

		public unsafe Matrix4x4* localToWorldMatrix;

		public unsafe Matrix4x4* prevLocalToWorldMatrix;

		public unsafe int* rendererGroupIndex;

		public int instanceCount;

		public unsafe EntityId* meshID;

		public unsafe GPUDrivenMeshLodInfo* meshLodInfo;

		public unsafe short* subMeshCount;

		public unsafe int* subMeshDescOffset;

		public int meshCount;

		public unsafe SubMeshDescriptor* subMeshDesc;

		public int subMeshDescCount;

		public unsafe int* materialIndex;

		public int materialIndexCount;

		public unsafe EntityId* materialID;

		public unsafe GPUDrivenPackedMaterialData* packedMaterialData;

		public unsafe int* materialFilterFlags;

		public int materialCount;
	}
}
