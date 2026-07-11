using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The mode used to generate new points in a shape (Shuriken).</para>
	/// </summary>
	public enum ParticleSystemShapeMultiModeValue
	{
		/// <summary>
		///   <para>Generate points randomly. (Default)</para>
		/// </summary>
		Random,
		/// <summary>
		///   <para>Animate the emission point around the shape.</para>
		/// </summary>
		Loop,
		/// <summary>
		///   <para>Animate the emission point around the shape, alternating between clockwise and counter-clockwise directions.</para>
		/// </summary>
		PingPong,
		/// <summary>
		///   <para>Distribute new particles around the shape evenly.</para>
		/// </summary>
		BurstSpread
	}
}
