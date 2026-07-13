using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics
{
	public struct TerrainGeometry : IGeometry
	{
		public GeometryType GeometryType
		{
			get
			{
				return GeometryType.Terrain;
			}
		}

		private int m_UnusedReserved;

		private IntPtr m_TerrainData;

		private float m_HeightScale;

		private float m_RowScale;

		private float m_ColumnScale;

		private byte m_TerrainFlags;

		[FixedBuffer(typeof(byte), 3)]
		private TerrainGeometry.<m_TerrainFlagsPadding>e__FixedBuffer m_TerrainFlagsPadding;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 3)]
		public struct <m_TerrainFlagsPadding>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
