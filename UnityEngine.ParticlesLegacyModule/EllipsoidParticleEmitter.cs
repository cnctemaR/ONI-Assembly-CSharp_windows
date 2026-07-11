using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Class used to allow GameObject.AddComponent / GameObject.GetComponent to be used.</para>
	/// </summary>
	[RequireComponent(typeof(Transform))]
	[Obsolete("This component is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false)]
	public class EllipsoidParticleEmitter : ParticleEmitter
	{
		internal EllipsoidParticleEmitter()
		{
		}
	}
}
