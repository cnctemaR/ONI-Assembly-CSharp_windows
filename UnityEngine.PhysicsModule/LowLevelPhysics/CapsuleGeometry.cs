using System;
using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct CapsuleGeometry : IGeometry
	{
		public float Radius
		{
			get
			{
				return this.m_Radius;
			}
			set
			{
				this.m_Radius = value;
			}
		}

		public float HalfLength
		{
			get
			{
				return this.m_HalfLength;
			}
			set
			{
				this.m_HalfLength = value;
			}
		}

		public CapsuleGeometry(float radius, float halfLength)
		{
			this.m_UnusedReserved = -1;
			this.m_Radius = radius;
			this.m_HalfLength = halfLength;
		}

		public GeometryType GeometryType
		{
			get
			{
				return GeometryType.Capsule;
			}
		}

		private int m_UnusedReserved;

		private float m_Radius;

		private float m_HalfLength;
	}
}
