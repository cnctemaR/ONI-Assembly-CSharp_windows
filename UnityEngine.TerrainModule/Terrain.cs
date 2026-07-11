using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>The Terrain component renders the terrain.</para>
	/// </summary>
	[StaticAccessor("GetITerrainManager()", StaticAccessorType.Arrow)]
	[NativeHeader("TerrainScriptingClasses.h")]
	[NativeHeader("Runtime/Interfaces/ITerrainManager.h")]
	[NativeHeader("Modules/Terrain/Public/Terrain.h")]
	[UsedByNativeCode]
	public sealed class Terrain : Behaviour
	{
		/// <summary>
		///   <para>The Terrain Data that stores heightmaps, terrain textures, detail meshes and trees.</para>
		/// </summary>
		public extern TerrainData terrainData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The maximum distance at which trees are rendered.</para>
		/// </summary>
		public extern float treeDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Distance from the camera where trees will be rendered as billboards only.</para>
		/// </summary>
		public extern float treeBillboardDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Total distance delta that trees will use to transition from billboard orientation to mesh orientation.</para>
		/// </summary>
		public extern float treeCrossFadeLength
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Maximum number of trees rendered at full LOD.</para>
		/// </summary>
		public extern int treeMaximumFullLODCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Detail objects will be displayed up to this distance.</para>
		/// </summary>
		public extern float detailObjectDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Density of detail objects.</para>
		/// </summary>
		public extern float detailObjectDensity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>An approximation of how many pixels the terrain will pop in the worst case when switching lod.</para>
		/// </summary>
		public extern float heightmapPixelError
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Lets you essentially lower the heightmap resolution used for rendering.</para>
		/// </summary>
		public extern int heightmapMaximumLOD
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Heightmap patches beyond basemap distance will use a precomputed low res basemap.</para>
		/// </summary>
		public extern float basemapDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		/// <summary>
		///   <para>The index of the baked lightmap applied to this terrain.</para>
		/// </summary>
		[NativeProperty("StaticLightmapIndexInt")]
		public extern int lightmapIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The index of the realtime lightmap applied to this terrain.</para>
		/// </summary>
		[NativeProperty("DynamicLightmapIndexInt")]
		public extern int realtimeLightmapIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The UV scale &amp; offset used for a baked lightmap.</para>
		/// </summary>
		[NativeProperty("StaticLightmapST")]
		public Vector4 lightmapScaleOffset
		{
			get
			{
				Vector4 vector;
				this.get_lightmapScaleOffset_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_lightmapScaleOffset_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The UV scale &amp; offset used for a realtime lightmap.</para>
		/// </summary>
		[NativeProperty("DynamicLightmapST")]
		public Vector4 realtimeLightmapScaleOffset
		{
			get
			{
				Vector4 vector;
				this.get_realtimeLightmapScaleOffset_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_realtimeLightmapScaleOffset_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Whether some per-camera rendering resources for the terrain should be freed after not being used for some frames.</para>
		/// </summary>
		[NativeProperty("GarbageCollectRenderers")]
		public extern bool freeUnusedRenderingResources
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should terrain cast shadows?.</para>
		/// </summary>
		public extern bool castShadows
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How reflection probes are used for terrain. See Rendering.ReflectionProbeUsage.</para>
		/// </summary>
		public extern ReflectionProbeUsage reflectionProbeUsage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result);

		/// <summary>
		///   <para>The type of the material used to render the terrain. Could be one of the built-in types or custom. See Terrain.MaterialType.</para>
		/// </summary>
		public extern Terrain.MaterialType materialType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The custom material used to render the terrain.</para>
		/// </summary>
		public extern Material materialTemplate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The specular color of the terrain.</para>
		/// </summary>
		public Color legacySpecular
		{
			get
			{
				Color color;
				this.get_legacySpecular_Injected(out color);
				return color;
			}
			set
			{
				this.set_legacySpecular_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The shininess value of the terrain.</para>
		/// </summary>
		public extern float legacyShininess
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Specify if terrain heightmap should be drawn.</para>
		/// </summary>
		public extern bool drawHeightmap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Specify if terrain trees and details should be drawn.</para>
		/// </summary>
		public extern bool drawTreesAndFoliage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set the terrain bounding box scale.</para>
		/// </summary>
		public Vector3 patchBoundsMultiplier
		{
			get
			{
				Vector3 vector;
				this.get_patchBoundsMultiplier_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_patchBoundsMultiplier_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Samples the height at the given position defined in world space, relative to the terrain space.</para>
		/// </summary>
		/// <param name="worldPosition"></param>
		public float SampleHeight(Vector3 worldPosition)
		{
			return this.SampleHeight_Injected(ref worldPosition);
		}

		/// <summary>
		///   <para>Update the terrain's LOD and vegetation information after making changes with TerrainData.SetHeightsDelayLOD.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ApplyDelayedHeightmapModification();

		/// <summary>
		///   <para>Adds a tree instance to the terrain.</para>
		/// </summary>
		/// <param name="instance"></param>
		public void AddTreeInstance(TreeInstance instance)
		{
			this.AddTreeInstance_Injected(ref instance);
		}

		/// <summary>
		///   <para>Lets you setup the connection between neighboring Terrains.</para>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetNeighbors(Terrain left, Terrain top, Terrain right, Terrain bottom);

		/// <summary>
		///   <para>The multiplier to the current LOD bias used for rendering LOD trees (i.e. SpeedTree trees).</para>
		/// </summary>
		public extern float treeLODBiasMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Collect detail patches from memory.</para>
		/// </summary>
		public extern bool collectDetailPatches
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Controls what part of the terrain should be rendered.</para>
		/// </summary>
		public extern TerrainRenderFlags editorRenderFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Get the position of the terrain.</para>
		/// </summary>
		public Vector3 GetPosition()
		{
			Vector3 vector;
			this.GetPosition_Injected(out vector);
			return vector;
		}

		/// <summary>
		///   <para>Flushes any change done in the terrain so it takes effect.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Flush();

		internal void RemoveTrees(Vector2 position, float radius, int prototypeIndex)
		{
			this.RemoveTrees_Injected(ref position, radius, prototypeIndex);
		}

		/// <summary>
		///   <para>Set the additional material properties when rendering the terrain heightmap using the splat material.</para>
		/// </summary>
		/// <param name="properties"></param>
		[NativeMethod("CopySplatMaterialCustomProps")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetSplatMaterialPropertyBlock(MaterialPropertyBlock properties);

		/// <summary>
		///   <para>Get the previously set splat material properties by copying to the dest MaterialPropertyBlock object.</para>
		/// </summary>
		/// <param name="dest"></param>
		public void GetSplatMaterialPropertyBlock(MaterialPropertyBlock dest)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			this.Internal_GetSplatMaterialPropertyBlock(dest);
		}

		[NativeMethod("GetSplatMaterialCustomProps")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetSplatMaterialPropertyBlock(MaterialPropertyBlock dest);

		/// <summary>
		///   <para>Allows you to specify how Unity chooses the for tree instances.</para>
		/// </summary>
		public extern bool preserveTreePrototypeLayers
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The active terrain. This is a convenience function to get to the main terrain in the scene.</para>
		/// </summary>
		public static extern Terrain activeTerrain
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The active terrains in the scene.</para>
		/// </summary>
		[NativeProperty("ActiveTerrainsScriptingArray")]
		public static extern Terrain[] activeTerrains
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Creates a Terrain including collider from TerrainData.</para>
		/// </summary>
		/// <param name="assignTerrain"></param>
		[UsedByNativeCode]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject CreateTerrainGameObject(TerrainData assignTerrain);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_lightmapScaleOffset_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_lightmapScaleOffset_Injected(ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_realtimeLightmapScaleOffset_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_realtimeLightmapScaleOffset_Injected(ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_legacySpecular_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_legacySpecular_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_patchBoundsMultiplier_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_patchBoundsMultiplier_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float SampleHeight_Injected(ref Vector3 worldPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddTreeInstance_Injected(ref TreeInstance instance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveTrees_Injected(ref Vector2 position, float radius, int prototypeIndex);

		/// <summary>
		///   <para>The type of the material used to render a terrain object. Could be one of the built-in types or custom.</para>
		/// </summary>
		public enum MaterialType
		{
			/// <summary>
			///   <para>A built-in material that uses the standard physically-based lighting model. Inputs supported: smoothness, metallic / specular, normal.</para>
			/// </summary>
			BuiltInStandard,
			/// <summary>
			///   <para>A built-in material that uses the legacy Lambert (diffuse) lighting model and has optional normal map support.</para>
			/// </summary>
			BuiltInLegacyDiffuse,
			/// <summary>
			///   <para>A built-in material that uses the legacy BlinnPhong (specular) lighting model and has optional normal map support.</para>
			/// </summary>
			BuiltInLegacySpecular,
			/// <summary>
			///   <para>Use a custom material given by Terrain.materialTemplate.</para>
			/// </summary>
			Custom
		}
	}
}
