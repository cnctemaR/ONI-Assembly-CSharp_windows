using System;

namespace UnityEngine.Rendering
{
	public struct RasterState : IEquatable<RasterState>
	{
		public RasterState(CullMode cullingMode = CullMode.Back, int offsetUnits = 0, float offsetFactor = 0f, bool depthClip = true)
		{
			this.m_CullingMode = cullingMode;
			this.m_OffsetUnits = offsetUnits;
			this.m_OffsetFactor = offsetFactor;
			this.m_DepthClip = Convert.ToByte(depthClip);
			this.m_Padding1 = 0;
			this.m_Padding2 = 0;
			this.m_Padding3 = 0;
		}

		public CullMode cullingMode
		{
			get
			{
				return this.m_CullingMode;
			}
			set
			{
				this.m_CullingMode = value;
			}
		}

		public bool depthClip
		{
			get
			{
				return Convert.ToBoolean(this.m_DepthClip);
			}
			set
			{
				this.m_DepthClip = Convert.ToByte(value);
			}
		}

		public int offsetUnits
		{
			get
			{
				return this.m_OffsetUnits;
			}
			set
			{
				this.m_OffsetUnits = value;
			}
		}

		public float offsetFactor
		{
			get
			{
				return this.m_OffsetFactor;
			}
			set
			{
				this.m_OffsetFactor = value;
			}
		}

		public bool Equals(RasterState other)
		{
			return this.m_CullingMode == other.m_CullingMode && this.m_OffsetUnits == other.m_OffsetUnits && this.m_OffsetFactor.Equals(other.m_OffsetFactor) && this.m_DepthClip == other.m_DepthClip;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is RasterState && this.Equals((RasterState)obj);
		}

		public override int GetHashCode()
		{
			int num = (int)this.m_CullingMode;
			num = (num * 397) ^ this.m_OffsetUnits;
			num = (num * 397) ^ this.m_OffsetFactor.GetHashCode();
			return (num * 397) ^ this.m_DepthClip.GetHashCode();
		}

		public static bool operator ==(RasterState left, RasterState right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RasterState left, RasterState right)
		{
			return !left.Equals(right);
		}

		public static readonly RasterState defaultValue = new RasterState(CullMode.Back, 0, 0f, true);

		private CullMode m_CullingMode;

		private int m_OffsetUnits;

		private float m_OffsetFactor;

		private byte m_DepthClip;

		private byte m_Padding1;

		private byte m_Padding2;

		private byte m_Padding3;
	}
}
