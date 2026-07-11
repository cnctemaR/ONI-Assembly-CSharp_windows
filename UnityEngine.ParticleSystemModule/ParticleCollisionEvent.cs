using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(Optional = true)]
	public struct ParticleCollisionEvent
	{
		public Vector3 intersection
		{
			get
			{
				return this.m_Intersection;
			}
		}

		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
		}

		public Vector3 velocity
		{
			get
			{
				return this.m_Velocity;
			}
		}

		public Component colliderComponent
		{
			get
			{
				return ParticleCollisionEvent.InstanceIDToColliderComponent(this.m_ColliderInstanceID);
			}
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::InstanceIDToColliderComponent")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Component InstanceIDToColliderComponent(int instanceID);

		internal Vector3 m_Intersection;

		internal Vector3 m_Normal;

		internal Vector3 m_Velocity;

		internal int m_ColliderInstanceID;
	}
}
