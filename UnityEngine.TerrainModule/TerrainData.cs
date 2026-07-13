using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/Terrain/Public/TerrainDataScriptingInterface.h")]
	[NativeHeader("TerrainScriptingClasses.h")]
	[UsedByNativeCode]
	public sealed class TerrainData : Object
	{
		[StaticAccessor("TerrainDataScriptingInterface", StaticAccessorType.DoubleColon)]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetBoundaryValue(TerrainData.BoundaryValueType type);

		public TerrainData()
		{
			TerrainData.Internal_Create(this);
		}

		[FreeFunction("TerrainDataScriptingInterface::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] TerrainData terrainData);

		[Obsolete("Please use DirtyHeightmapRegion instead.", false)]
		public void UpdateDirtyRegion(int x, int y, int width, int height, bool syncHeightmapTextureImmediately)
		{
			this.DirtyHeightmapRegion(new RectInt(x, y, width, height), syncHeightmapTextureImmediately ? TerrainHeightmapSyncControl.HeightOnly : TerrainHeightmapSyncControl.None);
		}

		[Obsolete("Please use heightmapResolution instead. (UnityUpgradable) -> heightmapResolution", false)]
		public int heightmapWidth
		{
			get
			{
				return this.heightmapResolution;
			}
		}

		[Obsolete("Please use heightmapResolution instead. (UnityUpgradable) -> heightmapResolution", false)]
		public int heightmapHeight
		{
			get
			{
				return this.heightmapResolution;
			}
		}

		public RenderTexture heightmapTexture
		{
			[NativeName("GetHeightmap().GetHeightmapTexture")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<RenderTexture>(TerrainData.get_heightmapTexture_Injected(intPtr));
			}
		}

		public int heightmapResolution
		{
			get
			{
				return this.internalHeightmapResolution;
			}
			set
			{
				int num = value;
				bool flag = value < 0 || value > TerrainData.k_MaximumResolution;
				if (flag)
				{
					Debug.LogWarning("heightmapResolution is clamped to the range of [0, " + TerrainData.k_MaximumResolution.ToString() + "].");
					num = Math.Min(TerrainData.k_MaximumResolution, Math.Max(value, 0));
				}
				this.internalHeightmapResolution = num;
			}
		}

		private int internalHeightmapResolution
		{
			[NativeName("GetHeightmap().GetResolution")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_internalHeightmapResolution_Injected(intPtr);
			}
			[NativeName("GetHeightmap().SetResolution")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_internalHeightmapResolution_Injected(intPtr, value);
			}
		}

		public Vector3 heightmapScale
		{
			[NativeName("GetHeightmap().GetScale")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				TerrainData.get_heightmapScale_Injected(intPtr, out vector);
				return vector;
			}
		}

		public Texture holesTexture
		{
			get
			{
				bool flag = this.IsHolesTextureCompressed();
				Texture texture;
				if (flag)
				{
					texture = this.GetCompressedHolesTexture();
				}
				else
				{
					texture = this.GetHolesTexture();
				}
				return texture;
			}
		}

		public bool enableHolesTextureCompression
		{
			[NativeName("GetHeightmap().GetEnableHolesTextureCompression")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_enableHolesTextureCompression_Injected(intPtr);
			}
			[NativeName("GetHeightmap().SetEnableHolesTextureCompression")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_enableHolesTextureCompression_Injected(intPtr, value);
			}
		}

		internal RenderTexture holesRenderTexture
		{
			get
			{
				return this.GetHolesTexture();
			}
		}

		[NativeName("GetHeightmap().IsHolesTextureCompressed")]
		internal bool IsHolesTextureCompressed()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.IsHolesTextureCompressed_Injected(intPtr);
		}

		[NativeName("GetHeightmap().GetHolesTexture")]
		internal RenderTexture GetHolesTexture()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(TerrainData.GetHolesTexture_Injected(intPtr));
		}

		[NativeName("GetHeightmap().GetCompressedHolesTexture")]
		internal Texture2D GetCompressedHolesTexture()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture2D>(TerrainData.GetCompressedHolesTexture_Injected(intPtr));
		}

		public int holesResolution
		{
			get
			{
				return this.heightmapResolution - 1;
			}
		}

		public Vector3 size
		{
			[NativeName("GetHeightmap().GetSize")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				TerrainData.get_size_Injected(intPtr, out vector);
				return vector;
			}
			[NativeName("GetHeightmap().SetSize")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_size_Injected(intPtr, ref value);
			}
		}

		public Bounds bounds
		{
			[NativeName("GetHeightmap().CalculateBounds")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				TerrainData.get_bounds_Injected(intPtr, out bounds);
				return bounds;
			}
		}

		[Obsolete("Terrain thickness is no longer required by the physics engine. Set appropriate continuous collision detection modes to fast moving bodies.")]
		public float thickness
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[NativeName("GetHeightmap().GetHeight")]
		public float GetHeight(int x, int y)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.GetHeight_Injected(intPtr, x, y);
		}

		[NativeName("GetHeightmap().GetInterpolatedHeight")]
		public float GetInterpolatedHeight(float x, float y)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.GetInterpolatedHeight_Injected(intPtr, x, y);
		}

		public float[,] GetInterpolatedHeights(float xBase, float yBase, int xCount, int yCount, float xInterval, float yInterval)
		{
			bool flag = xCount <= 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("xCount");
			}
			bool flag2 = yCount <= 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("yCount");
			}
			float[,] array = new float[yCount, xCount];
			this.Internal_GetInterpolatedHeights(array, xCount, 0, 0, xBase, yBase, xCount, yCount, xInterval, yInterval);
			return array;
		}

		public void GetInterpolatedHeights(float[,] results, int resultXOffset, int resultYOffset, float xBase, float yBase, int xCount, int yCount, float xInterval, float yInterval)
		{
			bool flag = results == null;
			if (flag)
			{
				throw new ArgumentNullException("results");
			}
			bool flag2 = xCount <= 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("xCount");
			}
			bool flag3 = yCount <= 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("yCount");
			}
			bool flag4 = resultXOffset < 0 || resultXOffset + xCount > results.GetLength(1);
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("resultXOffset");
			}
			bool flag5 = resultYOffset < 0 || resultYOffset + yCount > results.GetLength(0);
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("resultYOffset");
			}
			this.Internal_GetInterpolatedHeights(results, results.GetLength(1), resultXOffset, resultYOffset, xBase, yBase, xCount, yCount, xInterval, yInterval);
		}

		[FreeFunction("TerrainDataScriptingInterface::GetInterpolatedHeights", HasExplicitThis = true)]
		private unsafe void Internal_GetInterpolatedHeights(float[,] results, int resultXDimension, int resultXOffset, int resultYOffset, float xBase, float yBase, int xCount, int yCount, float xInterval, float yInterval)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			int num;
			void* ptr;
			if (results == null || (num = results.Length) == 0)
			{
				num = 0;
				ptr = null;
			}
			else
			{
				ptr = (void*)(&results[0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(ptr, num);
			TerrainData.Internal_GetInterpolatedHeights_Injected(intPtr, ref managedSpanWrapper, resultXDimension, resultXOffset, resultYOffset, xBase, yBase, xCount, yCount, xInterval, yInterval);
		}

		public float[,] GetHeights(int xBase, int yBase, int width, int height)
		{
			bool flag = xBase < 0 || yBase < 0 || xBase + width < 0 || yBase + height < 0 || xBase + width > this.heightmapResolution || yBase + height > this.heightmapResolution;
			if (flag)
			{
				throw new ArgumentException("Trying to access out-of-bounds terrain height information.");
			}
			return this.Internal_GetHeights(xBase, yBase, width, height);
		}

		[FreeFunction("TerrainDataScriptingInterface::GetHeights", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		private float[,] Internal_GetHeights(int xBase, int yBase, int width, int height)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.Internal_GetHeights_Injected(intPtr, xBase, yBase, width, height);
		}

		public void SetHeights(int xBase, int yBase, float[,] heights)
		{
			bool flag = heights == null;
			if (flag)
			{
				throw new NullReferenceException();
			}
			bool flag2 = xBase + heights.GetLength(1) > this.heightmapResolution || xBase + heights.GetLength(1) < 0 || yBase + heights.GetLength(0) < 0 || xBase < 0 || yBase < 0 || yBase + heights.GetLength(0) > this.heightmapResolution;
			if (flag2)
			{
				throw new ArgumentException(string.Format("X or Y base out of bounds. Setting up to {0}x{1} while map size is {2}x{2}", xBase + heights.GetLength(1), yBase + heights.GetLength(0), this.heightmapResolution));
			}
			this.Internal_SetHeights(xBase, yBase, heights.GetLength(1), heights.GetLength(0), heights);
		}

		[FreeFunction("TerrainDataScriptingInterface::SetHeights", HasExplicitThis = true)]
		private unsafe void Internal_SetHeights(int xBase, int yBase, int width, int height, float[,] heights)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			int num;
			void* ptr;
			if (heights == null || (num = heights.Length) == 0)
			{
				num = 0;
				ptr = null;
			}
			else
			{
				ptr = (void*)(&heights[0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(ptr, num);
			TerrainData.Internal_SetHeights_Injected(intPtr, xBase, yBase, width, height, ref managedSpanWrapper);
		}

		[FreeFunction("TerrainDataScriptingInterface::GetPatchMinMaxHeights", HasExplicitThis = true)]
		public PatchExtents[] GetPatchMinMaxHeights()
		{
			PatchExtents[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				TerrainData.GetPatchMinMaxHeights_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				PatchExtents[] array;
				blittableArrayWrapper.Unmarshal<PatchExtents>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("TerrainDataScriptingInterface::OverrideMinMaxPatchHeights", HasExplicitThis = true)]
		public unsafe void OverrideMinMaxPatchHeights(PatchExtents[] minMaxHeights)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<PatchExtents> span = new Span<PatchExtents>(minMaxHeights);
			fixed (PatchExtents* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				TerrainData.OverrideMinMaxPatchHeights_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[FreeFunction("TerrainDataScriptingInterface::GetMaximumHeightError", HasExplicitThis = true)]
		public float[] GetMaximumHeightError()
		{
			float[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				TerrainData.GetMaximumHeightError_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				float[] array;
				blittableArrayWrapper.Unmarshal<float>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("TerrainDataScriptingInterface::OverrideMaximumHeightError", HasExplicitThis = true)]
		public unsafe void OverrideMaximumHeightError(float[] maxError)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(maxError);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				TerrainData.OverrideMaximumHeightError_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		public void SetHeightsDelayLOD(int xBase, int yBase, float[,] heights)
		{
			bool flag = heights == null;
			if (flag)
			{
				throw new ArgumentNullException("heights");
			}
			int length = heights.GetLength(0);
			int length2 = heights.GetLength(1);
			bool flag2 = xBase < 0 || xBase + length2 < 0 || xBase + length2 > this.heightmapResolution;
			if (flag2)
			{
				throw new ArgumentException(string.Format("X out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", xBase, xBase + length2, this.heightmapResolution));
			}
			bool flag3 = yBase < 0 || yBase + length < 0 || yBase + length > this.heightmapResolution;
			if (flag3)
			{
				throw new ArgumentException(string.Format("Y out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", yBase, yBase + length, this.heightmapResolution));
			}
			this.Internal_SetHeightsDelayLOD(xBase, yBase, length2, length, heights);
		}

		[FreeFunction("TerrainDataScriptingInterface::SetHeightsDelayLOD", HasExplicitThis = true)]
		private unsafe void Internal_SetHeightsDelayLOD(int xBase, int yBase, int width, int height, float[,] heights)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			int num;
			void* ptr;
			if (heights == null || (num = heights.Length) == 0)
			{
				num = 0;
				ptr = null;
			}
			else
			{
				ptr = (void*)(&heights[0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(ptr, num);
			TerrainData.Internal_SetHeightsDelayLOD_Injected(intPtr, xBase, yBase, width, height, ref managedSpanWrapper);
		}

		public bool IsHole(int x, int y)
		{
			bool flag = x < 0 || x >= this.holesResolution || y < 0 || y >= this.holesResolution;
			if (flag)
			{
				throw new ArgumentException("Trying to access out-of-bounds terrain holes information.");
			}
			return this.Internal_IsHole(x, y);
		}

		public bool[,] GetHoles(int xBase, int yBase, int width, int height)
		{
			bool flag = xBase < 0 || yBase < 0 || width <= 0 || height <= 0 || xBase + width > this.holesResolution || yBase + height > this.holesResolution;
			if (flag)
			{
				throw new ArgumentException("Trying to access out-of-bounds terrain holes information.");
			}
			return this.Internal_GetHoles(xBase, yBase, width, height);
		}

		public void SetHoles(int xBase, int yBase, bool[,] holes)
		{
			bool flag = holes == null;
			if (flag)
			{
				throw new ArgumentNullException("holes");
			}
			int length = holes.GetLength(0);
			int length2 = holes.GetLength(1);
			bool flag2 = xBase < 0 || xBase + length2 > this.holesResolution;
			if (flag2)
			{
				throw new ArgumentException(string.Format("X out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", xBase, xBase + length2, this.holesResolution));
			}
			bool flag3 = yBase < 0 || yBase + length > this.holesResolution;
			if (flag3)
			{
				throw new ArgumentException(string.Format("Y out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", yBase, yBase + length, this.holesResolution));
			}
			this.Internal_SetHoles(xBase, yBase, holes.GetLength(1), holes.GetLength(0), holes);
		}

		public void SetHolesDelayLOD(int xBase, int yBase, bool[,] holes)
		{
			bool flag = holes == null;
			if (flag)
			{
				throw new ArgumentNullException("holes");
			}
			int length = holes.GetLength(0);
			int length2 = holes.GetLength(1);
			bool flag2 = xBase < 0 || xBase + length2 > this.holesResolution;
			if (flag2)
			{
				throw new ArgumentException(string.Format("X out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", xBase, xBase + length2, this.holesResolution));
			}
			bool flag3 = yBase < 0 || yBase + length > this.holesResolution;
			if (flag3)
			{
				throw new ArgumentException(string.Format("Y out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", yBase, yBase + length, this.holesResolution));
			}
			this.Internal_SetHolesDelayLOD(xBase, yBase, length2, length, holes);
		}

		[FreeFunction("TerrainDataScriptingInterface::SetHoles", HasExplicitThis = true)]
		private unsafe void Internal_SetHoles(int xBase, int yBase, int width, int height, bool[,] holes)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			int num;
			void* ptr;
			if (holes == null || (num = holes.Length) == 0)
			{
				num = 0;
				ptr = null;
			}
			else
			{
				ptr = (void*)(&holes[0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(ptr, num);
			TerrainData.Internal_SetHoles_Injected(intPtr, xBase, yBase, width, height, ref managedSpanWrapper);
		}

		[FreeFunction("TerrainDataScriptingInterface::GetHoles", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		private bool[,] Internal_GetHoles(int xBase, int yBase, int width, int height)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.Internal_GetHoles_Injected(intPtr, xBase, yBase, width, height);
		}

		[FreeFunction("TerrainDataScriptingInterface::IsHole", HasExplicitThis = true)]
		private bool Internal_IsHole(int x, int y)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.Internal_IsHole_Injected(intPtr, x, y);
		}

		[FreeFunction("TerrainDataScriptingInterface::SetHolesDelayLOD", HasExplicitThis = true)]
		private unsafe void Internal_SetHolesDelayLOD(int xBase, int yBase, int width, int height, bool[,] holes)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			int num;
			void* ptr;
			if (holes == null || (num = holes.Length) == 0)
			{
				num = 0;
				ptr = null;
			}
			else
			{
				ptr = (void*)(&holes[0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(ptr, num);
			TerrainData.Internal_SetHolesDelayLOD_Injected(intPtr, xBase, yBase, width, height, ref managedSpanWrapper);
		}

		[NativeName("GetHeightmap().GetSteepness")]
		public float GetSteepness(float x, float y)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.GetSteepness_Injected(intPtr, x, y);
		}

		[NativeName("GetHeightmap().GetInterpolatedNormal")]
		public Vector3 GetInterpolatedNormal(float x, float y)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			TerrainData.GetInterpolatedNormal_Injected(intPtr, x, y, out vector);
			return vector;
		}

		[NativeName("GetHeightmap().GetAdjustedSize")]
		internal int GetAdjustedSize(int size)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.GetAdjustedSize_Injected(intPtr, size);
		}

		public float wavingGrassStrength
		{
			[NativeName("GetDetailDatabase().GetWavingGrassStrength")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_wavingGrassStrength_Injected(intPtr);
			}
			[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassStrength", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_wavingGrassStrength_Injected(intPtr, value);
			}
		}

		public float wavingGrassAmount
		{
			[NativeName("GetDetailDatabase().GetWavingGrassAmount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_wavingGrassAmount_Injected(intPtr);
			}
			[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassAmount", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_wavingGrassAmount_Injected(intPtr, value);
			}
		}

		public float wavingGrassSpeed
		{
			[NativeName("GetDetailDatabase().GetWavingGrassSpeed")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_wavingGrassSpeed_Injected(intPtr);
			}
			[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassSpeed", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_wavingGrassSpeed_Injected(intPtr, value);
			}
		}

		public Color wavingGrassTint
		{
			[NativeName("GetDetailDatabase().GetWavingGrassTint")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				TerrainData.get_wavingGrassTint_Injected(intPtr, out color);
				return color;
			}
			[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassTint", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_wavingGrassTint_Injected(intPtr, ref value);
			}
		}

		public int detailWidth
		{
			[NativeName("GetDetailDatabase().GetWidth")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_detailWidth_Injected(intPtr);
			}
		}

		public int detailHeight
		{
			[NativeName("GetDetailDatabase().GetHeight")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_detailHeight_Injected(intPtr);
			}
		}

		public int maxDetailScatterPerRes
		{
			[NativeName("GetDetailDatabase().GetMaximumScatterPerRes")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_maxDetailScatterPerRes_Injected(intPtr);
			}
		}

		public void SetDetailResolution(int detailResolution, int resolutionPerPatch)
		{
			bool flag = detailResolution < 0;
			if (flag)
			{
				Debug.LogWarning("detailResolution must not be negative.");
				detailResolution = 0;
			}
			bool flag2 = resolutionPerPatch < TerrainData.k_MinimumDetailResolutionPerPatch || resolutionPerPatch > TerrainData.k_MaximumDetailResolutionPerPatch;
			if (flag2)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"resolutionPerPatch is clamped to the range of [",
					TerrainData.k_MinimumDetailResolutionPerPatch.ToString(),
					", ",
					TerrainData.k_MaximumDetailResolutionPerPatch.ToString(),
					"]."
				}));
				resolutionPerPatch = Math.Min(TerrainData.k_MaximumDetailResolutionPerPatch, Math.Max(resolutionPerPatch, TerrainData.k_MinimumDetailResolutionPerPatch));
			}
			int num = detailResolution / resolutionPerPatch;
			bool flag3 = num > TerrainData.k_MaximumDetailPatchCount;
			if (flag3)
			{
				Debug.LogWarning("Patch count (detailResolution / resolutionPerPatch) is clamped to the range of [0, " + TerrainData.k_MaximumDetailPatchCount.ToString() + "].");
				num = Math.Min(TerrainData.k_MaximumDetailPatchCount, Math.Max(num, 0));
			}
			this.Internal_SetDetailResolution(num, resolutionPerPatch);
		}

		[NativeName("GetDetailDatabase().SetDetailResolution")]
		private void Internal_SetDetailResolution(int patchCount, int resolutionPerPatch)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.Internal_SetDetailResolution_Injected(intPtr, patchCount, resolutionPerPatch);
		}

		public void SetDetailScatterMode(DetailScatterMode scatterMode)
		{
			this.Internal_SetDetailScatterMode(scatterMode);
		}

		[NativeName("GetDetailDatabase().SetDetailScatterMode")]
		private void Internal_SetDetailScatterMode(DetailScatterMode scatterMode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.Internal_SetDetailScatterMode_Injected(intPtr, scatterMode);
		}

		public int detailPatchCount
		{
			[NativeName("GetDetailDatabase().GetPatchCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_detailPatchCount_Injected(intPtr);
			}
		}

		public int detailResolution
		{
			[NativeName("GetDetailDatabase().GetResolution")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_detailResolution_Injected(intPtr);
			}
		}

		public int detailResolutionPerPatch
		{
			[NativeName("GetDetailDatabase().GetResolutionPerPatch")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_detailResolutionPerPatch_Injected(intPtr);
			}
		}

		public DetailScatterMode detailScatterMode
		{
			[NativeName("GetDetailDatabase().GetDetailScatterMode")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_detailScatterMode_Injected(intPtr);
			}
		}

		[NativeName("GetDetailDatabase().ResetDirtyDetails")]
		internal void ResetDirtyDetails()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.ResetDirtyDetails_Injected(intPtr);
		}

		[FreeFunction("TerrainDataScriptingInterface::RefreshPrototypes", HasExplicitThis = true)]
		public void RefreshPrototypes()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.RefreshPrototypes_Injected(intPtr);
		}

		public DetailPrototype[] detailPrototypes
		{
			[FreeFunction("TerrainDataScriptingInterface::GetDetailPrototypes", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_detailPrototypes_Injected(intPtr);
			}
			[FreeFunction("TerrainDataScriptingInterface::SetDetailPrototypes", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_detailPrototypes_Injected(intPtr, value);
			}
		}

		[FreeFunction("TerrainDataScriptingInterface::GetSupportedLayers", HasExplicitThis = true)]
		public int[] GetSupportedLayers(int xBase, int yBase, int totalWidth, int totalHeight)
		{
			int[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				TerrainData.GetSupportedLayers_Injected(intPtr, xBase, yBase, totalWidth, totalHeight, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		public int[] GetSupportedLayers(Vector2Int positionBase, Vector2Int size)
		{
			return this.GetSupportedLayers(positionBase.x, positionBase.y, size.x, size.y);
		}

		[FreeFunction("TerrainDataScriptingInterface::GetDetailLayer", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public int[,] GetDetailLayer(int xBase, int yBase, int width, int height, int layer)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.GetDetailLayer_Injected(intPtr, xBase, yBase, width, height, layer);
		}

		public int[,] GetDetailLayer(Vector2Int positionBase, Vector2Int size, int layer)
		{
			return this.GetDetailLayer(positionBase.x, positionBase.y, size.x, size.y, layer);
		}

		[FreeFunction("TerrainDataScriptingInterface::ComputeDetailInstanceTransforms", HasExplicitThis = true)]
		public DetailInstanceTransform[] ComputeDetailInstanceTransforms(int patchX, int patchY, int layer, float density, out Bounds bounds)
		{
			DetailInstanceTransform[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				TerrainData.ComputeDetailInstanceTransforms_Injected(intPtr, patchX, patchY, layer, density, out bounds, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				DetailInstanceTransform[] array;
				blittableArrayWrapper.Unmarshal<DetailInstanceTransform>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("TerrainDataScriptingInterface::ComputeDetailCoverage", HasExplicitThis = true)]
		public float ComputeDetailCoverage(int detailPrototypeIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.ComputeDetailCoverage_Injected(intPtr, detailPrototypeIndex);
		}

		public void SetDetailLayer(int xBase, int yBase, int layer, int[,] details)
		{
			this.Internal_SetDetailLayer(xBase, yBase, details.GetLength(1), details.GetLength(0), layer, details);
		}

		public void SetDetailLayer(Vector2Int basePosition, int layer, int[,] details)
		{
			this.SetDetailLayer(basePosition.x, basePosition.y, layer, details);
		}

		[FreeFunction("TerrainDataScriptingInterface::SetDetailLayer", HasExplicitThis = true)]
		private unsafe void Internal_SetDetailLayer(int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, int[,] data)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			int num;
			void* ptr;
			if (data == null || (num = data.Length) == 0)
			{
				num = 0;
				ptr = null;
			}
			else
			{
				ptr = (void*)(&data[0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(ptr, num);
			TerrainData.Internal_SetDetailLayer_Injected(intPtr, xBase, yBase, totalWidth, totalHeight, detailIndex, ref managedSpanWrapper);
		}

		public TreeInstance[] treeInstances
		{
			get
			{
				return this.Internal_GetTreeInstances();
			}
			set
			{
				this.SetTreeInstances(value, false);
			}
		}

		[NativeName("GetTreeDatabase().GetInstances")]
		private TreeInstance[] Internal_GetTreeInstances()
		{
			TreeInstance[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				TerrainData.Internal_GetTreeInstances_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				TreeInstance[] array;
				blittableArrayWrapper.Unmarshal<TreeInstance>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("TerrainDataScriptingInterface::SetTreeInstances", HasExplicitThis = true)]
		public unsafe void SetTreeInstances([NotNull] TreeInstance[] instances, bool snapToHeightmap)
		{
			if (instances == null)
			{
				ThrowHelper.ThrowArgumentNullException(instances, "instances");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<TreeInstance> span = new Span<TreeInstance>(instances);
			fixed (TreeInstance* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				TerrainData.SetTreeInstances_Injected(intPtr, ref managedSpanWrapper, snapToHeightmap);
			}
		}

		public TreeInstance GetTreeInstance(int index)
		{
			bool flag = index < 0 || index >= this.treeInstanceCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this.Internal_GetTreeInstance(index);
		}

		[FreeFunction("TerrainDataScriptingInterface::GetTreeInstance", HasExplicitThis = true)]
		private TreeInstance Internal_GetTreeInstance(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TreeInstance treeInstance;
			TerrainData.Internal_GetTreeInstance_Injected(intPtr, index, out treeInstance);
			return treeInstance;
		}

		[NativeThrows]
		[FreeFunction("TerrainDataScriptingInterface::SetTreeInstance", HasExplicitThis = true)]
		public void SetTreeInstance(int index, TreeInstance instance)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.SetTreeInstance_Injected(intPtr, index, ref instance);
		}

		public int treeInstanceCount
		{
			[NativeName("GetTreeDatabase().GetInstances().size")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_treeInstanceCount_Injected(intPtr);
			}
		}

		public TreePrototype[] treePrototypes
		{
			[FreeFunction("TerrainDataScriptingInterface::GetTreePrototypes", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_treePrototypes_Injected(intPtr);
			}
			[FreeFunction("TerrainDataScriptingInterface::SetTreePrototypes", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_treePrototypes_Injected(intPtr, value);
			}
		}

		[NativeName("GetTreeDatabase().RemoveTreePrototype")]
		internal void RemoveTreePrototype(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.RemoveTreePrototype_Injected(intPtr, index);
		}

		[NativeName("GetDetailDatabase().RemoveDetailPrototype")]
		public void RemoveDetailPrototype(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.RemoveDetailPrototype_Injected(intPtr, index);
		}

		[NativeName("GetTreeDatabase().NeedUpgradeScaledPrototypes")]
		internal bool NeedUpgradeScaledTreePrototypes()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.NeedUpgradeScaledTreePrototypes_Injected(intPtr);
		}

		[FreeFunction("TerrainDataScriptingInterface::UpgradeScaledTreePrototype", HasExplicitThis = true)]
		internal void UpgradeScaledTreePrototype()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.UpgradeScaledTreePrototype_Injected(intPtr);
		}

		public int alphamapLayers
		{
			[NativeName("GetSplatDatabase().GetSplatCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_alphamapLayers_Injected(intPtr);
			}
		}

		public float[,,] GetAlphamaps(int x, int y, int width, int height)
		{
			bool flag = x < 0 || y < 0 || width < 0 || height < 0;
			if (flag)
			{
				throw new ArgumentException("Invalid argument for GetAlphaMaps");
			}
			return this.Internal_GetAlphamaps(x, y, width, height);
		}

		[FreeFunction("TerrainDataScriptingInterface::GetAlphamaps", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		private float[,,] Internal_GetAlphamaps(int x, int y, int width, int height)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.Internal_GetAlphamaps_Injected(intPtr, x, y, width, height);
		}

		public int alphamapResolution
		{
			get
			{
				return this.Internal_alphamapResolution;
			}
			set
			{
				int num = value;
				bool flag = value < TerrainData.k_MinimumAlphamapResolution || value > TerrainData.k_MaximumAlphamapResolution;
				if (flag)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"alphamapResolution is clamped to the range of [",
						TerrainData.k_MinimumAlphamapResolution.ToString(),
						", ",
						TerrainData.k_MaximumAlphamapResolution.ToString(),
						"]."
					}));
					num = Math.Min(TerrainData.k_MaximumAlphamapResolution, Math.Max(value, TerrainData.k_MinimumAlphamapResolution));
				}
				this.Internal_alphamapResolution = num;
			}
		}

		[RequiredByNativeCode]
		[NativeName("GetSplatDatabase().GetAlphamapResolution")]
		internal float GetAlphamapResolutionInternal()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.GetAlphamapResolutionInternal_Injected(intPtr);
		}

		private int Internal_alphamapResolution
		{
			[NativeName("GetSplatDatabase().GetAlphamapResolution")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_Internal_alphamapResolution_Injected(intPtr);
			}
			[NativeName("GetSplatDatabase().SetAlphamapResolution")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_Internal_alphamapResolution_Injected(intPtr, value);
			}
		}

		public int alphamapWidth
		{
			get
			{
				return this.alphamapResolution;
			}
		}

		public int alphamapHeight
		{
			get
			{
				return this.alphamapResolution;
			}
		}

		public int baseMapResolution
		{
			get
			{
				return this.Internal_baseMapResolution;
			}
			set
			{
				int num = value;
				bool flag = value < TerrainData.k_MinimumBaseMapResolution || value > TerrainData.k_MaximumBaseMapResolution;
				if (flag)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"baseMapResolution is clamped to the range of [",
						TerrainData.k_MinimumBaseMapResolution.ToString(),
						", ",
						TerrainData.k_MaximumBaseMapResolution.ToString(),
						"]."
					}));
					num = Math.Min(TerrainData.k_MaximumBaseMapResolution, Math.Max(value, TerrainData.k_MinimumBaseMapResolution));
				}
				this.Internal_baseMapResolution = num;
			}
		}

		private int Internal_baseMapResolution
		{
			[NativeName("GetSplatDatabase().GetBaseMapResolution")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_Internal_baseMapResolution_Injected(intPtr);
			}
			[NativeName("GetSplatDatabase().SetBaseMapResolution")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_Internal_baseMapResolution_Injected(intPtr, value);
			}
		}

		public void SetAlphamaps(int x, int y, float[,,] map)
		{
			bool flag = map.GetLength(2) != this.alphamapLayers;
			if (flag)
			{
				throw new Exception(string.Format("Float array size wrong (layers should be {0})", this.alphamapLayers));
			}
			this.Internal_SetAlphamaps(x, y, map.GetLength(1), map.GetLength(0), map);
		}

		[FreeFunction("TerrainDataScriptingInterface::SetAlphamaps", HasExplicitThis = true)]
		private unsafe void Internal_SetAlphamaps(int x, int y, int width, int height, float[,,] map)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			int num;
			void* ptr;
			if (map == null || (num = map.Length) == 0)
			{
				num = 0;
				ptr = null;
			}
			else
			{
				ptr = (void*)(&map[0, 0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(ptr, num);
			TerrainData.Internal_SetAlphamaps_Injected(intPtr, x, y, width, height, ref managedSpanWrapper);
		}

		[NativeName("GetSplatDatabase().SetBaseMapsDirty")]
		public void SetBaseMapDirty()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.SetBaseMapDirty_Injected(intPtr);
		}

		[NativeName("GetSplatDatabase().GetAlphaTexture")]
		public Texture2D GetAlphamapTexture(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture2D>(TerrainData.GetAlphamapTexture_Injected(intPtr, index));
		}

		public int alphamapTextureCount
		{
			[NativeName("GetSplatDatabase().GetAlphaTextureCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_alphamapTextureCount_Injected(intPtr);
			}
		}

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

		[Obsolete("TerrainData.splatPrototypes is obsolete. Use TerrainData.terrainLayers instead.", false)]
		public SplatPrototype[] splatPrototypes
		{
			[FreeFunction("TerrainDataScriptingInterface::GetSplatPrototypes", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_splatPrototypes_Injected(intPtr);
			}
			[FreeFunction("TerrainDataScriptingInterface::SetSplatPrototypes", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_splatPrototypes_Injected(intPtr, value);
			}
		}

		public TerrainLayer[] terrainLayers
		{
			[FreeFunction("TerrainDataScriptingInterface::GetTerrainLayers", HasExplicitThis = true)]
			[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_terrainLayers_Injected(intPtr);
			}
			[FreeFunction("TerrainDataScriptingInterface::SetTerrainLayers", HasExplicitThis = true)]
			[param: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainData.set_terrainLayers_Injected(intPtr, value);
			}
		}

		[NativeName("GetTreeDatabase().AddTree")]
		internal void AddTree(ref TreeInstance tree)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.AddTree_Injected(intPtr, ref tree);
		}

		[NativeName("GetTreeDatabase().RemoveTrees")]
		internal int RemoveTrees(Vector2 position, float radius, int prototypeIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return TerrainData.RemoveTrees_Injected(intPtr, ref position, radius, prototypeIndex);
		}

		[NativeName("GetHeightmap().CopyHeightmapFromActiveRenderTexture")]
		private void Internal_CopyActiveRenderTextureToHeightmap(RectInt rect, int destX, int destY, TerrainHeightmapSyncControl syncControl)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.Internal_CopyActiveRenderTextureToHeightmap_Injected(intPtr, ref rect, destX, destY, syncControl);
		}

		[NativeName("GetHeightmap().DirtyHeightmapRegion")]
		private void Internal_DirtyHeightmapRegion(int x, int y, int width, int height, TerrainHeightmapSyncControl syncControl)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.Internal_DirtyHeightmapRegion_Injected(intPtr, x, y, width, height, syncControl);
		}

		[NativeName("GetHeightmap().SyncHeightmapGPUModifications")]
		public void SyncHeightmap()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.SyncHeightmap_Injected(intPtr);
		}

		[NativeName("GetHeightmap().CopyHolesFromActiveRenderTexture")]
		private void Internal_CopyActiveRenderTextureToHoles(RectInt rect, int destX, int destY, bool allowDelayedCPUSync)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.Internal_CopyActiveRenderTextureToHoles_Injected(intPtr, ref rect, destX, destY, allowDelayedCPUSync);
		}

		[NativeName("GetHeightmap().DirtyHolesRegion")]
		private void Internal_DirtyHolesRegion(int x, int y, int width, int height, bool allowDelayedCPUSync)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.Internal_DirtyHolesRegion_Injected(intPtr, x, y, width, height, allowDelayedCPUSync);
		}

		[NativeName("GetHeightmap().SyncHolesGPUModifications")]
		private void Internal_SyncHoles()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.Internal_SyncHoles_Injected(intPtr);
		}

		[NativeName("GetSplatDatabase().MarkDirtyRegion")]
		private void Internal_MarkAlphamapDirtyRegion(int alphamapIndex, int x, int y, int width, int height)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.Internal_MarkAlphamapDirtyRegion_Injected(intPtr, alphamapIndex, x, y, width, height);
		}

		[NativeName("GetSplatDatabase().ClearDirtyRegion")]
		private void Internal_ClearAlphamapDirtyRegion(int alphamapIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.Internal_ClearAlphamapDirtyRegion_Injected(intPtr, alphamapIndex);
		}

		[NativeName("GetSplatDatabase().SyncGPUModifications")]
		private void Internal_SyncAlphamaps()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TerrainData.Internal_SyncAlphamaps_Injected(intPtr);
		}

		internal TextureFormat atlasFormat
		{
			[NativeName("GetDetailDatabase().GetAtlasTextureFormat")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_atlasFormat_Injected(intPtr);
			}
		}

		internal Terrain[] users
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainData.get_users_Injected(intPtr);
			}
		}

		private static bool SupportsCopyTextureBetweenRTAndTexture
		{
			get
			{
				return (SystemInfo.copyTextureSupport & (CopyTextureSupport.TextureToRT | CopyTextureSupport.RTToTexture)) == (CopyTextureSupport.TextureToRT | CopyTextureSupport.RTToTexture);
			}
		}

		public void CopyActiveRenderTextureToHeightmap(RectInt sourceRect, Vector2Int dest, TerrainHeightmapSyncControl syncControl)
		{
			RenderTexture active = RenderTexture.active;
			bool flag = active == null;
			if (flag)
			{
				throw new InvalidOperationException("Active RenderTexture is null.");
			}
			bool flag2 = sourceRect.x < 0 || sourceRect.y < 0 || sourceRect.xMax > active.width || sourceRect.yMax > active.height;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("sourceRect");
			}
			bool flag3 = dest.x < 0 || dest.x + sourceRect.width > this.heightmapResolution;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("dest.x");
			}
			bool flag4 = dest.y < 0 || dest.y + sourceRect.height > this.heightmapResolution;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("dest.y");
			}
			this.Internal_CopyActiveRenderTextureToHeightmap(sourceRect, dest.x, dest.y, syncControl);
			TerrainCallbacks.InvokeHeightmapChangedCallback(this, new RectInt(dest.x, dest.y, sourceRect.width, sourceRect.height), syncControl == TerrainHeightmapSyncControl.HeightAndLod);
		}

		public void DirtyHeightmapRegion(RectInt region, TerrainHeightmapSyncControl syncControl)
		{
			int heightmapResolution = this.heightmapResolution;
			bool flag = region.x < 0 || region.x >= heightmapResolution;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("region.x");
			}
			bool flag2 = region.width <= 0 || region.xMax > heightmapResolution;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("region.width");
			}
			bool flag3 = region.y < 0 || region.y >= heightmapResolution;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("region.y");
			}
			bool flag4 = region.height <= 0 || region.yMax > heightmapResolution;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("region.height");
			}
			this.Internal_DirtyHeightmapRegion(region.x, region.y, region.width, region.height, syncControl);
			TerrainCallbacks.InvokeHeightmapChangedCallback(this, region, syncControl == TerrainHeightmapSyncControl.HeightAndLod);
		}

		public static string AlphamapTextureName
		{
			get
			{
				return "alphamap";
			}
		}

		public static string HolesTextureName
		{
			get
			{
				return "holes";
			}
		}

		public void CopyActiveRenderTextureToTexture(string textureName, int textureIndex, RectInt sourceRect, Vector2Int dest, bool allowDelayedCPUSync)
		{
			bool flag = string.IsNullOrEmpty(textureName);
			if (flag)
			{
				throw new ArgumentNullException("textureName");
			}
			RenderTexture active = RenderTexture.active;
			bool flag2 = active == null;
			if (flag2)
			{
				throw new InvalidOperationException("Active RenderTexture is null.");
			}
			bool flag3 = textureName == TerrainData.HolesTextureName;
			int num2;
			int num;
			if (flag3)
			{
				bool flag4 = textureIndex != 0;
				if (flag4)
				{
					throw new ArgumentOutOfRangeException("textureIndex");
				}
				bool flag5 = active == this.holesTexture;
				if (flag5)
				{
					throw new ArgumentException("source", "Active RenderTexture cannot be holesTexture.");
				}
				num = (num2 = this.holesResolution);
			}
			else
			{
				bool flag6 = textureName == TerrainData.AlphamapTextureName;
				if (!flag6)
				{
					throw new ArgumentException("Unrecognized terrain texture name: \"" + textureName + "\"");
				}
				bool flag7 = textureIndex < 0 || textureIndex >= this.alphamapTextureCount;
				if (flag7)
				{
					throw new ArgumentOutOfRangeException("textureIndex");
				}
				num = (num2 = this.alphamapResolution);
			}
			bool flag8 = sourceRect.x < 0 || sourceRect.y < 0 || sourceRect.xMax > active.width || sourceRect.yMax > active.height;
			if (flag8)
			{
				throw new ArgumentOutOfRangeException("sourceRect");
			}
			bool flag9 = dest.x < 0 || dest.x + sourceRect.width > num2;
			if (flag9)
			{
				throw new ArgumentOutOfRangeException("dest.x");
			}
			bool flag10 = dest.y < 0 || dest.y + sourceRect.height > num;
			if (flag10)
			{
				throw new ArgumentOutOfRangeException("dest.y");
			}
			bool flag11 = textureName == TerrainData.HolesTextureName;
			if (flag11)
			{
				this.Internal_CopyActiveRenderTextureToHoles(sourceRect, dest.x, dest.y, allowDelayedCPUSync);
			}
			else
			{
				Texture2D alphamapTexture = this.GetAlphamapTexture(textureIndex);
				allowDelayedCPUSync = allowDelayedCPUSync && TerrainData.SupportsCopyTextureBetweenRTAndTexture && QualitySettings.globalTextureMipmapLimit == 0;
				bool flag12 = allowDelayedCPUSync;
				if (flag12)
				{
					bool flag13 = alphamapTexture.mipmapCount > 1;
					if (flag13)
					{
						RenderTexture temporary = RenderTexture.GetTemporary(new RenderTextureDescriptor(alphamapTexture.width, alphamapTexture.height, active.graphicsFormat, active.depthStencilFormat)
						{
							sRGB = false,
							useMipMap = true,
							autoGenerateMips = false
						});
						bool flag14 = !temporary.IsCreated();
						if (flag14)
						{
							temporary.Create();
						}
						Graphics.CopyTexture(alphamapTexture, 0, 0, temporary, 0, 0);
						Graphics.CopyTexture(active, 0, 0, sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height, temporary, 0, 0, dest.x, dest.y);
						temporary.GenerateMips();
						Graphics.CopyTexture(temporary, alphamapTexture);
						RenderTexture.ReleaseTemporary(temporary);
					}
					else
					{
						Graphics.CopyTexture(active, 0, 0, sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height, alphamapTexture, 0, 0, dest.x, dest.y);
					}
					this.Internal_MarkAlphamapDirtyRegion(textureIndex, dest.x, dest.y, sourceRect.width, sourceRect.height);
				}
				else
				{
					alphamapTexture.ReadPixels(new Rect((float)sourceRect.x, (float)sourceRect.y, (float)sourceRect.width, (float)sourceRect.height), dest.x, dest.y);
					alphamapTexture.Apply(true);
					this.Internal_ClearAlphamapDirtyRegion(textureIndex);
				}
				TerrainCallbacks.InvokeTextureChangedCallback(this, textureName, new RectInt(dest.x, dest.y, sourceRect.width, sourceRect.height), !allowDelayedCPUSync);
			}
		}

		public void DirtyTextureRegion(string textureName, RectInt region, bool allowDelayedCPUSync)
		{
			bool flag = string.IsNullOrEmpty(textureName);
			if (flag)
			{
				throw new ArgumentNullException("textureName");
			}
			bool flag2 = textureName == TerrainData.AlphamapTextureName;
			int num;
			if (flag2)
			{
				num = this.alphamapResolution;
			}
			else
			{
				bool flag3 = textureName == TerrainData.HolesTextureName;
				if (!flag3)
				{
					throw new ArgumentException("Unrecognized terrain texture name: \"" + textureName + "\"");
				}
				num = this.holesResolution;
			}
			bool flag4 = region.x < 0 || region.x >= num;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("region.x");
			}
			bool flag5 = region.width <= 0 || region.xMax > num;
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("region.width");
			}
			bool flag6 = region.y < 0 || region.y >= num;
			if (flag6)
			{
				throw new ArgumentOutOfRangeException("region.y");
			}
			bool flag7 = region.height <= 0 || region.yMax > num;
			if (flag7)
			{
				throw new ArgumentOutOfRangeException("region.height");
			}
			bool flag8 = textureName == TerrainData.HolesTextureName;
			if (flag8)
			{
				this.Internal_DirtyHolesRegion(region.x, region.y, region.width, region.height, allowDelayedCPUSync);
			}
			else
			{
				this.Internal_MarkAlphamapDirtyRegion(-1, region.x, region.y, region.width, region.height);
				bool flag9 = !allowDelayedCPUSync;
				if (flag9)
				{
					this.SyncTexture(textureName);
				}
				else
				{
					TerrainCallbacks.InvokeTextureChangedCallback(this, textureName, region, false);
				}
			}
		}

		public void SyncTexture(string textureName)
		{
			bool flag = string.IsNullOrEmpty(textureName);
			if (flag)
			{
				throw new ArgumentNullException("textureName");
			}
			bool flag2 = textureName == TerrainData.AlphamapTextureName;
			if (flag2)
			{
				this.Internal_SyncAlphamaps();
			}
			else
			{
				bool flag3 = textureName == TerrainData.HolesTextureName;
				if (!flag3)
				{
					throw new ArgumentException("Unrecognized terrain texture name: \"" + textureName + "\"");
				}
				bool flag4 = this.IsHolesTextureCompressed();
				if (flag4)
				{
					throw new InvalidOperationException("Holes texture is compressed. Compressed holes texture can not be read back from GPU. Use TerrainData.enableHolesTextureCompression to disable holes texture compression.");
				}
				this.Internal_SyncHoles();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_heightmapTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_internalHeightmapResolution_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_internalHeightmapResolution_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_heightmapScale_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableHolesTextureCompression_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableHolesTextureCompression_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsHolesTextureCompressed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetHolesTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetCompressedHolesTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_size_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_size_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetHeight_Injected(IntPtr _unity_self, int x, int y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetInterpolatedHeight_Injected(IntPtr _unity_self, float x, float y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetInterpolatedHeights_Injected(IntPtr _unity_self, ref ManagedSpanWrapper results, int resultXDimension, int resultXOffset, int resultYOffset, float xBase, float yBase, int xCount, int yCount, float xInterval, float yInterval);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float[,] Internal_GetHeights_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetHeights_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height, ref ManagedSpanWrapper heights);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPatchMinMaxHeights_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OverrideMinMaxPatchHeights_Injected(IntPtr _unity_self, ref ManagedSpanWrapper minMaxHeights);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMaximumHeightError_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OverrideMaximumHeightError_Injected(IntPtr _unity_self, ref ManagedSpanWrapper maxError);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetHeightsDelayLOD_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height, ref ManagedSpanWrapper heights);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetHoles_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height, ref ManagedSpanWrapper holes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool[,] Internal_GetHoles_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_IsHole_Injected(IntPtr _unity_self, int x, int y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetHolesDelayLOD_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height, ref ManagedSpanWrapper holes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetSteepness_Injected(IntPtr _unity_self, float x, float y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetInterpolatedNormal_Injected(IntPtr _unity_self, float x, float y, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAdjustedSize_Injected(IntPtr _unity_self, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_wavingGrassStrength_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wavingGrassStrength_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_wavingGrassAmount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wavingGrassAmount_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_wavingGrassSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wavingGrassSpeed_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_wavingGrassTint_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wavingGrassTint_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_detailWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_detailHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_maxDetailScatterPerRes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetDetailResolution_Injected(IntPtr _unity_self, int patchCount, int resolutionPerPatch);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetDetailScatterMode_Injected(IntPtr _unity_self, DetailScatterMode scatterMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_detailPatchCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_detailResolution_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_detailResolutionPerPatch_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DetailScatterMode get_detailScatterMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetDirtyDetails_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RefreshPrototypes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DetailPrototype[] get_detailPrototypes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_detailPrototypes_Injected(IntPtr _unity_self, DetailPrototype[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSupportedLayers_Injected(IntPtr _unity_self, int xBase, int yBase, int totalWidth, int totalHeight, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int[,] GetDetailLayer_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height, int layer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ComputeDetailInstanceTransforms_Injected(IntPtr _unity_self, int patchX, int patchY, int layer, float density, out Bounds bounds, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float ComputeDetailCoverage_Injected(IntPtr _unity_self, int detailPrototypeIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetDetailLayer_Injected(IntPtr _unity_self, int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, ref ManagedSpanWrapper data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetTreeInstances_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTreeInstances_Injected(IntPtr _unity_self, ref ManagedSpanWrapper instances, bool snapToHeightmap);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetTreeInstance_Injected(IntPtr _unity_self, int index, out TreeInstance ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTreeInstance_Injected(IntPtr _unity_self, int index, [In] ref TreeInstance instance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_treeInstanceCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TreePrototype[] get_treePrototypes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_treePrototypes_Injected(IntPtr _unity_self, TreePrototype[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveTreePrototype_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveDetailPrototype_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool NeedUpgradeScaledTreePrototypes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpgradeScaledTreePrototype_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_alphamapLayers_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float[,,] Internal_GetAlphamaps_Injected(IntPtr _unity_self, int x, int y, int width, int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAlphamapResolutionInternal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_Internal_alphamapResolution_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_Internal_alphamapResolution_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_Internal_baseMapResolution_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_Internal_baseMapResolution_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetAlphamaps_Injected(IntPtr _unity_self, int x, int y, int width, int height, ref ManagedSpanWrapper map);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBaseMapDirty_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetAlphamapTexture_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_alphamapTextureCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SplatPrototype[] get_splatPrototypes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_splatPrototypes_Injected(IntPtr _unity_self, SplatPrototype[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TerrainLayer[] get_terrainLayers_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_terrainLayers_Injected(IntPtr _unity_self, TerrainLayer[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddTree_Injected(IntPtr _unity_self, ref TreeInstance tree);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RemoveTrees_Injected(IntPtr _unity_self, [In] ref Vector2 position, float radius, int prototypeIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CopyActiveRenderTextureToHeightmap_Injected(IntPtr _unity_self, [In] ref RectInt rect, int destX, int destY, TerrainHeightmapSyncControl syncControl);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DirtyHeightmapRegion_Injected(IntPtr _unity_self, int x, int y, int width, int height, TerrainHeightmapSyncControl syncControl);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SyncHeightmap_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CopyActiveRenderTextureToHoles_Injected(IntPtr _unity_self, [In] ref RectInt rect, int destX, int destY, bool allowDelayedCPUSync);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DirtyHolesRegion_Injected(IntPtr _unity_self, int x, int y, int width, int height, bool allowDelayedCPUSync);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SyncHoles_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_MarkAlphamapDirtyRegion_Injected(IntPtr _unity_self, int alphamapIndex, int x, int y, int width, int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ClearAlphamapDirtyRegion_Injected(IntPtr _unity_self, int alphamapIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SyncAlphamaps_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureFormat get_atlasFormat_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Terrain[] get_users_Injected(IntPtr _unity_self);

		private const string k_ScriptingInterfaceName = "TerrainDataScriptingInterface";

		private const string k_ScriptingInterfacePrefix = "TerrainDataScriptingInterface::";

		private const string k_HeightmapPrefix = "GetHeightmap().";

		private const string k_DetailDatabasePrefix = "GetDetailDatabase().";

		private const string k_TreeDatabasePrefix = "GetTreeDatabase().";

		private const string k_SplatDatabasePrefix = "GetSplatDatabase().";

		internal static readonly int k_MaximumResolution = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MaxHeightmapRes);

		internal static readonly int k_MinimumDetailResolutionPerPatch = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MinDetailResPerPatch);

		internal static readonly int k_MaximumDetailResolutionPerPatch = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MaxDetailResPerPatch);

		internal static readonly int k_MaximumDetailPatchCount = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MaxDetailPatchCount);

		internal static readonly int k_MinimumAlphamapResolution = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MinAlphamapRes);

		internal static readonly int k_MaximumAlphamapResolution = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MaxAlphamapRes);

		internal static readonly int k_MinimumBaseMapResolution = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MinBaseMapRes);

		internal static readonly int k_MaximumBaseMapResolution = TerrainData.GetBoundaryValue(TerrainData.BoundaryValueType.MaxBaseMapRes);

		private enum BoundaryValueType
		{
			MaxHeightmapRes,
			MinDetailResPerPatch,
			MaxDetailResPerPatch,
			MaxDetailPatchCount,
			MaxCoveragePerRes,
			MinAlphamapRes,
			MaxAlphamapRes,
			MinBaseMapRes,
			MaxBaseMapRes
		}
	}
}
