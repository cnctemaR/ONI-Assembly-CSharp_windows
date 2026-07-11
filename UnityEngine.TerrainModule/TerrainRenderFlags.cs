using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Enum provding terrain rendering options.</para>
	/// </summary>
	[Flags]
	public enum TerrainRenderFlags
	{
		[Obsolete("TerrainRenderFlags.heightmap is obsolete, use TerrainRenderFlags.Heightmap instead. (UnityUpgradable) -> Heightmap")]
		heightmap = 1,
		[Obsolete("TerrainRenderFlags.trees is obsolete, use TerrainRenderFlags.Trees instead. (UnityUpgradable) -> Trees")]
		trees = 2,
		[Obsolete("TerrainRenderFlags.details is obsolete, use TerrainRenderFlags.Details instead. (UnityUpgradable) -> Details")]
		details = 4,
		[Obsolete("TerrainRenderFlags.all is obsolete, use TerrainRenderFlags.All instead. (UnityUpgradable) -> All")]
		all = 7,
		/// <summary>
		///   <para>Render heightmap.</para>
		/// </summary>
		Heightmap = 1,
		/// <summary>
		///   <para>Render trees.</para>
		/// </summary>
		Trees = 2,
		/// <summary>
		///   <para>Render terrain details.</para>
		/// </summary>
		Details = 4,
		/// <summary>
		///   <para>Render all options.</para>
		/// </summary>
		All = 7
	}
}
