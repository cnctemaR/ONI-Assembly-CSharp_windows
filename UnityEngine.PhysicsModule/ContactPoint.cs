using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Modules/Physics/MessageParameters.h")]
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
				return Physics.GetColliderByInstanceID(this.m_ThisColliderInstanceID);
			}
		}

		public Collider otherCollider
		{
			get
			{
				return Physics.GetColliderByInstanceID(this.m_OtherColliderInstanceID);
			}
		}

		public float separation
		{
			get
			{
				return this.m_Separation;
			}
		}

		internal ContactPoint(Vector3 point, Vector3 normal, Vector3 impulse, float separation, int thisInstanceID, int otherInstenceID)
		{
			this.m_Point = point;
			this.m_Normal = normal;
			this.m_Impulse = impulse;
			this.m_Separation = separation;
			this.m_ThisColliderInstanceID = thisInstanceID;
			this.m_OtherColliderInstanceID = otherInstenceID;
		}

		internal Vector3 m_Point;

		internal Vector3 m_Normal;

		internal Vector3 m_Impulse;

		internal int m_ThisColliderInstanceID;

		internal int m_OtherColliderInstanceID;

		internal float m_Separation;
	}
}
