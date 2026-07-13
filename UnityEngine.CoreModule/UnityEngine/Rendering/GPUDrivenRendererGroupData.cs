using System;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	internal struct GPUDrivenRendererGroupData
	{
		public NativeArray<EntityId> rendererGroupID;

		public NativeArray<Bounds> localBounds;

		public NativeArray<Vector4> lightmapScaleOffset;

		public NativeArray<int> gameObjectLayer;

		public NativeArray<uint> renderingLayerMask;

		public NativeArray<uint> rendererUserValues;

		public NativeArray<EntityId> lodGroupID;

		public NativeArray<int> lightmapIndex;

		public NativeArray<GPUDrivenPackedRendererData> packedRendererData;

		public NativeArray<int> rendererPriority;

		public NativeArray<int> meshIndex;

		public NativeArray<short> subMeshStartIndex;

		public NativeArray<int> materialsOffset;

		public NativeArray<short> materialsCount;

		public NativeArray<int> instancesOffset;

		public NativeArray<int> instancesCount;

		public NativeArray<GPUDrivenRendererEditorData> editorData;

		public NativeArray<GPUDrivenRendererMeshLodData> meshLodData;

		public NativeArray<EntityId> invalidRendererGroupID;

		public NativeArray<Matrix4x4> localToWorldMatrix;

		public NativeArray<Matrix4x4> prevLocalToWorldMatrix;

		public NativeArray<int> rendererGroupIndex;

		public NativeArray<EntityId> meshID;

		public NativeArray<GPUDrivenMeshLodInfo> meshLodInfo;

		public NativeArray<short> subMeshCount;

		public NativeArray<int> subMeshDescOffset;

		public NativeArray<SubMeshDescriptor> subMeshDesc;

		public NativeArray<int> materialIndex;

		public NativeArray<EntityId> materialID;

		public NativeArray<GPUDrivenPackedMaterialData> packedMaterialData;

		public NativeArray<int> materialFilterFlags;
	}
}
