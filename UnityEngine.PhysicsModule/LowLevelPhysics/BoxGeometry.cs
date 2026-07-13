using System;
using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct BoxGeometry : IGeometry
	{
		public Vector3 HalfExtents
		{
			get
			{
				return this.m_HalfExtents;
			}
			set
			{
				this.m_HalfExtents = value;
			}
		}

		public BoxGeometry(Vector3 halfExtents)
		{
			this.m_UnusedReserved = -1;
			this.m_HalfExtents = halfExtents;
		}

		public GeometryType GeometryType
		{
			get
			{
				return GeometryType.Box;
			}
		}

		private int m_UnusedReserved;

		private Vector3 m_HalfExtents;
	}
}
