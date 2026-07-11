using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Describes when an AudioSource or AudioListener is updated.</para>
	/// </summary>
	public enum AudioVelocityUpdateMode
	{
		/// <summary>
		///   <para>Updates the source or listener in the MonoBehaviour.FixedUpdate loop if it is attached to a Rigidbody, dynamic MonoBehaviour.Update otherwise.</para>
		/// </summary>
		Auto,
		/// <summary>
		///   <para>Updates the source or listener in the MonoBehaviour.FixedUpdate loop.</para>
		/// </summary>
		Fixed,
		/// <summary>
		///   <para>Updates the source or listener in the dynamic MonoBehaviour.Update loop.</para>
		/// </summary>
		Dynamic
	}
}
