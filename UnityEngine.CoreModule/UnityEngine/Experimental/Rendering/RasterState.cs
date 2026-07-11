using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Values for the raster state.</para>
	/// </summary>
	public struct RasterState
	{
		public RasterState(CullMode cullingMode = CullMode.Back, int offsetUnits = 0, float offsetFactor = 0f, bool depthClip = true)
		{
			this.m_CullingMode = cullingMode;
			this.m_OffsetUnits = offsetUnits;
			this.m_OffsetFactor = offsetFactor;
			this.m_DepthClip = Convert.ToByte(depthClip);
		}

		/// <summary>
		///   <para>Controls which sides of polygons should be culled (not drawn).</para>
		/// </summary>
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

		/// <summary>
		///   <para>Enable clipping based on depth.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Scales the minimum resolvable depth buffer value.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Scales the maximum Z slope.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Default values for the raster state.</para>
		/// </summary>
		public static readonly RasterState Default = new RasterState(CullMode.Back, 0, 0f, true);

		private CullMode m_CullingMode;

		private int m_OffsetUnits;

		private float m_OffsetFactor;

		private byte m_DepthClip;
	}
}
