using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Whether to use 2D or 3D colliders for particle collisions.</para>
	/// </summary>
	public enum ParticleSystemCollisionMode
	{
		/// <summary>
		///   <para>Use 3D colliders to collide particles against.</para>
		/// </summary>
		Collision3D,
		/// <summary>
		///   <para>Use 2D colliders to collide particles against.</para>
		/// </summary>
		Collision2D
	}
}
