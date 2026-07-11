using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Choose how textures are applied to Lines and Trails.</para>
	/// </summary>
	public enum LineTextureMode
	{
		/// <summary>
		///   <para>Map the texture once along the entire length of the line.</para>
		/// </summary>
		Stretch,
		/// <summary>
		///   <para>Repeat the texture along the line, based on its length in world units. To set the tiling rate, use Material.SetTextureScale.</para>
		/// </summary>
		Tile,
		/// <summary>
		///   <para>Map the texture once along the entire length of the line, assuming all vertices are evenly spaced.</para>
		/// </summary>
		DistributePerSegment,
		/// <summary>
		///   <para>Repeat the texture along the line, repeating at a rate of once per line segment. To adjust the tiling rate, use Material.SetTextureScale.</para>
		/// </summary>
		RepeatPerSegment
	}
}
