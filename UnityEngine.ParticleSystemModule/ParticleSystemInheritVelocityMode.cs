using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>How to apply emitter velocity to particles.</para>
	/// </summary>
	public enum ParticleSystemInheritVelocityMode
	{
		/// <summary>
		///   <para>Each particle inherits the emitter's velocity on the frame when it was initially emitted.</para>
		/// </summary>
		Initial,
		/// <summary>
		///   <para>Each particle's velocity is set to the emitter's current velocity value, every frame.</para>
		/// </summary>
		Current
	}
}
