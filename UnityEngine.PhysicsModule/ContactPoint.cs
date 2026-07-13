using System;

namespace UnityEngine
{
	public struct ContactPoint
	{
		public Vector3 point
		{
			get
			{
				return this.m_Point;
			}
		}

		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
		}

		public Vector3 impulse
		{
			get
			{
				return this.m_Impulse;
			}
		}

		public Collider thisCollider
		{
			get
			{
				return Physics.GetColliderByInstanceID(this.m_ThisColliderEntityId);
			}
		}

		public Collider otherCollider
		{
			get
			{
				return Physics.GetColliderByInstanceID(this.m_OtherColliderEntityId);
			}
		}

		public float separation
		{
			get
			{
				return this.m_Separation;
			}
		}

		internal ContactPoint(Vector3 point, Vector3 normal, Vector3 impulse, float separation, EntityId thisEntityId, EntityId otherEntityId)
		{
			this.m_Point = point;
			this.m_Normal = normal;
			this.m_Impulse = impulse;
			this.m_Separation = separation;
			this.m_ThisColliderEntityId = thisEntityId;
			this.m_OtherColliderEntityId = otherEntityId;
		}

		internal Vector3 m_Point;

		internal Vector3 m_Normal;

		internal Vector3 m_Impulse;

		internal EntityId m_ThisColliderEntityId;

		internal EntityId m_OtherColliderEntityId;

		internal float m_Separation;
	}
}
