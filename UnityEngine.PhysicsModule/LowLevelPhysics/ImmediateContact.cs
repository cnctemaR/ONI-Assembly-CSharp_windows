using System;

namespace UnityEngine.LowLevelPhysics
{
	public struct ImmediateContact
	{
		public Vector3 Normal
		{
			get
			{
				return this.m_Normal;
			}
			set
			{
				this.m_Normal = value;
			}
		}

		public float Separation
		{
			get
			{
				return this.m_Separation;
			}
			set
			{
				this.m_Separation = value;
			}
		}

		public Vector3 Point
		{
			get
			{
				return this.m_Point;
			}
			set
			{
				this.m_Point = value;
			}
		}

		private Vector3 m_Normal;

		private float m_Separation;

		private Vector3 m_Point;

		private float m_MaxImpulse;

		private Vector3 m_TargetVel;

		private float m_StaticFriction;

		private byte m_MaterialFlags;

		private byte m_Pad;

		private ushort m_InternalUse;

		private uint m_InternalFaceIndex1;

		private float m_DynamicFriction;

		private float m_Restitution;
	}
}
