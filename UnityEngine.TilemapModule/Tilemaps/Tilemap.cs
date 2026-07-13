using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps
{
	[NativeHeader("Modules/Tilemap/Public/TilemapMarshalling.h")]
	[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
	[NativeHeader("Modules/Tilemap/Public/TilemapTile.h")]
	[NativeHeader("Modules/Grid/Public/Grid.h")]
	[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
	[RequireComponent(typeof(Transform))]
	[NativeType(Header = "Modules/Tilemap/Public/Tilemap.h")]
	public sealed class Tilemap : GridLayout
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Tilemap, Tilemap.SyncTile[]> tilemapTileChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Tilemap, NativeArray<Vector3Int>> tilemapPositionsChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Tilemap, NativeArray<Vector3Int>> loopEndedForTileAnimation;

		internal bool bufferSyncTile
		{
			get
			{
				return this.m_BufferSyncTile;
			}
			set
			{
				bool flag = !value && this.m_BufferSyncTile != value && Tilemap.HasSyncTileCallback();
				if (flag)
				{
					this.SendAndClearSyncTileBuffer();
				}
				this.m_BufferSyncTile = value;
			}
		}

		internal static bool HasLoopEndedForTileAnimationCallback()
		{
			return Tilemap.loopEndedForTileAnimation != null;
		}

		private unsafe void HandleLoopEndedForTileAnimationCallback(int count, IntPtr positionsIntPtr)
		{
			bool flag = !Tilemap.HasLoopEndedForTileAnimationCallback();
			if (!flag)
			{
				void* ptr = positionsIntPtr.ToPointer();
				NativeArray<Vector3Int> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3Int>(ptr, count, Allocator.Invalid);
				this.SendLoopEndedForTileAnimationCallback(nativeArray);
			}
		}

		private void SendLoopEndedForTileAnimationCallback(NativeArray<Vector3Int> positions)
		{
			try
			{
				Tilemap.loopEndedForTileAnimation(this, positions);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex, this);
			}
		}

		internal static bool HasSyncTileCallback()
		{
			return Tilemap.tilemapTileChanged != null;
		}

		internal static bool HasPositionsChangedCallback()
		{
			return Tilemap.tilemapPositionsChanged != null;
		}

		private void HandleSyncTileCallback(Tilemap.SyncTile[] syncTiles)
		{
			bool flag = Tilemap.tilemapTileChanged == null;
			if (!flag)
			{
				this.SendTilemapTileChangedCallback(syncTiles);
			}
		}

		private unsafe void HandlePositionsChangedCallback(int count, IntPtr positionsIntPtr)
		{
			bool flag = !Tilemap.HasPositionsChangedCallback();
			if (!flag)
			{
				void* ptr = positionsIntPtr.ToPointer();
				NativeArray<Vector3Int> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3Int>(ptr, count, Allocator.Invalid);
				this.SendTilemapPositionsChangedCallback(nativeArray);
			}
		}

		private void SendTilemapTileChangedCallback(Tilemap.SyncTile[] syncTiles)
		{
			try
			{
				Tilemap.tilemapTileChanged(this, syncTiles);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex, this);
			}
		}

		private void SendTilemapPositionsChangedCallback(NativeArray<Vector3Int> positions)
		{
			try
			{
				Tilemap.tilemapPositionsChanged(this, positions);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex, this);
			}
		}

		internal static void SetSyncTileCallback(Action<Tilemap, Tilemap.SyncTile[]> callback)
		{
			Tilemap.tilemapTileChanged += callback;
		}

		internal static void RemoveSyncTileCallback(Action<Tilemap, Tilemap.SyncTile[]> callback)
		{
			Tilemap.tilemapTileChanged -= callback;
		}

		public Grid layoutGrid
		{
			[NativeMethod(Name = "GetAttachedGrid")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Grid>(Tilemap.get_layoutGrid_Injected(intPtr));
			}
		}

		public Vector3 GetCellCenterLocal(Vector3Int position)
		{
			return base.CellToLocalInterpolated(position) + base.CellToLocalInterpolated(this.tileAnchor);
		}

		public Vector3 GetCellCenterWorld(Vector3Int position)
		{
			return base.LocalToWorld(base.CellToLocalInterpolated(position) + base.CellToLocalInterpolated(this.tileAnchor));
		}

		public BoundsInt cellBounds
		{
			get
			{
				return new BoundsInt(this.origin, this.size);
			}
		}

		[NativeProperty("TilemapBoundsScripting")]
		public Bounds localBounds
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				Tilemap.get_localBounds_Injected(intPtr, out bounds);
				return bounds;
			}
		}

		[NativeProperty("TilemapFrameBoundsScripting")]
		internal Bounds localFrameBounds
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				Tilemap.get_localFrameBounds_Injected(intPtr, out bounds);
				return bounds;
			}
		}

		public float animationFrameRate
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Tilemap.get_animationFrameRate_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Tilemap.set_animationFrameRate_Injected(intPtr, value);
			}
		}

		public Color color
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				Tilemap.get_color_Injected(intPtr, out color);
				return color;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Tilemap.set_color_Injected(intPtr, ref value);
			}
		}

		public Vector3Int origin
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3Int vector3Int;
				Tilemap.get_origin_Injected(intPtr, out vector3Int);
				return vector3Int;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Tilemap.set_origin_Injected(intPtr, ref value);
			}
		}

		public Vector3Int size
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3Int vector3Int;
				Tilemap.get_size_Injected(intPtr, out vector3Int);
				return vector3Int;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Tilemap.set_size_Injected(intPtr, ref value);
			}
		}

		[NativeProperty(Name = "TileAnchorScripting")]
		public Vector3 tileAnchor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Tilemap.get_tileAnchor_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Tilemap.set_tileAnchor_Injected(intPtr, ref value);
			}
		}

		public Tilemap.Orientation orientation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Tilemap.get_orientation_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Tilemap.set_orientation_Injected(intPtr, value);
			}
		}

		public Matrix4x4 orientationMatrix
		{
			[NativeMethod(Name = "GetTileOrientationMatrix")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Tilemap.get_orientationMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
			[NativeMethod(Name = "SetOrientationMatrix")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Tilemap.set_orientationMatrix_Injected(intPtr, ref value);
			}
		}

		internal Object GetTileAsset(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Object>(Tilemap.GetTileAsset_Injected(intPtr, ref position));
		}

		public TileBase GetTile(Vector3Int position)
		{
			return this.GetTileAsset(position) as TileBase;
		}

		public T GetTile<T>(Vector3Int position) where T : TileBase
		{
			return this.GetTileAsset(position) as T;
		}

		internal Object[] GetTileAssetsBlock(Vector3Int position, Vector3Int blockDimensions)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetTileAssetsBlock_Injected(intPtr, ref position, ref blockDimensions);
		}

		public TileBase[] GetTilesBlock(BoundsInt bounds)
		{
			Object[] tileAssetsBlock = this.GetTileAssetsBlock(bounds.min, bounds.size);
			TileBase[] array = new TileBase[tileAssetsBlock.Length];
			for (int i = 0; i < tileAssetsBlock.Length; i++)
			{
				array[i] = (TileBase)tileAssetsBlock[i];
			}
			return array;
		}

		[FreeFunction(Name = "TilemapBindings::GetTileAssetsBlockNonAlloc", HasExplicitThis = true)]
		internal int GetTileAssetsBlockNonAlloc(Vector3Int startPosition, Vector3Int endPosition, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] Object[] tiles)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetTileAssetsBlockNonAlloc_Injected(intPtr, ref startPosition, ref endPosition, tiles);
		}

		public int GetTilesBlockNonAlloc(BoundsInt bounds, TileBase[] tiles)
		{
			return this.GetTileAssetsBlockNonAlloc(bounds.min, bounds.size, tiles);
		}

		public int GetTilesRangeCount(Vector3Int startPosition, Vector3Int endPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetTilesRangeCount_Injected(intPtr, ref startPosition, ref endPosition);
		}

		[FreeFunction(Name = "TilemapBindings::GetTileAssetsRangeNonAlloc", HasExplicitThis = true)]
		internal unsafe int GetTileAssetsRangeNonAlloc(Vector3Int startPosition, Vector3Int endPosition, Vector3Int[] positions, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] Object[] tiles)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector3Int> span = new Span<Vector3Int>(positions);
			int tileAssetsRangeNonAlloc_Injected;
			fixed (Vector3Int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				tileAssetsRangeNonAlloc_Injected = Tilemap.GetTileAssetsRangeNonAlloc_Injected(intPtr, ref startPosition, ref endPosition, ref managedSpanWrapper, tiles);
			}
			return tileAssetsRangeNonAlloc_Injected;
		}

		public int GetTilesRangeNonAlloc(Vector3Int startPosition, Vector3Int endPosition, Vector3Int[] positions, TileBase[] tiles)
		{
			return this.GetTileAssetsRangeNonAlloc(startPosition, endPosition, positions, tiles);
		}

		internal void SetTileAsset(Vector3Int position, Object tile)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SetTileAsset_Injected(intPtr, ref position, Object.MarshalledUnityObject.Marshal<Object>(tile));
		}

		public void SetTile(Vector3Int position, TileBase tile)
		{
			this.SetTileAsset(position, tile);
		}

		internal unsafe void SetTileAssets(Vector3Int[] positionArray, Object[] tileArray)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector3Int> span = new Span<Vector3Int>(positionArray);
			fixed (Vector3Int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Tilemap.SetTileAssets_Injected(intPtr, ref managedSpanWrapper, tileArray);
			}
		}

		public void SetTiles(Vector3Int[] positionArray, TileBase[] tileArray)
		{
			this.SetTileAssets(positionArray, tileArray);
		}

		[NativeMethod(Name = "SetTileAssetsBlock")]
		private void INTERNAL_CALL_SetTileAssetsBlock(Vector3Int position, Vector3Int blockDimensions, Object[] tileArray)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.INTERNAL_CALL_SetTileAssetsBlock_Injected(intPtr, ref position, ref blockDimensions, tileArray);
		}

		public void SetTilesBlock(BoundsInt position, TileBase[] tileArray)
		{
			this.INTERNAL_CALL_SetTileAssetsBlock(position.min, position.size, tileArray);
		}

		[NativeMethod(Name = "SetTileChangeData")]
		public void SetTile(TileChangeData tileChangeData, bool ignoreLockFlags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SetTile_Injected(intPtr, ref tileChangeData, ignoreLockFlags);
		}

		[NativeMethod(Name = "SetTileChangeDataArray")]
		public void SetTiles(TileChangeData[] tileChangeDataArray, bool ignoreLockFlags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SetTiles_Injected(intPtr, tileChangeDataArray, ignoreLockFlags);
		}

		public bool HasTile(Vector3Int position)
		{
			return this.GetTileAsset(position) != null;
		}

		[NativeMethod(Name = "RefreshTileAsset")]
		public void RefreshTile(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.RefreshTile_Injected(intPtr, ref position);
		}

		[FreeFunction(Name = "TilemapBindings::RefreshTileAssetsNative", HasExplicitThis = true)]
		internal unsafe void RefreshTilesNative(void* positions, int count)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.RefreshTilesNative_Injected(intPtr, positions, count);
		}

		[NativeMethod(Name = "RefreshAllTileAssets")]
		public void RefreshAllTiles()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.RefreshAllTiles_Injected(intPtr);
		}

		internal void SwapTileAsset(Object changeTile, Object newTile)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SwapTileAsset_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Object>(changeTile), Object.MarshalledUnityObject.Marshal<Object>(newTile));
		}

		public void SwapTile(TileBase changeTile, TileBase newTile)
		{
			this.SwapTileAsset(changeTile, newTile);
		}

		internal bool ContainsTileAsset(Object tileAsset)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.ContainsTileAsset_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Object>(tileAsset));
		}

		public bool ContainsTile(TileBase tileAsset)
		{
			return this.ContainsTileAsset(tileAsset);
		}

		public int GetUsedTilesCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetUsedTilesCount_Injected(intPtr);
		}

		public int GetUsedSpritesCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetUsedSpritesCount_Injected(intPtr);
		}

		public int GetUsedTilesNonAlloc(TileBase[] usedTiles)
		{
			return this.Internal_GetUsedTilesNonAlloc(usedTiles);
		}

		public int GetUsedSpritesNonAlloc(Sprite[] usedSprites)
		{
			return this.Internal_GetUsedSpritesNonAlloc(usedSprites);
		}

		[FreeFunction(Name = "TilemapBindings::GetUsedTilesNonAlloc", HasExplicitThis = true)]
		internal int Internal_GetUsedTilesNonAlloc([UnityMarshalAs(NativeType.ScriptingObjectPtr)] Object[] usedTiles)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.Internal_GetUsedTilesNonAlloc_Injected(intPtr, usedTiles);
		}

		[FreeFunction(Name = "TilemapBindings::GetUsedSpritesNonAlloc", HasExplicitThis = true)]
		internal int Internal_GetUsedSpritesNonAlloc([UnityMarshalAs(NativeType.ScriptingObjectPtr)] Object[] usedSprites)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.Internal_GetUsedSpritesNonAlloc_Injected(intPtr, usedSprites);
		}

		public Sprite GetSprite(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Sprite>(Tilemap.GetSprite_Injected(intPtr, ref position));
		}

		public Matrix4x4 GetTransformMatrix(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			Tilemap.GetTransformMatrix_Injected(intPtr, ref position, out matrix4x);
			return matrix4x;
		}

		public void SetTransformMatrix(Vector3Int position, Matrix4x4 transform)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SetTransformMatrix_Injected(intPtr, ref position, ref transform);
		}

		[NativeMethod(Name = "GetTileColor")]
		public Color GetColor(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Color color;
			Tilemap.GetColor_Injected(intPtr, ref position, out color);
			return color;
		}

		[NativeMethod(Name = "SetTileColor")]
		public void SetColor(Vector3Int position, Color color)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SetColor_Injected(intPtr, ref position, ref color);
		}

		public TileFlags GetTileFlags(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetTileFlags_Injected(intPtr, ref position);
		}

		public void SetTileFlags(Vector3Int position, TileFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SetTileFlags_Injected(intPtr, ref position, flags);
		}

		public void AddTileFlags(Vector3Int position, TileFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.AddTileFlags_Injected(intPtr, ref position, flags);
		}

		public void RemoveTileFlags(Vector3Int position, TileFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.RemoveTileFlags_Injected(intPtr, ref position, flags);
		}

		[NativeMethod(Name = "GetTileInstantiatedObject")]
		public GameObject GetInstantiatedObject(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<GameObject>(Tilemap.GetInstantiatedObject_Injected(intPtr, ref position));
		}

		[NativeMethod(Name = "GetTileObjectToInstantiate")]
		public GameObject GetObjectToInstantiate(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<GameObject>(Tilemap.GetObjectToInstantiate_Injected(intPtr, ref position));
		}

		[NativeMethod(Name = "SetTileColliderType")]
		public void SetColliderType(Vector3Int position, Tile.ColliderType colliderType)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SetColliderType_Injected(intPtr, ref position, colliderType);
		}

		[NativeMethod(Name = "GetTileColliderType")]
		public Tile.ColliderType GetColliderType(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetColliderType_Injected(intPtr, ref position);
		}

		[NativeMethod(Name = "GetTileAnimationFrameCount")]
		public int GetAnimationFrameCount(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetAnimationFrameCount_Injected(intPtr, ref position);
		}

		[NativeMethod(Name = "GetTileAnimationFrame")]
		public int GetAnimationFrame(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetAnimationFrame_Injected(intPtr, ref position);
		}

		[NativeMethod(Name = "SetTileAnimationFrame")]
		public void SetAnimationFrame(Vector3Int position, int frame)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SetAnimationFrame_Injected(intPtr, ref position, frame);
		}

		[NativeMethod(Name = "GetTileAnimationTime")]
		public float GetAnimationTime(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetAnimationTime_Injected(intPtr, ref position);
		}

		[NativeMethod(Name = "SetTileAnimationTime")]
		public void SetAnimationTime(Vector3Int position, float time)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SetAnimationTime_Injected(intPtr, ref position, time);
		}

		public TileAnimationFlags GetTileAnimationFlags(Vector3Int position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Tilemap.GetTileAnimationFlags_Injected(intPtr, ref position);
		}

		public void SetTileAnimationFlags(Vector3Int position, TileAnimationFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SetTileAnimationFlags_Injected(intPtr, ref position, flags);
		}

		public void AddTileAnimationFlags(Vector3Int position, TileAnimationFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.AddTileAnimationFlags_Injected(intPtr, ref position, flags);
		}

		public void RemoveTileAnimationFlags(Vector3Int position, TileAnimationFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.RemoveTileAnimationFlags_Injected(intPtr, ref position, flags);
		}

		public void FloodFill(Vector3Int position, TileBase tile)
		{
			this.FloodFillTileAsset(position, tile);
		}

		[NativeMethod(Name = "FloodFill")]
		private void FloodFillTileAsset(Vector3Int position, Object tile)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.FloodFillTileAsset_Injected(intPtr, ref position, Object.MarshalledUnityObject.Marshal<Object>(tile));
		}

		public void BoxFill(Vector3Int position, TileBase tile, int startX, int startY, int endX, int endY)
		{
			this.BoxFillTileAsset(position, tile, startX, startY, endX, endY);
		}

		[NativeMethod(Name = "BoxFill")]
		private void BoxFillTileAsset(Vector3Int position, Object tile, int startX, int startY, int endX, int endY)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.BoxFillTileAsset_Injected(intPtr, ref position, Object.MarshalledUnityObject.Marshal<Object>(tile), startX, startY, endX, endY);
		}

		public void InsertCells(Vector3Int position, Vector3Int insertCells)
		{
			this.InsertCells(position, insertCells.x, insertCells.y, insertCells.z);
		}

		public void InsertCells(Vector3Int position, int numColumns, int numRows, int numLayers)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.InsertCells_Injected(intPtr, ref position, numColumns, numRows, numLayers);
		}

		public void DeleteCells(Vector3Int position, Vector3Int deleteCells)
		{
			this.DeleteCells(position, deleteCells.x, deleteCells.y, deleteCells.z);
		}

		public void DeleteCells(Vector3Int position, int numColumns, int numRows, int numLayers)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.DeleteCells_Injected(intPtr, ref position, numColumns, numRows, numLayers);
		}

		public void ClearAllTiles()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.ClearAllTiles_Injected(intPtr);
		}

		public void ResizeBounds()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.ResizeBounds_Injected(intPtr);
		}

		[NativeMethod(Name = "CompressBounds")]
		private void CompressTilemapBounds(bool keepEditorPreview)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.CompressTilemapBounds_Injected(intPtr, keepEditorPreview);
		}

		public void CompressBounds()
		{
			this.CompressTilemapBounds(false);
		}

		[RequiredByNativeCode]
		internal void GetLoopEndedForTileAnimationCallbackSettings(ref bool hasEndLoopForTileAnimationCallback)
		{
			hasEndLoopForTileAnimationCallback = Tilemap.HasLoopEndedForTileAnimationCallback();
		}

		[RequiredByNativeCode]
		private void DoLoopEndedForTileAnimationCallback(int count, IntPtr positionsIntPtr)
		{
			this.HandleLoopEndedForTileAnimationCallback(count, positionsIntPtr);
		}

		[RequiredByNativeCode]
		internal void GetSyncTileCallbackSettings(ref Tilemap.SyncTileCallbackSettings settings)
		{
			settings.hasSyncTileCallback = Tilemap.HasSyncTileCallback();
			settings.hasPositionsChangedCallback = Tilemap.HasPositionsChangedCallback();
			settings.isBufferSyncTile = this.bufferSyncTile;
		}

		internal void SendAndClearSyncTileBuffer()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Tilemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Tilemap.SendAndClearSyncTileBuffer_Injected(intPtr);
		}

		[RequiredByNativeCode]
		private void DoSyncTileCallback(Tilemap.SyncTile[] syncTiles)
		{
			this.HandleSyncTileCallback(syncTiles);
		}

		[RequiredByNativeCode]
		private void DoPositionsChangedCallback(int count, IntPtr positionsIntPtr)
		{
			this.HandlePositionsChangedCallback(count, positionsIntPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_layoutGrid_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localBounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localFrameBounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_animationFrameRate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_animationFrameRate_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_color_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_color_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_origin_Injected(IntPtr _unity_self, out Vector3Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_origin_Injected(IntPtr _unity_self, [In] ref Vector3Int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_size_Injected(IntPtr _unity_self, out Vector3Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_size_Injected(IntPtr _unity_self, [In] ref Vector3Int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_tileAnchor_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_tileAnchor_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Tilemap.Orientation get_orientation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_orientation_Injected(IntPtr _unity_self, Tilemap.Orientation value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_orientationMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_orientationMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetTileAsset_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object[] GetTileAssetsBlock_Injected(IntPtr _unity_self, [In] ref Vector3Int position, [In] ref Vector3Int blockDimensions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetTileAssetsBlockNonAlloc_Injected(IntPtr _unity_self, [In] ref Vector3Int startPosition, [In] ref Vector3Int endPosition, Object[] tiles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetTilesRangeCount_Injected(IntPtr _unity_self, [In] ref Vector3Int startPosition, [In] ref Vector3Int endPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetTileAssetsRangeNonAlloc_Injected(IntPtr _unity_self, [In] ref Vector3Int startPosition, [In] ref Vector3Int endPosition, ref ManagedSpanWrapper positions, Object[] tiles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTileAsset_Injected(IntPtr _unity_self, [In] ref Vector3Int position, IntPtr tile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTileAssets_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positionArray, Object[] tileArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTileAssetsBlock_Injected(IntPtr _unity_self, [In] ref Vector3Int position, [In] ref Vector3Int blockDimensions, Object[] tileArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTile_Injected(IntPtr _unity_self, [In] ref TileChangeData tileChangeData, bool ignoreLockFlags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTiles_Injected(IntPtr _unity_self, TileChangeData[] tileChangeDataArray, bool ignoreLockFlags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RefreshTile_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void RefreshTilesNative_Injected(IntPtr _unity_self, void* positions, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RefreshAllTiles_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SwapTileAsset_Injected(IntPtr _unity_self, IntPtr changeTile, IntPtr newTile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContainsTileAsset_Injected(IntPtr _unity_self, IntPtr tileAsset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetUsedTilesCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetUsedSpritesCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetUsedTilesNonAlloc_Injected(IntPtr _unity_self, Object[] usedTiles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetUsedSpritesNonAlloc_Injected(IntPtr _unity_self, Object[] usedSprites);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSprite_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTransformMatrix_Injected(IntPtr _unity_self, [In] ref Vector3Int position, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTransformMatrix_Injected(IntPtr _unity_self, [In] ref Vector3Int position, [In] ref Matrix4x4 transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetColor_Injected(IntPtr _unity_self, [In] ref Vector3Int position, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetColor_Injected(IntPtr _unity_self, [In] ref Vector3Int position, [In] ref Color color);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TileFlags GetTileFlags_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTileFlags_Injected(IntPtr _unity_self, [In] ref Vector3Int position, TileFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddTileFlags_Injected(IntPtr _unity_self, [In] ref Vector3Int position, TileFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveTileFlags_Injected(IntPtr _unity_self, [In] ref Vector3Int position, TileFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetInstantiatedObject_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetObjectToInstantiate_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetColliderType_Injected(IntPtr _unity_self, [In] ref Vector3Int position, Tile.ColliderType colliderType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Tile.ColliderType GetColliderType_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAnimationFrameCount_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAnimationFrame_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAnimationFrame_Injected(IntPtr _unity_self, [In] ref Vector3Int position, int frame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAnimationTime_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAnimationTime_Injected(IntPtr _unity_self, [In] ref Vector3Int position, float time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TileAnimationFlags GetTileAnimationFlags_Injected(IntPtr _unity_self, [In] ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTileAnimationFlags_Injected(IntPtr _unity_self, [In] ref Vector3Int position, TileAnimationFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddTileAnimationFlags_Injected(IntPtr _unity_self, [In] ref Vector3Int position, TileAnimationFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveTileAnimationFlags_Injected(IntPtr _unity_self, [In] ref Vector3Int position, TileAnimationFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FloodFillTileAsset_Injected(IntPtr _unity_self, [In] ref Vector3Int position, IntPtr tile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BoxFillTileAsset_Injected(IntPtr _unity_self, [In] ref Vector3Int position, IntPtr tile, int startX, int startY, int endX, int endY);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InsertCells_Injected(IntPtr _unity_self, [In] ref Vector3Int position, int numColumns, int numRows, int numLayers);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DeleteCells_Injected(IntPtr _unity_self, [In] ref Vector3Int position, int numColumns, int numRows, int numLayers);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearAllTiles_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResizeBounds_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CompressTilemapBounds_Injected(IntPtr _unity_self, bool keepEditorPreview);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendAndClearSyncTileBuffer_Injected(IntPtr _unity_self);

		private bool m_BufferSyncTile;

		public enum Orientation
		{
			XY,
			XZ,
			YX,
			YZ,
			ZX,
			ZY,
			Custom
		}

		[RequiredByNativeCode]
		public struct SyncTile
		{
			public Vector3Int position
			{
				get
				{
					return this.m_Position;
				}
			}

			public TileBase tile
			{
				get
				{
					return this.m_Tile;
				}
			}

			public TileData tileData
			{
				get
				{
					return this.m_TileData;
				}
			}

			internal Vector3Int m_Position;

			internal TileBase m_Tile;

			internal TileData m_TileData;
		}

		internal struct SyncTileCallbackSettings
		{
			internal bool hasSyncTileCallback;

			internal bool hasPositionsChangedCallback;

			internal bool isBufferSyncTile;
		}
	}
}
