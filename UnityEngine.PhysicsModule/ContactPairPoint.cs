using System;

namespace UnityEngine
{
	public readonly struct ContactPairPoint
	{
		public Vector3 Position
		{
			get
			{
				return this.m_Position;
			}
		}

		public float Separation
		{
			get
			{
				return this.m_Separation;
			}
		}

		public Vector3 Normal
		{
			get
			{
				return this.m_Normal;
			}
		}

		public Vector3 Impulse
		{
			get
			{
				return this.m_Impulse;
			}
		}

		internal readonly Vector3 m_Position;

		internal readonly float m_Separation;

		internal readonly Vector3 m_Normal;

		internal readonly uint m_InternalFaceIndex0;

		internal readonly Vector3 m_Impulse;

		internal readonly uint m_InternalFaceIndex1;
	}
}
