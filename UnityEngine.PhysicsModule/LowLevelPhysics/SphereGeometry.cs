using System;
using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SphereGeometry : IGeometry
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

		public SphereGeometry(float radius)
		{
			this.m_UnusedReserved = -1;
			this.m_Radius = radius;
		}

		public GeometryType GeometryType
		{
			get
			{
				return GeometryType.Sphere;
			}
		}

		private int m_UnusedReserved;

		private float m_Radius;
	}
}
