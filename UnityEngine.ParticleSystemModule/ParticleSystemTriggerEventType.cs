using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The different types of particle triggers.</para>
	/// </summary>
	public enum ParticleSystemTriggerEventType
	{
		/// <summary>
		///   <para>Trigger when particles are inside the collision volume.</para>
		/// </summary>
		Inside,
		/// <summary>
		///   <para>Trigger when particles are outside the collision volume.</para>
		/// </summary>
		Outside,
		/// <summary>
		///   <para>Trigger when particles enter the collision volume.</para>
		/// </summary>
		Enter,
		/// <summary>
		///   <para>Trigger when particles leave the collision volume.</para>
		/// </summary>
		Exit
	}
}
