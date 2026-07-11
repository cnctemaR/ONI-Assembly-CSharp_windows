using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The properties of sub-emitter particles.</para>
	/// </summary>
	[Flags]
	public enum ParticleSystemSubEmitterProperties
	{
		/// <summary>
		///   <para>When spawning new particles, do not inherit any properties from the parent particles.</para>
		/// </summary>
		InheritNothing = 0,
		/// <summary>
		///   <para>When spawning new particles, inherit all available properties from the parent particles.</para>
		/// </summary>
		InheritEverything = 15,
		/// <summary>
		///   <para>When spawning new particles, multiply the start color by the color of the parent particles.</para>
		/// </summary>
		InheritColor = 1,
		/// <summary>
		///   <para>When spawning new particles, multiply the start size by the size of the parent particles.</para>
		/// </summary>
		InheritSize = 2,
		/// <summary>
		///   <para>When spawning new particles, add the start rotation to the rotation of the parent particles.</para>
		/// </summary>
		InheritRotation = 4,
		/// <summary>
		///   <para>New particles will have a shorter lifespan, the closer their parent particles are to death.</para>
		/// </summary>
		InheritLifetime = 8
	}
}
