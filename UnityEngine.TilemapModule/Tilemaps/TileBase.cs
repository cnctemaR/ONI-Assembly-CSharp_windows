using System;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps
{
	/// <summary>
	///   <para>Base class for a tile in the Tilemap.</para>
	/// </summary>
	[RequiredByNativeCode]
	public abstract class TileBase : ScriptableObject
	{
		/// <summary>
		///   <para>This method is called when the tile is refreshed.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <param name="tilemap">The Tilemap the tile is present on.</param>
		[RequiredByNativeCode]
		public virtual void RefreshTile(Vector3Int position, ITilemap tilemap)
		{
			tilemap.RefreshTile(position);
		}

		[RequiredByNativeCode]
		public virtual void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
		}

		private TileData GetTileDataNoRef(Vector3Int position, ITilemap tilemap)
		{
			TileData tileData = default(TileData);
			this.GetTileData(position, tilemap, ref tileData);
			return tileData;
		}

		[RequiredByNativeCode]
		public virtual bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
		{
			return false;
		}

		private TileAnimationData GetTileAnimationDataNoRef(Vector3Int position, ITilemap tilemap)
		{
			TileAnimationData tileAnimationData = default(TileAnimationData);
			this.GetTileAnimationData(position, tilemap, ref tileAnimationData);
			return tileAnimationData;
		}

		/// <summary>
		///   <para>StartUp is called on the first frame of the running scene.</para>
		/// </summary>
		/// <param name="position">Position of the Tile on the Tilemap.</param>
		/// <param name="tilemap">The Tilemap the tile is present on.</param>
		/// <param name="go">The GameObject instantiated for the Tile.</param>
		/// <returns>
		///   <para>Whether the call was successful.</para>
		/// </returns>
		[RequiredByNativeCode]
		public virtual bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
		{
			return false;
		}
	}
}
