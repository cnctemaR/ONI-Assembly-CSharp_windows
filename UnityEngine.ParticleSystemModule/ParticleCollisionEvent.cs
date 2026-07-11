using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Information about a particle collision.</para>
	/// </summary>
	[RequiredByNativeCode(Optional = true)]
	public struct ParticleCollisionEvent
	{
		/// <summary>
		///   <para>Intersection point of the collision in world coordinates.</para>
		/// </summary>
		public Vector3 intersection
		{
			get
			{
				return this.m_Intersection;
			}
		}

		/// <summary>
		///   <para>Geometry normal at the intersection point of the collision.</para>
		/// </summary>
		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
		}

		/// <summary>
		///   <para>Incident velocity at the intersection point of the collision.</para>
		/// </summary>
		public Vector3 velocity
		{
			get
			{
				return this.m_Velocity;
			}
		}

		/// <summary>
		///   <para>The Collider or Collider2D for the GameObject struck by the particles.</para>
		/// </summary>
		public Component colliderComponent
		{
			get
			{
				return ParticleCollisionEvent.InstanceIDToColliderComponent(this.m_ColliderInstanceID);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Component InstanceIDToColliderComponent(int instanceID);

		private Vector3 m_Intersection;

		private Vector3 m_Normal;

		private Vector3 m_Velocity;

		private int m_ColliderInstanceID;
	}
}
