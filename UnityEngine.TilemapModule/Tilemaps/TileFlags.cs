using System;

namespace UnityEngine.Tilemaps
{
	/// <summary>
	///   <para>Flags controlling behavior for the TileBase.</para>
	/// </summary>
	[Flags]
	public enum TileFlags
	{
		/// <summary>
		///   <para>No TileFlags are set.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>TileBase locks any color set by brushes or the user.</para>
		/// </summary>
		LockColor = 1,
		/// <summary>
		///   <para>TileBase locks any transform matrix set by brushes or the user.</para>
		/// </summary>
		LockTransform = 2,
		/// <summary>
		///   <para>TileBase does not instantiate its associated GameObject in editor mode and instantiates it only during play mode.</para>
		/// </summary>
		InstantiateGameObjectRuntimeOnly = 4,
		/// <summary>
		///   <para>All lock flags.</para>
		/// </summary>
		LockAll = 3
	}
}
