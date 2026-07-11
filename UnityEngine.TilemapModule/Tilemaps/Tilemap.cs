using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Tilemaps
{
	/// <summary>
	///   <para>The tile map stores component.</para>
	/// </summary>
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/Tilemap/Public/TilemapMarshalling.h")]
	[NativeHeader("Modules/Tilemap/Public/TilemapTile.h")]
	[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
	[NativeHeader("Modules/Grid/Public/Grid.h")]
	[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
	[NativeType(Header = "Modules/Tilemap/Public/Tilemap.h")]
	public sealed class Tilemap : GridLayout
	{
		/// <summary>
		///   <para>Gets the Grid associated with this tile map.</para>
		/// </summary>
		public extern Grid layoutGrid
		{
			[NativeMethod(Name = "GetAttachedGrid")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get the logical center coordinate of a grid cell in local space.</para>
		/// </summary>
		/// <param name="position">Grid cell position.</param>
		/// <returns>
		///   <para>Center of the cell transformed into local space coordinates.</para>
		/// </returns>
		public Vector3 GetCellCenterLocal(Vector3Int position)
		{
			return base.CellToLocalInterpolated(position + this.tileAnchor);
		}

		/// <summary>
		///   <para>Get the logical center coordinate of a grid cell in world space.</para>
		/// </summary>
		/// <param name="position">Grid cell position.</param>
		/// <returns>
		///   <para>Center of the cell transformed into world space coordinates.</para>
		/// </returns>
		public Vector3 GetCellCenterWorld(Vector3Int position)
		{
			return base.LocalToWorld(base.CellToLocalInterpolated(position + this.tileAnchor));
		}

		/// <summary>
		///   <para>Returns the boundaries of the Tilemap in cell size.</para>
		/// </summary>
		public BoundsInt cellBounds
		{
			get
			{
				return new BoundsInt(this.origin, this.size);
			}
		}

		/// <summary>
		///   <para>Returns the boundaries of the Tilemap in local space size.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The frame rate for all tile animations in the tile map.</para>
		/// </summary>
		public extern float animationFrameRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The color of the tile map layer.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The origin of the Tilemap in cell position.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The size of the Tilemap in cells.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Gets the anchor point of tiles in the Tilemap.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Orientation of the tiles in the Tilemap.</para>
		/// </summary>
		public extern Tilemap.Orientation orientation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Orientation Matrix of the orientation of the tiles in the Tilemap.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Gets the.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <returns>
		///   <para>Tilemaps.TileBase|Tile of type T placed at the cell.</para>
		/// </returns>
		public TileBase GetTile(Vector3Int position)
		{
			return (TileBase)this.GetTileAsset(position);
		}

		public T GetTile<T>(Vector3Int position) where T : TileBase
		{
			return this.GetTileAsset(position) as T;
		}

		internal Object[] GetTileAssetsBlock(Vector3Int position, Vector3Int blockDimensions)
		{
			return this.GetTileAssetsBlock_Injected(ref position, ref blockDimensions);
		}

		/// <summary>
		///   <para>Retrieves an array of tiles with the given bounds.</para>
		/// </summary>
		/// <param name="bounds">Bounds to retrieve from.</param>
		/// <returns>
		///   <para>An array of at the given bounds.</para>
		/// </returns>
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

		internal void SetTileAsset(Vector3Int position, Object tile)
		{
			this.SetTileAsset_Injected(ref position, tile);
		}

		/// <summary>
		///   <para>Sets a.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <param name="tile"> to be placed the cell.</param>
		public void SetTile(Vector3Int position, TileBase tile)
		{
			this.SetTileAsset(position, tile);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetTileAssets(Vector3Int[] positionArray, Object[] tileArray);

		/// <summary>
		///   <para>Sets an array of.</para>
		/// </summary>
		/// <param name="positionArray">An array of positions of Tiles on the Tilemap.</param>
		/// <param name="tileArray">An array of to be placed.</param>
		public void SetTiles(Vector3Int[] positionArray, TileBase[] tileArray)
		{
			this.SetTileAssets(positionArray, tileArray);
		}

		[NativeMethod(Name = "SetTileAssetsBlock")]
		private void INTERNAL_CALL_SetTileAssetsBlock(Vector3Int position, Vector3Int blockDimensions, Object[] tileArray)
		{
			this.INTERNAL_CALL_SetTileAssetsBlock_Injected(ref position, ref blockDimensions, tileArray);
		}

		/// <summary>
		///   <para>Fills bounds with array of tiles.</para>
		/// </summary>
		/// <param name="position">Bounds to be filled.</param>
		/// <param name="tileArray">An array of to be placed.</param>
		public void SetTilesBlock(BoundsInt position, TileBase[] tileArray)
		{
			this.INTERNAL_CALL_SetTileAssetsBlock(position.min, position.size, tileArray);
		}

		/// <summary>
		///   <para>Returns whether there is a tile at the position.</para>
		/// </summary>
		/// <param name="position">Position to check.</param>
		/// <returns>
		///   <para>True if there is a tile at the position. False if not.</para>
		/// </returns>
		public bool HasTile(Vector3Int position)
		{
			return this.GetTileAsset(position) != null;
		}

		/// <summary>
		///   <para>Refreshes a.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		[NativeMethod(Name = "RefreshTileAsset")]
		public void RefreshTile(Vector3Int position)
		{
			this.RefreshTile_Injected(ref position);
		}

		/// <summary>
		///   <para>Refreshes all. The tile map will retrieve the rendering data, animation data and other data for all tiles and update all relevant components.</para>
		/// </summary>
		[NativeMethod(Name = "RefreshAllTileAssets")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RefreshAllTiles();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SwapTileAsset(Object changeTile, Object newTile);

		/// <summary>
		///   <para>Swaps all existing tiles of changeTile to newTile and refreshes all the swapped tiles.</para>
		/// </summary>
		/// <param name="changeTile">Tile to swap.</param>
		/// <param name="newTile">Tile to swap to.</param>
		public void SwapTile(TileBase changeTile, TileBase newTile)
		{
			this.SwapTileAsset(changeTile, newTile);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool ContainsTileAsset(Object tileAsset);

		/// <summary>
		///   <para>Returns true if the Tilemap contains the given. Returns false if not.</para>
		/// </summary>
		/// <param name="tileAsset">Tile to check.</param>
		/// <returns>
		///   <para>Whether the Tilemap contains the tile.</para>
		/// </returns>
		public bool ContainsTile(TileBase tileAsset)
		{
			return this.ContainsTileAsset(tileAsset);
		}

		/// <summary>
		///   <para>Get the total number of different.</para>
		/// </summary>
		/// <returns>
		///   <para>The total number of different.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetUsedTilesCount();

		/// <summary>
		///   <para>Fills the given array with the total number of different and returns the number of tiles filled.</para>
		/// </summary>
		/// <param name="usedTiles">The array to be filled.</param>
		/// <returns>
		///   <para>The number of tiles filled.</para>
		/// </returns>
		public int GetUsedTilesNonAlloc(TileBase[] usedTiles)
		{
			return this.Internal_GetUsedTilesNonAlloc(usedTiles);
		}

		[FreeFunction(Name = "TilemapBindings::GetUsedTilesNonAlloc", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int Internal_GetUsedTilesNonAlloc(Object[] usedTiles);

		/// <summary>
		///   <para>Gets the.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <returns>
		///   <para>Sprite at the XY coordinate.</para>
		/// </returns>
		public Sprite GetSprite(Vector3Int position)
		{
			return this.GetSprite_Injected(ref position);
		}

		/// <summary>
		///   <para>Gets the transform matrix of a.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <returns>
		///   <para>The transform matrix.</para>
		/// </returns>
		public Matrix4x4 GetTransformMatrix(Vector3Int position)
		{
			Matrix4x4 matrix4x;
			this.GetTransformMatrix_Injected(ref position, out matrix4x);
			return matrix4x;
		}

		/// <summary>
		///   <para>Sets the transform matrix of a tile given the XYZ coordinates of a cell in the.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <param name="transform">The transform matrix.</param>
		public void SetTransformMatrix(Vector3Int position, Matrix4x4 transform)
		{
			this.SetTransformMatrix_Injected(ref position, ref transform);
		}

		/// <summary>
		///   <para>Gets the color of a.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <returns>
		///   <para>Color of the at the XY coordinate.</para>
		/// </returns>
		[NativeMethod(Name = "GetTileColor")]
		public Color GetColor(Vector3Int position)
		{
			Color color;
			this.GetColor_Injected(ref position, out color);
			return color;
		}

		/// <summary>
		///   <para>Sets the color of a.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <param name="color">Color to set the to at the XY coordinate.</param>
		[NativeMethod(Name = "SetTileColor")]
		public void SetColor(Vector3Int position, Color color)
		{
			this.SetColor_Injected(ref position, ref color);
		}

		/// <summary>
		///   <para>Gets the TileFlags of the Tile at the given position.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <returns>
		///   <para>TileFlags from the Tile.</para>
		/// </returns>
		public TileFlags GetTileFlags(Vector3Int position)
		{
			return this.GetTileFlags_Injected(ref position);
		}

		/// <summary>
		///   <para>Sets the TileFlags onto the Tile at the given position.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <param name="flags">TileFlags to add onto the Tile.</param>
		public void SetTileFlags(Vector3Int position, TileFlags flags)
		{
			this.SetTileFlags_Injected(ref position, flags);
		}

		/// <summary>
		///   <para>Adds the TileFlags onto the Tile at the given position.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <param name="flags">TileFlags to add (with bitwise or) onto the flags provided by Tile.TileBase.</param>
		public void AddTileFlags(Vector3Int position, TileFlags flags)
		{
			this.AddTileFlags_Injected(ref position, flags);
		}

		/// <summary>
		///   <para>Removes the TileFlags onto the Tile at the given position.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <param name="flags">TileFlags to remove from the Tile.</param>
		public void RemoveTileFlags(Vector3Int position, TileFlags flags)
		{
			this.RemoveTileFlags_Injected(ref position, flags);
		}

		/// <summary>
		///   <para>Gets the.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <returns>
		///   <para>GameObject instantiated by the Tile at the position.</para>
		/// </returns>
		[NativeMethod(Name = "GetTileInstantiatedObject")]
		public GameObject GetInstantiatedObject(Vector3Int position)
		{
			return this.GetInstantiatedObject_Injected(ref position);
		}

		[NativeMethod(Name = "SetTileColliderType")]
		public void SetColliderType(Vector3Int position, Tile.ColliderType colliderType)
		{
			this.SetColliderType_Injected(ref position, colliderType);
		}

		/// <summary>
		///   <para>Gets the collider type of a.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <returns>
		///   <para>Collider type of the at the XY coordinate.</para>
		/// </returns>
		[NativeMethod(Name = "GetTileColliderType")]
		public Tile.ColliderType GetColliderType(Vector3Int position)
		{
			return this.GetColliderType_Injected(ref position);
		}

		/// <summary>
		///   <para>Does a flood fill with the given starting from the given coordinates.</para>
		/// </summary>
		/// <param name="position">Start position of the flood fill on the Tilemap.</param>
		/// <param name="tile"> to place.</param>
		public void FloodFill(Vector3Int position, TileBase tile)
		{
			this.FloodFillTileAsset(position, tile);
		}

		[NativeMethod(Name = "FloodFill")]
		private void FloodFillTileAsset(Vector3Int position, Object tile)
		{
			this.FloodFillTileAsset_Injected(ref position, tile);
		}

		/// <summary>
		///   <para>Does a box fill with the given. Starts from given coordinates and fills the limits from start to end (inclusive).</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <param name="tile"> to place.</param>
		/// <param name="startX">The minimum X coordinate limit to fill to.</param>
		/// <param name="startY">The minimum Y coordinate limit to fill to.</param>
		/// <param name="endX">The maximum X coordinate limit to fill to.</param>
		/// <param name="endY">The maximum Y coordinate limit to fill to.</param>
		public void BoxFill(Vector3Int position, TileBase tile, int startX, int startY, int endX, int endY)
		{
			this.BoxFillTileAsset(position, tile, startX, startY, endX, endY);
		}

		[NativeMethod(Name = "BoxFill")]
		private void BoxFillTileAsset(Vector3Int position, Object tile, int startX, int startY, int endX, int endY)
		{
			this.BoxFillTileAsset_Injected(ref position, tile, startX, startY, endX, endY);
		}

		/// <summary>
		///   <para>Clears all tiles that are placed in the Tilemap.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearAllTiles();

		/// <summary>
		///   <para>Resizes tiles in the Tilemap to bounds defined by origin and size.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResizeBounds();

		/// <summary>
		///   <para>Compresses the origin and size of the Tilemap to bounds where tiles exist.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CompressBounds();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_localBounds_Injected(out Bounds ret);

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
		private extern void SetTileAsset_Injected(ref Vector3Int position, Object tile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_CALL_SetTileAssetsBlock_Injected(ref Vector3Int position, ref Vector3Int blockDimensions, Object[] tileArray);

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
		private extern void SetColliderType_Injected(ref Vector3Int position, Tile.ColliderType colliderType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Tile.ColliderType GetColliderType_Injected(ref Vector3Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void FloodFillTileAsset_Injected(ref Vector3Int position, Object tile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void BoxFillTileAsset_Injected(ref Vector3Int position, Object tile, int startX, int startY, int endX, int endY);

		/// <summary>
		///   <para>Determines the orientation of.</para>
		/// </summary>
		public enum Orientation
		{
			/// <summary>
			///   <para>Orients tiles in the XY plane.</para>
			/// </summary>
			XY,
			/// <summary>
			///   <para>Orients tiles in the XZ plane.</para>
			/// </summary>
			XZ,
			/// <summary>
			///   <para>Orients tiles in the YX plane.</para>
			/// </summary>
			YX,
			/// <summary>
			///   <para>Orients tiles in the YZ plane.</para>
			/// </summary>
			YZ,
			/// <summary>
			///   <para>Orients tiles in the ZX plane.</para>
			/// </summary>
			ZX,
			/// <summary>
			///   <para>Orients tiles in the ZY plane.</para>
			/// </summary>
			ZY,
			/// <summary>
			///   <para>Use a custom orientation to all tiles in the tile map.</para>
			/// </summary>
			Custom
		}
	}
}
