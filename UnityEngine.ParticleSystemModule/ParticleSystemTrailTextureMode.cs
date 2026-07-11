using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Choose how textures are applied to Particle Trails.</para>
	/// </summary>
	public enum ParticleSystemTrailTextureMode
	{
		/// <summary>
		///   <para>Map the texture once along the entire length of the trail.</para>
		/// </summary>
		Stretch,
		/// <summary>
		///   <para>Repeat the texture along the trail. To set the tiling rate, use Material.SetTextureScale.</para>
		/// </summary>
		Tile,
		/// <summary>
		///   <para>Map the texture once along the entire length of the trail, assuming all vertices are evenly spaced.</para>
		/// </summary>
		DistributePerSegment,
		/// <summary>
		///   <para>Repeat the texture along the trail, repeating at a rate of once per trail segment. To adjust the tiling rate, use Material.SetTextureScale.</para>
		/// </summary>
		RepeatPerSegment
	}
}
