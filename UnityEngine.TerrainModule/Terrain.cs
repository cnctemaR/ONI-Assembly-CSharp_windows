using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Interfaces/ITerrainManager.h")]
	[NativeHeader("TerrainScriptingClasses.h")]
	[StaticAccessor("GetITerrainManager()", StaticAccessorType.Arrow)]
	[NativeHeader("Modules/Terrain/Public/Terrain.h")]
	public sealed class Terrain : Behaviour
	{
		public TerrainData terrainData
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<TerrainData>(Terrain.get_terrainData_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_terrainData_Injected(intPtr, Object.MarshalledUnityObject.Marshal<TerrainData>(value));
			}
		}

		public float treeDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_treeDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_treeDistance_Injected(intPtr, value);
			}
		}

		public float treeBillboardDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_treeBillboardDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_treeBillboardDistance_Injected(intPtr, value);
			}
		}

		public float treeCrossFadeLength
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_treeCrossFadeLength_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_treeCrossFadeLength_Injected(intPtr, value);
			}
		}

		public int treeMaximumFullLODCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_treeMaximumFullLODCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_treeMaximumFullLODCount_Injected(intPtr, value);
			}
		}

		public float detailObjectDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_detailObjectDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_detailObjectDistance_Injected(intPtr, value);
			}
		}

		public float detailObjectDensity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_detailObjectDensity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_detailObjectDensity_Injected(intPtr, value);
			}
		}

		public float heightmapPixelError
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_heightmapPixelError_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_heightmapPixelError_Injected(intPtr, value);
			}
		}

		public int heightmapMaximumLOD
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_heightmapMaximumLOD_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_heightmapMaximumLOD_Injected(intPtr, value);
			}
		}

		public int heightmapMinimumLODSimplification
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_heightmapMinimumLODSimplification_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_heightmapMinimumLODSimplification_Injected(intPtr, value);
			}
		}

		public float basemapDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_basemapDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_basemapDistance_Injected(intPtr, value);
			}
		}

		[NativeProperty("StaticLightmapIndexInt")]
		public int lightmapIndex
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_lightmapIndex_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_lightmapIndex_Injected(intPtr, value);
			}
		}

		[NativeProperty("DynamicLightmapIndexInt")]
		public int realtimeLightmapIndex
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_realtimeLightmapIndex_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_realtimeLightmapIndex_Injected(intPtr, value);
			}
		}

		[NativeProperty("StaticLightmapST")]
		public Vector4 lightmapScaleOffset
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector4 vector;
				Terrain.get_lightmapScaleOffset_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_lightmapScaleOffset_Injected(intPtr, ref value);
			}
		}

		[NativeProperty("DynamicLightmapST")]
		public Vector4 realtimeLightmapScaleOffset
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector4 vector;
				Terrain.get_realtimeLightmapScaleOffset_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_realtimeLightmapScaleOffset_Injected(intPtr, ref value);
			}
		}

		[NativeProperty("FreeUnusedRenderingResourcesObsolete")]
		[Obsolete("Terrain.freeUnusedRenderingResources is obsolete; use keepUnusedRenderingResources instead.")]
		public bool freeUnusedRenderingResources
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_freeUnusedRenderingResources_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_freeUnusedRenderingResources_Injected(intPtr, value);
			}
		}

		[NativeProperty("KeepUnusedRenderingResources")]
		public bool keepUnusedRenderingResources
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_keepUnusedRenderingResources_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_keepUnusedRenderingResources_Injected(intPtr, value);
			}
		}

		public bool GetKeepUnusedCameraRenderingResources(EntityId cameraEntityId)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Terrain.GetKeepUnusedCameraRenderingResources_Injected(intPtr, ref cameraEntityId);
		}

		public void SetKeepUnusedCameraRenderingResources(EntityId cameraEntityId, bool keepUnused)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Terrain.SetKeepUnusedCameraRenderingResources_Injected(intPtr, ref cameraEntityId, keepUnused);
		}

		[Obsolete("GetKeepUnusedCameraRenderingResources(int) is obsolete. Use GetKeepUnusedCameraRenderingResources(EntityId) instead.")]
		public bool GetKeepUnusedCameraRenderingResources(int cameraInstanceID)
		{
			return this.GetKeepUnusedCameraRenderingResources(cameraInstanceID);
		}

		[Obsolete("SetKeepUnusedCameraRenderingResources(int, bool) is obsolete. Use SetKeepUnusedCameraRenderingResources(EntityId, bool) instead.")]
		public void SetKeepUnusedCameraRenderingResources(int cameraInstanceID, bool keepUnused)
		{
			this.SetKeepUnusedCameraRenderingResources(cameraInstanceID, keepUnused);
		}

		public ShadowCastingMode shadowCastingMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_shadowCastingMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_shadowCastingMode_Injected(intPtr, value);
			}
		}

		public ReflectionProbeUsage reflectionProbeUsage
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_reflectionProbeUsage_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_reflectionProbeUsage_Injected(intPtr, value);
			}
		}

		public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Terrain.GetClosestReflectionProbes_Injected(intPtr, result);
		}

		public Material materialTemplate
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Material>(Terrain.get_materialTemplate_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_materialTemplate_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(value));
			}
		}

		public bool drawHeightmap
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_drawHeightmap_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_drawHeightmap_Injected(intPtr, value);
			}
		}

		public bool allowAutoConnect
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_allowAutoConnect_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_allowAutoConnect_Injected(intPtr, value);
			}
		}

		public int groupingID
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_groupingID_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_groupingID_Injected(intPtr, value);
			}
		}

		public bool drawInstanced
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_drawInstanced_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_drawInstanced_Injected(intPtr, value);
			}
		}

		public bool enableHeightmapRayTracing
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_enableHeightmapRayTracing_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_enableHeightmapRayTracing_Injected(intPtr, value);
			}
		}

		public RenderTexture normalmapTexture
		{
			[NativeMethod("TryGetNormalMapTexture")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<RenderTexture>(Terrain.get_normalmapTexture_Injected(intPtr));
			}
		}

		public bool drawTreesAndFoliage
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_drawTreesAndFoliage_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_drawTreesAndFoliage_Injected(intPtr, value);
			}
		}

		public Vector3 patchBoundsMultiplier
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Terrain.get_patchBoundsMultiplier_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_patchBoundsMultiplier_Injected(intPtr, ref value);
			}
		}

		public float SampleHeight(Vector3 worldPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Terrain.SampleHeight_Injected(intPtr, ref worldPosition);
		}

		public void AddTreeInstance(TreeInstance instance)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Terrain.AddTreeInstance_Injected(intPtr, ref instance);
		}

		public void SetNeighbors(Terrain left, Terrain top, Terrain right, Terrain bottom)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Terrain.SetNeighbors_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Terrain>(left), Object.MarshalledUnityObject.Marshal<Terrain>(top), Object.MarshalledUnityObject.Marshal<Terrain>(right), Object.MarshalledUnityObject.Marshal<Terrain>(bottom));
		}

		public float treeLODBiasMultiplier
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_treeLODBiasMultiplier_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_treeLODBiasMultiplier_Injected(intPtr, value);
			}
		}

		public bool collectDetailPatches
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_collectDetailPatches_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_collectDetailPatches_Injected(intPtr, value);
			}
		}

		public bool ignoreQualitySettings
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_ignoreQualitySettings_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_ignoreQualitySettings_Injected(intPtr, value);
			}
		}

		public TerrainRenderFlags editorRenderFlags
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_editorRenderFlags_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_editorRenderFlags_Injected(intPtr, value);
			}
		}

		public Vector3 GetPosition()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Terrain.GetPosition_Injected(intPtr, out vector);
			return vector;
		}

		public void Flush()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Terrain.Flush_Injected(intPtr);
		}

		internal void RemoveTrees(Vector2 position, float radius, int prototypeIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Terrain.RemoveTrees_Injected(intPtr, ref position, radius, prototypeIndex);
		}

		[NativeMethod("CopySplatMaterialCustomProps")]
		public void SetSplatMaterialPropertyBlock(MaterialPropertyBlock properties)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Terrain.SetSplatMaterialPropertyBlock_Injected(intPtr, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		public void GetSplatMaterialPropertyBlock(MaterialPropertyBlock dest)
		{
			bool flag = dest == null;
			if (flag)
			{
				throw new ArgumentNullException("dest");
			}
			this.Internal_GetSplatMaterialPropertyBlock(dest);
		}

		[NativeMethod("GetSplatMaterialCustomProps")]
		private void Internal_GetSplatMaterialPropertyBlock(MaterialPropertyBlock dest)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Terrain.Internal_GetSplatMaterialPropertyBlock_Injected(intPtr, (dest == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(dest));
		}

		public TreeMotionVectorModeOverride treeMotionVectorModeOverride
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_treeMotionVectorModeOverride_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_treeMotionVectorModeOverride_Injected(intPtr, value);
			}
		}

		public bool preserveTreePrototypeLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_preserveTreePrototypeLayers_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_preserveTreePrototypeLayers_Injected(intPtr, value);
			}
		}

		[StaticAccessor("Terrain", StaticAccessorType.DoubleColon)]
		public static extern GraphicsFormat heightmapFormat
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static TextureFormat heightmapTextureFormat
		{
			get
			{
				return GraphicsFormatUtility.GetTextureFormat(Terrain.heightmapFormat);
			}
		}

		public static RenderTextureFormat heightmapRenderTextureFormat
		{
			get
			{
				return GraphicsFormatUtility.GetRenderTextureFormat(Terrain.heightmapFormat);
			}
		}

		[StaticAccessor("Terrain", StaticAccessorType.DoubleColon)]
		public static extern GraphicsFormat normalmapFormat
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static TextureFormat normalmapTextureFormat
		{
			get
			{
				return GraphicsFormatUtility.GetTextureFormat(Terrain.normalmapFormat);
			}
		}

		public static RenderTextureFormat normalmapRenderTextureFormat
		{
			get
			{
				return GraphicsFormatUtility.GetRenderTextureFormat(Terrain.normalmapFormat);
			}
		}

		[StaticAccessor("Terrain", StaticAccessorType.DoubleColon)]
		public static extern GraphicsFormat holesFormat
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static RenderTextureFormat holesRenderTextureFormat
		{
			get
			{
				return GraphicsFormatUtility.GetRenderTextureFormat(Terrain.holesFormat);
			}
		}

		[StaticAccessor("Terrain", StaticAccessorType.DoubleColon)]
		public static extern GraphicsFormat compressedHolesFormat
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static TextureFormat compressedHolesTextureFormat
		{
			get
			{
				return GraphicsFormatUtility.GetTextureFormat(Terrain.compressedHolesFormat);
			}
		}

		public static Terrain activeTerrain
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Terrain>(Terrain.get_activeTerrain_Injected());
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetConnectivityDirty();

		[NativeProperty("ActiveTerrainsScriptingArray")]
		public static extern Terrain[] activeTerrains
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			get;
		}

		public static void GetActiveTerrains(List<Terrain> terrainList)
		{
			Terrain.Internal_FillActiveTerrainList(terrainList);
		}

		private static void Internal_FillActiveTerrainList([NotNull] object terrainList)
		{
			if (terrainList == null)
			{
				ThrowHelper.ThrowArgumentNullException(terrainList, "terrainList");
			}
			Terrain.Internal_FillActiveTerrainList_Injected(terrainList);
		}

		[UsedByNativeCode]
		public static GameObject CreateTerrainGameObject(TerrainData assignTerrain)
		{
			return Unmarshal.UnmarshalUnityObject<GameObject>(Terrain.CreateTerrainGameObject_Injected(Object.MarshalledUnityObject.Marshal<TerrainData>(assignTerrain)));
		}

		public Terrain leftNeighbor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Terrain>(Terrain.get_leftNeighbor_Injected(intPtr));
			}
		}

		public Terrain rightNeighbor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Terrain>(Terrain.get_rightNeighbor_Injected(intPtr));
			}
		}

		public Terrain topNeighbor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Terrain>(Terrain.get_topNeighbor_Injected(intPtr));
			}
		}

		public Terrain bottomNeighbor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Terrain>(Terrain.get_bottomNeighbor_Injected(intPtr));
			}
		}

		public uint renderingLayerMask
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Terrain.get_renderingLayerMask_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Terrain>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Terrain.set_renderingLayerMask_Injected(intPtr, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("splatmapDistance is deprecated, please use basemapDistance instead. (UnityUpgradable) -> basemapDistance", true)]
		public float splatmapDistance
		{
			get
			{
				return this.basemapDistance;
			}
			set
			{
				this.basemapDistance = value;
			}
		}

		[Obsolete("castShadows is deprecated, please use shadowCastingMode instead.")]
		public bool castShadows
		{
			get
			{
				return this.shadowCastingMode > ShadowCastingMode.Off;
			}
			set
			{
				this.shadowCastingMode = (value ? ShadowCastingMode.TwoSided : ShadowCastingMode.Off);
			}
		}

		[Obsolete("Property materialType is not used any more. Set materialTemplate directly.", false)]
		public Terrain.MaterialType materialType
		{
			get
			{
				return Terrain.MaterialType.Custom;
			}
			set
			{
			}
		}

		[Obsolete("Property legacySpecular is not used any more. Set materialTemplate directly.", false)]
		public Color legacySpecular
		{
			get
			{
				return Color.gray;
			}
			set
			{
			}
		}

		[Obsolete("Property legacyShininess is not used any more. Set materialTemplate directly.", false)]
		public float legacyShininess
		{
			get
			{
				return 0.078125f;
			}
			set
			{
			}
		}

		[Obsolete("Use TerrainData.SyncHeightmap to notify all Terrain instances using the TerrainData.", false)]
		public void ApplyDelayedHeightmapModification()
		{
			TerrainData terrainData = this.terrainData;
			if (terrainData != null)
			{
				terrainData.SyncHeightmap();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_terrainData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_terrainData_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_treeDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_treeDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_treeBillboardDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_treeBillboardDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_treeCrossFadeLength_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_treeCrossFadeLength_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_treeMaximumFullLODCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_treeMaximumFullLODCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_detailObjectDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_detailObjectDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_detailObjectDensity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_detailObjectDensity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_heightmapPixelError_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_heightmapPixelError_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_heightmapMaximumLOD_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_heightmapMaximumLOD_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_heightmapMinimumLODSimplification_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_heightmapMinimumLODSimplification_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_basemapDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_basemapDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_lightmapIndex_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lightmapIndex_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_realtimeLightmapIndex_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_realtimeLightmapIndex_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_lightmapScaleOffset_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lightmapScaleOffset_Injected(IntPtr _unity_self, [In] ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_realtimeLightmapScaleOffset_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_realtimeLightmapScaleOffset_Injected(IntPtr _unity_self, [In] ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_freeUnusedRenderingResources_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_freeUnusedRenderingResources_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_keepUnusedRenderingResources_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_keepUnusedRenderingResources_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetKeepUnusedCameraRenderingResources_Injected(IntPtr _unity_self, [In] ref EntityId cameraEntityId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetKeepUnusedCameraRenderingResources_Injected(IntPtr _unity_self, [In] ref EntityId cameraEntityId, bool keepUnused);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ShadowCastingMode get_shadowCastingMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowCastingMode_Injected(IntPtr _unity_self, ShadowCastingMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ReflectionProbeUsage get_reflectionProbeUsage_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_reflectionProbeUsage_Injected(IntPtr _unity_self, ReflectionProbeUsage value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetClosestReflectionProbes_Injected(IntPtr _unity_self, List<ReflectionProbeBlendInfo> result);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_materialTemplate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_materialTemplate_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_drawHeightmap_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_drawHeightmap_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_allowAutoConnect_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_allowAutoConnect_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_groupingID_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_groupingID_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_drawInstanced_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_drawInstanced_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableHeightmapRayTracing_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableHeightmapRayTracing_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_normalmapTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_drawTreesAndFoliage_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_drawTreesAndFoliage_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_patchBoundsMultiplier_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_patchBoundsMultiplier_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float SampleHeight_Injected(IntPtr _unity_self, [In] ref Vector3 worldPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddTreeInstance_Injected(IntPtr _unity_self, [In] ref TreeInstance instance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNeighbors_Injected(IntPtr _unity_self, IntPtr left, IntPtr top, IntPtr right, IntPtr bottom);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_treeLODBiasMultiplier_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_treeLODBiasMultiplier_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_collectDetailPatches_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_collectDetailPatches_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_ignoreQualitySettings_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_ignoreQualitySettings_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TerrainRenderFlags get_editorRenderFlags_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_editorRenderFlags_Injected(IntPtr _unity_self, TerrainRenderFlags value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Flush_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveTrees_Injected(IntPtr _unity_self, [In] ref Vector2 position, float radius, int prototypeIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSplatMaterialPropertyBlock_Injected(IntPtr _unity_self, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetSplatMaterialPropertyBlock_Injected(IntPtr _unity_self, IntPtr dest);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TreeMotionVectorModeOverride get_treeMotionVectorModeOverride_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_treeMotionVectorModeOverride_Injected(IntPtr _unity_self, TreeMotionVectorModeOverride value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_preserveTreePrototypeLayers_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_preserveTreePrototypeLayers_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_activeTerrain_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_FillActiveTerrainList_Injected(object terrainList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateTerrainGameObject_Injected(IntPtr assignTerrain);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_leftNeighbor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_rightNeighbor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_topNeighbor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_bottomNeighbor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_renderingLayerMask_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderingLayerMask_Injected(IntPtr _unity_self, uint value);

		[Obsolete("Enum type MaterialType is not used any more.", false)]
		public enum MaterialType
		{
			BuiltInStandard,
			BuiltInLegacyDiffuse,
			BuiltInLegacySpecular,
			Custom
		}
	}
}
