using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Control how particle systems apply transform scale.</para>
	/// </summary>
	public enum ParticleSystemScalingMode
	{
		/// <summary>
		///   <para>Scale the particle system using the entire transform hierarchy.</para>
		/// </summary>
		Hierarchy,
		/// <summary>
		///   <para>Scale the particle system using only its own transform scale. (Ignores parent scale).</para>
		/// </summary>
		Local,
		/// <summary>
		///   <para>Only apply transform scale to the shape component, which controls where
		///   particles are spawned, but does not affect their size or movement.
		///   </para>
		/// </summary>
		Shape
	}
}
