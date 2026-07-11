using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Indicate the types of changes to the terrain in OnTerrainChanged callback.</para>
	/// </summary>
	[Flags]
	public enum TerrainChangedFlags
	{
		/// <summary>
		///   <para>Indicates a change to the heightmap data.</para>
		/// </summary>
		Heightmap = 1,
		/// <summary>
		///   <para>Indicates a change to the tree data.</para>
		/// </summary>
		TreeInstances = 2,
		/// <summary>
		///   <para>Indicates a change to the heightmap data without computing LOD.</para>
		/// </summary>
		DelayedHeightmapUpdate = 4,
		/// <summary>
		///   <para>Indicates that a change was made to the terrain that was so significant that the internal rendering data need to be flushed and recreated.</para>
		/// </summary>
		FlushEverythingImmediately = 8,
		/// <summary>
		///   <para>Indicates a change to the detail data.</para>
		/// </summary>
		RemoveDirtyDetailsImmediately = 16,
		/// <summary>
		///   <para>Indicates that the TerrainData object is about to be destroyed.</para>
		/// </summary>
		WillBeDestroyed = 256
	}
}
