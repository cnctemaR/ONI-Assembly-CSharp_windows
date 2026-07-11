using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Control how a Particle System calculates its velocity.</para>
	/// </summary>
	public enum ParticleSystemEmitterVelocityMode
	{
		/// <summary>
		///   <para>Calculate the Particle System velocity by using the Transform component.</para>
		/// </summary>
		Transform,
		/// <summary>
		///   <para>Calculate the Particle System velocity by using a Rigidbody or Rigidbody2D component, if one exists on the Game Object.</para>
		/// </summary>
		Rigidbody
	}
}
