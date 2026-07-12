using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps
{
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
	[NativeHeader("Modules/Grid/Public/Grid.h")]
	[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
	[NativeHeader("Modules/Tilemap/Public/TilemapTile.h")]
	[NativeHeader("Modules/Tilemap/Public/TilemapMarshalling.h")]
	[NativeType(Header = "Modules/Tilemap/Public/Tilemap.h")]
	public sealed class Tilemap : GridLayout
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Tilemap, Tilemap.SyncTile[]> tilemapTileChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Tilemap, NativeArray<Vector3Int>> tilemapPositionsChanged;

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
			bool flag = Tilemap.tilemapPositionsChanged == null;
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

		public extern Grid layoutGrid
		{
			[NativeMethod(Name = "GetAttachedGrid")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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
				Bounds bounds;
				this.get_localBounds_Injected(out bounds);
				return bounds;
			}
		}

		[NativeProperty("TilemapFrameBoundsScripting")]
		internal Bounds localFrameBounds
		{
			get
			{
				Bounds bounds;
				this.get_localFrameBounds_Injected(out bounds);
				return bounds;
			}
		}

		public extern float animationFrameRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color color
		{
			get
			{
				Color color;
				this.get_color_Injected(out color);
				return color;
			}
			set
			{
				this.set_color_Injected(ref value);
			}
		}

		public Vector3Int origin
		{
			get
			{
				Vector3Int vector3Int;
				this.get_origin_Injected(out vector3Int);
				return vector3Int;
			}
			set
			{
				this.set_origin_Injected(ref value);
			}
		}

		public Vector3Int size
		{
			get
			{
				Vector3Int vector3Int;
				this.get_size_Injected(out vector3Int);
				return vector3Int;
			}
			set
			{
				this.set_size_Injected(ref value);
			}
		}

		[NativeProperty(Name = "TileAnchorScripting")]
		public Vector3 tileAnchor
		{
			get
			{
				Vector3 vector;
				this.get_tileAnchor_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_tileAnchor_Injected(ref value);
			}
		}

		public extern Tilemap.Orientation orientation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Matrix4x4 orientationMatrix
		{
			[NativeMethod(Name = "GetTileOrientationMatrix")]
			get
			{
				Matrix4x4 matrix4x;
				this.get_orientationMatrix_Injected(out matrix4x);
				return matrix4x;
			}
			[NativeMethod(Name = "SetOrientationMatrix")]
			set
			{
				this.set_orientationMatrix_Injected(ref value);
			}
		}

		internal Object GetTileAsset(Vector3Int position)
		{
			return this.GetTileAsset_Injected(ref position);
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
			return this.GetTileAssetsBlock_Injected(ref position, ref blockDimensions);
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
		internal int GetTileAssetsBlockNonAlloc(Vector3Int startPosition, Vector3Int endPosition, [Unmarshalled] Object[] tiles)
		{
			return this.GetTileAssetsBlockNonAlloc_Injected(ref startPosition, ref endPosition, tiles);
		}

		public int GetTilesBlockNonAlloc(BoundsInt bounds, TileBase[] tiles)
		{
			return this.GetTileAssetsBlockNonAlloc(bounds.min, bounds.size, tiles);
		}

		public int GetTilesRangeCount(Vector3Int startPosition, Vector3Int endPosition)
		{
			return this.GetTilesRangeCount_Injected(ref startPosition, ref endPosition);
		}

		[FreeFunction(Name = "TilemapBindings::GetTileAssetsRangeNonAlloc", HasExplicitThis = true)]
		internal int GetTileAssetsRangeNonAlloc(Vector3Int startPosition, Vector3Int endPosition, [Unmarshalled] Vector3Int[] positions, [Unmarshalled] Object[] tiles)
		{
			return this.GetTileAssetsRangeNonAlloc_Injected(ref startPosition, ref endPosition, positions, tiles);
		}

		public int GetTilesRangeNonAlloc(Vector3Int startPosition, Vector3Int endPosition, Vector3Int[] positions, TileBase[] tiles)
		{
			return this.GetTileAssetsRangeNonAlloc(startPosition, endPosition, positions, tiles);
		}

		internal void SetTileAsset(Vector3Int position, Object tile)
		{
			this.SetTileAsset_Injected(ref position, tile);
		}

		public void SetTile(Vector3Int position, TileBase tile)
		{
			this.SetTileAsset(position, tile);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetTileAssets(Vector3Int[] positionArray, Object[] tileArray);

		public void SetTiles(Vector3Int[] positionArray, TileBase[] tileArray)
		{
			this.SetTileAssets(positionArray, tileArray);
		}

		[NativeMethod(Name = "SetTileAssetsBlock")]
		private void INTERNAL_CALL_SetTileAssetsBlock(Vector3Int position, Vector3Int blockDimensions, Object[] tileArray)
		{
			this.INTERNAL_CALL_SetTileAssetsBlock_Injected(ref position, ref blockDimensions, tileArray);
		}

		public void SetTilesBlock(BoundsInt position, TileBase[] tileArray)
		{
			this.INTERNAL_CALL_SetTileAssetsBlock(position.min, position.size, tileArray);
		}

		[NativeMethod(Name = "SetTileChangeData")]
		public void SetTile(TileChangeData tileChangeData, bool ignoreLockFlags)
		{
			this.SetTile_Injected(ref tileChangeData, ignoreLockFlags);
		}

		[NativeMethod(Name = "SetTileChangeDataArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTiles(TileChangeData[] tileChangeDataArray, bool ignoreLockFlags);

		public bool HasTile(Vector3Int position)
		{
			return this.GetTileAsset(position) != null;
		}

		[NativeMethod(Name = "RefreshTileAsset")]
		public void RefreshTile(Vector3Int position)
		{
			this.RefreshTile_Injected(ref position);
		}

		[FreeFunction(Name = "TilemapBindings::RefreshTileAssetsNative", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe extern void RefreshTilesNative(void* positions, int count);

		[NativeMethod(Name = "RefreshAllTileAssets")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RefreshAllTiles();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SwapTileAsset(Object changeTile, Object newTile);

		public void SwapTile(TileBase changeTile, TileBase newTile)
		{
			this.SwapTileAsset(changeTile, newTile);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool ContainsTileAsset(Object tileAsset);

		public bool ContainsTile(TileBase tileAsset)
		{
			return this.ContainsTileAsset(tileAsset);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetUsedTilesCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetUsedSpritesCount();

		public int GetUsedTilesNonAlloc(TileBase[] usedTiles)
		{
			return this.Internal_GetUsedTilesNonAlloc(usedTiles);
		}

		public int GetUsedSpritesNonAlloc(Sprite[] usedSprites)
		{
			return this.Internal_GetUsedSpritesNonAlloc(usedSprites);
		}

		[FreeFunction(Name = "TilemapBindings::GetUsedTilesNonAlloc", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int Internal_GetUsedTilesNonAlloc([Unmarshalled] Object[] usedTiles);

		[FreeFunction(Name = "TilemapBindings::GetUsedSpritesNonAlloc", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int Internal_GetUsedSpritesNonAlloc([Unmarshalled] Object[] usedSprites);

		public Sprite GetSprite(Vector3Int position)
		{
			return this.GetSprite_Injected(ref position);
		}

		public Matrix4x4 GetTransformMatrix(Vector3Int position)
		{
			Matrix4x4 matrix4x;
			this.GetTransformMatrix_Injected(ref position, out matrix4x);
			return matrix4x;
		}

		public void SetTransformMatrix(Vector3Int position, Matrix4x4 transform)
		{
			this.SetTransformMatrix_Injected(ref position, ref transform);
		}

		[NativeMethod(Name = "GetTileColor")]
		public Color GetColor(Vector3Int position)
		{
			Color color;
			this.GetColor_Injected(ref position, out color);
			return color;
		}

		[NativeMethod(Name = "SetTileColor")]
		public void SetColor(Vector3Int position, Color color)
		{
			this.SetColor_Injected(ref position, ref color);
		}

		public TileFlags GetTileFlags(Vector3Int position)
		{
			return this.GetTileFlags_Injected(ref position);
		}

		public void SetTileFlags(Vector3Int position, TileFlags flags)
		{
			this.SetTileFlags_Injected(ref position, flags);
		}

		public void AddTileFlags(Vector3Int position, TileFlags flags)
		{
			this.AddTileFlags_Injected(ref position, flags);
		}

		public void RemoveTileFlags(Vector3Int position, TileFlags flags)
		{
			this.RemoveTileFlags_Injected(ref position, flags);
		}

		[NativeMethod(Name = "GetTileInstantiatedObject")]
		public GameObject GetInstantiatedObject(Vector3Int position)
		{
			return this.GetInstantiatedObject_Injected(ref position);
		}

		[NativeMethod(Name = "GetTileObjectToInstantiate")]
		public GameObject GetObjectToInstantiate(Vector3Int position)
		{
			return this.GetObjectToInstantiate_Injected(ref position);
		}

		[NativeMethod(Name = "SetTileColliderType")]
		public void SetColliderType(Vector3Int position, Tile.ColliderType colliderType)
		{
			this.SetColliderType_Injected(ref position, colliderType);
		}

		[NativeMethod(Name = "GetTileColliderType")]
		public Tile.ColliderType GetColliderType(Vector3Int position)
		{
			return this.GetColliderType_Injected(ref position);
		}

		[NativeMethod(Name = "GetTileAnimationFrameCount")]
		public int GetAnimationFrameCount(Vector3Int position)
		{
			return this.GetAnimationFrameCount_Injected(ref position);
		}

		[NativeMethod(Name = "GetTileAnimationFrame")]
		public int GetAnimationFrame(Vector3Int position)
		{
			return this.GetAnimationFrame_Injected(ref position);
		}

		[NativeMethod(Name = "SetTileAnimationFrame")]
		public void SetAnimationFrame(Vector3Int position, int frame)
		{
			this.SetAnimationFrame_Injected(ref position, frame);
		}

		[NativeMethod(Name = "GetTileAnimationTime")]
		public float GetAnimationTime(Vector3Int position)
		{
			return this.GetAnimationTime_Injected(ref position);
		}

		[NativeMethod(Name = "SetTileAnimationTime")]
		public void SetAnimationTime(Vector3Int position, float time)
		{
			this.SetAnimationTime_Injected(ref position, time);
		}

		public TileAnimationFlags GetTileAnimationFlags(Vector3Int position)
		{
			return this.GetTileAnimationFlags_Injected(ref position);
		}

		public void SetTileAnimationFlags(Vector3Int position, TileAnimationFlags flags)
		{
			this.SetTileAnimationFlags_Injected(ref position, flags);
		}

		public void AddTileAnimationFlags(Vector3Int position, TileAnimationFlags flags)
		{
			this.AddTileAnimationFlags_Injected(ref position, flags);
		}

		public void RemoveTileAnimationFlags(Vector3Int position, TileAnimationFlags flags)
		{
			this.RemoveTileAnimationFlags_Injected(ref position, flags);
		}

		public void FloodFill(Vector3Int position, TileBase tile)
		{
			this.FloodFillTileAsset(position, tile);
		}

		[NativeMethod(Name = "FloodFill")]
		private void FloodFillTileAsset(Vector3Int position, Object tile)
		{
			this.FloodFillTileAsset_Injected(ref position, tile);
		}

		public void BoxFill(Vector3Int position, TileBase tile, int startX, int startY, int endX, int endY)
		{
			this.BoxFillTileAsset(position, tile, startX, startY, endX, endY);
		}

		[NativeMethod(Name = "BoxFill")]
		private void BoxFillTileAsset(Vector3Int position, Object tile, int startX, int startY, int endX, int endY)
		{
			this.BoxFillTileAsset_Injected(ref position, tile, startX, startY, endX, endY);
		}

		public void InsertCells(Vector3Int position, Vector3Int insertCells)
		{
			this.InsertCells(position, insertCells.x, insertCells.y, insertCells.z);
		}

		public void InsertCells(Vector3Int position, int numColumns, int numRows, int numLayers)
		{
			this.InsertCells_Injected(ref position, numColumns, numRows, numLayers);
		}

		public void DeleteCells(Vector3Int position, Vector3Int deleteCells)
		{
			this.DeleteCells(position, deleteCells.x, deleteCells.y, deleteCells.z);
		}

		public void DeleteCells(Vector3Int position, int numColumns, int numRows, int numLayers)
		{
			this.DeleteCells_Injected(ref position, numColumns, numRows, numLayers);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearAllTiles();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResizeBounds();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CompressBounds();

		[RequiredByNativeCode]
		internal void GetSyncTileCallbackSettings(ref Tilemap.SyncTileCallbackSettings settings)
		{
			settings.hasSyncTileCallback = Tilemap.HasSyncTileCallback();
			settings.hasPositionsChangedCallback = Tilemap.HasPositionsChangedCallback();
			settings.isBufferSyncTile = this.bufferSyncTile;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SendAndClearSyncTileBuffer();

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
		private extern void get_localBounds_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_localFrameBounds_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_color_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_color_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_origin_Injected(out Vector3Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_origin_Injected(ref Vector3Int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_size_Injected(out Vector3Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_size_Injected(ref Vector3Int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_tileAnchor_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_tileAnchor_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_orientationMatrix_Injected(out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_orientationMatrix_Injected(ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Object GetTileAsset_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Object[] GetTileAssetsBlock_Injected(ref Vector3Int position, ref Vector3Int blockDimensions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetTileAssetsBlockNonAlloc_Injected(ref Vector3Int startPosition, ref Vector3Int endPosition, Object[] tiles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetTilesRangeCount_Injected(ref Vector3Int startPosition, ref Vector3Int endPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetTileAssetsRangeNonAlloc_Injected(ref Vector3Int startPosition, ref Vector3Int endPosition, Vector3Int[] positions, Object[] tiles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTileAsset_Injected(ref Vector3Int position, Object tile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_CALL_SetTileAssetsBlock_Injected(ref Vector3Int position, ref Vector3Int blockDimensions, Object[] tileArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTile_Injected(ref TileChangeData tileChangeData, bool ignoreLockFlags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RefreshTile_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Sprite GetSprite_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetTransformMatrix_Injected(ref Vector3Int position, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTransformMatrix_Injected(ref Vector3Int position, ref Matrix4x4 transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetColor_Injected(ref Vector3Int position, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetColor_Injected(ref Vector3Int position, ref Color color);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern TileFlags GetTileFlags_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTileFlags_Injected(ref Vector3Int position, TileFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddTileFlags_Injected(ref Vector3Int position, TileFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveTileFlags_Injected(ref Vector3Int position, TileFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern GameObject GetInstantiatedObject_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern GameObject GetObjectToInstantiate_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetColliderType_Injected(ref Vector3Int position, Tile.ColliderType colliderType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Tile.ColliderType GetColliderType_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetAnimationFrameCount_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetAnimationFrame_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetAnimationFrame_Injected(ref Vector3Int position, int frame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetAnimationTime_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetAnimationTime_Injected(ref Vector3Int position, float time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern TileAnimationFlags GetTileAnimationFlags_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTileAnimationFlags_Injected(ref Vector3Int position, TileAnimationFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddTileAnimationFlags_Injected(ref Vector3Int position, TileAnimationFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveTileAnimationFlags_Injected(ref Vector3Int position, TileAnimationFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void FloodFillTileAsset_Injected(ref Vector3Int position, Object tile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void BoxFillTileAsset_Injected(ref Vector3Int position, Object tile, int startX, int startY, int endX, int endY);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InsertCells_Injected(ref Vector3Int position, int numColumns, int numRows, int numLayers);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DeleteCells_Injected(ref Vector3Int position, int numColumns, int numRows, int numLayers);

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
