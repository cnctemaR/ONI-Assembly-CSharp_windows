using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Camera/GPUDrivenProcessor.h")]
	internal class GPUDrivenProcessor
	{
		internal List<Mesh> scratchMeshes { get; private set; }

		internal List<Material> scratchMaterials { get; private set; }

		public GPUDrivenProcessor()
		{
			this.m_Ptr = GPUDrivenProcessor.Internal_Create();
			this.scratchMeshes = new List<Mesh>();
			this.scratchMaterials = new List<Material>();
		}

		~GPUDrivenProcessor()
		{
			this.Destroy();
		}

		public void Dispose()
		{
			this.scratchMeshes = null;
			this.scratchMaterials = null;
			this.Destroy();
			GC.SuppressFinalize(this);
		}

		private void Destroy()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				GPUDrivenProcessor.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		private unsafe void EnableGPUDrivenRenderingAndDispatchRendererData(ReadOnlySpan<EntityId> renderersID, GPUDrivenRendererDataNativeCallback callback, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback param, bool materialUpdateOnly)
		{
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<EntityId> readOnlySpan = renderersID;
			fixed (EntityId* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				GPUDrivenProcessor.EnableGPUDrivenRenderingAndDispatchRendererData_Injected(intPtr, ref managedSpanWrapper, callback, meshes, materials, param, materialUpdateOnly);
			}
		}

		public void EnableGPUDrivenRenderingAndDispatchRendererData(ReadOnlySpan<EntityId> renderersID, GPUDrivenRendererDataCallback callback, bool materialUpdateOnly = false)
		{
			this.scratchMeshes.Clear();
			this.scratchMaterials.Clear();
			this.EnableGPUDrivenRenderingAndDispatchRendererData(renderersID, GPUDrivenProcessor.s_NativeRendererCallback, this.scratchMeshes, this.scratchMaterials, callback, materialUpdateOnly);
		}

		public unsafe void DisableGPUDrivenRendering(ReadOnlySpan<EntityId> renderersID)
		{
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<EntityId> readOnlySpan = renderersID;
			fixed (EntityId* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				GPUDrivenProcessor.DisableGPUDrivenRendering_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		private unsafe void DispatchLODGroupData(ReadOnlySpan<EntityId> lodGroupID, GPUDrivenLODGroupDataNativeCallback callback, GPUDrivenLODGroupDataCallback param)
		{
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<EntityId> readOnlySpan = lodGroupID;
			fixed (EntityId* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				GPUDrivenProcessor.DispatchLODGroupData_Injected(intPtr, ref managedSpanWrapper, callback, param);
			}
		}

		public void DispatchLODGroupData(ReadOnlySpan<EntityId> lodGroupID, GPUDrivenLODGroupDataCallback callback)
		{
			Debug.Assert(UnsafeUtility.SizeOf<EntityId>() == 4, "EntityId size has changed, please fix the code.");
			this.DispatchLODGroupData(lodGroupID, GPUDrivenProcessor.s_NativeLODGroupCallback, callback);
		}

		public bool enablePartialRendering
		{
			get
			{
				IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GPUDrivenProcessor.get_enablePartialRendering_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GPUDrivenProcessor.set_enablePartialRendering_Injected(intPtr, value);
			}
		}

		public bool enableMaterialFilters
		{
			get
			{
				IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GPUDrivenProcessor.get_enableMaterialFilters_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GPUDrivenProcessor.set_enableMaterialFilters_Injected(intPtr, value);
			}
		}

		public void AddMaterialFilters([NotNull] GPUDrivenMaterialFilterEntry[] filters)
		{
			if (filters == null)
			{
				ThrowHelper.ThrowArgumentNullException(filters, "filters");
			}
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GPUDrivenProcessor.AddMaterialFilters_Injected(intPtr, filters);
		}

		public void ClearMaterialFilters()
		{
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GPUDrivenProcessor.ClearMaterialFilters_Injected(intPtr);
		}

		public int GetMaterialFilterFlags(Material material)
		{
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GPUDrivenProcessor.GetMaterialFilterFlags_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material));
		}

		[FreeFunction("GPUDrivenProcessor::ClassifyMaterials", IsThreadSafe = true)]
		private unsafe static int ClassifyMaterialsImpl(ReadOnlySpan<EntityId> materialIDs, Span<EntityId> unsupportedMaterialIDs, Span<EntityId> supportedMaterialIDs, Span<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
		{
			ReadOnlySpan<EntityId> readOnlySpan = materialIDs;
			fixed (EntityId* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<EntityId> span = unsupportedMaterialIDs;
				fixed (EntityId* ptr2 = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span.Length);
					Span<EntityId> span2 = supportedMaterialIDs;
					fixed (EntityId* ptr3 = span2.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, span2.Length);
						Span<GPUDrivenPackedMaterialData> span3 = supportedPackedMaterialDatas;
						int num;
						fixed (GPUDrivenPackedMaterialData* pinnableReference = span3.GetPinnableReference())
						{
							ManagedSpanWrapper managedSpanWrapper4 = new ManagedSpanWrapper((void*)pinnableReference, span3.Length);
							num = GPUDrivenProcessor.ClassifyMaterialsImpl_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, ref managedSpanWrapper3, ref managedSpanWrapper4);
							ptr = null;
							ptr2 = null;
							ptr3 = null;
						}
						return num;
					}
				}
			}
		}

		public static int ClassifyMaterials(NativeArray<EntityId> materialIDs, NativeArray<EntityId> unsupportedMaterialIDs, NativeArray<EntityId> supportedMaterialIDs, NativeArray<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
		{
			return GPUDrivenProcessor.ClassifyMaterialsImpl(in materialIDs, in unsupportedMaterialIDs, in supportedMaterialIDs, in supportedPackedMaterialDatas);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableGPUDrivenRenderingAndDispatchRendererData_Injected(IntPtr _unity_self, ref ManagedSpanWrapper renderersID, GPUDrivenRendererDataNativeCallback callback, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback param, bool materialUpdateOnly);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableGPUDrivenRendering_Injected(IntPtr _unity_self, ref ManagedSpanWrapper renderersID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DispatchLODGroupData_Injected(IntPtr _unity_self, ref ManagedSpanWrapper lodGroupID, GPUDrivenLODGroupDataNativeCallback callback, GPUDrivenLODGroupDataCallback param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enablePartialRendering_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enablePartialRendering_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableMaterialFilters_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableMaterialFilters_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddMaterialFilters_Injected(IntPtr _unity_self, GPUDrivenMaterialFilterEntry[] filters);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearMaterialFilters_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMaterialFilterFlags_Injected(IntPtr _unity_self, IntPtr material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ClassifyMaterialsImpl_Injected(ref ManagedSpanWrapper materialIDs, ref ManagedSpanWrapper unsupportedMaterialIDs, ref ManagedSpanWrapper supportedMaterialIDs, ref ManagedSpanWrapper supportedPackedMaterialDatas);

		internal IntPtr m_Ptr;

		private static GPUDrivenRendererDataNativeCallback s_NativeRendererCallback = delegate(in GPUDrivenRendererGroupDataNative nativeData, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback callback)
		{
			NativeArray<EntityId> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>((void*)nativeData.rendererGroupID, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<Bounds> nativeArray2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Bounds>((void*)nativeData.localBounds, (nativeData.localBounds == null) ? 0 : nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<Vector4> nativeArray3 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>((void*)nativeData.lightmapScaleOffset, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> nativeArray4 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.gameObjectLayer, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<uint> nativeArray5 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<uint>((void*)nativeData.renderingLayerMask, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<uint> nativeArray6 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<uint>((void*)nativeData.rendererUserValues, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<EntityId> nativeArray7 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>((void*)nativeData.lodGroupID, (nativeData.lodGroupID == null) ? 0 : nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> nativeArray8 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.motionVecGenMode, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<GPUDrivenPackedRendererData> nativeArray9 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenPackedRendererData>((void*)nativeData.packedRendererData, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> nativeArray10 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.rendererPriority, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> nativeArray11 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.meshIndex, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<short> nativeArray12 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>((void*)nativeData.subMeshStartIndex, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> nativeArray13 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.materialsOffset, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<short> nativeArray14 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>((void*)nativeData.materialsCount, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> nativeArray15 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);
			NativeArray<int> nativeArray16 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);
			NativeArray<GPUDrivenRendererEditorData> nativeArray17 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenRendererEditorData>((void*)nativeData.editorData, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<GPUDrivenRendererMeshLodData> nativeArray18 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenRendererMeshLodData>((void*)nativeData.meshLodData, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<EntityId> nativeArray19 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>((void*)nativeData.invalidRendererGroupID, nativeData.invalidRendererGroupIDCount, Allocator.Invalid);
			NativeArray<Matrix4x4> nativeArray20 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>((void*)nativeData.localToWorldMatrix, (nativeData.localToWorldMatrix == null) ? 0 : nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<Matrix4x4> nativeArray21 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>((void*)nativeData.prevLocalToWorldMatrix, (nativeData.prevLocalToWorldMatrix == null) ? 0 : nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> nativeArray22 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);
			NativeArray<EntityId> nativeArray23 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>((void*)nativeData.meshID, nativeData.meshCount, Allocator.Invalid);
			NativeArray<GPUDrivenMeshLodInfo> nativeArray24 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenMeshLodInfo>((void*)nativeData.meshLodInfo, nativeData.meshCount, Allocator.Invalid);
			NativeArray<short> nativeArray25 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>((void*)nativeData.subMeshCount, nativeData.meshCount, Allocator.Invalid);
			NativeArray<int> nativeArray26 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.subMeshDescOffset, nativeData.meshCount, Allocator.Invalid);
			NativeArray<SubMeshDescriptor> nativeArray27 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<SubMeshDescriptor>((void*)nativeData.subMeshDesc, nativeData.subMeshDescCount, Allocator.Invalid);
			NativeArray<int> nativeArray28 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.materialIndex, nativeData.materialIndexCount, Allocator.Invalid);
			NativeArray<EntityId> nativeArray29 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>((void*)nativeData.materialID, nativeData.materialCount, Allocator.Invalid);
			NativeArray<GPUDrivenPackedMaterialData> nativeArray30 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenPackedMaterialData>((void*)nativeData.packedMaterialData, (nativeData.packedMaterialData == null) ? 0 : nativeData.materialCount, Allocator.Invalid);
			NativeArray<int> nativeArray31 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.materialFilterFlags, (nativeData.packedMaterialData == null) ? 0 : nativeData.materialCount, Allocator.Invalid);
			GPUDrivenRendererGroupData gpudrivenRendererGroupData = new GPUDrivenRendererGroupData
			{
				rendererGroupID = nativeArray,
				localBounds = nativeArray2,
				lightmapScaleOffset = nativeArray3,
				gameObjectLayer = nativeArray4,
				renderingLayerMask = nativeArray5,
				rendererUserValues = nativeArray6,
				lodGroupID = nativeArray7,
				lightmapIndex = nativeArray8,
				packedRendererData = nativeArray9,
				rendererPriority = nativeArray10,
				meshIndex = nativeArray11,
				subMeshStartIndex = nativeArray12,
				materialsOffset = nativeArray13,
				materialsCount = nativeArray14,
				instancesOffset = nativeArray15,
				instancesCount = nativeArray16,
				editorData = nativeArray17,
				invalidRendererGroupID = nativeArray19,
				meshLodData = nativeArray18,
				localToWorldMatrix = nativeArray20,
				prevLocalToWorldMatrix = nativeArray21,
				rendererGroupIndex = nativeArray22,
				meshID = nativeArray23,
				meshLodInfo = nativeArray24,
				subMeshCount = nativeArray25,
				subMeshDescOffset = nativeArray26,
				subMeshDesc = nativeArray27,
				materialIndex = nativeArray28,
				materialID = nativeArray29,
				packedMaterialData = nativeArray30,
				materialFilterFlags = nativeArray31
			};
			callback(in gpudrivenRendererGroupData, meshes, materials);
		};

		private static GPUDrivenLODGroupDataNativeCallback s_NativeLODGroupCallback = delegate(in GPUDrivenLODGroupDataNative nativeData, GPUDrivenLODGroupDataCallback callback)
		{
			NativeArray<EntityId> nativeArray32 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>((void*)nativeData.lodGroupID, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<int> nativeArray33 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.lodOffset, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<int> nativeArray34 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.lodCount, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<LODFadeMode> nativeArray35 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<LODFadeMode>((void*)nativeData.fadeMode, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<Vector3> nativeArray36 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>((void*)nativeData.worldSpaceReferencePoint, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<float> nativeArray37 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>((void*)nativeData.worldSpaceSize, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<short> nativeArray38 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>((void*)nativeData.renderersCount, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<bool> nativeArray39 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<bool>((void*)nativeData.lastLODIsBillboard, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<byte> nativeArray40 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)nativeData.forceLODMask, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<EntityId> nativeArray41 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>((void*)nativeData.invalidLODGroupID, nativeData.invalidLODGroupCount, Allocator.Invalid);
			NativeArray<short> nativeArray42 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>((void*)nativeData.lodRenderersCount, nativeData.lodDataCount, Allocator.Invalid);
			NativeArray<float> nativeArray43 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>((void*)nativeData.lodScreenRelativeTransitionHeight, nativeData.lodDataCount, Allocator.Invalid);
			NativeArray<float> nativeArray44 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>((void*)nativeData.lodFadeTransitionWidth, nativeData.lodDataCount, Allocator.Invalid);
			GPUDrivenLODGroupData gpudrivenLODGroupData = new GPUDrivenLODGroupData
			{
				lodGroupID = nativeArray32,
				lodOffset = nativeArray33,
				lodCount = nativeArray34,
				fadeMode = nativeArray35,
				worldSpaceReferencePoint = nativeArray36,
				worldSpaceSize = nativeArray37,
				renderersCount = nativeArray38,
				lastLODIsBillboard = nativeArray39,
				forceLODMask = nativeArray40,
				invalidLODGroupID = nativeArray41,
				lodRenderersCount = nativeArray42,
				lodScreenRelativeTransitionHeight = nativeArray43,
				lodFadeTransitionWidth = nativeArray44
			};
			callback(in gpudrivenLODGroupData);
		};

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(GPUDrivenProcessor obj)
			{
				return obj.m_Ptr;
			}
		}
	}
}
