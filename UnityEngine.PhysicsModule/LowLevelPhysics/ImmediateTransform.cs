using System;

namespace UnityEngine.LowLevelPhysics
{
	public struct ImmediateTransform
	{
		public Quaternion Rotation
		{
			get
			{
				return this.m_Rotation;
			}
			set
			{
				this.m_Rotation = value;
			}
		}

		public Vector3 Position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		private Quaternion m_Rotation;

		private Vector3 m_Position;
	}
}
