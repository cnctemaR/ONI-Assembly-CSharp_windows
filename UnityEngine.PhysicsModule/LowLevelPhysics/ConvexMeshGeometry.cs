using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics
{
	public struct ConvexMeshGeometry : IGeometry
	{
		public Vector3 Scale
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				this.m_Scale = value;
			}
		}

		public Quaternion ScaleAxisRotation
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

		public GeometryType GeometryType
		{
			get
			{
				return GeometryType.ConvexMesh;
			}
		}

		private int m_UnusedReserved;

		private Vector3 m_Scale;

		private Quaternion m_Rotation;

		private IntPtr m_ConvexMesh;

		private byte m_MeshFlags;

		[FixedBuffer(typeof(byte), 3)]
		private ConvexMeshGeometry.<m_MeshFlagsPadding>e__FixedBuffer m_MeshFlagsPadding;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 3)]
		public struct <m_MeshFlagsPadding>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
