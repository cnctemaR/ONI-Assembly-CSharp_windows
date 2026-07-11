using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The events that cause new particles to be spawned.</para>
	/// </summary>
	public enum ParticleSystemSubEmitterType
	{
		/// <summary>
		///   <para>Spawns new particles when particles from the parent system are born.</para>
		/// </summary>
		Birth,
		/// <summary>
		///   <para>Spawns new particles when particles from the parent system collide with something.</para>
		/// </summary>
		Collision,
		/// <summary>
		///   <para>Spawns new particles when particles from the parent system die.</para>
		/// </summary>
		Death,
		/// <summary>
		///   <para>Spawns new particles when particles from the parent system pass conditions in the Trigger Module.</para>
		/// </summary>
		Trigger,
		/// <summary>
		///   <para>Spawns new particles when triggered from script using ParticleSystem.TriggerSubEmitter.</para>
		/// </summary>
		Manual
	}
}
