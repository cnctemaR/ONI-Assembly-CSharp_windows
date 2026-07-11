using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>The TerrainData class stores heightmaps, detail mesh positions, tree instances, and terrain texture alpha maps.</para>
	/// </summary>
	[NativeHeader("Modules/Terrain/Public/TerrainDataScriptingInterface.h")]
	[NativeHeader("TerrainScriptingClasses.h")]
	public sealed class TerrainData : Object
	{
		public TerrainData()
		{
			TerrainData.Internal_Create(this);
		}

		[ThreadSafe]
		[StaticAccessor("TerrainDataScriptingInterface", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetBoundaryValue(TerrainData.BoundaryValueType type);

		[FreeFunction("TerrainDataScriptingInterface::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] TerrainData terrainData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasUser(GameObject user);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddUser(GameObject user);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveUser(GameObject user);

		/// <summary>
		///   <para>Width of the terrain in samples (Read Only).</para>
		/// </summary>
		public extern int heightmapWidth
		{
			[NativeName("GetHeightmap().GetWidth")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Height of the terrain in samples (Read Only).</para>
		/// </summary>
		public extern int heightmapHeight
		{
			[NativeName("GetHeightmap().GetHeight")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Resolution of the heightmap.</para>
		/// </summary>
		public int heightmapResolution
		{
			get
			{
				return this.internalHeightmapResolution;
			}
			set
			{
				int num = value;
				if (value < 0 || value > TerrainData.k_MaximumResolution)
				{
					Debug.LogWarning("heightmapResolution is clamped to the range of [0, " + TerrainData.k_MaximumResolution + "].");
					num = Math.Min(TerrainData.k_MaximumResolution, Math.Max(value, 0));
				}
				this.internalHeightmapResolution = num;
			}
		}

		private extern int internalHeightmapResolution
		{
			[NativeName("GetHeightmap().GetResolution")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("GetHeightmap().SetResolution")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The size of each heightmap sample.</para>
		/// </summary>
		public Vector3 heightmapScale
		{
			[NativeName("GetHeightmap().GetScale")]
			get
			{
				Vector3 vector;
				this.get_heightmapScale_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>The total size in world units of the terrain.</para>
		/// </summary>
		public Vector3 size
		{
			[NativeName("GetHeightmap().GetSize")]
			get
			{
				Vector3 vector;
				this.get_size_Injected(out vector);
				return vector;
			}
			[NativeName("GetHeightmap().SetSize")]
			set
			{
				this.set_size_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The local bounding box of the TerrainData object.</para>
		/// </summary>
		public Bounds bounds
		{
			[NativeName("GetHeightmap().CalculateBounds")]
			get
			{
				Bounds bounds;
				this.get_bounds_Injected(out bounds);
				return bounds;
			}
		}

		/// <summary>
		///   <para>The thickness of the terrain used for collision detection.</para>
		/// </summary>
		public extern float thickness
		{
			[NativeName("GetHeightmap().GetThickness")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("GetHeightmap().SetThickness")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Gets the height at a certain point x,y.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		[NativeName("GetHeightmap().GetHeight")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetHeight(int x, int y);

		/// <summary>
		///   <para>Gets an interpolated height at a point x,y.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		[NativeName("GetHeightmap().GetInterpolatedHeight")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetInterpolatedHeight(float x, float y);

		/// <summary>
		///   <para>Get an array of heightmap samples.</para>
		/// </summary>
		/// <param name="xBase">First x index of heightmap samples to retrieve.</param>
		/// <param name="yBase">First y index of heightmap samples to retrieve.</param>
		/// <param name="width">Number of samples to retrieve along the heightmap's x axis.</param>
		/// <param name="height">Number of samples to retrieve along the heightmap's y axis.</param>
		public float[,] GetHeights(int xBase, int yBase, int width, int height)
		{
			if (xBase < 0 || yBase < 0 || xBase + width < 0 || yBase + height < 0 || xBase + width > this.heightmapWidth || yBase + height > this.heightmapHeight)
			{
				throw new ArgumentException("Trying to access out-of-bounds terrain height information.");
			}
			return this.Internal_GetHeights(xBase, yBase, width, height);
		}

		[FreeFunction("TerrainDataScriptingInterface::GetHeights", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float[,] Internal_GetHeights(int xBase, int yBase, int width, int height);

		public void SetHeights(int xBase, int yBase, float[,] heights)
		{
			if (heights == null)
			{
				throw new NullReferenceException();
			}
			if (xBase + heights.GetLength(1) > this.heightmapWidth || xBase + heights.GetLength(1) < 0 || yBase + heights.GetLength(0) < 0 || xBase < 0 || yBase < 0 || yBase + heights.GetLength(0) > this.heightmapHeight)
			{
				throw new ArgumentException(UnityString.Format("X or Y base out of bounds. Setting up to {0}x{1} while map size is {2}x{3}", new object[]
				{
					xBase + heights.GetLength(1),
					yBase + heights.GetLength(0),
					this.heightmapWidth,
					this.heightmapHeight
				}));
			}
			this.Internal_SetHeights(xBase, yBase, heights.GetLength(1), heights.GetLength(0), heights);
		}

		[FreeFunction("TerrainDataScriptingInterface::SetHeights", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetHeights(int xBase, int yBase, int width, int height, float[,] heights);

		/// <summary>
		///   <para>Returns an array of min max height values for all the renderable patches in a terrain.  The returned array can be modified and then passed to OverrideMinMaxPatchHeights.</para>
		/// </summary>
		/// <returns>
		///   <para>Minimum and maximum height values for each patch.</para>
		/// </returns>
		[FreeFunction("TerrainDataScriptingInterface::GetPatchMinMaxHeights", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern PatchExtents[] GetPatchMinMaxHeights();

		/// <summary>
		///   <para>Override the minimum and maximum patch heights for every renderable terrain patch.  Note that the overriden values get reset when the terrain resolution is changed and stays unchanged when the terrain heightmap is painted or changed via script.</para>
		/// </summary>
		/// <param name="minMaxHeights">Array of minimum and maximum terrain patch height values.</param>
		[FreeFunction("TerrainDataScriptingInterface::OverrideMinMaxPatchHeights", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OverrideMinMaxPatchHeights(PatchExtents[] minMaxHeights);

		/// <summary>
		///   <para>Returns an array of tesselation maximum height error values per renderable terrain patch.  The returned array can be modified and passed to OverrideMaximumHeightError.</para>
		/// </summary>
		/// <returns>
		///   <para>Float array of maximum height error values.</para>
		/// </returns>
		[FreeFunction("TerrainDataScriptingInterface::GetMaximumHeightError", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float[] GetMaximumHeightError();

		/// <summary>
		///   <para>Override the maximum tessellation height error with user provided values.  Note that the overriden values get reset when the terrain resolution is changed and stays unchanged when the terrain heightmap is painted or changed via script.</para>
		/// </summary>
		/// <param name="maxError">Provided maximum height error values.</param>
		[FreeFunction("TerrainDataScriptingInterface::OverrideMaximumHeightError", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OverrideMaximumHeightError(float[] maxError);

		public void SetHeightsDelayLOD(int xBase, int yBase, float[,] heights)
		{
			if (heights == null)
			{
				throw new ArgumentNullException("heights");
			}
			int length = heights.GetLength(0);
			int length2 = heights.GetLength(1);
			if (xBase < 0 || xBase + length2 < 0 || xBase + length2 > this.heightmapWidth)
			{
				throw new ArgumentException(UnityString.Format("X out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", new object[]
				{
					xBase,
					xBase + length2,
					this.heightmapWidth
				}));
			}
			if (yBase < 0 || yBase + length < 0 || yBase + length > this.heightmapHeight)
			{
				throw new ArgumentException(UnityString.Format("Y out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", new object[]
				{
					yBase,
					yBase + length,
					this.heightmapHeight
				}));
			}
			this.Internal_SetHeightsDelayLOD(xBase, yBase, length2, length, heights);
		}

		[FreeFunction("TerrainDataScriptingInterface::SetHeightsDelayLOD", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetHeightsDelayLOD(int xBase, int yBase, int width, int height, float[,] heights);

		/// <summary>
		///   <para>Gets the gradient of the terrain at point (x,y).</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		[NativeName("GetHeightmap().GetSteepness")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetSteepness(float x, float y);

		/// <summary>
		///   <para>Get an interpolated normal at a given location.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		[NativeName("GetHeightmap().GetInterpolatedNormal")]
		public Vector3 GetInterpolatedNormal(float x, float y)
		{
			Vector3 vector;
			this.GetInterpolatedNormal_Injected(x, y, out vector);
			return vector;
		}

		[NativeName("GetHeightmap().GetAdjustedSize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetAdjustedSize(int size);

		/// <summary>
		///   <para>Strength of the waving grass in the terrain.</para>
		/// </summary>
		public extern float wavingGrassStrength
		{
			[NativeName("GetDetailDatabase().GetWavingGrassStrength")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassStrength", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Amount of waving grass in the terrain.</para>
		/// </summary>
		public extern float wavingGrassAmount
		{
			[NativeName("GetDetailDatabase().GetWavingGrassAmount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassAmount", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Speed of the waving grass.</para>
		/// </summary>
		public extern float wavingGrassSpeed
		{
			[NativeName("GetDetailDatabase().GetWavingGrassSpeed")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassSpeed", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Color of the waving grass that the terrain has.</para>
		/// </summary>
		public Color wavingGrassTint
		{
			[NativeName("GetDetailDatabase().GetWavingGrassTint")]
			get
			{
				Color color;
				this.get_wavingGrassTint_Injected(out color);
				return color;
			}
			[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassTint", HasExplicitThis = true)]
			set
			{
				this.set_wavingGrassTint_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Detail width of the TerrainData.</para>
		/// </summary>
		public extern int detailWidth
		{
			[NativeName("GetDetailDatabase().GetWidth")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Detail height of the TerrainData.</para>
		/// </summary>
		public extern int detailHeight
		{
			[NativeName("GetDetailDatabase().GetHeight")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Set the resolution of the detail map.</para>
		/// </summary>
		/// <param name="detailResolution">Specifies the number of pixels in the detail resolution map. A larger detailResolution, leads to more accurate detail object painting.</param>
		/// <param name="resolutionPerPatch">Specifies the size in pixels of each individually rendered detail patch. A larger number reduces draw calls, but might increase triangle count since detail patches are culled on a per batch basis. A recommended value is 16. If you use a very large detail object distance and your grass is very sparse, it makes sense to increase the value.</param>
		public void SetDetailResolution(int detailResolution, int resolutionPerPatch)
		{
			if (detailResolution < 0)
			{
				Debug.LogWarning("detailResolution must not be negative.");
				detailResolution = 0;
			}
			if (resolutionPerPatch < TerrainData.k_MinimumDetailResolutionPerPatch || resolutionPerPatch > TerrainData.k_MaximumDetailResolutionPerPatch)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"resolutionPerPatch is clamped to the range of [",
					TerrainData.k_MinimumDetailResolutionPerPatch,
					", ",
					TerrainData.k_MaximumDetailResolutionPerPatch,
					"]."
				}));
				resolutionPerPatch = Math.Min(TerrainData.k_MaximumDetailResolutionPerPatch, Math.Max(resolutionPerPatch, TerrainData.k_MinimumDetailResolutionPerPatch));
			}
			int num = detailResolution / resolutionPerPatch;
			if (num > TerrainData.k_MaximumDetailPatchCount)
			{
				Debug.LogWarning("Patch count (detailResolution / resolutionPerPatch) is clamped to the range of [0, " + TerrainData.k_MaximumDetailPatchCount + "].");
				num = Math.Min(TerrainData.k_MaximumDetailPatchCount, Math.Max(num, 0));
			}
			this.Internal_SetDetailResolution(num, resolutionPerPatch);
		}

		[NativeName("GetDetailDatabase().SetDetailResolution")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetDetailResolution(int patchCount, int resolutionPerPatch);

		/// <summary>
		///   <para>Detail Resolution of the TerrainData.</para>
		/// </summary>
		public extern int detailResolution
		{
			[NativeName("GetDetailDatabase().GetResolution")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern int detailResolutionPerPatch
		{
			[NativeName("GetDetailDatabase().GetResolutionPerPatch")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeName("GetDetailDatabase().ResetDirtyDetails")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ResetDirtyDetails();

		/// <summary>
		///   <para>Reloads all the values of the available prototypes (ie, detail mesh assets) in the TerrainData Object.</para>
		/// </summary>
		[FreeFunction("TerrainDataScriptingInterface::RefreshPrototypes", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RefreshPrototypes();

		/// <summary>
		///   <para>Contains the detail texture/meshes that the terrain has.</para>
		/// </summary>
		public extern DetailPrototype[] detailPrototypes
		{
			[FreeFunction("TerrainDataScriptingInterface::GetDetailPrototypes", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("TerrainDataScriptingInterface::SetDetailPrototypes", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns an array of all supported detail layer indices in the area.</para>
		/// </summary>
		/// <param name="xBase"></param>
		/// <param name="yBase"></param>
		/// <param name="totalWidth"></param>
		/// <param name="totalHeight"></param>
		[FreeFunction("TerrainDataScriptingInterface::GetSupportedLayers", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int[] GetSupportedLayers(int xBase, int yBase, int totalWidth, int totalHeight);

		/// <summary>
		///   <para>Returns a 2D array of the detail object density in the specific location.</para>
		/// </summary>
		/// <param name="xBase"></param>
		/// <param name="yBase"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="layer"></param>
		[FreeFunction("TerrainDataScriptingInterface::GetDetailLayer", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int[,] GetDetailLayer(int xBase, int yBase, int width, int height, int layer);

		public void SetDetailLayer(int xBase, int yBase, int layer, int[,] details)
		{
			this.Internal_SetDetailLayer(xBase, yBase, details.GetLength(1), details.GetLength(0), layer, details);
		}

		[FreeFunction("TerrainDataScriptingInterface::SetDetailLayer", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetDetailLayer(int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, int[,] data);

		/// <summary>
		///   <para>Contains the current trees placed in the terrain.</para>
		/// </summary>
		public TreeInstance[] treeInstances
		{
			get
			{
				return this.Internal_GetTreeInstances();
			}
			set
			{
				this.Internal_SetTreeInstances(value);
			}
		}

		[NativeName("GetTreeDatabase().GetInstances")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern TreeInstance[] Internal_GetTreeInstances();

		[FreeFunction("TerrainDataScriptingInterface::SetTreeInstances", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetTreeInstances([NotNull] TreeInstance[] instances);

		/// <summary>
		///   <para>Get the tree instance at the specified index. It is used as a faster version of treeInstances[index] as this function doesn't create the entire tree instances array.</para>
		/// </summary>
		/// <param name="index">The index of the tree instance.</param>
		public TreeInstance GetTreeInstance(int index)
		{
			if (index < 0 || index >= this.treeInstanceCount)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this.Internal_GetTreeInstance(index);
		}

		[FreeFunction("TerrainDataScriptingInterface::GetTreeInstance", HasExplicitThis = true)]
		private TreeInstance Internal_GetTreeInstance(int index)
		{
			TreeInstance treeInstance;
			this.Internal_GetTreeInstance_Injected(index, out treeInstance);
			return treeInstance;
		}

		/// <summary>
		///   <para>Set the tree instance with new parameters at the specified index. However, TreeInstance.prototypeIndex and TreeInstance.position can not be changed otherwise an ArgumentException will be thrown.</para>
		/// </summary>
		/// <param name="index">The index of the tree instance.</param>
		/// <param name="instance">The new TreeInstance value.</param>
		[FreeFunction("TerrainDataScriptingInterface::SetTreeInstance", HasExplicitThis = true)]
		[NativeThrows]
		public void SetTreeInstance(int index, TreeInstance instance)
		{
			this.SetTreeInstance_Injected(index, ref instance);
		}

		/// <summary>
		///   <para>Returns the number of tree instances.</para>
		/// </summary>
		public extern int treeInstanceCount
		{
			[NativeName("GetTreeDatabase().GetInstances().size")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The list of tree prototypes this are the ones available in the inspector.</para>
		/// </summary>
		public extern TreePrototype[] treePrototypes
		{
			[FreeFunction("TerrainDataScriptingInterface::GetTreePrototypes", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("TerrainDataScriptingInterface::SetTreePrototypes", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeName("GetTreeDatabase().RemoveTreePrototype")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveTreePrototype(int index);

		[NativeName("GetTreeDatabase().RecalculateTreePositions")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RecalculateTreePositions();

		[NativeName("GetDetailDatabase().RemoveDetailPrototype")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveDetailPrototype(int index);

		[NativeName("GetTreeDatabase().NeedUpgradeScaledPrototypes")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool NeedUpgradeScaledTreePrototypes();

		[FreeFunction("TerrainDataScriptingInterface::UpgradeScaledTreePrototype", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void UpgradeScaledTreePrototype();

		/// <summary>
		///   <para>Number of alpha map layers.</para>
		/// </summary>
		public extern int alphamapLayers
		{
			[NativeName("GetSplatDatabase().GetDepth")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the alpha map at a position x, y given a width and height.</para>
		/// </summary>
		/// <param name="x">The x offset to read from.</param>
		/// <param name="y">The y offset to read from.</param>
		/// <param name="width">The width of the alpha map area to read.</param>
		/// <param name="height">The height of the alpha map area to read.</param>
		/// <returns>
		///   <para>A 3D array of floats, where the 3rd dimension represents the mixing weight of each splatmap at each x,y coordinate.</para>
		/// </returns>
		public float[,,] GetAlphamaps(int x, int y, int width, int height)
		{
			if (x < 0 || y < 0 || width < 0 || height < 0)
			{
				throw new ArgumentException("Invalid argument for GetAlphaMaps");
			}
			return this.Internal_GetAlphamaps(x, y, width, height);
		}

		[FreeFunction("TerrainDataScriptingInterface::GetAlphamaps", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float[,,] Internal_GetAlphamaps(int x, int y, int width, int height);

		/// <summary>
		///   <para>Resolution of the alpha map.</para>
		/// </summary>
		public int alphamapResolution
		{
			get
			{
				return this.Internal_alphamapResolution;
			}
			set
			{
				int num = value;
				if (value < TerrainData.k_MinimumAlphamapResolution || value > TerrainData.k_MaximumAlphamapResolution)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"alphamapResolution is clamped to the range of [",
						TerrainData.k_MinimumAlphamapResolution,
						", ",
						TerrainData.k_MaximumAlphamapResolution,
						"]."
					}));
					num = Math.Min(TerrainData.k_MaximumAlphamapResolution, Math.Max(value, TerrainData.k_MinimumAlphamapResolution));
				}
				this.Internal_alphamapResolution = num;
			}
		}

		[RequiredByNativeCode]
		[NativeName("GetSplatDatabase().GetAlphamapResolution")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetAlphamapResolutionInternal();

		private extern int Internal_alphamapResolution
		{
			[NativeName("GetSplatDatabase().GetAlphamapResolution")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("GetSplatDatabase().SetAlphamapResolution")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Width of the alpha map.</para>
		/// </summary>
		public int alphamapWidth
		{
			get
			{
				return this.alphamapResolution;
			}
		}

		/// <summary>
		///   <para>Height of the alpha map.</para>
		/// </summary>
		public int alphamapHeight
		{
			get
			{
				return this.alphamapResolution;
			}
		}

		/// <summary>
		///   <para>Resolution of the base map used for rendering far patches on the terrain.</para>
		/// </summary>
		public int baseMapResolution
		{
			get
			{
				return this.Internal_baseMapResolution;
			}
			set
			{
				int num = value;
				if (value < TerrainData.k_MinimumBaseMapResolution || value > TerrainData.k_MaximumBaseMapResolution)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"baseMapResolution is clamped to the range of [",
						TerrainData.k_MinimumBaseMapResolution,
						", ",
						TerrainData.k_MaximumBaseMapResolution,
						"]."
					}));
					num = Math.Min(TerrainData.k_MaximumBaseMapResolution, Math.Max(value, TerrainData.k_MinimumBaseMapResolution));
				}
				this.Internal_baseMapResolution = num;
			}
		}

		private extern int Internal_baseMapResolution
		{
			[NativeName("GetSplatDatabase().GetBaseMapResolution")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("GetSplatDatabase().SetBaseMapResolution")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public void SetAlphamaps(int x, int y, float[,,] map)
		{
			if (map.GetLength(2) != this.alphamapLayers)
			{
				throw new Exception(UnityString.Format("Float array size wrong (layers should be {0})", new object[] { this.alphamapLayers }));
			}
			this.Internal_SetAlphamaps(x, y, map.GetLength(1), map.GetLength(0), map);
		}

		[FreeFunction("TerrainDataScriptingInterface::SetAlphamaps", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetAlphamaps(int x, int y, int width, int height, float[,,] map);

		[NativeName("GetSplatDatabase().RecalculateBasemapIfDirty")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RecalculateBasemapIfDirty();

		[NativeName("GetSplatDatabase().SetBasemapDirty")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetBasemapDirty(bool dirty);

		[NativeName("GetSplatDatabase().GetAlphaTexture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Texture2D GetAlphamapTexture(int index);

		private extern int alphamapTextureCount
		{
			[NativeName("GetSplatDatabase().GetAlphaTextureCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Alpha map textures used by the Terrain. Used by Terrain Inspector for undo.</para>
		/// </summary>
		public Texture2D[] alphamapTextures
		{
			get
			{
				Texture2D[] array = new Texture2D[this.alphamapTextureCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.GetAlphamapTexture(i);
				}
				return array;
			}
		}

		/// <summary>
		///   <para>Splat texture used by the terrain.</para>
		/// </summary>
		public extern SplatPrototype[] splatPrototypes
		{
			[FreeFunction("TerrainDataScriptingInterface::GetSplatPrototypes", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("TerrainDataScriptingInterface::SetSplatPrototypes", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeName("GetTreeDatabase().AddTree")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddTree(ref TreeInstance tree);

		[NativeName("GetTreeDatabase().RemoveTrees")]
		internal int RemoveTrees(Vector2 position, float radius, int prototypeIndex)
		{
			return this.RemoveTrees_Injected(ref position, radius, prototypeIndex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_heightmapScale_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_size_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_size_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_bounds_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetInterpolatedNormal_Injected(float x, float y, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_wavingGrassTint_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_wavingGrassTint_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetTreeInstance_Injected(int index, out TreeInstance ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTreeInstance_Injected(int index, ref TreeInstance instance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int RemoveTrees_Injected(ref Vector2 position, float radius, int prototypeIndex);

		private const string k_ScriptingInterfaceName = "TerrainDataScriptingInterface";

		private const string k_ScriptingInterfacePrefix = "TerrainDataScriptingInterface::";

		private const string k_HeightmapPrefix = "GetHeightmap().";

		private const string k_DetailDatabasePrefix = "GetDetailDatabase().";

		private const string k_TreeDatabasePrefix = "GetTreeDatabase().";

		private const string k_SplatDatabasePrefix = "GetSplatDatabase().";

		private static readonly int k_MaximumResolution = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MaxHeightmapRes);

		private static readonly int k_MinimumDetailResolutionPerPatch = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MinDetailResPerPatch);

		private static readonly int k_MaximumDetailResolutionPerPatch = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MaxDetailResPerPatch);

		private static readonly int k_MaximumDetailPatchCount = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MaxDetailPatchCount);

		private static readonly int k_MinimumAlphamapResolution = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MinAlphamapRes);

		private static readonly int k_MaximumAlphamapResolution = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MaxAlphamapRes);

		private static readonly int k_MinimumBaseMapResolution = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MinBaseMapRes);

		private static readonly int k_MaximumBaseMapResolution = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MaxBaseMapRes);

		private enum BoundaryValueType
		{
			MaxHeightmapRes,
			MinDetailResPerPatch,
			MaxDetailResPerPatch,
			MaxDetailPatchCount,
			MinAlphamapRes,
			MaxAlphamapRes,
			MinBaseMapRes,
			MaxBaseMapRes
		}
	}
}
