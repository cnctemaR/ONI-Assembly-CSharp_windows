using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Choose how Particle Trails are generated.</para>
	/// </summary>
	public enum ParticleSystemTrailMode
	{
		/// <summary>
		///   <para>Makes a trail behind each particle as the particle moves.</para>
		/// </summary>
		PerParticle,
		/// <summary>
		///   <para>Draws a line between each particle, connecting the youngest particle to the oldest.</para>
		/// </summary>
		Ribbon
	}
}
